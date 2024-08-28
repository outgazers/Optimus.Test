using System.Text.RegularExpressions;
using Optimus.Services.Identity.Core.Entities;
using Optimus.Services.Identity.Core.Exceptions;
using Optimus.Services.Identity.Core.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Optimus.Services.Identity.Application.Commands;
using Optimus.Services.Identity.Application.DTO;
using Optimus.Services.Identity.Application.Events;
using Optimus.Services.Identity.Application.Exceptions;
using Optimus.Services.Identity.Application.Services.Message;

namespace Optimus.Services.Identity.Application.Services.Identity;

public class IdentityService : IIdentityService
{
    private static readonly Regex EmailRegex = new Regex(
        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex UsernameRegex = new Regex(
        "^[a-zA-Z][a-zA-Z0-9]*$");

    private static readonly Regex PasswordRegex = new Regex(
        @"^(?=.*[A-Za-z])(?=.*\d).{8,}$");
    
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<IdentityService> _logger;
    private readonly IEmailService _emailService;
    private readonly ICrmService _crmService;

    public IdentityService(IUserRepository userRepository, IPasswordService passwordService,
        IJwtProvider jwtProvider, IRefreshTokenService refreshTokenService,
        IMessageBroker messageBroker, ILogger<IdentityService> logger, IEmailService emailService, ICrmService crmService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtProvider = jwtProvider;
        _refreshTokenService = refreshTokenService;
        _messageBroker = messageBroker;
        _logger = logger;
        _emailService = emailService;
        _crmService = crmService;
    }

    public async Task<UserDto> GetAsync(Guid id)
    {
        var user = await _userRepository.GetAsync(id);

        return user is null ? null : new UserDto(user);
    }

    public async Task<AuthDto> SignInAsync(SignIn command)
    {
        if (!EmailRegex.IsMatch(command.Email))
        {
            _logger.LogError($"Invalid email: {command.Email}");
            throw new InvalidEmailException(command.Email);
        }

        var user = await _userRepository.GetAsync(email:command.Email);

        if (user is null || !_passwordService.IsValid(user.Password, command.Password))
        {
            _logger.LogError($"User with email: {command.Email} was not found.");
            throw new InvalidCredentialsException(command.Email);
        }

        if (!user.SignUpState.Equals(SignUpState.Complete))
        {
            _logger.LogError($"User with email: {command.Email} is not verified");
            throw new IsNotVerifiedException(command.Email);
        }
        
        if (!user.IsVerified)
        {
            _logger.LogError($"User with email: {command.Email} is not verified");
            throw new IsNotVerifiedException(command.Email);
        }

        if (!_passwordService.IsValid(user.Password, command.Password))
        {
            _logger.LogError($"Invalid password for user with id: {user.Id.Value}");
            throw new InvalidCredentialsException(command.Email);
        }

        var claims = user.Permissions.Any()
            ? new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = user.Permissions
            }
            : null;
        var auth = _jwtProvider.Create(user.Id, user.Role, claims: claims);
        auth.RefreshToken = await _refreshTokenService.CreateAsync(user.Id);

        _logger.LogInformation($"User with id: {user.Id} has been authenticated.");
        await _messageBroker.PublishAsync(new SignedIn(user.Id, user.Role));

        return auth;
    }

    public async Task<SignUpState?> SignUpAsync(string email, string username, string password , bool ignoreVerify = false)
    {
        if (!EmailRegex.IsMatch(email))
        {
            _logger.LogError($"Invalid email: {email}");
            throw new InvalidEmailException(email);
        }

        if (!UsernameRegex.IsMatch(username))
        {
            _logger.LogError($"Invalid username: {username}");
            throw new InvalidUsernameException(username);
        }

        var user = await _userRepository.GetAsync(username, email);
        
        if (user is not null)
        {
            return user.SignUpState;
        }

        if (user is { SentVerificationCodeAt: { } } && DateTime.Now.Subtract((DateTime) user.SentVerificationCodeAt) < TimeSpan.FromMinutes(2))
        {
            _logger.LogError("Verification code was send less than 2 minutes ago");
            throw new VerificationCodeTimeException(DateTime.Now.Subtract((DateTime) user.SentVerificationCodeAt));
        }

        var verificationCode = ValidationHelper.GenerateValidationCode();
        if (!ignoreVerify)
        {
            var isSucceed = await _emailService.SendVerificationCodeAsync(email, verificationCode);

            if (!isSucceed)
            {
                throw new SendSmsWasNotSucceeded();
            }
        }
        
        var hashedPassword = _passwordService.Hash(password);
        
        var crmAccountId = await _crmService.AddUserAsync(email, username, password);
        if(crmAccountId is 0)
            throw new TimeoutException();

        user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            VerificationCode = verificationCode,
            Password = hashedPassword,
            Role = Role.User,
            Permissions = Enumerable.Empty<string>(),
            IsVerified = ignoreVerify,
            SignUpState = ignoreVerify ? SignUpState.Complete : SignUpState.VerifyingEmail,
            SentVerificationCount = user?.SentVerificationCount is null ? 1 : user.SentVerificationCount++,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            UpdatedAt = DateTime.Now.ToUniversalTime(),
            SentVerificationCodeAt = DateTime.Now.ToUniversalTime(),
            CrmAccountId = crmAccountId
        };
        
        await _userRepository.AddAsync(user);
        
        await _messageBroker.PublishAsync(new SignedUp(user.Id, user.Email, user.Username, user.Role, crmAccountId));
        _logger.LogInformation($"Created an account for the user with id: {user.Id}.");
        return user.SignUpState;
    }

    public async Task SendVerifyEmailCodeAsync(string email)
    {
        if (!EmailRegex.IsMatch(email))
        {
            _logger.LogError($"Invalid email: {email}");
            throw new InvalidEmailException(email);
        }

        var user = await _userRepository.GetAsync(email:email);
        
        if (user is null)
        {
            _logger.LogError($"User not found with email: {email}");
            throw new UserNotFoundException(email);
        }
        
        if (user.SentVerificationCount.HasValue && user.SentVerificationCount > 5)
        {
            _logger.LogError($"User try verify code with email: {email} exceeded");
            throw new ExceededTryVerifyCode();
        }

        if (user != null && user.SentVerificationCodeAt != null &&
            DateTime.Now.Subtract((DateTime) user.SentVerificationCodeAt) < TimeSpan.FromMinutes(2))
        {
            _logger.LogError("Verification code was send less than 2 minutes ago");
            throw new VerificationCodeTimeException(new TimeSpan(0,2,0).Subtract(DateTime.Now.Subtract((DateTime) user.SentVerificationCodeAt)));
        }

        var verificationCode = ValidationHelper.GenerateValidationCode();
        var isSucceed = await _emailService.SendVerificationCodeAsync(email, verificationCode);
        
        if (!isSucceed)
        {
            throw new SendSmsWasNotSucceeded();
        }

        user.VerificationCode = verificationCode;
        user.SentVerificationCount = user?.SentVerificationCount is null ? 1 : user.SentVerificationCount++;
        user.SentVerificationCodeAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        
        _logger.LogInformation($"Updated password of user with id: {user.Id}.");
    }

    public async Task VerifyEmail(string email, string verificationCode)
    {
        if (!EmailRegex.IsMatch(email))
        {
            _logger.LogError($"Invalid email: {email}");
            throw new InvalidEmailException(email);
        }
        
        if (email.IsNullOrEmpty())
            throw new InvalidEmailException(email);
        
        var user = await _userRepository.GetAsyncForUpdate(email);
        if (user is null)
            throw new UserNotFoundException(email);

        if (verificationCode.IsNullOrEmpty() || !user.VerificationCode.Equals(verificationCode))
            throw new InvalidVerificationCodeException();
        
        // TODO: validation for time of expiration date of verification code
        
        if (!user.SignUpState.Equals(SignUpState.VerifyingEmail) && !user.SignUpState.Equals(SignUpState.UpdatingPassword))
        {
            _logger.LogError($"User with email: {email} is not on verifying state");
            throw new IsNotVerifiedException(email);
        }

        user.IsVerified = true;
        user.SignUpState = SignUpState.Complete;
        user.SentVerificationCount = 0;
        
        await _userRepository.UpdateAsync(user);
    }
    
    public async Task<AuthDto> UpdatePasswordAsync(string email, string password, string verificationCode)
    {
        if (email.IsNullOrEmpty())
            throw new InvalidEmailException(email);
        
        if (!EmailRegex.IsMatch(email))
        {
            _logger.LogError($"Invalid email: {email}");
            throw new InvalidEmailException(email);
        }
        
        var user = await _userRepository.GetAsyncForUpdate(email);
        if (user is null)
            throw new UserNotFoundException(email);
        
        if (!user.SignUpState.Equals(SignUpState.Complete))
        {
            _logger.LogError($"User with email: {email} is not completed sign up state.");
            throw new IsNotVerifiedException(email);
        }
        
        if (!user.IsVerified)
        {
            _logger.LogError($"User with email: {email} is not verified");
            throw new IsNotVerifiedException(email);
        }
        
        if (password.IsNullOrEmpty())
            throw new InvalidPasswordException();

        if (!PasswordRegex.IsMatch(password))
            throw new InvalidPasswordException();
        
        if (verificationCode.IsNullOrEmpty() || !user.VerificationCode.Equals(verificationCode))
            throw new InvalidVerificationCodeException();
        
        var hashedPassword = _passwordService.Hash(password);
        user.Password = hashedPassword;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        
        var claims = user.Permissions.Any()
            ? new Dictionary<string, IEnumerable<string>>
            {
                ["permissions"] = user.Permissions
            }
            : null;
        var auth = _jwtProvider.Create(user.Id, user.Role, claims: claims);
        auth.RefreshToken = await _refreshTokenService.CreateAsync(user.Id);

        _logger.LogInformation($"Changed password of account for the user with id: {user.Id} Successfully.");
        return auth;
    }
}
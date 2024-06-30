using Optimus.Services.Identity.Core.Entities;
using Optimus.Services.Identity.Application.Commands;
using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Services;

public interface IIdentityService
{
    Task<UserDto> GetAsync(Guid id);
    Task<AuthDto> SignInAsync(SignIn command);
    Task<SignUpState?> SignUpAsync(string email, string username, string password, bool ignoreVerify = false);
    Task VerifyEmail(string email, string verificationCode);
    Task<AuthDto> UpdatePasswordAsync(string email, string password, string verificationCode);
    Task SendVerifyEmailCodeAsync(string email);
}
using Convey.Auth;
using Optimus.Services.Identity.Application.Commands;
using Optimus.Services.Identity.Application.DTO;
using Optimus.Services.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Optimus.Services.Identity.Api.Models.Requests;
using Optimus.Services.Identity.Application;
using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ICrmService _crmService;
    private readonly IAppContext _appContext;

    public AuthController(IIdentityService identityService, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService, ICrmService crmService, IAppContext appContext)
    {
        _identityService = identityService;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _crmService = crmService;
        _appContext = appContext;
    }

    [HttpPost("/sign-in")]
    public async Task<ActionResult<AuthDto>> SignIn([FromBody] SignIn req)
    {
        var token = await _identityService.SignInAsync(req);
        
        return Ok(token);
    }

    [HttpPost("/access-tokens/revoke")]
    public async Task<IActionResult> RevokeAccessToken(string token)
    {
        await _accessTokenService.DeactivateAsync(token);
        return Ok();
    }
    
    [HttpPost("/refresh-tokens/use")]
    public async Task<ActionResult<AuthDto>> UseRefreshToken(string refreshToken)
    {
        var authDto = await _refreshTokenService.UseAsync(refreshToken);

        return Ok(authDto);
    }
    
    [HttpPost("/refresh-tokens/revoke")]
    public async Task<IActionResult> RevokeRefreshToken(string refreshToken)
    {
        await _refreshTokenService.RevokeAsync(refreshToken);
        
        return Ok();
    }

    [HttpPost("/sign-up")]
    public async Task<ActionResult<SignUpState>> SignUp([FromBody] SignUpRequest signUpRequest)
    {
        var state = await _identityService.SignUpAsync(signUpRequest.Email, signUpRequest.Username,
            signUpRequest.Password, true);

        if (state == null)
            return BadRequest("Something bad happened , please wait");

        return Ok(state);

    }

    [HttpPost("/send-verification-code")]
    public async Task SendVerificationCode([FromBody] string email)
    {
        await _identityService.SendVerifyEmailCodeAsync(email);
    }

    [HttpPost("/verify-email")]
    public async Task<ActionResult<string>> VerifyEmail([FromBody] VerifyEmailRequest req)
    {
        await _identityService.VerifyEmail(req.Email, req.Code);

        return Ok();
    }
    
    [HttpPost("/update-password")]
    public async Task<ActionResult<AuthDto>> UpdatePassword([FromBody] UpdatePasswordRequest req)
    {
        var authDto = await _identityService.UpdatePasswordAsync(req.Email, req.Password, req.VerificationCode);

        return Ok(authDto);
    }
    
    [HttpGet("/get-crm-login-url")]
    public async Task<ActionResult<string>> GetLoginUrl()
    {
        var user = await _identityService.GetAsync(_appContext.Identity.Id);
        var loginUrl = await _crmService.GetLoginUrlAsync(user.Email);
        return Ok(loginUrl);
    }
}
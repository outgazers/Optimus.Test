using Convey.Auth;
using Optimus.Services.Identity.Application.Commands;
using Optimus.Services.Identity.Application.DTO;
using Optimus.Services.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Optimus.Services.Identity.Api.Models.Requests;
using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IIdentityService identityService, IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
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
}
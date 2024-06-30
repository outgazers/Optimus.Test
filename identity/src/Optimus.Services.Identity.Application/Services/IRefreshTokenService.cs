using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Services;

public interface IRefreshTokenService
{
    Task<string> CreateAsync(Guid userId);
    Task RevokeAsync(string refreshToken);
    Task<AuthDto> UseAsync(string refreshToken);
}
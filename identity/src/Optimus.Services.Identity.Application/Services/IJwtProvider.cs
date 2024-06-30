using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Services;

public interface IJwtProvider
{
    AuthDto Create(Guid userId, string role, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null);
}
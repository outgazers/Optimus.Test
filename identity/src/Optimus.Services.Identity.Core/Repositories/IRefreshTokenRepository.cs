using Optimus.Services.Identity.Core.Entities;

namespace Optimus.Services.Identity.Core.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetAsync(string token);
        Task AddAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
    }
}
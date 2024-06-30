using Microsoft.EntityFrameworkCore;
using Optimus.Services.Identity.Core.Entities;
using Optimus.Services.Identity.Core.Repositories;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _context;
    private readonly DbSet<RefreshToken> _refreshTokens;

    public RefreshTokenRepository(IdentityDbContext context)
    {
        _context = context;
        _refreshTokens = _context.RefreshTokens;
    }

    public async Task<RefreshToken?> GetAsync(string token)
        => (await _refreshTokens.SingleOrDefaultAsync(x => x.Token == token));

    public async Task AddAsync(RefreshToken token)
    {
        await _refreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _refreshTokens.Update(token);
        await _context.SaveChangesAsync();        }
}
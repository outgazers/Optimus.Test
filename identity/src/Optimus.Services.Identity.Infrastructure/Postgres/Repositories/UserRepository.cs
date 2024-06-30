using Microsoft.EntityFrameworkCore;
using Optimus.Services.Identity.Core.Entities;
using Optimus.Services.Identity.Core.Repositories;

namespace Optimus.Services.Identity.Infrastructure.Postgres.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;
    private readonly DbSet<User> _users;
    
    public UserRepository(IdentityDbContext context)
    {
        _context = context;
        _users = context.Users;
    }
    
    public async Task<User?> GetAsync(Guid id)
        => (await _users.SingleOrDefaultAsync(x => x.Id == id));

    public async Task<User?> GetAsync(string? username = null, string email = null)
    {
        var user = await _users.SingleOrDefaultAsync(x => x.Username == username || x.Email == email);
        return user ?? null;
    }

    public async Task<User?> GetAsyncForUpdate(string email)
        => await _users.SingleOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
    {
        await _users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(User? user)
    {
        _users.Update(user);
        await _context.SaveChangesAsync();
    }
}

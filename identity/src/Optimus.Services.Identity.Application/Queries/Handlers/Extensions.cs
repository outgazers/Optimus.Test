using Optimus.Services.Identity.Core.Entities;
using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Queries.Handlers;

public static class Extensions
{
    public static UserDto AsDto(this User user)
        => user.Map<UserDto>();
    
    private static T Map<T>(this User user) where T : UserDto, new()
        => new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            Permissions = user.Permissions
        };
}
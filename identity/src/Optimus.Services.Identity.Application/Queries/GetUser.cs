using Convey.CQRS.Queries;
using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Queries;

public class GetUser : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
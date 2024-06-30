using Convey.CQRS.Queries;
using Optimus.Services.Identity.Core.Repositories;
using Optimus.Services.Identity.Application.DTO;

namespace Optimus.Services.Identity.Application.Queries.Handlers;

internal sealed  class GetUserHandler : IQueryHandler<GetUser, UserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserDto> HandleAsync(GetUser query, CancellationToken cancellationToken = new CancellationToken())
    {
        var user = await _userRepository.GetAsync(query.UserId);

        return user?.AsDto();
    }
}
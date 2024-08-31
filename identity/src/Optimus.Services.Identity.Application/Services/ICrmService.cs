namespace Optimus.Services.Identity.Application.Services;

public interface ICrmService
{
    Task<int> AddUserAsync(string email, string username, string password);
    Task<string> GetLoginUrlAsync(string email);
    Task AddGroupAsync(string accountId, string groupName);
    Task<string> AuthenticateAsync(string email, string password);
}
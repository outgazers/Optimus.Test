using Optimus.Services.Worker.Cores;
using Optimus.Services.Worker.Infrastructure.Models.CrmModels;

namespace Optimus.Services.Worker.Infrastructure;

public interface ICrmService
{
    Task<List<SearchContact>> GetNewLeadsAsync(string crmToken, string email, int groupId);
    Task<List<Lead>> GetDetailsLeadsAsync(string crmToken, string email, List<SearchContact> searchContacts,
        Guid userId);
    Task<bool> UnsubscribeLeadsAsync(string crmToken, string email, List<SearchContact> searchContacts);
}
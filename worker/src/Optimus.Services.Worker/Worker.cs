using Microsoft.EntityFrameworkCore;
using Optimus.Services.Worker.Cores;
using Optimus.Services.Worker.Infrastructure;
using Optimus.Services.Worker.Infrastructure.Models.CrmModels;
using Optimus.Services.Worker.Infrastructure.Postgres;

namespace Optimus.Services.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetService<ChatsDbContext>();
            var crmService = services.GetService<ICrmService>();

            var userContext = context.Set<User>();
            var users = await userContext.Where(x => !string.IsNullOrEmpty(x.CrmToken)).ToListAsync(stoppingToken).ConfigureAwait(false);
            var leadContext = context.Set<Lead>();

            foreach (var user in users)
            {
                _logger.LogWarning($"Processing user: {user.Email}");

                var subscribeContacts = await crmService.GetNewLeadsAsync(user.CrmToken, user.Email, (int)CrmTierGroup.Exchange).ConfigureAwait(false);
                var leads = await crmService.GetDetailsLeadsAsync(user.CrmToken, user.Email, subscribeContacts, user.Id).ConfigureAwait(false);
                var leadsToUnsubscribe = new List<SearchContact>();

                foreach (var lead in leads)
                {
                    if (await leadContext.AnyAsync(x => x.Email == lead.Email && x.UserId == user.Id, stoppingToken).ConfigureAwait(false))
                    {
                        leadsToUnsubscribe.Add(new SearchContact { Email = lead.Email, ContactId = subscribeContacts.First(sc => sc.Email == lead.Email).ContactId });
                        continue;
                    }

                    await leadContext.AddAsync(lead, stoppingToken).ConfigureAwait(false);
                    user.LeadExchangeCreated(1);
                    leadsToUnsubscribe.Add(new SearchContact { Email = lead.Email, ContactId = subscribeContacts.First(sc => sc.Email == lead.Email).ContactId });
                }

                await context.SaveChangesAsync(stoppingToken).ConfigureAwait(false);

                if (leadsToUnsubscribe.Count != 0)
                {
                    await crmService.UnsubscribeLeadsAsync(user.CrmToken, user.Email, leadsToUnsubscribe).ConfigureAwait(false);
                }

                _logger.LogInformation($"Finished processing user: {user.Email}");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken).ConfigureAwait(false);
        }
    }
}
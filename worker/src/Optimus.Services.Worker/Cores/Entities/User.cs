namespace Optimus.Services.Worker.Cores;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; }

    public int CrmAccountId { get; set; } = 0;

    public string CrmToken { get; set; }
    
    public int Credit { get; set; } = 0;

    public int LeadExchangeCounter { get; set; } = 0;

    public void LeadExchangeCreated(int leadExchangeNumber)
    {
        LeadExchangeCounter += leadExchangeNumber;
        while (LeadExchangeCounter >= 5)
        {
            Credit += 1;
            LeadExchangeCounter -= 5;
        }
    }
}
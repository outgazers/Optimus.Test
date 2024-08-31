namespace Optimus.Services.Worker.Cores;

public class Lead
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public LeadType LeadType { get; set; }
    public Tier Tier { get; set; }
    public string VectorKey { get; set; }
    public string CompanyName { get; set; }
    public string ContactName { get; set; }
    public string Position { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string CompanyPhoneNumber { get; set; }
    public string Volume { get; set; }
    public string ModeOfTransportation { get; set; }
    public string Competitor { get; set; }
    public string Industry { get; set; }
    public string Address { get; set; }
    public bool IsValid { get; set; } = false;
    public User User { get; set; }
}

public enum LeadType
{
    System,
    User
}

public enum Tier
{
    Silver,
    Gold,
    Diamond
}

public enum CrmTierGroup
{
    Gold = 2,
    Silver = 3,
    Diamond = 4,
    Exchange = 5
}
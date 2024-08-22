namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidIndustryException : DomainException
{
    public override string Code { get; } = "invalid_customer_industry";
    public Guid Id { get; }
    public string Industry { get; }

    public InvalidIndustryException(Guid id, string industry) : base(
        $"Customer with id: {id} has invalid industry.")
    {
        Id = id;
        Industry = industry;
    }
}
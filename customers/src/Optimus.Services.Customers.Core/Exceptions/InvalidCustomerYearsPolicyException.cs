namespace Optimus.Services.Customers.Core.Exceptions;

public class InvalidCustomerYearsPolicyException : DomainException
{
    public override string Code { get; } = "invalid_customer_birth_date";
    public Guid Id { get; }
    public DateTime BirthDate { get; }

    public InvalidCustomerYearsPolicyException(Guid id, DateTime birthDate) : base(
        $"Customer with id: {id} has invalid birthDate : {birthDate} (lower than 18 year).")
    {
        Id = id;
        BirthDate = birthDate;
    }
}
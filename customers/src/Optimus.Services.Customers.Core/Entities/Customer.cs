using Optimus.Services.Customers.Core.Events;
using Optimus.Services.Customers.Core.Exceptions;
using Optimus.Services.Customers.Core.ValueObjects;

namespace Optimus.Services.Customers.Core.Entities;

public class Customer : AggregateRoot
{
    public string Email { get; private set; }
    public string Username { get; set; }
    public FullName? FullName { get; private set; }
    public Address? Address { get; private set; }
    public string? CompanyName { get; set; }
    public string? MC { get; set; }
    public string? PhoneNumber { get; set; } 
    public string? NetTerms { get; set; } 
    public string? TMS { get; set; } 
    public bool? IsAssetBase { get; set; } 
    public List<ModsOfTransportation>? ModsOfTransportation { get; set; }
    public string? Industry { get; set; }
    public int? YearsInBusiness { get; set; }
    public bool IsVip { get; private set; }
    public State State { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Customer()
    {
    }

    public Customer(Guid id, string email, DateTime createdAt, DateTime updatedAt, string username) : this(id, email,
        createdAt, updatedAt, null, null, false, State.Incomplete, username, null, null, null, null, null, false, null, null, 0)
    {
    }

    public Customer(Guid id, string email, DateTime createdAt, DateTime updatedAt, string fullName,
        string address,
        bool isVip, State state, string username, string companyName, string? mc, string? phoneNumber, string? tMS,
        string? netTerms, bool isAssetBase, List<ModsOfTransportation> modsOfTransportation, string? industry,
        int yearsInBusiness)
    {
        Id = id;
        Email = email;
        Username = username;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        FullName = fullName;
        Address = address;
        CompanyName = companyName;
        MC = mc;
        PhoneNumber = phoneNumber;
        NetTerms = netTerms;
        TMS = tMS;
        IsAssetBase = isAssetBase;
        ModsOfTransportation = modsOfTransportation;
        Industry = industry;
        YearsInBusiness = yearsInBusiness;
        IsVip = isVip;
        State = state;
    }

    public void CompleteRegistration(string fullName, string address, string nationalCode,
        string companyName, string mc, string phoneNumber, string netTerms, string tMS, bool isAssetBase,
        List<ModsOfTransportation> modsOfTransportation, string industry, int yearsInBusiness)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidCustomerFullNameException(Id, fullName);
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidCustomerAddressException(Id, address);
        }

        if (State != State.Incomplete && State != State.AwaitForValidate)
        {
            throw new CannotChangeCustomerStateException(Id, State);
        }

        FullName = fullName;
        Address = address;
        CompanyName = companyName;
        MC = mc;
        PhoneNumber = phoneNumber;
        NetTerms = netTerms;
        TMS = tMS;
        IsAssetBase = isAssetBase;
        ModsOfTransportation = modsOfTransportation;
        Industry = industry;
        YearsInBusiness = yearsInBusiness;
        State = State.Valid;
        AddEvent(new CustomerRegistrationCompleted(this));
    }

    public void CompleteRegistrationFromUser(string fullName, string address,
        string companyName, string mc, string phoneNumber, string netTerms, string tMS, bool isAssetBase,
        List<ModsOfTransportation> modsOfTransportation, string industry, int yearsInBusiness)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new InvalidCustomerFullNameException(Id, fullName);
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidCustomerAddressException(Id, address);
        }

        if (string.IsNullOrWhiteSpace(industry))
        {
            throw new InvalidIndustryException(Id, industry);
        }

        if (!modsOfTransportation.Any())
        {
            throw new InvalidModsOfTransportation(Id);
        }

        if (State != State.Incomplete)
        {
            throw new CannotChangeCustomerStateException(Id, State);
        }

        FullName = fullName;
        Address = address;
        CompanyName = companyName;
        MC = mc;
        PhoneNumber = phoneNumber;
        NetTerms = netTerms;
        TMS = tMS;
        IsAssetBase = isAssetBase;
        ModsOfTransportation = modsOfTransportation;
        Industry = industry;
        YearsInBusiness = yearsInBusiness;
        State = State.Valid;
        AddEvent(new CustomerRegistrationCompletedFromUser(this));
    }

    public void SetValid() => SetState(State.Valid);

    public void SetIncomplete() => SetState(State.Incomplete);

    public void Lock() => SetState(State.Locked);

    public void MarkAsSuspicious() => SetState(State.Suspicious);

    private void SetState(State state)
    {
        var previousState = State;
        State = state;
        AddEvent(new CustomerStateChanged(this, previousState));
    }

    public void SetVip()
    {
        if (IsVip)
        {
            return;
        }

        IsVip = true;
        AddEvent(new CustomerBecameVip(this));
    }
}
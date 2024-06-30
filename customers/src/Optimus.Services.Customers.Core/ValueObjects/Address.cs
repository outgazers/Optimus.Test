using Optimus.Services.Customers.Core.Exceptions;

namespace Optimus.Services.Customers.Core.ValueObjects;

public class Address : IEquatable<Address>
{
    public string Value { get; }
        
    public Address(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 500 or < 3)
        {
            throw new InvalidAddressException(value);
        }
            
        Value = value.Trim();
    }

    public static implicit operator Address(string value) => value is null ? null : new Address(value);

    public static implicit operator string(Address value) => value?.Value;

    public bool Equals(Address other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Address)obj);
    }

    public override int GetHashCode() => Value is not null ? Value.GetHashCode() : 0;
        
    public override string ToString() => Value;
}
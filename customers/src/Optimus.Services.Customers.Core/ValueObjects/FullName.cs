﻿using System;
using Optimus.Services.Customers.Core.Exceptions;

namespace Optimus.Services.Customers.Core.ValueObjects;

public class FullName : IEquatable<FullName>
{
    public string Value { get; }
        
    public FullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length is > 50 or < 3)
        {
            throw new InvalidFullNameException(value);
        }

        Value = value.Trim().ToLowerInvariant().Replace(" ", ".");
    }

    public static implicit operator FullName(string value) => value is null ? null : new FullName(value);

    public static implicit operator string(FullName value) => value?.Value;

    public bool Equals(FullName other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((FullName)obj);
    }

    public override int GetHashCode() => Value is not null ? Value.GetHashCode() : 0;
        
    public override string ToString() => Value;
}
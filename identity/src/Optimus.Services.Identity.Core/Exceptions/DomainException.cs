namespace Optimus.Services.Identity.Core.Exceptions;

public abstract class DomainException : Exception
{
    public virtual string Code { get; }

    protected DomainException(string message) : base(message)
    {
    }
}
namespace Optimus.Services.Customers.Application.Exceptions;

public class InvalidCustomerImageAuthorizationSizeException: AppException
{
    public override string Code { get; } = "invalid_image_authorization_size";

    public InvalidCustomerImageAuthorizationSizeException()
        : base($"Invalid image file size uploaded.")
    {
    }
}
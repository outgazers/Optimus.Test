namespace Optimus.Services.Customers.Application.Exceptions;

public class InvalidCustomerImageAuthorizationFileException: AppException
{
    public override string Code { get; } = "invalid_image_authorization_file";

    public InvalidCustomerImageAuthorizationFileException()
        : base($"Invalid image file uploaded.")
    {
    }
}
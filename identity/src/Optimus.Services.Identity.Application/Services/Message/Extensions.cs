namespace Optimus.Services.Identity.Application.Services.Message;

public static class ValidationHelper
{
    public static string GenerateValidationCode()
    {
        int number;
        char code;
        string checkCode = String.Empty;
        
        var random = new Random();
        
        for (int i = 0; i < 5; i++)
        {
            number = random.Next();
            if (number % 2 == 0)
                code = (char)('0' + (char)(number % 10));
            else
                code = (char)('0' + (char)(number % 10));
            
            checkCode += code.ToString();
        }

        return checkCode;
    }
}
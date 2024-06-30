using Optimus.Services.Identity.Application.Services.Auth;
using HashidsNet;

namespace Optimus.Services.Identity.Infrastructure.Auth;

public  class HashHelper : IHashHelper
{
    private const string HashSalt =
        "OptimusIdentity-gai4yevohth0Zahphaevaibae3ahs6eeghoot1eekaeheib1Ju";

    private const int HashLength = 40;
    private const string HashAlphabets = "abcdefghklmnoprstuvw123456789";

    private static readonly Hashids Hasher =
        new Hashids(HashSalt, HashLength, HashAlphabets);

    public string Encode(long password)
    {
        return Hasher.EncodeLong(password);
    }

    public long Decode(string ePassword)
    {
        try
        {
            return Hasher.DecodeLong(ePassword).First();
        }
        catch
        {
            throw new ArgumentException("Invalid encoded password value");
        }
    }
}
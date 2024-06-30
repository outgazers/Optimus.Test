namespace Optimus.Services.Identity.Application.Services.Auth;

public interface IHashHelper
{
    string Encode(long password);
    long Decode(string ePassword);
}
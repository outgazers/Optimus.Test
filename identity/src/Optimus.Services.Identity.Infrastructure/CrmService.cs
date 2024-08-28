using System.Xml;
using Optimus.Services.Identity.Application.Services;

namespace Optimus.Services.Identity.Infrastructure;

public class CrmService : ICrmService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://api.stgi.net/api-xml";
    private readonly string _email = "ramtin@optimusgo.ai";
    private readonly string _authToken = "08dfa496058a15525dd01873a217892e9e0baacc6e3c1a8df6d4134c5054980c";

    public CrmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> AddUserAsync(string email, string username, string password)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", _email),
            new KeyValuePair<string, string>("auth_token", _authToken),
            new KeyValuePair<string, string>("xml", $"<CreateAccountRequest><Name>{username}</Name><Email>{email}</Email><Password>{password}</Password></CreateAccountRequest>")
        });

        var response = await _httpClient.PostAsync(_apiUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseContent);

        var accountIdNode = xmlDoc.SelectSingleNode("/CreateAccountResponse/AccountID");
        if (accountIdNode != null && int.TryParse(accountIdNode.InnerText, out int accountId))
        {
            return accountId;
        }

        throw new Exception("Failed to create account or parse AccountID from response.");
    }
    
    public async Task<string> GetLoginUrlAsync(string email)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", _email),
            new KeyValuePair<string, string>("auth_token", _authToken),
            new KeyValuePair<string, string>("xml", $"<GetLoginLinkRequest email=\"{email}\"/>")
        });

        var response = await _httpClient.PostAsync(_apiUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseContent);

        var loginUrlNode = xmlDoc.SelectSingleNode("/GetLoginLinkResponse/LoginURL");
        if (loginUrlNode != null)
        {
            return loginUrlNode.InnerText;
        }

        throw new Exception("Failed to get login URL from response.");
    }
}
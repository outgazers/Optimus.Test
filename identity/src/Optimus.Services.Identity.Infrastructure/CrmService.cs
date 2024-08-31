using System.Xml;
using Optimus.Services.Identity.Application.Services;

namespace Optimus.Services.Identity.Infrastructure;

public class CrmService : ICrmService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.stgi.net/api-xml";
    private const string Email = "ramtin@optimusgo.ai";
    private const string AuthToken = "08dfa496058a15525dd01873a217892e9e0baacc6e3c1a8df6d4134c5054980c";

    public CrmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> AddUserAsync(string email, string username, string password)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", Email),
            new KeyValuePair<string, string>("auth_token", AuthToken),
            new KeyValuePair<string, string>("xml", $"<CreateAccountRequest><Name>{username}</Name><Email>{email}</Email><Password>{password}</Password></CreateAccountRequest>")
        });

        var response = await _httpClient.PostAsync(ApiUrl, requestContent);
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
            new KeyValuePair<string, string>("email", Email),
            new KeyValuePair<string, string>("auth_token", AuthToken),
            new KeyValuePair<string, string>("xml", $"<GetLoginLinkRequest email=\"{email}\"/>")
        });

        var response = await _httpClient.PostAsync(ApiUrl, requestContent);
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

    public async Task AddGroupAsync(string accountId, string groupName)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", Email),
            new KeyValuePair<string, string>("auth_token", AuthToken),
            new KeyValuePair<string, string>("xml", $"<CreateGroupRequest account_id=\"{accountId}\"><Name>\"{groupName}\"/Name><GroupType>Private</GroupType></CreateGroupRequest>")
        });

        var response = await _httpClient.PostAsync(ApiUrl, requestContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task<string> AuthenticateAsync(string email, string password)
    {
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("email", email),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("xml", "<GetAuthTokenRequest></GetAuthTokenRequest>")
        });

        var response = await _httpClient.PostAsync(ApiUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(responseContent);

        var tokenNode = xmlDoc.SelectSingleNode("/GetAuthTokenResponse/Token");
        if (tokenNode != null)
        {
            return tokenNode.InnerText;
        }

        throw new Exception("Failed to authenticate or parse token from response.");
    }
}
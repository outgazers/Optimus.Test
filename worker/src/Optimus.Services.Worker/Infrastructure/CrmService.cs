using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using Optimus.Services.Worker.Cores;
using Optimus.Services.Worker.Infrastructure.Models.CrmModels;

namespace Optimus.Services.Worker.Infrastructure;

public class CrmService : ICrmService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.stgi.net/api-xml";

    public CrmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<List<SearchContact>> GetNewLeadsAsync(string crmToken, string email, int groupId)
    {
        var currentDate = DateTime.Now.ToString("yyyy/MM/dd");

        var requestContent = new StringContent(
            $"email={email}&auth_token={crmToken}&xml=<SearchContactsRequest group_id='{groupId}'>" +
            $"<From>single</From><Filter>any</Filter><Rules><Rule>" +
            $"<Field>ContactCreated</Field><Range>on</Range><Date>{currentDate}</Date></Rule></Rules>" +
            "<Includes><Unsubscribers>N</Unsubscribers></Includes><OrderBy>ContactCreated</OrderBy>" +
            "<OrderDirection>asc</OrderDirection><Page>1</Page></SearchContactsRequest>",
            Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.PostAsync(ApiUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var xmlDoc = XDocument.Parse(responseContent);

        var contactElements = xmlDoc.Descendants("Contact");
        var searchContacts = new List<SearchContact>();

        foreach (var contactElement in contactElements)
        {
            var leadCompany = contactElement.Element("Company")?.Value;
            var leadEmail = contactElement.Element("Email")?.Value;

            if (string.IsNullOrEmpty(leadCompany) || string.IsNullOrEmpty(leadEmail))
                continue;
            
            searchContacts.Add(new SearchContact
            {
                Email = leadEmail,
                ContactId = contactElement.Element("ID")?.Value
            });
        }

        return searchContacts;
    }

    public async Task<List<Lead>> GetDetailsLeadsAsync(string crmToken, string email, List<SearchContact> searchContacts, Guid userId)
    {
        var leads = new List<Lead>();

        foreach (var searchContact in searchContacts)
        {
            var requestContent = new StringContent(
                $"email={email}&auth_token={crmToken}&xml=<GetContactsRequest email=\"{searchContact.Email}\"></GetContactsRequest>",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync(ApiUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var xmlDoc = XDocument.Parse(responseContent);

            var contactElements = xmlDoc.Descendants("Contact");

            foreach (var contactElement in contactElements)
            {
                var leadCompany = contactElement.Element("Company")?.Value;
                var leadEmailValue = contactElement.Element("Email")?.Value;
                var firstName = contactElement.Element("Firstname")?.Value;
                var lastName = contactElement.Element("Lastname")?.Value;
                var phone = contactElement.Element("Phone")?.Value;
                var address1 = contactElement.Element("Address1")?.Value;
                var address2 = contactElement.Element("Address2")?.Value;
                var city = contactElement.Element("City")?.Value;
                var state = contactElement.Element("State")?.Value;
                var address = !string.IsNullOrEmpty(address1) ? address1 :
                    !string.IsNullOrEmpty(address2) ? address2 :
                    (!string.IsNullOrEmpty(city) && !string.IsNullOrEmpty(state)) ? $"{city}, {state}" : null;
                var companyPhoneNumber = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "CompanyPhoneNumber")?.Element("FieldValue")?.Value;
                var position = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "Position")?.Element("FieldValue")?.Value;
                var volume = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "Volume")?.Element("FieldValue")?.Value;
                var modeOfTransportation = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "ModeOfTransportation")?.Element("FieldValue")?.Value;
                var competitor = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "Competitor")?.Element("FieldValue")?.Value;
                var industry = contactElement.Element("UserDefinedFields")?.Descendants("UserDefinedField")
                    .FirstOrDefault(x => x.Element("FieldName")?.Value == "Industry")?.Element("FieldValue")?.Value;

                if (string.IsNullOrEmpty(leadCompany) || string.IsNullOrEmpty(leadEmailValue) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(position) || string.IsNullOrEmpty(volume) || string.IsNullOrEmpty(modeOfTransportation) || string.IsNullOrEmpty(competitor) || string.IsNullOrEmpty(industry) || string.IsNullOrEmpty(address))
                    continue;

                var lead = new Lead
                {
                    UserId = userId,
                    LeadType = LeadType.User,
                    Tier = Tier.Diamond,
                    VectorKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(leadCompany)).Replace('+', '-').Replace('/', '_').TrimEnd('='),
                    CompanyName = leadCompany,
                    ContactName = $"{firstName} {lastName}",
                    Email = leadEmailValue,
                    PhoneNumber = phone,
                    CompanyPhoneNumber = companyPhoneNumber,
                    Position = position,
                    Volume = volume,
                    ModeOfTransportation = modeOfTransportation,
                    Competitor = competitor,
                    Industry = industry,
                    Address = address,
                    IsValid = true
                };

                leads.Add(lead);
            }
        }

        return leads;
    }

    public async Task<bool> UnsubscribeLeadsAsync(string crmToken, string email, List<SearchContact> searchContacts)
    {
        var allUnsubscribed = true;
        const int maxRetryAttempts = 3;

        foreach (var searchContact in searchContacts)
        {
            var requestId = Guid.NewGuid().ToString();
            var requestContent = new StringContent(
                $"email={email}&auth_token={crmToken}&xml=<UnsubscribeContactsRequest>" +
                $"<Contacts><Contact contact_id=\"{searchContact.ContactId}\" request_id=\"{requestId}\" /></Contacts>" +
                $"</UnsubscribeContactsRequest>",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            int retryAttempts = 0;
            bool success = false;

            while (retryAttempts < maxRetryAttempts && !success)
            {
                var response = await _httpClient.PostAsync(ApiUrl, requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var xmlDoc = XDocument.Parse(responseContent);

                var result = xmlDoc.Descendants("Contact")
                    .FirstOrDefault(x => x.Element("request_id")?.Value == requestId)?
                    .Element("Result")?.Value;

                if (result == "Success")
                {
                    success = true;
                }
                else
                {
                    retryAttempts++;
                    if (retryAttempts == maxRetryAttempts)
                    {
                        allUnsubscribed = false;
                    }
                }
            }
        }

        return allUnsubscribed;
    }
}
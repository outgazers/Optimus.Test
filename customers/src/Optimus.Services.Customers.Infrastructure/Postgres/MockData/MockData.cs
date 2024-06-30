using Newtonsoft.Json;
using Optimus.Services.Customers.Core.Repositories;
using Optimus.Services.Customers.Infrastructure.Postgres.MockData.Models;

namespace Optimus.Services.Customers.Infrastructure.Postgres.MockData;

public class MockData
{
    private readonly ICustomerRepository _customerRepository;
    private readonly HttpClient _httpClient;
    public MockData(HttpClient httpClient, ICustomerRepository customerRepository)
    {
        _httpClient = httpClient;
        _customerRepository = customerRepository;
    }

    public async Task Initialize()
    {
        if (await _customerRepository.ExistsAsync())
        {
            return;
        }
        
        var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var customers = JsonConvert.DeserializeObject<MockCustomer[]>(content);

        var i = 0;
        foreach (var customerDto in customers)
        {
            var customer = MockCustomerMapper.MapToCustomer(customerDto);
            await _customerRepository.AddAsync(customer);

            if (i > 10)
                break;
            i++;
        }
    }
}
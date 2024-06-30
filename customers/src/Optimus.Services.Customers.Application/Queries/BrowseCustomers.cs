using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Application.DTO;
using SharedAbstractions.Queries;

namespace Optimus.Services.Customers.Application.Queries;

public class BrowseCustomers : PagedQuery<CustomerDto>
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public bool? IsVip { get; set; }
    public State? State { get; set; }
}
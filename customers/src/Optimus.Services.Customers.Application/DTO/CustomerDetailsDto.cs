using Optimus.Services.Customers.Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Optimus.Services.Customers.Application.DTO;

public class CustomerDetailsDto : CustomerDto
{
    public string? FullName { get; set; }
    public string? NationalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    public bool IsVip { get; set; }
}

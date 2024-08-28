using Convey.CQRS.Commands;
using Convey.CQRS.Queries;
using Optimus.Services.Customers.Application.Commands;
using Optimus.Services.Customers.Application.DTO;
using Optimus.Services.Customers.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Optimus.Services.Customers.Application.Events;
using Optimus.Services.Customers.Application.Services;
using Optimus.Services.Customers.Core.Entities;
using Optimus.Services.Customers.Core.Events;
using SharedAbstractions.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace Optimus.Services.Customers.Api.Controllers;

[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventMapper _eventMapper;

    public CustomerController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IMessageBroker messageBroker, IEventMapper eventMapper)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _messageBroker = messageBroker;
        _eventMapper = eventMapper;
    }
    [HttpGet("/test")]
    public async Task<IActionResult> Test()
    {

        var modesOfTransportation = new List<ModesOfTransportation>
        {
            ModesOfTransportation.AirFreight,
            ModesOfTransportation.FTL,
            ModesOfTransportation.CrossBorder,
            ModesOfTransportation.OceanFreight
        };
        var address = "Los Angles";
        var industry = "Vitamins";

        var customer = new Customer(
            Guid.NewGuid(),
            "optimus@gmail.com",
            DateTime.UtcNow,
            DateTime.UtcNow,
            "optimusUser",
            1
        );
        
        customer.CompleteRegistrationFromUser(
            "Optimus Prime",
            "Los Angles",
            "Autobots Inc.",
            "123456",
            "123-456-7890",
            "30 days",
            "OptimusTMS",
            true,
            modesOfTransportation,
            industry,
            10
        ); 
        var e = _eventMapper.MapAll(customer.Events);
        if(e.Any())
            await _messageBroker.PublishAsync(e.ToArray());
        // var customerEvent = new CustomerCreated(new Guid("2adc0434-5f2c-4fd3-ac6e-9bd445855d0b"), modesOfTransportation, industry, address);
        // await _messageBroker.PublishAsync(customerEvent);
        return Ok("Test");
    }

    [HttpGet("/customers/me")]
    [SwaggerOperation("Get user info")]
    public async Task<ActionResult<CustomerDetailsDto>> Me()
    {
        var result = await _queryDispatcher.QueryAsync(new GetCustomerForUser());
        return Ok(result);
    }

    [HttpPost("/customers/complete-profile")]
    [SwaggerOperation("Complete profile of customer(user)")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteCustomerRegistrationFromUser command)
    {
        await _commandDispatcher.SendAsync(command);
        return Ok();
    }

    [HttpGet("/customers/{customerId:guid}")]
    [SwaggerOperation("Get customer")]
    public async Task<ActionResult<CustomerDetailsDto>> GetCustomer([FromRoute] Guid customerId)
    {
        return Ok(await _queryDispatcher.QueryAsync(new GetCustomer {CustomerId = customerId}));
    }

    [HttpGet("/customers")]
    [SwaggerOperation("Browse customers")]
    public async Task<ActionResult<Paged<CustomerDetailsDto>>> GetCustomers([FromQuery] BrowseCustomers query)
    {
        var res = await _queryDispatcher.QueryAsync(query);
        return Ok(res);
    }

    [HttpPost("/customers/complete-profile-from-admin")]
    [SwaggerOperation("Complete profile of customer(admin)")]
    public async Task<IActionResult> CompleteProfile([FromBody] CompleteCustomerRegistration command)
    {
        await _commandDispatcher.SendAsync(command);
        return Ok();
    }


    [HttpPut("/customers/{customerId}/state/{state}")]
    [SwaggerOperation("Change state of customer")]
    public async Task<IActionResult> ChangeState([FromRoute] Guid customerId, [FromRoute] State state)
    {
        await _commandDispatcher.SendAsync(new ChangeCustomerState(customerId, state.ToString()));
        return Ok();
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using renteasy_api.Application.DTOs.Properties;
using renteasy_api.Application.Services;

namespace renteasy_api.Controllers;

[ApiController]
[Route("api/properties")]
[Authorize(Roles = "Landlord")]
public class PropertiesController : ControllerBase
{
    private readonly PropertyService _propertyService;

    public PropertiesController(PropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        if (!request.BillCategories.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one bill category is required."
            });
        }

        var landlordId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var dto = await _propertyService.CreatePropertyAsync(landlordId, request);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var dto = await _propertyService.GetPropertyAsync(id);
        if (dto == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "Property not found."
            });
        }

        return Ok(dto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        if (!request.BillCategories.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one bill category is required."
            });
        }

        var dto = await _propertyService.UpdatePropertyAsync(id, request);
        if (dto == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "Property not found."
            });
        }

        return Ok(dto);
    }

    [HttpPut("{id:guid}/payment-methods")]
    public async Task<IActionResult> UpdatePaymentMethods(Guid id, [FromBody] UpdatePaymentMethodsRequest request)
    {
        var dto = await _propertyService.UpdatePaymentMethodsAsync(id, request);
        if (dto == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "Property not found."
            });
        }

        return Ok(dto);
    }
}

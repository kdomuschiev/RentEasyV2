using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEasy.Api.Application.DTOs.Properties;
using RentEasy.Api.Application.Services;

namespace RentEasy.Api.Controllers;

[ApiController]
[Route("api/properties")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly PropertyService _propertyService;

    public PropertiesController(PropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    private IActionResult? TryGetLandlordId(out Guid landlordId)
    {
        var claim = User.FindFirstValue("landlord_id");
        if (!Guid.TryParse(claim, out landlordId))
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Missing or invalid landlord identity claim."
            });
        }
        return null;
    }

    [HttpPost]
    [Authorize(Roles = "Landlord")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        if (request.BillCategories == null || !request.BillCategories.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one bill category is required."
            });
        }

        var authError = TryGetLandlordId(out var landlordId);
        if (authError != null) return authError;

        var dto = await _propertyService.CreatePropertyAsync(landlordId, request);
        return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var authError = TryGetLandlordId(out var landlordId);
        if (authError != null) return authError;

        var dto = await _propertyService.GetPropertyAsync(landlordId, id);
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
    [Authorize(Roles = "Landlord")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePropertyRequest request)
    {
        if (request.BillCategories == null || !request.BillCategories.Any())
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7807",
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "At least one bill category is required."
            });
        }

        var authError = TryGetLandlordId(out var landlordId);
        if (authError != null) return authError;

        var dto = await _propertyService.UpdatePropertyAsync(landlordId, id, request);
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
    [Authorize(Roles = "Landlord")]
    public async Task<IActionResult> UpdatePaymentMethods(Guid id, [FromBody] UpdatePaymentMethodsRequest request)
    {
        var authError = TryGetLandlordId(out var landlordId);
        if (authError != null) return authError;

        var dto = await _propertyService.UpdatePaymentMethodsAsync(landlordId, id, request);
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

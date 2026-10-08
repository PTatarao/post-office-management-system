using Microsoft.AspNetCore.Mvc;
using PostOffice.Application.Contracts.Shipments;
using PostOffice.Application.Services;
using PostOffice.Domain.Entities;

namespace PostOffice.Api.Controllers;

[ApiController]
[Route("api/shipments")]
public sealed class ShipmentsController(ShipmentService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<ShipmentDto> Get(Guid id)
    {
     return  service.GetAsync(id);
    }

    [HttpGet]
    public Task<PostOffice.Application.Common.Pagination.PagedResult<ShipmentDto>> Search(
        [FromQuery] ShipmentStatus? status,
        [FromQuery] Guid? locationPostOfficeId,
        [FromQuery] WeightCategory? weight,
        [FromQuery] string? shipmentNumber,
        [FromQuery] string? shipmentType,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var filter = new ShipmentFilter(status, locationPostOfficeId, weight, shipmentNumber, shipmentType);
        return service.SearchAsync(filter, pageNumber, pageSize);
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(CreateShipmentRequest request)
    {
        var result = await service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateShipmentRequest request)
    {
        await service.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateShipmentStatusRequest request)
    {
        await service.UpdateStatusAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}

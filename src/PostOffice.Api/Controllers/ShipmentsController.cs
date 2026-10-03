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
    public Task<ShipmentDto> Get(Guid id, CancellationToken ct) => service.GetAsync(id, ct);

    [HttpGet]
    public Task<PostOffice.Application.Common.Pagination.PagedResult<ShipmentDto>> Search(
        [FromQuery] ShipmentStatus? status,
        [FromQuery] Guid? locationPostOfficeId,
        [FromQuery] WeightCategory? weight,
        [FromQuery] string? shipmentNumber,
        [FromQuery] string? shipmentType,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var filter = new ShipmentFilter(status, locationPostOfficeId, weight, shipmentNumber, shipmentType);
        return service.SearchAsync(filter, pageNumber, pageSize, ct);
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(CreateShipmentRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateShipmentRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateShipmentStatusRequest request, CancellationToken ct)
    {
        await service.UpdateStatusAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using PostOffice.Application.Contracts.PostOffices;
using PostOffice.Application.Services;

namespace PostOffice.Api.Controllers;

[ApiController]
[Route("api/post-offices")]
public sealed class PostOfficesController(PostOfficeService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<PostOfficeDto> Get(Guid id, CancellationToken ct) => service.GetAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<PostOfficeDto>> Create(CreatePostOfficeRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePostOfficeRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id, request, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}

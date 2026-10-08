using Microsoft.AspNetCore.Mvc;
using PostOffice.Application.Contracts.PostOffices;
using PostOffice.Application.Services;

namespace PostOffice.Api.Controllers;

[ApiController]
[Route("api/post-offices")]
public sealed class PostOfficesController(PostOfficeService service) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<PostOfficeDto> Get(Guid id) 
    { 
        return service.GetAsync(id);
    }

    [HttpGet]
    public Task<IEnumerable<PostOfficeDto>> GetAll()
    {
       return service.GetAllAsync();
    }

    [HttpPost]
    public async Task<ActionResult<PostOfficeDto>> Create(CreatePostOfficeRequest request)
    {
        var result = await service.CreateAsync(request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePostOfficeRequest request)
    {
        await service.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}

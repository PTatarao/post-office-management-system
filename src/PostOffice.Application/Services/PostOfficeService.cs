using PostOffice.Application.Abstractions;
using PostOffice.Application.Common.Exceptions;
using PostOffice.Application.Contracts.PostOffices;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Application.Services;

public sealed class PostOfficeService(IPostOfficeRepository repository)
{
    public async Task<PostOfficeDto> GetAsync(Guid id, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Post office '{id}' was not found.");
        return Map(entity);
    }

    public async Task<PostOfficeDto> CreateAsync(CreatePostOfficeRequest request, CancellationToken ct)
    {
        if (await repository.ExistsByZipCodeAsync(request.ZipCode, null, ct))
            throw new InvalidOperationException($"ZIP code '{request.ZipCode}' already exists.");

        var entity = new PostOfficeEntity(request.ZipCode, request.Name, request.City);
        await repository.AddAsync(entity, ct);
        await repository.SaveChangesAsync(ct);
        return Map(entity);
    }

    public async Task UpdateAsync(Guid id, UpdatePostOfficeRequest request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Post office '{id}' was not found.");

        if (await repository.ExistsByZipCodeAsync(request.ZipCode, id, ct))
            throw new InvalidOperationException($"ZIP code '{request.ZipCode}' already exists.");

        entity.Update(request.ZipCode, request.Name, request.City);
        await repository.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Post office '{id}' was not found.");

        repository.Remove(entity);
        await repository.SaveChangesAsync(ct);
    }

    private static PostOfficeDto Map(PostOfficeEntity x) => new(x.Id, x.ZipCode, x.Name, x.City);
}

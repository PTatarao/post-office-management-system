using System.Linq;
using PostOffice.Application.Common.Exceptions;
using PostOffice.Application.Contracts.PostOffices;
using PostOffice.Application.Interface;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Application.Services;

public sealed class PostOfficeService(IPostOfficeRepository repository)
{
    public async Task<PostOfficeDto> GetAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
            return new PostOfficeDto(Guid.Empty, string.Empty, string.Empty, string.Empty);
            return MarshalPostOffice(entity);
    }

    public async Task<PostOfficeDto> CreateAsync(CreatePostOfficeRequest request)
    {
        if (await repository.ExistsByZipCodeAsync(request.ZipCode, null))
            throw new InvalidOperationException($"ZIP code '{request.ZipCode}' already exists.");

        var entity = new PostOfficeEntity(request.ZipCode, request.Name, request.City);
        await repository.AddAsync(entity);
        return MarshalPostOffice(entity);
    }

    public async Task UpdateAsync(Guid id, UpdatePostOfficeRequest request)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is not null)
        {

            if (await repository.ExistsByZipCodeAsync(request.ZipCode, id))
                throw new InvalidOperationException($"ZIP code '{request.ZipCode}' already exists.");

            entity.Update(request.ZipCode, request.Name, request.City);
           repository.Update(entity);
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null)
        {
            throw new NotFoundException($"Post office '{id}' was not found.");
        }

        repository.Remove(entity);
    }

    public async Task<IEnumerable<PostOfficeDto>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        return entities.Select(MarshalPostOffice).ToList();
    }

    private static PostOfficeDto MarshalPostOffice(PostOfficeEntity x) => new(x.Id, x.ZipCode, x.Name, x.City);
}

using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Application.Abstractions;

public interface IPostOfficeRepository
{
    Task<PostOfficeEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByZipCodeAsync(string zipCode, Guid? excludeId, CancellationToken ct);
    Task AddAsync(PostOfficeEntity postOffice, CancellationToken ct);
    void Remove(PostOfficeEntity postOffice);
    Task SaveChangesAsync(CancellationToken ct);
}

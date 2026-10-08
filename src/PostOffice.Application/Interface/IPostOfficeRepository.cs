using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Application.Interface;

public interface IPostOfficeRepository
{
    Task<PostOfficeEntity?> GetByIdAsync(Guid id);
    Task<List<PostOfficeEntity>> GetAllAsync();
    Task<bool> ExistsByZipCodeAsync(string zipCode, Guid? excludeId);
    Task AddAsync(PostOfficeEntity postOffice);
    void Remove(PostOfficeEntity postOffice);
}

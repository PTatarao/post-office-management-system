using MongoDB.Driver;
using PostOffice.Application.Interface;
using PostOffice.Domain.Entities;
using PostOfficeEntity = PostOffice.Domain.Entities.PostOffice;

namespace PostOffice.Infrastructure
{
    public sealed class PostOfficeRepository(PostOfficeDataAccess db) : IPostOfficeRepository
    {
        private readonly IMongoCollection<PostOfficeEntity> _collection = db.PostOffices();

        public Task<PostOfficeEntity?> GetByIdAsync(Guid id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<PostOfficeEntity>> GetAllAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }
        public Task<bool> ExistsByZipCodeAsync(string zipCode, Guid? excludeId)
        {
            var filter = Builders<PostOfficeEntity>.Filter.Eq(x => x.ZipCode, zipCode);
            if (excludeId.HasValue) filter &= Builders<PostOfficeEntity>.Filter.Ne(x => x.Id, excludeId.Value);
            return _collection.Find(filter).AnyAsync();
        }

        public Task AddAsync(PostOfficeEntity entity)
        {
           return _collection.InsertOneAsync(entity);
        }
        public void Remove(PostOfficeEntity entity)
        {
            _collection.DeleteOne(x => x.Id == entity.Id);
        }
        public void Update(PostOfficeEntity entity)
        {
            _collection.ReplaceOne(x => x.Id == entity.Id, entity);
        }
    }

}

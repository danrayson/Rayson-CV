namespace Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    IQueryable<T> Query();
    Task AddAsync(T entity);
    Task AddManyAsync(T[] entities);
    Task UpdateAsync(T entity);
    Task UpdateManyAsync(T[] entities);
    Task SoftDeleteAsync(T entity);
    Task SoftDeleteManyAsync(T[] entities);
    Task HardDeleteAsync(T entity);
    Task HardDeleteManyAsync(T[] entities);
    Task<List<T>> GetListAsync(IQueryable<T>? predicate = null, bool includeDeleted = false);
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetByIdManyAsync(int[] ids);
}
using Domain;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class Repository<T>(RaysonDevDbContext context) : IRepository<T> where T : Entity
{
    protected readonly DbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public IQueryable<T> Query()
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(T entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task HardDeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> GetListAsync(IQueryable<T>? predicate = null, bool includeDeleted = false)
    {
        IQueryable<T> query = (predicate ?? Query()).Where(e => includeDeleted || e.DeletedAt == null);
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddManyAsync(T[] entities)
    {
        foreach (var entity in entities)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateManyAsync(T[] entities)
    {
        foreach (var entity in entities)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteManyAsync(T[] entities)
    {
        foreach (var entity in entities)
        {
            entity.DeletedAt = DateTime.UtcNow;
            _context.Entry(entity).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
    }

    public async Task HardDeleteManyAsync(T[] entities)
    {
        foreach (var entity in entities)
        {
            _context.Set<T>().Remove(entity);
        }
        await _context.SaveChangesAsync();
    }
    public async Task<List<T>> GetByIdManyAsync(int[] ids)
    {
        return await _context.Set<T>().Where(e => ids.Contains(e.Id)).ToListAsync();
    }
}

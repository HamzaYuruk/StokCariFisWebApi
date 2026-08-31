namespace CariWebApi.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
    
    IQueryable<T> Query();
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}
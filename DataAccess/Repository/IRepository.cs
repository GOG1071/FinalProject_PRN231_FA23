namespace DataAccess.Repository;

using BusinessObject.Models;

public interface IRepository<T> where T : IModel
{
    void Add(T entity);
    void Update(T entity);
    void Delete(object[]? objects);
    T    Get(object[]? objects);
    List<T> GetAll();
}
namespace DataAccess;

using BusinessObject.Models;

public interface IDao<T> where T : class, IModel
{
    static abstract T Get(object[]? objects);
    static abstract T Get(T t);
    static abstract List<T> GetAll();
    static abstract void Add(T t);
    static abstract void Update(T t);
    static abstract void Delete(object[]? objects);
}
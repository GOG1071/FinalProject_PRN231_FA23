namespace DataAccess.Repository.Base;

using BusinessObject.Models;
using DataAccess.DAO.Base;

public class Repository<TModel, TDao> : IRepository<TModel> where TModel : class, IModel where TDao : IDao<TModel>
{
    public void         Add(TModel entity)        => TDao.Add(entity);
    public void         Update(TModel entity)     => TDao.Update(entity);
    public void         Delete(object[]? objects) => TDao.Delete(objects);
    public TModel       Get(object[]? objects)    => TDao.Get(objects);
    public List<TModel> GetAll()                  => TDao.GetAll();
}
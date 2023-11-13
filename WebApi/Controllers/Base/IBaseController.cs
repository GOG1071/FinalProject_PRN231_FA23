namespace WebApi.Controllers.Base;

using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;

public interface IBaseController<T> where T : class, IModel
{
    ActionResult<IEnumerable<T>> GetAll();
    ActionResult<T>              Add(T t);
    ActionResult<T>              Update(T t);
    
    ActionResult<T> DeleteAbstract(object[]? objects);
    ActionResult<T> GetAbstract(object[]? objects);
}
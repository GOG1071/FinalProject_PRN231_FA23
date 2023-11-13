namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

public class CategoryController : BaseController<Category, CategoryRepo>
{
    public CategoryController()
    {
        this.repo = new CategoryRepo();
    }
    
    [HttpGet("{id:int}")]
    public ActionResult<Category> Get(int id)
    {
        return this.GetAbstract(new object[] { id });
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult<Category> Delete(int id)
    {
        return this.DeleteAbstract(new object[] { id });
    }
    
}
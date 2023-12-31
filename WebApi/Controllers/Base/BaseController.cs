﻿namespace WebApi.Controllers.Base;

using BusinessObject.Models;
using DataAccess.Repository.Base;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]/[action]")]
public abstract class BaseController<TModel,TRepo> : ControllerBase, IBaseController<TModel> where TModel : class, IModel where TRepo : IRepository<TModel> 
{
    protected TRepo repo;
    
    [HttpGet]
    public virtual ActionResult<IEnumerable<TModel>> GetAll()
    {
        return this.repo.GetAll();
    }
    
    [HttpPost]
    public virtual ActionResult<TModel> Add(TModel t)
    {
        var model = this.repo.Get(t.PrimaryKey);
        if (model != null) { return Conflict(); }
        this.repo.Add(t);
        return this.NoContent();
    }
    
    [HttpPut]
    public virtual ActionResult<TModel> Update(TModel t)
    {
        var model = this.repo.Get(t.PrimaryKey);
        if (model == null) { return NotFound(); }
        this.repo.Update(t);
        return this.NoContent();
    }
    [NonAction]
    public virtual ActionResult<TModel> DeleteAbstract(object[]? objects)
    {
        var model = this.repo.Get(objects);
        if (model == null) { return NotFound(); }
        this.repo.Delete(objects);
        return this.NoContent();
    }
    [NonAction]
    public virtual ActionResult<TModel> GetAbstract(object[]? objects)
    {
        var model = this.repo.Get(objects);
        if (model == null) { return NotFound(); }
        return model;
    }
}
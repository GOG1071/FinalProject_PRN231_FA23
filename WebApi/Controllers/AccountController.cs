namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

public class AccountController : BaseController<Account,AccountRepo>
{
    public AccountController() {
        this.repo = new AccountRepo();
    }
    
    [HttpGet]
    public ActionResult<Account> Get(string email, string password)
    {
        var model = this.repo.Get(email, password);
        if (model == null) { return NotFound(); }
        return model;
    }
    
    [HttpDelete]
    public ActionResult<Account> Delete(string email, string password)
    {
        var model = this.repo.Get(email, password);
        if (model == null) { return NotFound(); }
        this.repo.Delete(model.PrimaryKey);
        return NoContent();
    }
}
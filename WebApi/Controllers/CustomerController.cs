namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

public class CustomerController : BaseController<Customer, CustomerRepo>
{
    public CustomerController() {
        this.repo = new CustomerRepo();
    }
    
    [HttpGet("{id:int}")]
    public ActionResult<Customer> Get(int id)
    {
        return this.GetAbstract(new object[] { id });
    }
    
    [HttpDelete("{id:int}")]
    public ActionResult<Customer> Delete(int id)
    {
        return this.DeleteAbstract(new object[] { id });
    }
}
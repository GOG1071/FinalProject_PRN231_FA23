namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using WebApi.Controllers.Base;

public class ProductController : BaseController<Product, ProductRepo>
{
    public ProductController()
    {
        this.repo = new ProductRepo();
    }
}
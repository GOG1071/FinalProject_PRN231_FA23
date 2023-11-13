namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using WebApi.Controllers.Base;

public class OrderController : BaseController<Order, OrderRepo>
{
    public OrderController() {
        this.repo = new OrderRepo();
    }
}
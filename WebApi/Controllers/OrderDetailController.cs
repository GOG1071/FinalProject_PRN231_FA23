namespace WebApi.Controllers;

using BusinessObject.Models;
using DataAccess.Repository;
using WebApi.Controllers.Base;

public class OrderDetailController : BaseController<OrderDetail, OrderDetailRepo>
{
    public OrderDetailController()
    {
        this.repo = new OrderDetailRepo();
    }
}
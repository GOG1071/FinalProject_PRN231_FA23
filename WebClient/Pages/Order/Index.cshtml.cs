namespace WebClient.Pages.Order
{
    using System.Security.Claims;
    using System.Text.Json;
    using BusinessObject.Models;
    using DataAccess.DTO;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using WebClient.Helpers;

    [Authorize]
    public class IndexModel : PageModel
    {
        private HttpClient client;
        private string AccountApiUrl;
        private string OrderApiUrl;

        public IndexModel()
        {
            this.client        = new HttpClient();
        }

        [BindProperty]
        public AccountDTO Auth { get; set; }
        public List<Order> Orders { get; set; }

        private int perPage = 5;

        [FromQuery(Name = "page")] public int Page { get; set; } = 1;

        public List<String> PagesLink { get; set; } = new List<string>();

        public async Task getData()
        {
            var accId = Int32.Parse(this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            this.AccountApiUrl = "https://localhost:5000/api/Account/getAccountDTO/" + accId;
            var response = await this.client.GetAsync(this.AccountApiUrl);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Auth = JsonSerializer.Deserialize<AccountDTO>(data, options);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> OnGetAsync()
        {
            var check = this.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (check != null && check.Value.Equals("Employee"))
            {
                return this.NotFound();
            }

            await this.getData();

            this.Orders = this.Auth.Customer.Orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((this.Page - 1) * this.perPage).Take(this.perPage).ToList();

            var page = new PageLink(this.perPage);
            this.PagesLink = page.getLink(this.Page, this.Auth.Customer.Orders.Count(), "/Order/Index?" );

            return this.Page();
        }

        private Dictionary<int, int> getCart()
        {
            var cart = this.HttpContext.Session.GetString("cart");

            Dictionary<int, int> list;

            if (cart != null)
            {
                list = JsonSerializer.Deserialize<Dictionary<int, int>>(cart);
            }
            else
            {
                list = new Dictionary<int, int>();
            }

            return list;
        }

        public async Task<IActionResult> OnGetAdd(int? id)
        {

            var check = this.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (check != null && check.Value.Equals("Employee"))
            {
                return this.NotFound();
            }

            if (id == null)
            {
                return this.NotFound();
            }

            var api      = "https://localhost:5000/api/Product/GetAll";
            var response = await this.client.GetAsync(api);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var productList = JsonSerializer.Deserialize<List<Product>>(data, options);

            if (productList == null)
            {
                return this.Redirect("/Product/Detail/" + id); 
            }

            var product = productList.FirstOrDefault(p => p.ProductId == id);
            
            if (product == null || product.UnitsInStock == 0)
            {
                this.TempData["fail"] = "Quantity = 0";
                return this.Redirect("/Product/Detail/" + id);
            } else
            {
                Dictionary<int, int> list = this.getCart();

                if ((list.Where(p => p.Key == id)).Count() == 0)
                {
                    list.Add((int)id, 1);
                }


                this.HttpContext.Session.SetString("cart", JsonSerializer.Serialize(list));
                return this.Redirect("/Cart/Index");
            }    


        }


    }
}

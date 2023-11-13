using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace WebRazor.Pages.Order
{
    using System.Text.Json;
    using BusinessObject.Models;
    using DataAccess.DTO;
    using WebClient.Helpers;

    [Authorize(Roles = "Customer")]
    public class CancelModel : PageModel
    {
        private HttpClient client;
        
        
        [BindProperty]
        public AccountDTO Auth { get;       set; }
        public List<Order> Orders { get; set; }

        private int perPage = 5;

        [FromQuery(Name = "page")] public int Page { get; set; } = 1;

        public List<string> PagesLink { get; set; } = new List<string>();

        public async Task getData()
        {
            var accId    = int.Parse(this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var api      = "https://localhost:5000/api/Account/getAccountDTO/" + accId;
            var response = await this.client.GetAsync(api);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Auth = JsonSerializer.Deserialize<AccountDTO>(data, options);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await this.getData();

            this.Orders = this.Auth.Customer.Orders.Where(o => o.RequiredDate == null)
                .OrderByDescending(o => o.OrderDate)
                .Skip((this.Page - 1) * this.perPage).Take(perPage).ToList();

            var page = new PageLink(this.perPage);
            this.PagesLink = page.getLink(this.Page, Auth.Customer.Orders.Where(o => o.RequiredDate == null).ToList().Count(), "/Order/Index?");

            return this.Page();
        }

    }
}
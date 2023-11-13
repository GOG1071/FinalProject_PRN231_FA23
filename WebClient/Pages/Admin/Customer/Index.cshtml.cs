namespace WebClient.Pages.Admin.Customer
{
    using System.Text;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using WebClient.Helpers;

    [Authorize]
    public class IndexModel : PageModel
    {
        private HttpClient client = new HttpClient();
        
        [FromQuery(Name = "page")] public int Page { get; set; } = 1;
        private int perPage = 10;
        public List<Customer> Customers { get; set; }
        [FromQuery(Name = "txtSearch")] public string Search { get; set; } = "";

        public List<String> PagesLink { get; set; } = new List<string>();
        public async Task<IActionResult> OnGetAsync()
        {

            await this.Load();

            return this.Page();
        }

        public async Task Load()
        {
            if (this.Search == null) this.Search = "";

            var result = await this.client.GetAsync("https://localhost:5000/api/Customer/GetAll");
            var data = await result.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var customers = JsonSerializer.Deserialize<List<Customer>>(data, options);
            this.Customers = customers?.Where(c =>  c.CompanyName.Contains(this.Search)).OrderBy(c=> c.CustomerId).Skip((this.Page - 1) * this.perPage).Take(this.perPage).ToList();

            PageLink page  = new PageLink(this.perPage);
            String   param = "txtSearch=" + this.Search;
            this.PagesLink = page.getLink(this.Page, customers.Count, "/Admin/Customer/Index?" + param + "&");
        }

        public async Task<IActionResult> OnGetActive(string? id)
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Customer/Get/" + id);
            var data = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var customer = JsonSerializer.Deserialize<Customer>(data, options);
            if (customer != null)
            {
                customer.Active = !customer.Active;
                var content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, "application/json");
                await this.client.PutAsync("https://localhost:5000/api/Customer/Add/", content);
            }

            return this.Redirect("/Admin/Customer/Index");
        }
    }
}

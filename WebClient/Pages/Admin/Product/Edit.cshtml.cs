namespace WebClient.Pages.Admin.Product
{
    using System.Text;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.SignalR;
    using WebClient.Hub;

    [Authorize(Roles = "Employee")]
    public class EditModel : PageModel
    {
        private HttpClient client = new HttpClient();
        [BindProperty] public Product Product { get; set; }
        public List<Category> Categories;
        private readonly IHubContext<HubServer> hubContext;

        public EditModel(IHubContext<HubServer> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task LoadProduct(int? id)
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Category/GetAll");
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            
            this.Categories = JsonSerializer.Deserialize<List<Category>>(data, options);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Product/Get/" + id);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Product = JsonSerializer.Deserialize<Product>(data, options);

            await this.LoadProduct(id);

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {

            await this.LoadProduct(id);

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }
            
            var response = await this.client.GetAsync("https://localhost:5000/api/Product/Get/" + id);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var product = JsonSerializer.Deserialize<Product>(data, options);
            product.ProductName     = this.Product.ProductName;
            product.CategoryId      = this.Product.CategoryId;
            product.UnitPrice       = this.Product.UnitPrice;
            product.QuantityPerUnit = this.Product.QuantityPerUnit;
            product.UnitsInStock    = product.UnitsInStock;

            this.ViewData["success"] = "Update successfully";
            
            var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
            await this.client.PutAsync("https://localhost:5000/api/Product/Add/", content);
            
            await this.hubContext.Clients.All.SendAsync("Reload");

            return this.Page();
        }

        public async Task<IActionResult> OnGetDelete(int? id)
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Product/Get/" + id);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Product = JsonSerializer.Deserialize<Product>(data, options);
            if (this.Product == null || this.Product.DeletedAt != null)
                return this.Redirect("/Admin/Product/Index");

            this.Product.DeletedAt = DateTime.Now;
            
            var content = new StringContent(JsonSerializer.Serialize(this.Product), Encoding.UTF8, "application/json");
            await this.client.PutAsync("https://localhost:5000/api/Product/Add/", content);

            await this.hubContext.Clients.All.SendAsync("Reload");

            return this.Redirect("/Admin/Product/Index");
        }
    }
}

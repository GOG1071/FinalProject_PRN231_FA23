namespace WebClient.Pages.Product
{
    using System.Security.Claims;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class DetailModel : PageModel
    {
        private HttpClient client;
        private string ProductApiUrl = "";
        public DetailModel()
        {
            this.client = new HttpClient();
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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
            this.ProductApiUrl = "https://localhost:5000/api/Product/GetAll/";
            var response = await this.client.GetAsync(this.ProductApiUrl);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var productList = JsonSerializer.Deserialize<List<Product>>(data, options);
            if (productList == null)
            {
                return this.NotFound();
            }
            
            this.Product = productList.FirstOrDefault(p => p.ProductId == id && p.DeletedAt == null);

            if (this.Product == null)
            {
                return this.NotFound();
            }
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
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

            this.ProductApiUrl = "https://localhost:5000/api/Product/GetAll/";
            var response = await this.client.GetAsync(this.ProductApiUrl);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var productList                       = JsonSerializer.Deserialize<List<Product>>(data, options);
            if (productList == null)
            {
                return this.NotFound();
            }
            
            this.Product = productList.FirstOrDefault(p => p.ProductId == id && p.DeletedAt == null);

            if (this.Product == null)
            {
                return this.NotFound();
            }
            return this.Page();
        }

        
    }
}

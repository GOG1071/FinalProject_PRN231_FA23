namespace WebClient.Pages.Admin.Order
{
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [Authorize(Roles = "Employee")] public class DetailModel : PageModel
    {
        public Order Order { get; set; }

        public int ID;

        private HttpClient client = new HttpClient();
        public async Task<IActionResult> OnGet(int? id)
        {
            this.ID = (int)id;

            var response = await this.client.GetAsync("https://localhost:5000/api/Order/Get/" + id);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Order = JsonSerializer.Deserialize<Order>(data, options);

            response = await this.client.GetAsync("https://localhost:5000/api/Product/GetAll");
            data     = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(data, options);

            foreach (var item in this.Order.OrderDetails)
            {
                item.Product = products.Where(p => p.DeletedAt == null)
                    .FirstOrDefault(p => p.ProductId == item.ProductId);
            }

            return this.Page();
        }
    }
}
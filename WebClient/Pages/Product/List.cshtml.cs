namespace WebClient.Pages.Product
{
    using System.Security.Claims;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using WebClient.Helpers;

    public class ListModel : PageModel
    {
        private HttpClient client;
        private string ProductApiUrl;
        private string CategoryApiUrl;
        [BindProperty] public List<Product> Products { get; set; } = new();

        [BindProperty] public List<Category> Categories { get; set; }

        [FromQuery(Name = "page")] public int Page { get; set; } = 1;

        [FromQuery(Name = "order")] public string Order { get; set; } = "None";

        private int perPage = 4;

        public int Id { get; set; }

        public List<String> PagesLink { get; set; } = new();

        public ListModel()
        {
            this.client = new HttpClient();
        }

        public async Task<IActionResult> OnGet(int? categoryId)
        {
            var check = this.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (check is { Value: "Employee" })
            {
                return this.NotFound();
            }

            this.CategoryApiUrl = "https://localhost:5000/api/Category/GetAll";
            var responseCategory = await this.client.GetAsync(this.CategoryApiUrl);
            var dataCat          = await responseCategory.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Categories = JsonSerializer.Deserialize<List<Category>>(dataCat, options).ToList();

            this.Id = (int)categoryId;

            this.ProductApiUrl = "https://localhost:5000/api/Product/GetAll";
            var responseNewProducts = await this.client.GetAsync(this.ProductApiUrl);
            var dataNewProducts     = await responseNewProducts.Content.ReadAsStringAsync();
            var products                = JsonSerializer.Deserialize<List<Product>>(dataNewProducts, options);

            var list =products.Where(p => p.CategoryId == categoryId && p.DeletedAt == null);
            var size = list.Count();

            var total = this.CalcPagesCount(size);

            if (this.Page < 1 || this.Page > total)
            {
                return this.NotFound();
            }

            string orderUrl = "";

            if (this.Order != "None")
            {
                orderUrl = "order=" + this.Order;
            }

            var page = new PageLink(this.perPage);
            this.PagesLink = page.getLink(this.Page, size, "/Product/List/" + categoryId + "?" + orderUrl + "&");

            switch (this.Order)
            {
                case "Asc":
                    list = (List<Product>?)list.OrderBy(p => p.UnitPrice);
                    break;
                case "Desc":
                    list = (List<Product>?)list.OrderByDescending(p => p.UnitPrice);
                    break;
            }

            this.Products = list
                .Skip((this.Page - 1) * this.perPage)
                .Take(this.perPage)
                .ToList();

            return this.Page();
        }

        private int CalcPagesCount(int size)
        {
            int totalPage = size / this.perPage;

            if (size % this.perPage != 0) totalPage++;
            return totalPage;
        }
    }
}

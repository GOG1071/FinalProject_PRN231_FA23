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

        public List<String> PagesLink { get; set; } = new List<string>();

        public ListModel()
        {
            this.client = new HttpClient();
        }

        public async Task<IActionResult> OnGet(int? id)
        {
            var check = this.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (check is { Value: "Employee" })
            {
                return this.NotFound();
            }

            this.CategoryApiUrl = "https://localhost:5000/api/Category";
            var responseCategory = await this.client.GetAsync(this.CategoryApiUrl);
            var dataCat          = await responseCategory.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            this.Categories = JsonSerializer.Deserialize<List<Category>>(dataCat, options).ToList();

            this.Id = (int)id;

            this.ProductApiUrl = "https://localhost:5000/api/Product/getNumberOfProductsByCategory/" + id;
            var responseNewProducts = await this.client.GetAsync(this.ProductApiUrl);
            var dataNewProducts     = await responseNewProducts.Content.ReadAsStringAsync();
            var size                = JsonSerializer.Deserialize<int>(dataNewProducts, options);

            var total = this.CalcPagesCount(size);

            if (this.Page < 1 || this.Page > total)
            {
                return this.NotFound();
            }

            String orderUrl = "";

            if (this.Order != "None")
            {
                orderUrl = "order=" + this.Order;
            }

            PageLink page = new PageLink(this.perPage);
            this.PagesLink = page.getLink(this.Page, size, "/Product/List/" + id + "?" + orderUrl + "&");

            this.ProductApiUrl = "https://localhost:5000/api/Product/" + id;
            var response = await this.client.GetAsync(this.ProductApiUrl);
            var data     = await response.Content.ReadAsStringAsync();
            var list     = JsonSerializer.Deserialize<List<Product>>(data, options);

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

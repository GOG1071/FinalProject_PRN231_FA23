namespace WebClient.Pages.Admin.Product
{
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.SignalR;
    using WebClient.Helpers;
    using WebClient.Hub;

    [Authorize(Roles = "Employee")]
    public class IndexModel : PageModel
    {
        private HttpClient client = new HttpClient();
        private string CategoryApiUrl;
        [FromQuery(Name = "page")] public int Page { get; set; } = 1;
        [FromQuery(Name = "txtSearch")] public string Search { get; set; } = "";
        [FromQuery(Name = "categoryId")] public int CatId { get; set; } = 0;
        public List<String> PagesLink { get; set; } = new List<string>();
        [BindProperty]
        [Required(ErrorMessage = "File is required")]
        public IFormFile FileUpload { get; set; }

        public List<Product> Products;
        public List<Category> Categories;
        private int perPage = 10;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly IHubContext<HubServer> hubContext;

        public IndexModel( IWebHostEnvironment environment, IHubContext<HubServer> hubContext)
        {
            this._hostingEnvironment = environment;
            this.hubContext          = hubContext;
        }

        public async Task Load(bool paging = true)
        {
            if (this.Search == null) this.Search = "";
            
            var response = await this.client.GetAsync("https://localhost:5000/api/Category/GetAll");
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            

            this.Categories = JsonSerializer.Deserialize<List<Category>>(data, options);
            
            response = await this.client.GetAsync("https://localhost:5000/api/Product/GetAll");
            data     = await response.Content.ReadAsStringAsync();
            
            var products = JsonSerializer.Deserialize<List<Product>>(data, options);
            
            var queryRaw = products
                .Where(p => p.DeletedAt == null)
               .Where(p => p.ProductName.Contains(this.Search));

            if (this.CatId != 0)
            {
                queryRaw = queryRaw.Where(p => p.CategoryId == this.CatId);
            }


            if (paging)
            {
                queryRaw = queryRaw.Skip((this.Page - 1) * this.perPage).Take(this.perPage);
            }

            this.Products = queryRaw.ToList();

            PageLink page  = new PageLink(this.perPage);
            String   param = "categoryId=" + this.CatId + "&txtSearch=" + this.Search;
            this.PagesLink = page.getLink(this.Page, queryRaw.Count(), "/Admin/Product/Index?" + param + "&");
        }

        public async Task<IActionResult> OnGetAsync()
        {

            await this.Load();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            await this.Load();

            return this.Page();
        }
    }
}

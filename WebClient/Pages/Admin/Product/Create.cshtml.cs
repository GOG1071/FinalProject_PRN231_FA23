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
    public class CreateModel : PageModel
    {
        private HttpClient client = new HttpClient();
        [BindProperty] public Product Product { get; set; }
        public List<Category> Categories;
        private readonly IHubContext<HubServer> hubContext;

        public CreateModel(IHubContext<HubServer> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task LoadProduct()
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Category/GetAll");
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            this.Categories = JsonSerializer.Deserialize<List<Category>>(data, options);
        }

        public async Task<IActionResult> OnGetAsync()
        {

            await this.LoadProduct();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            await this.LoadProduct();

            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }
            
            var content = new StringContent(JsonSerializer.Serialize(this.Product), Encoding.UTF8, "application/json");
            var response = await this.client.PostAsync("https://localhost:5000/api/Product/Add", content);
            await this.hubContext.Clients.All.SendAsync("Reload");

            return this.Redirect("/Admin/Product/Index");
        }
    }
}

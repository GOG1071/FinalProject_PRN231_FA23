namespace WebClient.Pages.Admin.Order
{
    using System.Data.SqlTypes;
    using System.Text;
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using WebClient.Helpers;

    [Authorize(Roles = "Employee")]
    public class IndexModel : PageModel
    {
        private HttpClient client = new HttpClient();
        [FromQuery(Name = "page")] public int Page { get; set; } = 1;
        [BindProperty] public List<Order> Orders { get; set; }
        public List<String> PagesLink { get; set; } = new List<string>();

        [FromQuery(Name = "txtStartOrderDate")] public DateTime StartDate { get; set; }
        [FromQuery(Name = "txtEndOrderDate")] public DateTime EndDate { get; set; }
        private int perPage = 10;

        private SqlDateTime? validateDateTime(DateTime time)
        {
            DateTime Min = (DateTime)SqlDateTime.MinValue;
            DateTime Max = (DateTime)SqlDateTime.MaxValue;
            

            if (time >= Min && time <= Max)
            {
                return SqlDateTime.Parse(time.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            return null;
        }

        public async Task LoadData(bool paging = true)
        {
            
            var result = await this.client.GetAsync("https://localhost:5000/api/Order/GetAll");
            var data   = await result.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var orders = JsonSerializer.Deserialize<List<Order>>(data, options);

            SqlDateTime? sqlStartDate = this.validateDateTime(this.StartDate);
            SqlDateTime? sqlEndDate   = this.validateDateTime(this.EndDate);


            if (sqlStartDate != null)
            {
                orders = orders.Where(q => (DateTime)q.OrderDate >= this.StartDate).ToList();
            }

            if (sqlEndDate != null)
            {
                orders = orders.Where(q => (DateTime)q.OrderDate <= this.EndDate).ToList();
            }
            

            if (paging)
                orders = orders.Skip((this.Page - 1) * this.perPage).Take(this.perPage).ToList();

            this.Orders = orders;

            this.ViewData["StartDate"] = sqlStartDate != null ? this.StartDate.Date.ToString("yyyy-MM-dd") : "";
            this.ViewData["EndDate"]   = sqlEndDate != null ? this.EndDate.Date.ToString("yyyy-MM-dd") : "";

            PageLink page = new PageLink(this.perPage);
            String param = (!(this.ViewData["StartDate"].Equals("") && this.ViewData["EndDate"].Equals("")) 
                ? "txtStartOrderDate=" + this.ViewData["StartDate"] 
                + "&txtEndOrderDate=" + this.ViewData["EndDate"] : "") + "&";
            this.PagesLink = page.getLink(this.Page, orders.Count, "/Admin/Order/Index?" + param);
        }


        public async Task<IActionResult> OnGetAsync()
        {

            await this.LoadData();

            return this.Page();
        }

        public async Task<IActionResult> OnGetCancel(int? id, string target)
        {
            var response = await this.client.GetAsync("https://localhost:5000/api/Order/Get/" + id);
            var data     = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var order = JsonSerializer.Deserialize<Order>(data, options);

            await this.LoadData();

            if (order == null)
            {
                return this.Redirect(target);
            }
            order.RequiredDate = null;
            
            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            await this.client.PutAsync("https://localhost:5000/api/Order/Add/", content);
            
            return this.Redirect(target);
        }

        string format(DateTime? date)
        {
            if (date == null)
                return "";
            return ((DateTime)date).Date.ToString("dd-MM-yyyy");
        }

        string getStatus(DateTime? requiredDate, DateTime? shippedDate)
        {
            if (shippedDate != null)
            {
                return "Completed";
            }

            if (requiredDate != null)
            {
                return "Pending";
            }

            return "Order canceled";
        }
    }
}

namespace WebClient.Pages.Admin.Dashboard
{
    using System.Text.Json;
    using BusinessObject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [Authorize(Roles = "Employee")]
    public class IndexModel : PageModel
    {

        private HttpClient client = new HttpClient();
        [FromQuery(Name = "txtDate")] public DateTime? Date { get; set; }

        public decimal TotalSales;
        public decimal WeeklySales;
        public decimal?[] StatisticOrders = {null, null, null, null, null, null, null, null, null, null, null, null};

        public int TotalCus;
        public float TotalCusMonth;

        public async Task<IActionResult> OnGetAsync()
        {

            var result = await this.client.GetAsync("https://localhost:5000/api/Order/GetAll");
            var data   = await result.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var Orders = JsonSerializer.Deserialize<List<Order>>(data, options);

            this.TotalSales = (decimal)Orders.Sum(x => x.Freight);

            var now = this.Date != null ? (DateTime)this.Date : DateTime.Now;
            this.ViewData["Date"] = now.Date.ToString("yyyy-MM-dd");
            var sub = now.AddDays(-7);
            var OrdersWeek = Orders.Where(x => x.OrderDate >= sub
                                               && x.OrderDate <= now).ToList();

            this.WeeklySales = (decimal)OrdersWeek.Sum(x => x.Freight)!;

            var OrderYear = Orders.Where(o => o.OrderDate.Value.Year == now.Year).ToList();

            for(int i = 0; i < now.Month; i++)
            {
                var OrdersMonth = OrderYear.Where(o => o.OrderDate.Value.Month == i + 1).ToList();
                this.StatisticOrders[i] = (decimal) OrdersMonth.Sum(x => x.Freight);
            }
            
            var response     = await this.client.GetAsync("https://localhost:5000/api/Customer/GetAll");
            var dataCustomer = await response.Content.ReadAsStringAsync();
            var optionsCustomer = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var customers = JsonSerializer.Deserialize<List<Customer>>(dataCustomer, optionsCustomer);
            this.TotalCus   = customers.Count();
            this.TotalCusMonth = customers.Where(x => x.CreatedAt != null && x.CreatedAt.Value.Year == now.Year).ToList().Count() / now.Month;

            return this.Page();
        }
    }
}

using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Customer : IModel
    {
        public Customer() { Orders = new HashSet<Order>(); }

        public string    CustomerId   { get; set; } = null!;
        public string    CompanyName  { get; set; } = null!;
        public string?   ContactName  { get; set; }
        public string?   ContactTitle { get; set; }
        public string?   Address      { get; set; }
        public DateTime? CreatedAt    { get; set; }
        public bool      Active       { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public object[]? PrimaryKey => new object[] { this.CustomerId };
    }
}
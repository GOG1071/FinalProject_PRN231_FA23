using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Account : IModel
    {
        public int       AccountId  { get; set; }
        public string?   Email      { get; set; }
        public string?   Password   { get; set; }
        public string?   CustomerId { get; set; }
        public int?      EmployeeId { get; set; }
        public int?      Role       { get; set; }
        
        public object[]? PrimaryKey => new object[] { this.AccountId };
    }
}

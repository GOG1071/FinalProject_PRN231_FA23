﻿using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class Product : IModel
    {
        public Product() { OrderDetails = new HashSet<OrderDetail>(); }

        public int       ProductId       { get; set; }
        public string    ProductName     { get; set; } = null!;
        public int?      CategoryId      { get; set; }
        public string?   QuantityPerUnit { get; set; }
        public decimal?  UnitPrice       { get; set; }
        public short?    UnitsInStock    { get; set; }
        public short?    UnitsOnOrder    { get; set; }
        public short?    ReorderLevel    { get; set; }
        public bool      Discontinued    { get; set; }
        public DateTime? DeletedAt       { get; set; }
        
        public virtual Category?                Category     { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public object[]? PrimaryKey => new object[] { this.ProductId };
    }
}
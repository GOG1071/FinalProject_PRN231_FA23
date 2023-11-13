using System;
using System.Collections.Generic;

namespace BusinessObject.Models
{
    public partial class OrderDetail : IModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        public virtual Order     Order      { get; set; } = null!;
        public virtual Product   Product    { get; set; } = null!;
        
        public         object[]? PrimaryKey => new object[] { this.OrderId, this.ProductId };
    }
}

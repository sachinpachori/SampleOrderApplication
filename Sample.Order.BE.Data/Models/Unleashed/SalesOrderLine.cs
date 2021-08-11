using System;

namespace Sample.Order.BE.Data.Models.Unleashed
{
    public class SalesOrderLine
    {
        public int LineNumber { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTax { get; set; }

        public decimal LineTotal { get; set; }

        public decimal OrderQuantity { get; set; }

        public Product Product { get; set; }
    }
}

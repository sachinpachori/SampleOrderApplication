using System;
using System.Text;

namespace Sample.Order.BE.Data.Models.Unleashed
{
    public class SalesOrder
    {
        public Guid Guid { get; set; }

        public Customer Customer { get; set; }
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderStatus { get; set; }

        public string CustomerRef { get; set; }

        public string Comments { get; set; }

        public decimal TaxRate { get; set; }
        public SalesOrderLine[] SalesOrderLines { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Total { get; set; }

        public string DeliveryInstruction { get; set; }

        public string DeliveryName { get; set; }

        public string DeliveryStreetAddress { get; set; }

        public string DeliveryStreetAddress2 { get; set; }

        public string DeliverySuburb { get; set; }

        public string DeliveryCity { get; set; }

        public string DeliveryRegion { get; set; }

        public string DeliveryCountry { get; set; }

        public string DeliveryPostCode { get; set; }

        public DateTime RequiredDate { get; set; }

        public Currency Currency { get; set; }

        public string SourceId { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public SalesPerson SalesPerson { get; set; }

        public ErrorItem[] Items { get; set; }
    }

    public class ErrorItem
    {
        public string Field { get; set; }
        public string Description { get; set; }
    }
}

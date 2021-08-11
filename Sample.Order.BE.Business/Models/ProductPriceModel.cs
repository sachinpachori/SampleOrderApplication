namespace Sample.Order.BE.Business.Models
{
    public class ProductPriceModel
    {
        public string Name { get; set; }

        public int BaseUnitQty { get; set; }

        public CurrencyValueModel Price { get; set; }

        //AU/NZ only
        public CurrencyValueModel PreviousPrice { get; set; }
    }
}

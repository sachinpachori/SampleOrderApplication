using System.Collections.Generic;

namespace Sample.Order.BE.Business.Models
{
    public class DeliveryInformationResponseModel
    {
        public string BodyText1 { get; set; }

        public DeliveryInfoLabelValueModel[] RowArray1 { get; set; }

        public string BodyText2 { get; set; }

        public DeliveryInfoLabelValueModel[] RowArray2 { get; set; }

        public string BodyText3 { get; set; }

        public string Link { get; set; }

        public string LinkText { get; set; }
    }
}

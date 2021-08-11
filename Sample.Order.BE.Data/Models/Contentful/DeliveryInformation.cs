using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Order.BE.Data.Models.Contentful
{
    public class DeliveryInformation
    {
        public SystemProperties Sys { get; set; }

        public string BodyText1 { get; set; }

        public DeliveryInformationKeyValuePair[] RowArray1 { get; set; }

        public string BodyText2 { get; set; }

        public DeliveryInformationKeyValuePair[] RowArray2 { get; set; }

        public string BodyText3 { get; set; }

        public string Link { get; set; }

        public string LinkText { get; set; }
    }
}

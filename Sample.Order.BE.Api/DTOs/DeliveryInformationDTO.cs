using System.Collections.Generic;

namespace Sample.Order.BE.Api.DTOs
{
    public class DeliveryInformationDTO
    {
        /// <summary>
        /// The delivery information first body text
        /// </summary>
        public string BodyText1 { get; set; }

        /// <summary>
        ///First array of labels and values
        /// </summary>
        public DeliveryInfoLabelValueDTO[] RowArray1 { get; set; }

        /// <summary>
        ///Optional second body text
        /// </summary>
        public string BodyText2 { get; set; }

        /// <summary>
        ///Second array of labels and values 
        /// </summary>
        public DeliveryInfoLabelValueDTO[] RowArray2 { get; set; }

        /// <summary>
        /// Optional third body text
        /// </summary>
        public string BodyText3 { get; set; }

        /// <summary>
        /// The Link URL
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// The Link Text
        /// </summary>
        public string LinkText { get; set; }
    }
}
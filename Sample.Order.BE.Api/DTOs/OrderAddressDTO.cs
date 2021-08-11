using Newtonsoft.Json;

namespace Sample.Order.BE.Api.DTOs
{
    public class OrderAddressDTO
    {
        /// <summary>
        /// The address name
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// The address line 1
        /// </summary>
        [JsonProperty("line1")]
        public string Line1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("line2")]
        public string Line2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("suburb")]
        public string Suburb { get; set; }

        /// <summary>
        /// The address city
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The address region
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        /// The address postcode
        /// </summary>
        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        /// <summary>
        /// The address country
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// The delivery notes for order
        /// </summary>
        public string DeliveryNote { get; set; }

    }
}
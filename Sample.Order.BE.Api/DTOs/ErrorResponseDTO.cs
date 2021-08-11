using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sample.Order.BE.Api.DTOs
{
    /// <summary>
    /// DTO object for errors
    /// </summary>
    public class ErrorResponseDTO
    {
        /// <summary>
        /// The error code
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Boolean representing if the error should be displayed on screen
        /// </summary>
        [JsonProperty("isTargetUx")]
        public bool IsTargetUx { get; set; }

        /// <summary>
        /// List of input error objects
        /// </summary>
        [JsonProperty("inputErrors")]
        public List<InputErrorResponseDTO> InputErrors { get; set; }
    }

    public class InputErrorResponseDTO
    {
        /// <summary>
        /// The name of the input field
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The error code 
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

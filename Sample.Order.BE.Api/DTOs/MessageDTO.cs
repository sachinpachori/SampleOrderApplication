namespace Sample.Order.BE.Api.DTOs
{
    public class MessageDTO
    {
        /// <summary>
        /// Type of message information, warning, error
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Text { get; set; }
    }
}
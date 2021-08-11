using Sample.Order.BE.Business.Enums;

namespace Sample.Order.BE.Business.Models
{
    public class MessageModel
    {
        public MessageTypeEnum MessageType { get; set; }

        public string Text { get; set; }
    }
}

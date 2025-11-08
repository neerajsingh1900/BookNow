using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookNow.Application.DTOs.CustomerDTOs.PaymentDTOs
{
    public class RazorpayWebhookDTO
    {
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty; 

        [JsonPropertyName("payload")]
        public RazorpayPayload Payload { get; set; } = new RazorpayPayload();
    }

    public class RazorpayPayload
    {
        [JsonPropertyName("payment")]
        public RazorpayPayment Payment { get; set; } = new RazorpayPayment();
    }

    public class RazorpayPayment
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty; 

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; } = string.Empty; 

        [JsonPropertyName("notes")]
      
        public Dictionary<string, string> Notes { get; set; } = new Dictionary<string, string>();
    }
}
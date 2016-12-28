using System;
using Newtonsoft.Json;

namespace DocumentPublishChallenge.Service.Models
{
    public class SnsPayload
    {
        [JsonProperty(PropertyName = "user_email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "uploaded_at")]
        public string UplodedAt { get; } = DateTime.UtcNow.ToString("o");

        [JsonProperty(PropertyName = "uploaded_document_id")]
        public string DocumentId { get; set; }

        [JsonProperty(PropertyName = "uploaded_document_url")]
        public string DocumentUrl { get; set; }
    }
}
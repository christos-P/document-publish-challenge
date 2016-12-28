using System;
using Newtonsoft.Json;

namespace DocumentPublishChallenge.Domain
{
    /// <summary>
    /// The payload that Legacy API expects from us.
    /// At the creation of the DocumentPayload the contents
    /// of the file care converted to Base64 string as this is a prerequisite 
    /// of the Legacy API. 
    /// </summary>
    public class DocumentPayload
    {
        [JsonProperty(PropertyName = "content_type")]
        public string ContentType { get; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; }

        public DocumentPayload(string contentType, byte[] contentAsByteArray)
        {
            ContentType = contentType;
            Content = Convert.ToBase64String(contentAsByteArray);
        }
    }
}
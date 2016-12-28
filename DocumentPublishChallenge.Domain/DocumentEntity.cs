using System;

namespace DocumentPublishChallenge.Domain
{
    public class DocumentEntity
    {
        public Guid Id { get; set; }
        public string Link { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
    }
}
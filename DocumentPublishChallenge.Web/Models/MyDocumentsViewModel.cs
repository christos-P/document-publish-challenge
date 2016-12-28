using System.Collections.Generic;
using DocumentPublishChallenge.Domain;

namespace DocumentPublishChallenge.Web.Models
{
    public class MyDocumentsViewModel
    {
        public IEnumerable<UserDocument> Documents { get; set; }
    }
}
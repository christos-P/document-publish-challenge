using System;
using static System.Configuration.ConfigurationManager;

namespace DocumentPublishChallenge.Web.Code
{
    public enum ServiceEndpoint
    {
        DocumentUpload,
        DocumentRetrieve
    }

    public class ServiceUrlFactory
    {
        private static readonly Uri BaseUri = new Uri(AppSettings["DocumentPublishChallenge.Service.BasePath"]);

        public static Uri Create(ServiceEndpoint endpoint)
        {
            switch (endpoint)
            {
                case ServiceEndpoint.DocumentUpload:
                    return new Uri(BaseUri, AppSettings["DocumentPublishChallenge.Service.UploadFilePath"]);
                case ServiceEndpoint.DocumentRetrieve:
                    return new Uri(BaseUri, AppSettings["DocumentPublishChallenge.Service.RetrieveFilePath"]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null);
            }
        }
    }
}
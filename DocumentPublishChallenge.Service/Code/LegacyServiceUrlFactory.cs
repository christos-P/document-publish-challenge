using System;
using static System.Configuration.ConfigurationManager;

namespace DocumentPublishChallenge.Service.Code
{
    public enum LegacyServiceEndpoint
    {
        Upload,
        Retrieve
    }

    public class LegacyServiceUrlFactory
    {
        private static readonly Uri BaseUri = new Uri(AppSettings["legacyServiceUrl"]);

        public static Uri Create(LegacyServiceEndpoint endpoint)
        {
            switch (endpoint)
            {
                case LegacyServiceEndpoint.Upload:
                    return new Uri(BaseUri, AppSettings["legacyServiceUrl.Upload"]);
                case LegacyServiceEndpoint.Retrieve:
                    return new Uri(BaseUri, AppSettings["legacyServiceUrl.Retrieve"]);
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null);
            }
        }
    }
}
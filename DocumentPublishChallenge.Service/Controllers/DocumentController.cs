using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Amazon;
using Amazon.SimpleNotificationService.Model;
using DocumentPublishChallenge.DataAccessLayer;
using DocumentPublishChallenge.Domain;
using DocumentPublishChallenge.Service.Code;
using DocumentPublishChallenge.Service.Models;
using Newtonsoft.Json;
using static System.Configuration.ConfigurationManager;

namespace DocumentPublishChallenge.Service.Controllers
{
    public class DocumentController : ApiController
    {
        /// <summary>
        /// The Upload Endpoint 
        /// </summary>
        [HttpPost]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Not Mime Multipart Content");
            }
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var file = provider.Contents.FirstOrDefault();
            if (file == null)
            {
                return BadRequest("The request doesn't contains any file");
            }
            var contentType = file.Headers.ContentType.MediaType;
            var content = await file.ReadAsByteArrayAsync();
            var documentPayload = new DocumentPayload(contentType, content);
            var uploadResponse = await InternalUpload(documentPayload);
            if (uploadResponse.Response.StatusCode != HttpStatusCode.OK)
            {
                return uploadResponse;
            }
            var uploadedFile = ((ObjectContent) uploadResponse.Response.Content).Value as UploadFileResponse;
            if (uploadedFile != null)
            {
                var publishMessageTpAwsTask = PublishMessageToAws(uploadedFile);
                var insertDocumentTask = new DocumentContext().InsertDocument(
                    new DocumentEntity
                    {
                        Id = new Guid(uploadedFile.Id),
                        Link = uploadedFile.Url,
                        Name = file.Headers.ContentDisposition.FileName,
                        UserId = 1 // Normally this would be the user id
                    });
                await Task.WhenAll(publishMessageTpAwsTask, insertDocumentTask);
            }
            return Ok("File Uploaded");
        }

        /// <summary>
        /// The GetUserDocuments Endpoint.
        /// </summary>
        /// <returns>It returns the documents that the user uploaded.</returns>
        [HttpGet]
        public async Task<IHttpActionResult> GetUserDocuments()
        {
            var documentContext = new DocumentContext();
            // Normally we should pass user id to retrieve his documents.
            var documents = await documentContext.GetUserDocuments(1);
            var documentCreationDateFormat = AppSettings["DocumentCreationDateFormat"];
            var formattedDocuments = documents.Select(document => new
            {
                id = document.Id.ToString("N"),
                name = document.Name,
                link = document.Link,
                created = document.Created.ToString(documentCreationDateFormat)
            });
            return Ok(JsonConvert.SerializeObject(formattedDocuments));
        }

        [HttpGet]
        public async Task<IHttpActionResult> Retrieve(string id)
        {
            var uri = $"{LegacyServiceUrlFactory.Create(LegacyServiceEndpoint.Retrieve)}/{id}";
            var client = new HttpClient();
            var legacyResponse = await client.GetAsync(uri);
            Func<string, string> okFuncContent = content => content;
            return await HandleLegacyResponse(legacyResponse, okFuncContent);
        }

        /// <summary>
        /// Publishes a SNS message, when a user uploads a file. 
        /// </summary>
        /// <param name="uploadFileResponse"></param>
        private static async Task<PublishResponse> PublishMessageToAws(UploadFileResponse uploadFileResponse)
        {
            var accessKeyId = AppSettings["IAMUser.AccessKeyId"];
            var secretKey = AppSettings["IAMUser.SecretKey"];
            var sns = AWSClientFactory.CreateAmazonSimpleNotificationServiceClient(accessKeyId, secretKey,
                RegionEndpoint.EUWest1);
            var topicArn = AppSettings["SNS.Topic.ARN"];
            var snsPayload = new SnsPayload
            {
                Email = "cpaisios@gmail.com", // Normally here we should pass user email.
                DocumentId = uploadFileResponse.Id,
                DocumentUrl = uploadFileResponse.Url
            };
            var awsRequest = new PublishRequest(topicArn, JsonConvert.SerializeObject(snsPayload));
            return await sns.PublishAsync(awsRequest);
        }

        /// <summary>
        /// Helper method used from the Upload Endpoint in order to communicate with the corresponding Endpoint of the Legacy API.
        /// </summary>
        /// <param name="documentPayload">The document that should be uploaded.</param>
        /// <returns></returns>
        private async Task<ResponseMessageResult> InternalUpload(DocumentPayload documentPayload)
        {
            var client = new HttpClient();
            const string appJson = "application/json";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(appJson));
            var strContent = new StringContent(JsonConvert.SerializeObject(documentPayload), Encoding.UTF8, appJson);
            var uri = LegacyServiceUrlFactory.Create(LegacyServiceEndpoint.Upload);
            var legacyResponse = await client.PostAsync(uri, strContent);
            Func<string, UploadFileResponse> okFuncContent = JsonConvert.DeserializeObject<UploadFileResponse>;
            return await HandleLegacyResponse(legacyResponse, okFuncContent);
        }

        /// <summary>
        /// Helper method to handle the responses of the Legacy API.
        /// </summary>
        /// <typeparam name="T">The type of the value returned, when the response status is 200.</typeparam>
        /// <param name="legacyResponse">The legacy response.</param>
        /// <param name="okContentFunc">A function that is called, when the response status is 200 and has the value that would be returned to the caller.</param>
        /// <returns></returns>
        private async Task<ResponseMessageResult> HandleLegacyResponse<T>(HttpResponseMessage legacyResponse,
            Func<string, T> okContentFunc)
        {
            HttpResponseMessage response;
            switch (legacyResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    var legaceResponseContent = await legacyResponse.Content.ReadAsStringAsync();
                    response = Request.CreateResponse(HttpStatusCode.OK, okContentFunc(legaceResponseContent));
                    return new ResponseMessageResult(response);
                case HttpStatusCode.BadRequest:
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Legacy Error");
                    return new ResponseMessageResult(response);
                case HttpStatusCode.NotFound:
                    response = Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
                    return new ResponseMessageResult(response);
            }
            response = Request.CreateResponse(legacyResponse.StatusCode, "Unknown Error");
            return new ResponseMessageResult(response);
        }
    }
}
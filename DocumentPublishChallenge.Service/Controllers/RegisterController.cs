
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using DocumentPublishChallenge.DataAccessLayer;
using DocumentPublishChallenge.Domain;
using DocumentPublishChallenge.Service.Code;

namespace DocumentPublishChallenge.Service.Controllers
{
    public class RegisterController : ApiController
    {
        private readonly UserContext _userContext = new UserContext();

        [HttpPost]
        public async Task<IHttpActionResult> Singup(SignupModel model)
        {
            var userModel = new UserModel
            {
                Email = model.Email,
                Password = PasswordHash.HashPassword(model.Password)
            };
            var userId = await _userContext.CreateUser(userModel);
            if (userId > 0)
            {
                return Ok(userId);
            }
            const HttpStatusCode stsCode = HttpStatusCode.InternalServerError;
            const string msg = "Unknown Error";
            return new ResponseMessageResult(Request.CreateErrorResponse(stsCode, msg));
        }
    }
}

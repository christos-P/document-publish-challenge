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
    public class AccountController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Login(UserModel model)
        {
            var userContext = new UserContext();
            var storedPassword = await userContext.GetUserStoredPassword(model.Email);
            if (PasswordHash.ValidatePassword(model.Password, storedPassword))
            {
                return Ok("Login successful");
            }
            const HttpStatusCode stsCode = HttpStatusCode.Unauthorized;
            const string msg = "Wrong email or password";
            return new ResponseMessageResult(Request.CreateErrorResponse(stsCode, msg));
        }
    }
}
using LogApi.Models;
using System.Web.Http;
using static LogApi.App_Code.Utils;

namespace LogApi.Controllers
{
    /// <summary>
    /// Authentication controller
    /// </summary>
    public class AuthController : ApiController
    {
        /// <summary>
        /// Returns AuthSuccessResult object containing auth Token
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult Index()
        {
            var userData = GetUserData(User.Identity.Name);
            return userData == null ? 
                BadRequest() as IHttpActionResult : 
                Ok(new AuthSuccessResult { access_token = userData.Token });
        }
    }
}

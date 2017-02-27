using LogApi.App_Code;
using LogApi.Models;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using static LogApi.App_Code.Utils;

namespace LogApi.Controllers
{
    /// <summary>
    /// Register application controller
    /// </summary>
    public class RegisterController : ApiController
    {
        /// <summary>
        /// Data access object
        /// </summary>
        DataLayer dataLayer = new DataLayer();

        /// <summary>
        /// Adds received application to database
        /// </summary>
        /// <param name="reg">Application for registration</param>
        /// <returns>RegisterAppResult object</returns>
        [HttpPost]
        public async Task<IHttpActionResult> Index([FromBody]AppRegistration reg)
        {
            return await Task<IHttpActionResult>.Factory.StartNew(() =>
            {
                try
                {
                    var app = dataLayer.AddApplication(new application
                    {
                        display_name = reg.display_name,
                        application_id = NewGuidString(),
                        secret = NewGuidString()
                    });
                    return Ok(new RegisterAppResult {
                        display_name = app.display_name,
                        application_id = app.application_id,
                        secret = app.secret
                    });
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            });
        }
    }
}

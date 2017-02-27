using LogApi.App_Code;
using LogApi.Models;
using System.Threading.Tasks;
using System.Web.Http;

namespace LogApi.Controllers
{
    /// <summary>
    /// Log controller
    /// </summary>
    public class LogController : ApiController
    {
        /// <summary>
        /// Database access object
        /// </summary>
        DataLayer dataLayer = new DataLayer();

        /// <summary>
        /// Adds received log item
        /// </summary>
        /// <param name="item">log record</param>
        /// <returns>AddLogResult object</returns>
        [HttpPost]
        [Authorize]
        public async Task<IHttpActionResult> Index([FromBody]log item)
        {
            return await Task<IHttpActionResult>.Factory.StartNew(() =>
            {
                try
                {
                    dataLayer.AddLog(item);
                } catch
                {
                    return Ok(new AddLogResult { success = false });
                }
                return Ok(new AddLogResult { success = true });
            });
        }
    }
}

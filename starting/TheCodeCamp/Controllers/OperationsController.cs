using System;
using System.Configuration;
using System.Web.Http;

namespace TheCodeCamp.Controllers
{
    public class OperationsController : ApiController
    {
        /* Reloading configuration without
         * restarting application using
         * ConfigurationManager.RefreshSection*/
        [HttpOptions]
        [Route("api/refreshconfig")]
        public IHttpActionResult RefreshAppSettings()
        {
            try
            {
                ConfigurationManager.RefreshSection("AppSettings");
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}

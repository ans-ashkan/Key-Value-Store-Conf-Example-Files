using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AspNetRedisPersistedUserSession.Controllers
{
    [Authorize]
    public class SecuredSectionController : Controller
    {
        // GET: SecuredSection
        public ActionResult Index()
        {
            return Content("You are Authorized");
        }
    }
}
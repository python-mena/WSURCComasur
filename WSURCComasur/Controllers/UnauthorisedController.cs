using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace WSURCComasur.Areas.Steria.Controllers
{

    public class UnauthorisedController : Controller
    {
        // GET: Unauthorised
        public ActionResult Index()
        {
            Session.Abandon();
            return View();
        }
    }
}
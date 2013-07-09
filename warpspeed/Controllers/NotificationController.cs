using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using warpspeed.Models;
using warpspeed.Helpers;

namespace warpspeed.Controllers
{
    public class NotificationController : Controller
    {
        [AuthenticationHelper.IsUser]
        [HttpPost]
        public JsonResult Seen(Guid ElementID, string ElementType)
        {
            NotificationHelper nH = new NotificationHelper();
            return Json(nH.NotificationsSeen(ElementID, ElementType));
        }

    }
}

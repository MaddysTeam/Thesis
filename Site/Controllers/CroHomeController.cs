
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Res;
using Symber.Web.Data;
using Res.Business;
using System.IO;
using System.Threading.Tasks;


namespace Res.Controllers
{

   public class CroHomeController : CroBaseController
   {

      //
      // 首页
      // GET:		/Home/Index
      //
      [AllowAnonymous]
      public ActionResult Index(string type)
      {
         return RedirectToAction("Login", "Account");
      }

   }

}
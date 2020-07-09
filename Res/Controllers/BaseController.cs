using Res.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Symber.Web.Data;

namespace Res.Controllers
{

   [Authorize]
   public class BaseController : Controller
   {
      public BaseController()
      {
         db = new APDBDef();

         //CurrentActive = ResSettings.SettingsInSession.CurrentActive;
      }

      // protected Active CurrentActive { get; private set; }

      protected APDBDef db { get; set; }

      protected override void Dispose(bool disposing)
      {
         if (db != null)
            db.Dispose();
         base.Dispose(disposing);
      }

      protected override void OnException(ExceptionContext filterContext)
      {
         // 标记异常已处理
         filterContext.ExceptionHandled = true;
         // 跳转到错误页
         filterContext.Result = View("Error", filterContext.Exception); //RedirectToAction("Error //RedirectResult(Url.Action("Error", "Shared"));
      }

      /// <summary>
      /// Not Ajax call.
      /// </summary>
      /// <returns></returns>
      protected ActionResult IsNotAjax()
      {
         return Content("Is Not Ajax.");
      }


      protected void ThrowNotAjax()
      {
         if (!Request.IsAjaxRequest())
            throw new NotSupportedException("Action must be Ajax call.");
      }



      /// <summary>
      /// Intial area drop down data
      /// </summary>
      protected void InitAreaDropDownData(bool filterByuser = false)
      {
         //删除单位的缓存信息
         //ResSettings.SettingsInSession.RemoveCache(typeof(List<ResCompany>));

         var user = ResSettings.SettingsInSession.User;
         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();

         if (filterByuser)
         {
            if (user.ProvinceId > 0)
            {
               provinces = provinces.Where(x => x.CompanyId == user.ProvinceId).ToList();
            }
            if (user.AreaId > 0)
            {
               areas = areas.Where(x => x.CompanyId == user.AreaId).ToList();
            }
         }

         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.Actives = ResSettings.SettingsInSession.Actives;

         ViewBag.ProvincesDic = CrosourceController.GetStrengthDict(areas);
         ViewBag.AreasDic = CrosourceController.GetStrengthDict(areas);
      }




   }

}
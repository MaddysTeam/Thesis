using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;

namespace Res.Controllers
{

   public class ActiveController : BaseController
   {

      static APDBDef.ActiveTableDef a = APDBDef.Active;

      //
      //	项目管理 - 首页
      // GET:		/Active/Index
      //

      public ActionResult Search()
      {
         return View();
      }

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase)
      {
         var a = APDBDef.Active;
         var query = APQuery
             .select(a.ActiveId, a.ActiveName, a.Description, a.StartDate, a.EndDate)
             .from(a);

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(a.ActiveName.Match(searchPhrase));
         }

         query.primary(a.ActiveId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);
         var actives = db.Query(query, a.TolerantMap).ToList();
         var list = (from ac in actives
                     select new
                     {
                        id = ac.ActiveId,
                        name = ac.ActiveName,
                       // company = ac.Company,
                        description = ac.Description,
                        start = ac.StartDate.ToString("yyyy-MM-dd"),
                        end = ac.EndDate.ToString("yyyy-MM-dd")
                     }).ToList();


         return Json(new
         {
            rows = list,
            current,
            rowCount,
            total
         });
      }


      //
      //	项目管理 - 编辑/创建
      // GET:		/Active/Edit
      // POST:		/Active/Edit
      //

      public ActionResult Edit(long? id)
      {
         if (id == null)
         {
            return PartialView();
         }
         else
         {
            var model = APBplDef.ActiveBpl.PrimaryGet(id.Value);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
         }
      }

      [HttpPost]
      public ActionResult Edit(long? id, Active model, FormCollection fc)
      {
         ThrowNotAjax();

         if (id == null)
         {
            model.Insert();
         }
         else
         {
            model.Update();
         }

         return Json(new
         {
            status = "success",
            msg = "编辑成功！"
         });
      }


      //
      //	项目管理 - 删除
      // POST:		/Active/Delete
      //

      [HttpPost]
      public ActionResult Delete(long id)
      {
         //if (Request.IsAjaxRequest())
         //{

         //	if (APBplDef.ResUserRoleBpl.ConditionQueryCount(APDBDef.ResUserRole.RoleId == id) > 0)
         //		return Json(new { cmd = "Error", msg = "不可删除含有用户的角色。" });

         //	APBplDef.ResRoleBpl.PrimaryDelete(id);
         //	return Json(new { cmd = "Deleted", msg = "角色已删除。" });
         //}

         return IsNotAjax();
      }

   }

}
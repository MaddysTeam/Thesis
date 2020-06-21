using Microsoft.AspNet.Identity.Owin;
using Res.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Res.Controllers
{
   [Authorize]
   public class CroMyController : CroBaseController
   {
      #region [ UserManager ]

      private ApplicationSignInManager _signInManager;
      private ApplicationUserManager _userManager;

      public CroMyController()
      {
      }

      public CroMyController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
      {
         UserManager = userManager;
         SignInManager = signInManager;
      }

      public ApplicationSignInManager SignInManager
      {
         get
         {
            return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
         }
         private set
         {
            _signInManager = value;
         }
      }

      public ApplicationUserManager UserManager
      {
         get
         {
            return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
         }
         private set
         {
            _userManager = value;
         }
      }

      #endregion


      //
      // 我的信息
      // GET:		/My/Index
      //

      public ActionResult Index(long id)
      {
         var user = db.ResUserDal.PrimaryGet(id);

         return View(user);
      }


      //
      // 修改个人信息
      // GET:		/My/Edit
      // POST:		/My/Edit
      //

      public ActionResult Edit()
      {
         var model = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);

         return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
      }

      [HttpPost]
      public ActionResult Edit(ResUser model)
      {
         APBplDef.ResUserBpl.UpdatePartial(ResSettings.SettingsInSession.UserId, new { model.RealName, model.Email, model.PhotoPath });

         return RedirectToAction("Index", new { id = ResSettings.SettingsInSession.UserId, });
      }


      //平台 公告
      public ActionResult More(long id, string type, int page = 1)
      {
         var t = APDBDef.CroBulletin;
         int total;
         ViewBag.RankingBulletin = HomeCroBulltinList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
         ViewBag.Title = "公告列表";
         ViewBag.ParamType = type;
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;
         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);

      }


      //上传论文

      public ActionResult Upload(long? resid)
      {
         ResSettings.SettingsInSession.CleanCompanyCache();
         var user = ResSettings.SettingsInSession.User;
         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();

         if (user.ProvinceId > 0)
         {
            provinces = provinces.Where(x => x.CompanyId == user.ProvinceId).ToList();
         }
         if (user.AreaId > 0)
         {
            areas = areas.Where(x => x.CompanyId == user.AreaId).ToList();
         }

         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.ProvincesDic = GetStrengthDict(areas);
         ViewBag.AreasDic = GetStrengthDict(areas);
         ViewBag.ResTypes = GetStrengthDict(CroResourceHelper.ResourceType.GetItems());
         ViewBag.Themes = new List<ResPickListItem>();

         var model = resid == null ?
                       new CroResource { ProvinceId = user.ProvinceId, AreaId = user.AreaId } :
                       APBplDef.CroResourceBpl.GetResource(db, resid.Value,user.UserId);

         return View(model);
      }


      [HttpPost]
      [ValidateInput(true)]
      public ActionResult Upload(CroResource model)
      {
         if (!ModelState.IsValid)
            return Upload(model.CrosourceId);

         var user = ResSettings.SettingsInSession.User;
         var active = APBplDef.ActiveBpl.GetAll().Find(x => x.IsCurrent);

         model.StatePKID = CroResourceHelper.StateAllow;
         model.AuditedTime = DateTime.Now;
         
         model.ActiveId = active.ActiveId;

         if (model.CrosourceId > 0)
         {
            model.LastModifier = ResSettings.SettingsInSession.UserId;
            db.CroResourceDal.Update(model);
         }
         else
         {
            model.Creator = user.UserId;
            model.CreatedTime = model.LastModifiedTime = DateTime.Now;

            db.CroResourceDal.Insert(model);
         }

         return Request.IsAjaxRequest() ? Json(new
         {
            state = "ok",
            msg = "本作品审核完成。"
         }) : (ActionResult)RedirectToAction("CroMyResource", new { id = user.Id });

      }


      private string GetSafeExt(string path)
      {
         int idx = path.IndexOf('?');
         if (idx != -1)
            path = path.Substring(0, idx);
         string ext = Path.GetExtension(path);
         if (ext.Length >= 20)
            ext = "";
         return ext;
      }


      //我的论文
      public ActionResult CroMyResource(long id, int page = 1)
      {
         int total = 0;
         ViewBag.ListofResource = MyCroResource(id, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;
         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }


      public ActionResult ZcView(long id)
      {
         var user = ResSettings.SettingsInSession.User;
         var model = APBplDef.CroResourceBpl.GetResource(db, id, user.UserId);

         ViewBag.Title = model.Title;

         return View(model);

      }


      public ActionResult CroMyMedal(long id, int page = 1)
      {
         int total = 0;
         ViewBag.ListofMedals = MyMedals(id, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }

      //
      // 微课作品查看
      // GET:		/CroResource/View
      //


      public ActionResult Details(long id)
      {

         var model = APBplDef.CroResourceBpl.PrimaryGet(id);
         // model.GhostFileName = model.ResourcePath;// model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
         return View(model);
      }

      //删除微课作品

      public ActionResult Delete(long id, long resid)
      {
         APBplDef.CroResourceBpl.UpdatePartial(resid, new { StatePKID = CroResourceHelper.StateDelete });

         return RedirectToAction("CroMyResource", new { id = id });
      }


      public ActionResult NewsView(long id)
      {
         var model = APBplDef.CroBulletinBpl.PrimaryGet(id);

         return View(model);

      }


      public ActionResult Declare(long id)
      {
         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }


      //
      // 修改个人密码
      // GET:		/My/ChgPwd
      // POST:		/My/ChgPwd
      //
      public ActionResult ChgPwd(long id)
      {
         return View();
      }

      [HttpPost]
      public async Task<ActionResult> ChgPwd(ChgPwd model)
      {
         var newPassword = model.Password;
         var user = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);
         var result = await UserManager.ChangePasswordAsync(user.UserId, user.Password, newPassword);

         if (result.Succeeded)
            APBplDef.ResUserBpl.UpdatePartial(user.UserId, new { Password = newPassword });

         return RedirectToAction("Index", new { id = ResSettings.SettingsInSession.UserId, });
      }

   }

}
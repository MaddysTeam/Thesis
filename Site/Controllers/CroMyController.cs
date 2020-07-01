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

      public ActionResult Index()
      {
         var userId = ResSettings.SettingsInSession.UserId;
         var user = db.ResUserDal.PrimaryGet(userId);

         return View(user);
      }


      //
      // 修改个人信息
      // GET:		/My/Edit
      // POST:	/My/Edit
      //

      public ActionResult Edit()
      {
         InitAreaDropDownData();

         var model = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);

         return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
      }

      [HttpPost]
      [ValidateInput(true)]
      [ValidateAntiForgeryToken]
      public ActionResult Edit(ResUser model)
      {
         //if (!ModelState.IsValid)
         //{
         //   return Edit();
         //}

         var errormsg = string.Empty;
         var currentUser = ResSettings.SettingsInSession.User;
         var t = APDBDef.ResUser;

         if (model.AreaId <= 0)
         {
            errormsg = "必须选择地区！";
            ModelState.AddModelError("AreaId", errormsg);
            return !Request.IsAjaxRequest() ? Edit() : (ActionResult)Json(new { error = "error", msg = errormsg });
         }

         if (model.UserId != currentUser.UserId)
         {
            errormsg = "抱歉，您无法修改他人的信息！";
            ModelState.AddModelError("Email", errormsg);
            return !Request.IsAjaxRequest() ? Edit() : (ActionResult)Json(new { error = "error", msg = errormsg });
         }

         if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserId != currentUser.UserId & t.Phone == model.Phone) > 0)
         {
            errormsg = "该手机已被使用";
            ModelState.AddModelError("Phone", errormsg);
            return !Request.IsAjaxRequest() ? Edit() : (ActionResult)Json(new { error = "error", msg = errormsg });
         }

         if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserId != currentUser.UserId & t.Email == model.Email) > 0)
         {
            errormsg = "该邮箱已被使用";
            ModelState.AddModelError("Email", errormsg);
            return !Request.IsAjaxRequest() ? Edit() : (ActionResult)Json(new { error = "error", msg = errormsg });
         }


         APBplDef.ResUserBpl.UpdatePartial(ResSettings.SettingsInSession.UserId, new { model.RealName, model.Email, model.PhotoPath, model.Phone, model.Company, model.ProvinceId, model.AreaId, model.IDCard, model.Position });

         return !Request.IsAjaxRequest() ? RedirectToAction("Index", new { id = ResSettings.SettingsInSession.UserId, }) : (ActionResult)Json(new { error = "none", returnUrl = Url.Action("Index", "CroMy") });
      }


      //平台 公告
      public ActionResult More(string type, int page = 1)
      {
         var userId = ResSettings.SettingsInSession.UserId;

         var t = APDBDef.CroBulletin;
         int total;
         ViewBag.RankingBulletin = HomeCroBulltinList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
         ViewBag.Title = "公告列表";
         ViewBag.ParamType = type;
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;
         ResUser user = new ResUser();
         user.UserId = userId;
         return View(user);

      }


      //上传论文

      public ActionResult Upload(long? resid)
      {

         var active = ResSettings.SettingsInSession.Actives.First();

         if (active == null)
         {
            throw new ApplicationException("没有任何活动，请联系管理员");
         }

         var user = ResSettings.SettingsInSession.User;
         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();

         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.ProvincesDic = GetStrengthDict(areas);
         ViewBag.AreasDic = GetStrengthDict(areas);
         ViewBag.ResTypes = GetStrengthDict(CroResourceHelper.ResourceType.GetItems());
         ViewBag.Themes = CroResourceHelper.Theme.GetItems();


         CroResource model = null;

         if (resid > 0)
         {
            model = APBplDef.CroResourceBpl.GetResource(db, resid.Value, user.UserId);
         }

         // 如果是新增，判断是否在当前活动上传过论文
         if (model == null)
         {
            var r = APDBDef.CroResource;
            model = APBplDef.CroResourceBpl.GetActiveResource(db, active.ActiveId, user.UserId);
         }

         if (model == null)
         {
            model = new CroResource { ProvinceId = user.ProvinceId, AreaId = user.AreaId };
         }

         return View(model);
      }


      [HttpPost]
      [ValidateInput(true)]
      [ValidateAntiForgeryToken]
      public ActionResult Upload(CroResource model)
      {
         var active = ResSettings.SettingsInSession.Actives.First();
         if (!active.IsInUploadPeriod)
         {
            var errormsg = "当前不在上传论文周期内，请联系管理员！";
            ModelState.AddModelError("ActiveId", errormsg);
            return !Request.IsAjaxRequest() ? Upload(model.CrosourceId) : (ActionResult)Json(new { error = "error", msg = errormsg });
         }

         if (!ModelState.IsValid)
            return Upload(model.CrosourceId);

         var userId = ResSettings.SettingsInSession.UserId;
         var currentActive = ResSettings.SettingsInSession.Actives.First(x => x.IsCurrent);
         var existThesis = APBplDef.CroResourceBpl.GetActiveResource(db, currentActive.ActiveId, userId);

         // ensure upload thesis for once only, if model.crosourceId is been changed the original id will replace
         if (existThesis != null)
            model.CrosourceId = existThesis.CrosourceId;

         var cr = APDBDef.CroResource;

         if (model.AreaId <= 0)
         {
            var errormsg = "必须选择地区！";
            ModelState.AddModelError("AreaId", errormsg);
            return !Request.IsAjaxRequest() ? Upload(model.CrosourceId) : (ActionResult)Json(new { error = "error", msg = errormsg });
         }

         if (APBplDef.CroResourceBpl.ConditionQueryCount((cr.Creator != userId & cr.LastModifier != userId) & cr.AuthorEmail == model.AuthorEmail) > 0)
         {
            var errormsg = "作者邮箱已经使用,一个作者仅能上传一篇论文！";
            ModelState.AddModelError("AuthorEmail", errormsg);
            return !Request.IsAjaxRequest() ? Upload(model.CrosourceId) : (ActionResult)Json(new { error = "error", msg = errormsg });
         }
         else if (APBplDef.CroResourceBpl.ConditionQueryCount((cr.Creator != userId & cr.LastModifier != userId) & cr.AuthorPhone == model.AuthorPhone) > 0)
         {
            var errormsg = "作者手机号码已被使用,一个作者仅能上传一篇论文！";
            ModelState.AddModelError("AuthorPhone", errormsg);
            return !Request.IsAjaxRequest() ? Upload(model.CrosourceId) : (ActionResult)Json(new { error = "error", msg = errormsg });
         }


         model.StatePKID = CroResourceHelper.StateAllow;
         model.AuditedTime = DateTime.Now;
         model.ActiveId = currentActive.ActiveId;

         if (model.CrosourceId > 0)
         {
            model.Creator = existThesis.Creator;
            model.CreatedTime = existThesis.CreatedTime;
            model.LastModifier = ResSettings.SettingsInSession.UserId;
            db.CroResourceDal.Update(model);
         }
         else
         {
            model.Creator = userId;
            model.CreatedTime = model.LastModifiedTime = DateTime.Now;

            db.CroResourceDal.Insert(model);
         }

         return Request.IsAjaxRequest() ? Json(new
         {
            state = "ok",
            msg = "论文作品上传成功"
         }) : (ActionResult)RedirectToAction("CroMyResource", "CroMy");

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
      public ActionResult CroMyResource(int page = 1)
      {
         var userId = ResSettings.SettingsInSession.UserId;
         int total = 0;
         ViewBag.ListofResource = MyCroResource(userId, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;
         ResUser user = new ResUser();
         user.UserId = userId;
         return View(user);
      }


      public ActionResult ZcView(long id)
      {
         var user = ResSettings.SettingsInSession.User;
         var model = APBplDef.CroResourceBpl.GetResource(db, id, user.UserId);

         ViewBag.Title = model.Title;

         return View(model);

      }


      public ActionResult CroMyMedal(int page = 1)
      {
         var userId = ResSettings.SettingsInSession.UserId;

         int total = 0;
         ViewBag.ListofMedals = MyMedals(userId, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         ResUser user = new ResUser();
         user.UserId = userId;
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
      [ValidateAntiForgeryToken]
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
﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Symber.Web.Data;
using Res.Business;
using Res.Business.Utils;
using System.Collections.Generic;
using System.IO;
using Util.NPOI;

namespace Res.Controllers
{

   public class UserController : BaseController
   {

      #region [ UserManager ]

      private ApplicationSignInManager _signInManager;
      private ApplicationUserManager _userManager;

      public UserController()
      {
      }

      public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
      //	用户 - 首页
      // GET:		/User/Index
      //

      public ActionResult Index()
      {
         return View();
      }


      //
      //	用户 - 编辑
      // GET:		/User/Edit
      // POST:		/User/Edit
      //

      public ActionResult Edit(long? id)
      {
         //删除单位的缓存信息
         ResSettings.SettingsInSession.RemoveCache(typeof(List<ResCompany>));

         var user = ResSettings.SettingsInSession.User;
         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();
         var schools = ResSettings.SettingsInSession.AllSchools();

         var allRoles = ResUserHelper.UserType.GetItems();
         var roles = new List<ResPickListItem>();
         roles.AddRange(roles);

         if (user.ProvinceId > 0)
         {
            roles.Clear();
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.CityAdmin));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.SchoolAdmin));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.Export));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.RegistedUser));
            provinces = provinces.Where(x => x.CompanyId == user.ProvinceId).ToList();
         }
         if (user.AreaId > 0)
         {
            roles.Clear();
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.SchoolAdmin));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.Export));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.RegistedUser));
            areas = areas.Where(x => x.CompanyId == user.AreaId).ToList();
         }
         if (user.CompanyId > 0)
         {
            roles.Clear();
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.Export));
            roles.Add(allRoles.Find(x => x.PickListItemId == ResUserHelper.RegistedUser));
            schools = schools.Where(x => x.CompanyId == user.CompanyId).ToList();
         }

         if (user.UserTypePKID == ResUserHelper.Admin)
            roles = allRoles;


         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.Companies = schools;
         ViewBag.Roles = roles;

         //ViewBag.ProvincesDic = CrosourceController.GetStrengthDict(ResSettings.SettingsInSession.AllProvince());
         ViewBag.AreasDic = CrosourceController.GetStrengthDict(areas);
         ViewBag.SchoolsDic = CrosourceController.GetStrengthDict(schools);

         if (id == null)
         {
            return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
         }
         else
         {
            var model = APBplDef.ResUserBpl.PrimaryGet(id.Value);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
         }
      }

      [HttpPost]
      public async Task<ActionResult> Edit(ResUser model)
      {
         var t = APDBDef.ResUser;

         model.GenderPKID = ResUserHelper.GenderMale;

         if (model.UserId == 0)
         {
            if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserName == model.UserName) > 0)
            {
               return Json(new
               {
                  error = "Username",
                  msg = "登录名称已经被使用"
               });
            }

            var password = "teacher";
            model.RegisterTime = DateTime.Now;
            model.LastLoginTime = DateTime.Now;
            model.Password = password;
            model.Actived = true; //默认激活
            var result = await UserManager.CreateAsync(model, password);
            if (!result.Succeeded)
            {
               return Json(new
               {
                  error = "Signin",
                  msg = string.Join(",", result.Errors)
               });
            }
         }
         else
         {
            APBplDef.ResUserBpl.UpdatePartial(model.UserId, new
            {
               model.Email,
               model.RealName,
               model.PhotoPath,
               model.CompanyId,
               model.IDCard,
               model.UserTypePKID,
               model.ProvinceId,
               model.AreaId
            });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      //	用户 - 查询
      // GET:		/User/Search
      // POST:		/User/Search
      //

      public ActionResult Search()
      {
         InitAreaDropDownData(true); //根据用户当前角色进行数据范围筛选

         return View();
      }

      [HttpPost]
      public ActionResult Search(long provinceId, long areaId, int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         var user = ResSettings.SettingsInSession.User;

         //----------------------------------------------------------
         var t = APDBDef.ResUser;
         var c = APDBDef.ResCompany;
         APSqlOrderPhrase order = null;
         APSqlWherePhrase where = t.Removed == false;

         // 取排序
         var co = GridOrder.GetSortDef(fc);
         if (co != null)
         {
            switch (co.Id)
            {
               case "UserName": order = new APSqlOrderPhrase(t.UserName, co.Order); break;
               case "RealName": order = new APSqlOrderPhrase(t.RealName, co.Order); break;
               case "CompanyName": order = new APSqlOrderPhrase(c.CompanyName, co.Order); break;
               case "RoleName": order = new APSqlOrderPhrase(APDBDef.ResRole.RoleId, co.Order); break;
               case "Gender": order = new APSqlOrderPhrase(t.GenderPKID, co.Order); break;
               case "Email": order = new APSqlOrderPhrase(t.Email, co.Order); break;
               case "RegisterTime": order = new APSqlOrderPhrase(t.RegisterTime, co.Order); break;
               case "LoginCount": order = new APSqlOrderPhrase(t.LoginCount, co.Order); break;
               case "Actived": order = new APSqlOrderPhrase(t.Actived, co.Order); break;
            }
         }

         // 默认排序
         // if (order == null) order = new APSqlOrderPhrase(t.RegisterTime, APSqlOrderAccording.Desc);

         // 按用户名或真实姓名过滤
         if (searchPhrase != null)
         {
            searchPhrase = searchPhrase.Trim();
            if (searchPhrase != "")
               where &= (t.UserName.Match(searchPhrase) | t.RealName.Match(searchPhrase));
         }


         // 用户数据范围
         if (user.ProvinceId > 0)
            where &= t.ProvinceId == user.ProvinceId;
         if (user.AreaId > 0)
            where &= t.AreaId == user.AreaId;
         if (user.CompanyId > 0)
            where &= t.CompanyId == user.CompanyId;


         int total;
         var list = APBplDef.ResUserBpl.TolerantSearch(out total, current, rowCount, where, order);
         //----------------------------------------------------------

         if (Request.IsAjaxRequest())
         {
            return Json(
               new
               {
                  rows = from res in list
                         select new
                         {
                            id = res.UserId,
                            res.UserName,
                            res.RealName,
                            res.CompanyName,
                            res.UserType,
                            res.Gender,
                            res.Email,
                            RegisterTime = res.RegisterTime.ToString("yyyy-MM-dd"),
                            res.LoginCount,
                            Actived = res.Actived ? "有效" : "无效"
                         },
                  current = current,
                  rowCount = rowCount,
                  total = total

               });
         }
         else
         {
            return View(list);
         }
      }


      //
      //	用户 - 有效/无效
      // POST:		/User/Actived
      //

      [HttpPost]
      public ActionResult Actived(long id, bool value)
      {
         if (Request.IsAjaxRequest())
         {
            APBplDef.ResUserBpl.UpdatePartial(id, new { Actived = value });
            return Json(new { cmd = "Processed", value = value, msg = "用户是否有效设置完成。" });
         }

         return IsNotAjax();
      }


      //
      //	用户 - 授权
      // GET:		/User/Approve
      // POST:		/User/Approve
      //

      public ActionResult Approve(long id)
      {
         if (Request.IsAjaxRequest())
         {
            return PartialView(id);
         }

         return IsNotAjax();
      }

      [HttpPost]
      public ActionResult Approve(long id, long roleId)
      {
         var t = APDBDef.ResUserRole;

         if (Request.IsAjaxRequest())
         {
            var item = APBplDef.ResUserRoleBpl.ConditionQuery(t.UserId == id, null).FirstOrDefault();
            if (item == null)
            {
               new ResUserRole() { UserId = id, RoleId = roleId }.Insert();
            }
            else if (item.RoleId != roleId)
            {
               item.RoleId = roleId;
               item.Update();
            }

            return Json(new
            {
               error = "none",
               msg = "权限设置成功"
            });
         }

         return IsNotAjax();
      }


      //
      //	用户 - 详情
      // GET:		/User/Info
      //

      public ActionResult Info(long? id)
      {
         if (id == null)
            id = ResSettings.SettingsInSession.UserId;

         var model = APBplDef.ResUserBpl.PrimaryGet(id.Value);
         var company = APBplDef.ResCompanyBpl.PrimaryGet(model.CompanyId);
         if (company != null)
            model.CompanyName = company.CompanyName;

         return View(model);
      }


      [HttpPost]
      public async Task<ActionResult> ResetPwd(long? id)
      {
         if (id == null)
            id = ResSettings.SettingsInSession.UserId;

         var user = APBplDef.ResUserBpl.PrimaryGet(id.Value);

         var Token = await UserManager.GeneratePasswordResetTokenAsync(id.Value);
         var result = await UserManager.ResetPasswordAsync(id.Value, Token, ThisApp.DefaultPassword);
         APBplDef.ResUserBpl.UpdatePartial(id.Value, new { Password = ThisApp.DefaultPassword });

         if (result.Succeeded)
         {
            return Json("用户密码已经被重置为" + ThisApp.DefaultPassword);
         }
         else
         {
            return Json("error");
         }
      }

   }

}
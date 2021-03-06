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
using System.Security.Cryptography;
using System.Text;
using Util.Security;

namespace Res.Controllers
{
	public class AccountController : BaseController
	{

		#region [ UserManager ]

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
      //	用户登录
      // GET:		/Account/Login
      // POST:		/Account/Login

      [AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// 如果是旧系统密码，则也让其登录
			var md5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(model.Password))).Replace("-", "").Substring(0, 15);
			var u = APDBDef.ResUser;
			var userInfo = APBplDef.ResUserBpl.ConditionQuery(u.UserName == model.UserName & u.MD5 == md5, null).FirstOrDefault();
			if (userInfo != null)
				model.Password = ThisApp.Default_Password;


			//这不会计入到为执行帐户锁定而统计的登录失败次数中
			//若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					APBplDef.ResUserBpl.SetLastLoginTime(model.UserName);
					var user = await UserManager.FindByNameAsync(model.UserName);
					return RedirectToLocal(Url.Action("Index", "CroMy", new { user.Id }));
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "用户名或密码不正确。");
					return View(model);
			}
		}


		//
		// 用户登出
		// GET:		/Account/LogOff
		//

		//public ActionResult LogOff()
		//{
		//	AuthenticationManager.SignOut();
		//	ResSettings.SettingsInSession.ResetCurrent();
		//	return RedirectToAction("Login", "Account");
		//}

		public ActionResult LogOff2()
		{
			AuthenticationManager.SignOut();
			ResSettings.SettingsInSession.ResetCurrent();
			return RedirectToAction("Login", "Account");
		}


		//
		// 用户注册
		// GET:		/Account/Register
		// POST:		/Account/Register

		public ActionResult Register()
		{
			InitAreaDropDownData();

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(Register model)
		{
			if (!ModelState.IsValid)
			{
				return Register();
			}

			model.Username = model.Username.Trim();
			model.Password = model.Password.Trim(); ;
			model.Email = model.Email.Trim();

			var t = APDBDef.ResUser;
			if (model.AreaId <= 0)
			{
				var errormsg = "必须选择地区";
				ModelState.AddModelError("AreaId", errormsg);
				return !Request.IsAjaxRequest() ? Register() : (ActionResult)Json(new { error = "error", msg = errormsg });
			}
			if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserName == model.Username) > 0)
			{
				var errormsg = "用户名已被使用";
				ModelState.AddModelError("Username", errormsg);
				return !Request.IsAjaxRequest() ? Register() : (ActionResult)Json(new { error = "error", msg = errormsg });
			}

			if (APBplDef.ResUserBpl.ConditionQueryCount(t.Email == model.Email) > 0)
			{
				var errormsg = "该邮箱已经使用";
				ModelState.AddModelError("Email", errormsg);
				return !Request.IsAjaxRequest() ? Register() : (ActionResult)Json(new { error = "error", msg = errormsg });
			}

			if (APBplDef.ResUserBpl.ConditionQueryCount(t.IDCard == model.IdCard) > 0)
			{
				var errormsg = "该身份证件号已被使用";
				ModelState.AddModelError("IdCard", errormsg);
				return !Request.IsAjaxRequest() ? Register() : (ActionResult)Json(new { error = "error", msg = errormsg });
			}

			if (APBplDef.ResUserBpl.ConditionQueryCount(t.Phone == model.Phone) > 0)
			{
				var errormsg = "该手机号码已被使用";
				ModelState.AddModelError("Phone", errormsg);
				return !Request.IsAjaxRequest() ? Register() : (ActionResult)Json(new { error = "error", msg = errormsg });
			}


			var user = new ResUser
			{
				UserName = model.Username,
				Email = model.Email,
				Password = model.Password,
				RealName = model.RealName,
				PhotoPath = "",
				GenderPKID = ResUserHelper.GenderMale,
				ProvinceId = model.ProvinceId,
				AreaId = model.AreaId,
				Actived = true,
				Removed = false,
				Company = model.Company,
				RegisterTime = DateTime.Now,
				LastLoginTime = DateTime.Now,
				LoginCount = 1,
				UserTypePKID = ResUserHelper.Teacher,
				IDCard = model.IdCard,
				Phone = model.Phone,
				Question = model.Question,
				Answer = model.Answer,
            Position=model.Position
			};
			var result = await UserManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				APBplDef.ResUserRoleBpl.Insert(new ResUserRole() { UserId = user.UserId, RoleId = 2 });
			}

			return !Request.IsAjaxRequest() ? RedirectToAction("Login", "Account") : (ActionResult)Json(new { returnUrl = Url.Action("Login", "Account") });
		}


		public ActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgetPassword(ForgetPassword model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			string email = model.Email;

			var u = APDBDef.ResUser;
			var user = db.ResUserDal.ConditionQuery(u.Email == email, null, null, null).FirstOrDefault();

			if (user != null)
			{
				string pwdMd5 = MD5Helper.MD5Str(user.ToString());
				return RedirectToLocal(Url.Action("ChgPwd", "Account", new { id = user.Id, code = pwdMd5 }));
			}
			else
			{
				ModelState.AddModelError("error", "抱歉，信息有误无法找回你的密码");

				return View(model);
			}
		}



		public ActionResult ChgPwd(int id, string code)
		{
			var user = ResUser.PrimaryGet(id);
			if (user == null)
				return RedirectToLocal(Url.Action("Login", "Account"));

			string pwdMd5 = MD5Helper.MD5Str(user.ToString());
			if (string.Compare(code, pwdMd5, true) != 0)
			{
				return RedirectToLocal(Url.Action("Login", "Account"));
			}

			return View(new ChgPwd() { UserId = id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChgPwd(ChgPwd model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.UserId > 0 && db.ResUserDal.PrimaryGet(model.UserId) != null)
			{
				var newPassword = model.Password;
				var user = APBplDef.ResUserBpl.PrimaryGet(model.UserId);
				var result = await UserManager.ChangePasswordAsync(user.UserId, user.Password, newPassword);

				if (result.Succeeded)
					APBplDef.ResUserBpl.UpdatePartial(user.UserId, new { Password = newPassword });


				return RedirectToAction("Login", "Account");
			}
			else
			{
				ModelState.AddModelError("Password", "用户不存在");

				return View(model);
			}

		}


		#region 帮助程序
		// 用于在添加外部登录名时提供 XSRF 保护
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "CroHome");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
			   : this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}
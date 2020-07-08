using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Res.Business
{

	public partial class ResRole : ResRoleBase, IRole<long>
	{

		public long Id
		{
			get { return RoleId; }
		}


		public string Name
		{
			get { return RoleName; }
			set { RoleName = value; }
		}

	}


	public partial class ResUser : ResUserBase, IUser<long>
	{

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ResUser, long> manager)
		{
			// 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// 在此处添加自定义用户声明
			return userIdentity;
		}


		public long Id
		{
			get { return UserId; }
		}

	}


	public class LoginViewModel
	{
		[Required(ErrorMessage ="必须填写用户名")]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "必须填写密码")]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		[Display(Name = "记住我?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }
	}

	public class LocalPasswordModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "当前密码")]
		public string OldPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "新密码")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "确认新密码")]
		[Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }
	}


	public class Register
	{
		public long RealId { get; set; }

		[Required(ErrorMessage = "必须填写用户名")]
		[Display(Name = "登录名称")]
		[RegularExpression("[a-zA-Z0-9_-]{6,12}", ErrorMessage = "用户名可以包含字母和数字，长度为6至12位")]
		public string Username { get; set; }

		[Required(ErrorMessage = "必须填写密码")]
		[Display(Name = "登录密码")]
		[RegularExpression("(?=.*[0-9])(?=.*[a-zA-Z])(.{8,16})", ErrorMessage = "密码必须包含数字和字母，且长度为8至16位！")]
		public string Password { get; set; }

		[Required(ErrorMessage = "必须填写电子邮箱")]
		[Display(Name = "电子邮箱")]
		[RegularExpression(@"[A-Za-z0-9]+([_\.][A-Za-z0-9]+)*@([A-Za-z0-9\-]+\.)+[A-Za-z]{2,6}", ErrorMessage = "请填写正确的邮箱地址！")]
		public string Email { get; set; }

		[Required(ErrorMessage = "必须填写真实姓名")]
		[MaxLength(50, ErrorMessage ="姓名过长")]
      [RegularExpression(@"[\u4E00-\u9FA5A-Za-z0-9_]+", ErrorMessage = "真实姓名不能包含特殊符号！")]
      [Display(Name = "真实姓名")]
		public string RealName { get; set; }

		[Required(ErrorMessage = "必须选择省市")]
		[Display(Name = "省市")]
		public long ProvinceId { get; set; }

		[Required(ErrorMessage = "必须选择地区")]
		[Display(Name = "地区")]
		public long AreaId { get; set; }

		[Required(ErrorMessage = "必须填写单位全称")]
		[MaxLength(200, ErrorMessage = "单位全称过长")]
		[Display(Name = "单位全称")]
		public string Company { get; set; }

		[Required(ErrorMessage = "必须填写身份证件号")]
		[RegularExpression(@"[1-9]\d{5}(18|19|20|(3\d))\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]", ErrorMessage = "请输入正确的身份证号码")]
		[Display(Name = "身份证件号")]
		public string IdCard { get; set; }

		[Required(ErrorMessage = "必须填写手机号码")]
		[RegularExpression(@"[1][3,4,5,7,8][0-9]{9}", ErrorMessage = "请输入正确的手机号码")]
		[Display(Name = "手机号码")]
		public string Phone { get; set; }

      [Required(ErrorMessage = "必须填写职称或职务")]
      [MaxLength(50, ErrorMessage = "职称/职位内容过长")]
      [RegularExpression(@"[\u4E00-\u9FA5A-Za-z0-9_]+", ErrorMessage = "职称/职位不能包含特殊符号！")]
      [Display(Name = "职称/职位")]
      public string Position { get; set; }

      [Display(Name = "密保问题")]
		public string Question { get; set; }

		[Display(Name = "密保答案")]
		public String Answer { get; set; }


	}

	public class ChgPwd
	{

		[Required]
		[Display(Name = "新密码")]
		[RegularExpression("(?=.*[0-9])(?=.*[a-zA-Z])(.{8,16})", ErrorMessage = "密码必须包含数字和字母，且长度为8至16位！")]
		public string Password { get; set; }

		[Required]
		[Display(Name = "确认新密码")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }


		public long UserId { get; set; }


	}


	public class ForgetPassword
	{
		[Required(ErrorMessage = "必须填写电子邮箱")]
		[EmailAddress]
		[Display(Name = "电子邮箱")]
		public string Email { get; set; }

		[Display(Name = "密保问题")]
		public string Question { get; set; }

		[Display(Name = "密保答案")]
		public String Answer { get; set; }
	}
}
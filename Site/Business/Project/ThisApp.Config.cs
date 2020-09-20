using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static partial class ThisApp
	{

		// 应用 ID
		public const long AppId = 1;


		// 系统开发商 ID
		public const long AppUser_Designer_Id = 1;


		// Session 中缺省的缓存项时间（分钟）
		public const int CacheMinutes = 5;

		// 默认密码
		public const string Default_Password = "Win@123";

		// 默认评分星数
		public const int TotalStar = 5;

		// 后台地址
		public static string AdminSystemUrl = ConfigurationManager.AppSettings["adminSystemUrl"].ToString();

		public static string DomainCookie = "thesis";

		public static long CurrentActiveId = 1001;

   }

}
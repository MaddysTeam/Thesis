using System;
using System.Collections.Generic;
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

		// 系统默认密码
		public const string DefaultPassword = "Win@123";

		public const string DefaultPasswordHash = "AK+wLXNW7S/B7aGUl1RLIdhOKgfIuI1P+nUkU8+B6UOErgIcL0na05YtW0vrw4JzoA==";

		public const string DefaultSecurityStmap = "3f6a9a7e-3afe-4833-af34-ecdf88f6d761";

		public static string DomainCookie = "csj_Admin";

		// 当前活动id
		public static long CurrentActiveId = 1001;

		// 上海各区允许上传总数
		public const int DeliveryCountForShanghaiArea = 80;

		// 其他各省市允许上传总数
		public const int DeliveryCountForPerOtherProvince = 100;

	}

}
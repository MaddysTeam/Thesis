using System.Collections.Generic;

namespace Res.Business
{

	/// <summary>
	/// resouce delivery helper
	/// </summary>
	public static class ResDeliveryHelper
	{

		public static int GetDeliveryCount(long provinceId, long areaId, APDBDef db = null)
		{
			var session = ResSettings.SettingsInSession;
			if (session == null)
				return 0;

			var r = APDBDef.CroResource;
			Symber.Web.Data.APSqlWherePhrase where = r.DeliveryStatus == CroResourceHelper.IsDelivery;

			if (session.IsProvinceAdmin)
				where &= r.ProvinceId == provinceId;

			if (session.IsCityAdmin)
				where &= r.AreaId == areaId;

			db = db ?? new APDBDef();

			return db.CroResourceDal.ConditionQueryCount(where);
		}


		public static bool IsExccedMaxCount(long provinceId, long areaId, int allowdMaxCount, APDBDef db)
		{
			var count = GetDeliveryCount(provinceId, areaId, db);
			return count >= allowdMaxCount;
		}

		/// <summary>
		/// get allowd max resource delivery count for province and area
		/// </summary>
		/// <returns>max count</returns>
		public static int GetMaxAllowCount(long provinceId, long areaId)
		{
			Dictionary<long, int> dic = null;
			if (provinceId == ResCompanyHelper.Shanghai)
			{
				dic = new Dictionary<long, int>
				{
					{310120,31 }, //奉贤区
					{310101,12 }, //黄浦区
					{310115,53 },
					{310107,24 },
					{310117,14 },
					{310110,19 },
					{310105,15 },
					{310106,34 },
					{310112,117 },
					{310151,3 },
					{310116,29 },
					{310118,6 },
					{310113,28 },
					{310109,15 },
					{310104,9 },
					{310114,55 },
				};

				return dic[areaId];
			}
			else
			{
				dic = new Dictionary<long, int>
				{
					{0,int.MaxValue }, //管理员
					{330000,300 },//江苏
					{320000,300 }, // 浙江
					{340000,300}, // 安徽
				};

				return dic[provinceId];
			}


			//			310151  310000  NULL 崇明区
			//310101  310000  NULL 黄浦区
			//310104  310000  NULL 徐汇区
			//310105  310000  NULL 长宁区
			//310106  310000  NULL 静安区
			//310107  310000  NULL 普陀区
			//310109  310000  NULL 虹口区
			//310110  310000  NULL 杨浦区
			//310112  310000  NULL 闵行区
			//310113  310000  NULL 宝山区
			//310114  310000  NULL 嘉定区
			//310115  310000  NULL 浦东新区
			//310116  310000  NULL 金山区
			//310117  310000  NULL 松江区
			//310118  310000  NULL 青浦区
			//310120  310000  NULL 奉贤区
		}

	}

}
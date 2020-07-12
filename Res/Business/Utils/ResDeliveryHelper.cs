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
			Symber.Web.Data.APSqlWherePhrase where = null;
			db = db ?? new APDBDef();

			if (session.IsAdmin)
				where = (r.DeliveryStatus == CroResourceHelper.IsDelivery);

			else if (provinceId > 0)
				where = r.ProvinceId == provinceId;

			else if (areaId > 0)
				where = where & r.AreaId == areaId;

			return db.CroResourceDal.ConditionQueryCount(where);
		}


		public static bool IsExccedMaxCount(long provinceId, long areaId, int allowdMaxCount, APDBDef db)
		{
			var count = GetDeliveryCount(provinceId, areaId, db);
			return count > allowdMaxCount;
		}

		/// <summary>
		/// get allowd max resource delivery count for province and area
		/// </summary>
		/// <returns>max count</returns>
		public static int GetMaxAllowCount()
		{
			var session = ResSettings.SettingsInSession;
			if (session == null)
				return 0;

			int maxCount = 0;
			var user = session.User;

			if (session.IsAdmin)
				maxCount = 23 * ThisApp.DeliveryCountForPerOtherProvince + 16 * ThisApp.DeliveryCountForShanghaiArea;
			if ((session.IsProvinceAdmin || session.IsCityAdmin) && user.ProvinceId == ResCompanyHelper.Shanghai) //是上海市区级管理员，每个区允许80篇
				maxCount = ResSettings.SettingsInSession.IsCityAdmin ? ThisApp.DeliveryCountForShanghaiArea : 16 * ThisApp.DeliveryCountForShanghaiArea;
			else if (session.IsProvinceAdmin && user.ProvinceId != ResCompanyHelper.Shanghai) //是省管理员，每个省允许100篇
				maxCount = ThisApp.DeliveryCountForPerOtherProvince;

			return maxCount;
		}

	}

}
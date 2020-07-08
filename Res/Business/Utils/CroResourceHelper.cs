using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class CroResourceHelper
	{
		public static PickListAPRptColumn Domain;
		public static PickListAPRptColumn Deformity;
		public static PickListAPRptColumn LearnFrom;
		public static PickListAPRptColumn SchoolType;
		public static PickListAPRptColumn Stage;
		public static PickListAPRptColumn Grade;
		public static PickListAPRptColumn ImportSource;
		public static PickListAPRptColumn MediumType;
		public static PickListAPRptColumn ResourceType;
		public static PickListAPRptColumn Subject;
		public static PickListAPRptColumn State;
		public static PickListAPRptColumn CourseType;
		public static PickListAPRptColumn WinLevel;
		public static PickListAPRptColumn PublicState;
		public static PickListAPRptColumn DownloadState;
		public static PickListAPRptColumn Theme;

		static CroResourceHelper()
		{
			ResourceType = new PickListAPRptColumn(APDBDef.CroResource.ResourceTypePKID, ThisApp.PLKey_ResourceType);
			State = new PickListAPRptColumn(APDBDef.CroResource.StatePKID, ThisApp.PLKey_ResourceState);
			WinLevel = new PickListAPRptColumn(APDBDef.CroResource.WinLevelPKID, ThisApp.PLKey_WinLevel);
			Theme = new PickListAPRptColumn(APDBDef.CroResource.ThemeId, ThisApp.PLKey_ThemeType);
			DictDeliveryStatus = new Dictionary<long, string> {
			{1,"未报送" }, {2,"已报送" }
		 };


			DictWinLevel = new Dictionary<long, string> {
			{ WinLevelSpecial, WinLevel.GetName(WinLevelSpecial)},
			{ WinLevel1, WinLevel.GetName(WinLevel1)},
			{ WinLevel2, WinLevel.GetName(WinLevel2)},
			{ WinLevel3, WinLevel.GetName(WinLevel3)}
		 };
			DictApprove = new Dictionary<long, string> {
			{ StateAllow, State.GetName(StateAllow)},
			{ StateDeny, State.GetName(StateDeny)}
		 };
			DictThemes = new Dictionary<long, string>{
			{ThemeID1,Theme.GetName(ThemeID1) },
			{ThemeID2,Theme.GetName(ThemeID2) },
			{ThemeID3,Theme.GetName(ThemeID3) },
			{ThemeID4,Theme.GetName(ThemeID4) },
			{ThemeID5,Theme.GetName(ThemeID5) }
		 };
		}


		// 论文状态
		public static long StateWait = 10351;
		public static long StateAllow = 10352;
		public static long StateDeny = 10353;
		public static long StateDelete = 10359;


		// 作品类型
		//public static long MediumText = 10211;
		//public static long MediumImage = 10212;
		//public static long MediumVideo = 10213;
		//public static long MediumAudio = 10214;
		//public static long MediumAnimation = 10215;
		//public static long MediumMix = 10216;

		// 搜索类型
		//public static string Hot = "rmyc";
		//public static string Latest = "zxyc";

		// 作品类型
		//public static long MicroClass = 5010;
		//public static long MicroCourse = 5011;
		//public static long MicroExample = 5012;

		// 作品省份
		//public static long Zhejiang = 1312;
		//public static long Jiangsu = 1181;
		//public static long Shanghai = 1161;
		//public static long Anhui = 1425;


		// 奖项
		public static long WinLevelSpecial = 208;
		public static long WinLevel1 = 205;
		public static long WinLevel2 = 206;
		public static long WinLevel3 = 207;

		// 报送类型
		public static long CityLevelDelivery = 10457;
		public static long ProviceLevelDelivery = 10456;
		public static long IsDelivery = 2;
		public static long NotDelivery = 1;

		// 论文主题
		public static long ThemeID1 = 10468;
		public static long ThemeID2 = 10469;
		public static long ThemeID3 = 10470;
		public static long ThemeID4 = 10471;
		public static long ThemeID5 = 10472;


		// 奖项级别字典
		public static Dictionary<long, string> DictWinLevel;

		// 状态字典
		public static Dictionary<long, string> DictApprove;

		// 报送状态
		public static Dictionary<long, string> DictDeliveryStatus;

		// 论文主题
		public static Dictionary<long, string> DictThemes;
	}

}
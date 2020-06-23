using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public partial class APDalDef
	{

		/// <summary>
		/// Initialize ResPickList and ResPickListItem
		/// </summary>
		public partial class ResPickListDal : ResPickListDalBase
		{

			public override void InitData(APDBDef db)
			{
				long key, lessthen;


				#region [ 1000 < 1010 : PLKey_Gender ]

				{
					key = 1000; lessthen = 1010;

					var pk = new ResPickList(key, ThisApp.PLKey_Gender, "性别", "对性别进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] { "男", "女" },
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}


				#endregion

				#region [ 10230 < 10270 : PLKey_ResourceType ]

				{
					key = 10230; lessthen = 10270;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceType, "资源类型", "对资源类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"政策", "评估工具与方法", "评估报告", "课程方案与标准", "教学设计",
							"教学课件", "教学实录", "个别化教育计划", "康复训练设计", "康复训练课件",
							"康复训练实录", "残疾人支持服务项目介绍", "评估视频", "教学案例", "教学资源包",
							"校本教材", "辅助器具介绍", "教具学具介绍", "支持与服务个案报告", "文献",
							"康复训练案例", "个别化康复训练计划", "家庭教育"
						},
						null,
						null);
					long[] strengths = new long[]{
						10001, 10002, 10002, 10003, 10003,
						10003, 10003, 10003, 10004, 10004,
						10004, 10005, 10002, 10003, 10003,
						10003, 10005, 10005, 10004, 10001,
						10004, 10004, 10005
					};

					for (var i = 0; i < strengths.Length; i++)
					{
						items[i].StrengthenValue = strengths[i];
					}

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10350 < 10360 : PLKey_ResourceState ]

				{
					key = 10350; lessthen = 10360;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceState, "状态", "对状态进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"未审核", "审核合格", "审核不合格", "已删除"
						},
						null,
						null);
					items[items.Count - 1].PickListItemId = lessthen - 1;

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

			}

			#region [ Helper methods ]


			private void AddItemToList(List<ResPickListItem> list, long strengthenValue, long baseId, string[] items, string[] code, string defaultItem)
			{
				for (int i = 0, len = items.Length; i < len; i++)
				{
					ResPickListItem item = new ResPickListItem(items[i], strengthenValue) { PickListItemId = baseId++ };
					if (code != null)
						item.Code = code[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					list.Add(item);
				}
			}


			private List<ResPickListItem> FromArray(long strengthenValue, string[] itemNames, string[] codes, string defaultItem)
			{
				List<ResPickListItem> items = new List<ResPickListItem>();
				for (int i = 0, len = itemNames.Length; i < len; i++)
				{
					ResPickListItem item = new ResPickListItem(itemNames[i], strengthenValue);
					if (codes != null)
						item.Code = codes[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					items.Add(item);
				}

				return items;
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, null/* codes */, null/* defaultItem */);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames, string[] codes)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, codes, null/* defaultItem */);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames, string defaultItem)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, null/* codes */, defaultItem);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, List<ResPickListItem> items)
			{
				SyncInitData(db, apResAppID, data, items, 0);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, List<ResPickListItem> items, int baseInc)
			{
				List<ResPickList> res = db.ResPickListDal.ConditionQuery(APDBDef.ResPickList.InnerKey == data.InnerKey, null, 1, null);
				if (res.Count == 0)
				{
					DateTime now = DateTime.Now;

					if (data.PickListId == 0)
						throw new Exception("Has not special PickListId！This is a Obvious Mistake");
					data.CreatedTime = data.LastModifiedTime = now;
					data.Creator = data.LastModifier = ThisApp.AppUser_Designer_Id;
					db.ResPickListDal.Insert(data);

					SyncItems(db, data.PickListId, baseInc, now, items);
				}
			}


			private void SyncItems(APDBDef db, long pickListId, int baseInc, DateTime now, List<ResPickListItem> items)
			{
				long baseId = pickListId + 1 + baseInc;
				foreach (ResPickListItem item in items)
				{
					if (item.PickListItemId == 0)
						item.PickListItemId = baseId++; //db.ResObjectCurrentIdDal.GetNewCoreAppObjId();
					item.PickListId = pickListId;
					item.CreatedTime = item.LastModifiedTime = now;
					item.Creator = item.LastModifier = ThisApp.AppUser_Designer_Id;
					db.ResPickListItemDal.Insert(item);
				}
			}


			#endregion

		}

	}

}
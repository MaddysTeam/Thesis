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
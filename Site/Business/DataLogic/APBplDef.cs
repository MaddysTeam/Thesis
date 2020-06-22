using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public partial class APBplDef
	{

		#region [ ResPickListBpl & ResPickListItemBpl ]


		/// <summary>
		/// Partial implementation of ResPickListBpl
		/// </summary>
		public partial class ResPickListBpl : ResPickListBplBase
		{

			#region [ Cache ]


			public class ItemCache
			{
				private long _pickListId;
				private List<ResPickListItem> _items;
				private ResPickListItem _defaultItem;
				private Dictionary<long, ResPickListItem> _idItemDict;
				private Dictionary<string, long> _nameIdDict;

				public ItemCache(List<ResPickListItem> list)
				{
					if (list.Count > 0)
						_pickListId = list[0].PickListId;
					_items = list;
					_idItemDict = new Dictionary<long, ResPickListItem>(list.Count);
					_nameIdDict = new Dictionary<string, long>(list.Count);

					// 临时变量 ss 和 s 无实际用处，可能在预定义字典的时候冲突时，可以在调试状态下检测
					// 冲突的提示。
					List<string> ss = new List<string>();
					foreach (ResPickListItem item in list)
					{
						try
						{
							if (item.IsDefault)
								_defaultItem = item;
							_idItemDict.Add(item.PickListItemId, item);
							_nameIdDict.Add(item.Name, item.PickListItemId);
						}
						catch // (Exception ex)
						{
							ss.Add(item.Name);
						}
					}
					string s = String.Join(",", ss.ToArray());
				}
				public List<ResPickListItem> Items { get { return _items; } }
				public ResPickListItem DefaultItem { get { return _defaultItem; } }
				public Dictionary<long, ResPickListItem> IdItemDict { get { return _idItemDict; } }
				public long PKID { get { return _pickListId; } }
				public string ItemName(long pickListItemId) { return _idItemDict.ContainsKey(pickListItemId) ? _idItemDict[pickListItemId].Name : ""; }
				public long ItemId(string name) { return _nameIdDict[name]; }
			}


			public static ItemCache Cached(string innerKey)
			{
				Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;


				if (cache == null)
					ResSettings.SetCache(cache = new Dictionary<string, ItemCache>(), typeof(ResPickList));


				if (cache.ContainsKey(innerKey))
					return cache[innerKey];


				ItemCache itemCache = new ItemCache(APBplDef.ResPickListItemBpl.GetByPickListInnerKey(innerKey));
				cache[innerKey] = itemCache;


				return itemCache;
			}


			public static ItemCache Cached(long pickListId)
			{
				Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;


				if (cache == null)
					ResSettings.SetCache(cache = new Dictionary<string, ItemCache>(), typeof(ResPickList));


				foreach (var p in cache)
				{
					if (p.Value.PKID == pickListId)
						return p.Value;
				}

				return Cached(APBplDef.ResPickListBpl.PrimaryGet(pickListId).InnerKey);
			}


			/// <summary>
			/// 清除缓存
			/// </summary>
			public static void ClearCache()
			{
				ResSettings.RemoveCache(typeof(ResPickList));
			}


			/// <summary>
			/// 移除缓存
			/// </summary>
			/// <param name="innerKey"></param>
			public static void RemoveCache(string innerKey)
			{
				Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;
				if (cache != null && cache.ContainsKey(innerKey))
					cache.Remove(innerKey);
			}


			#endregion

		}


		/// <summary>
		/// Partial implementation of ResPickListBpl
		/// </summary>
		public partial class ResPickListItemBpl : ResPickListItemBplBase
		{

			/// <summary>
			/// 根据 PickList 的 InnerKey 获得所有子项
			/// </summary>
			/// <param name="innerKey"></param>
			/// <returns></returns>
			public static List<ResPickListItem> GetByPickListInnerKey(string innerKey)
			{
				var query = APQuery
				   .select(APDBDef.ResPickListItem.Asterisk)
				   .from(
					  APDBDef.ResPickListItem,
					  APDBDef.ResPickList.Join(APSqlJoinType.Inner, APDBDef.Res_PickList_Item)
					  )
				   .where(APDBDef.ResPickList.InnerKey == innerKey);


				using (APDBDef db = new APDBDef())
				{
					return APDBDef.ResPickListItem.MapList(db.ExecuteReader(query));
				}
			}

		}


		#endregion


		#region [ ResUserBpl ]


		public partial class ResUserBpl : ResUserBplBase
		{

			/// <summary>
			/// Return a list for admin UI list. 
			/// </summary>
			/// <param name="total"></param>
			/// <param name="current"></param>
			/// <param name="rowCount"></param>
			/// <param name="where"></param>
			/// <param name="order"></param>
			/// <returns></returns>
			public static List<ResUser> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
			{
				var t = APDBDef.ResUser;
				var c = APDBDef.ResCompany;
				var r = APDBDef.ResRole;
				var ur = APDBDef.ResUserRole;

				var query = APQuery
				   .select(t.UserId, t.UserName, t.RealName, t.GenderPKID, t.Email, t.RegisterTime, t.LoginCount, t.Actived, r.RoleName)
				   .from(t,
					  //c.JoinInner(t.CompanyId == c.CompanyId),
					  ur.JoinInner(t.UserId == ur.UserId),
					  r.JoinInner(r.RoleId == ur.RoleId))
				   .where(where)
				   .primary(t.UserId)
				   .skip((current - 1) * rowCount)
				   .take(rowCount);

				if (order != null)
					query.order_by(order);

				using (APDBDef db = new APDBDef())
				{
					total = db.ExecuteSizeOfSelect(query);
					return db.Query(query, reader =>
					{
						return new ResUser()
						{
							UserId = t.UserId.GetValue(reader),
							UserName = t.UserName.GetValue(reader),
							RealName = t.RealName.GetValue(reader),
							GenderPKID = t.GenderPKID.GetValue(reader),
							Email = t.Email.GetValue(reader),
							RegisterTime = t.RegisterTime.GetValue(reader),
							LoginCount = t.LoginCount.GetValue(reader),
							Actived = t.Actived.GetValue(reader),
							Company = c.CompanyName.GetValue(reader),
					   //RoleName = r.RoleName.GetValue(reader),
				   };
					}).ToList();
				}
			}


			public static void SetLastLoginTime(string username)
			{
				var t = APDBDef.ResUser;
				var query = APQuery.update(t)
				   .set(t.LastLoginTime, DateTime.Now)
				   .set(t.LoginCount, APSqlThroughExpr.Expr("LoginCount + 1"))
				   .where(t.UserName == username);
				using (var db = new APDBDef())
				{
					db.ExecuteNonQuery(query);
				}
			}

		}


		#endregion


		#region [ ResRoleApproveBpl ]


		public partial class ResRoleApproveBpl : ResRoleApproveBplBase
		{

			public static void Sync(long roleId, List<long> approveIds)
			{
				var t = APDBDef.ResRoleApprove;

				using (APDBDef db = new APDBDef())
				{

					var existIds = APQuery.select(t.ApproveId)
					   .from(t)
					   .where(t.RoleId == roleId).query(db, reader =>
					   {
						   return reader.GetInt64(0);
					   }).ToList();

					db.BeginTrans();
					try
					{
						foreach (var id in approveIds)
						{
							if (existIds.Contains(id))
							{
								existIds.Remove(id);
							}
							else
							{
								db.ResRoleApproveDal.Insert(new ResRoleApprove(0, roleId, id));
							}
						}
						if (existIds.Count > 0)
							db.ResRoleApproveDal.ConditionDelete(t.RoleId == roleId & t.ApproveId.In(existIds.ToArray()));

						db.Commit();
					}
					catch
					{
						db.Rollback();
					}
				}
			}

		}


		#endregion


		#region [ CroResource ]


		public partial class CroResourceBpl : CroResourceBplBase
		{

			/// <summary>
			/// Return a list for admin UI list. 
			/// </summary>
			/// <param name="total"></param>
			/// <param name="current"></param>
			/// <param name="rowCount"></param>
			/// <param name="where"></param>
			/// <param name="order"></param>
			/// <returns></returns>
			public static List<CroResource> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
			{
				var t = APDBDef.CroResource;
				var u = APDBDef.ResUser;

				var query = APQuery
				   .select(t.CrosourceId, t.Title, u.RealName.As("Author") //TODO:t.MediumTypePKID
				   , t.CreatedTime, t.StatePKID)
				   .from(t, u.JoinInner(t.Creator == u.UserId))
				   .where(where)
				   .primary(t.CrosourceId)
				   .skip((current - 1) * rowCount)
				   .take(rowCount);

				if (order != null)
					query.order_by(order);

				using (APDBDef db = new APDBDef())
				{
					total = db.ExecuteSizeOfSelect(query);
					return db.Query(query, t.TolerantMap).ToList();
				}
			}




			/// <summary>
			///  get complex resource object
			/// </summary>
			/// <param name="db">db</param>
			/// <param name="resourceId">resourceId</param>
			/// <returns>CroResource</returns>
			public static CroResource GetResource(APDBDef db, long resourceId, long userId)
			{
				var cr = APDBDef.CroResource;
				var f = APDBDef.Files;
				var query = APQuery.select(cr.Asterisk, f.Asterisk)
								   .from(cr,
										 f.JoinInner(cr.AttachmentId == f.FileId)
									 )
									.where(cr.CrosourceId == resourceId & (cr.Creator == userId | cr.LastModifier == userId));

				CroResource model = null;
				return query.query(db, r =>
				{
					model = new CroResource();
					cr.Fullup(r, model, false);

					model.AttachmentName = f.FileName.GetValue(r);
					model.AttachmentPath = f.FilePath.GetValue(r);

					return model;
				}).FirstOrDefault();

			}


			public static CroResource GetActiveResource(APDBDef db, long activeId, long userId)
			{
				var cr = APDBDef.CroResource;
				var f = APDBDef.Files;
				var query = APQuery.select(cr.Asterisk, f.Asterisk)
								   .from(cr,
										 f.JoinInner(cr.AttachmentId == f.FileId)
									 )
									.where(cr.ActiveId == activeId & (cr.Creator == userId | cr.LastModifier == userId));

				CroResource model = null;
				return query.query(db, r =>
				{
					model = new CroResource();
					cr.Fullup(r, model, false);

					model.AttachmentName = f.FileName.GetValue(r);
					model.AttachmentPath = f.FilePath.GetValue(r);

					return model;
				}).FirstOrDefault();

			}

		}




		#endregion


	}

}


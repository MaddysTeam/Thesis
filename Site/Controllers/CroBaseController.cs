using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

   public class CroBaseController : BaseController
   {

      //以下为活动作品

      #region [ 活动作品查询 ]

      public List<MicroCourseRanking> SearchCroResourceList(APSqlWherePhrase where, APSqlOrderPhrase order, out int total, int take, int skip = -1)
      {
         //var cr = APDBDef.CroResource;
         //var cf = APDBDef.Files;
         //var rc = APDBDef.ResCompany;


         //var query = APQuery.select(cr.CrosourceId, cr.Title, cr.Author, cr.FavoriteCount, cr.ProvinceId, cr.AreaId, cr.CompanyId,
         //   cr.AuthorCompany, cr.Description, cr.CreatedTime, rc.Path, cr.ViewCount, cr.CommentCount, cr.DownCount,//cr.FileExtName
         //   mc.CourseId, mc.CourseTitle, mc.PlayCount, cf.FilePath
         //   )
         //   .from(cr,
         //         mc.JoinInner(mc.ResourceId == cr.CrosourceId),
         //         rc.JoinInner(rc.CompanyId == cr.CompanyId),
         //         cf.JoinLeft(cf.FileId == mc.CoverId)
         //         //a.JoinInner(a.ActiveId==cr.ActiveId)
         //         )
         //   .where(cr.StatePKID == CroResourceHelper.StateAllow & cr.PublicStatePKID == CroResourceHelper.Public)
         //   //.where(cr.StatePKID == CroResourceHelper.StateAllow & cr.PublicStatePKID==CroResourceHelper.Public & cr.ProvinceId==ResCompanyHelper.ShangHai) // TODO:审核通过和公开的作品 
         //   .order_by(cr.ActiveId.Desc)
         //   .primary(cr.CrosourceId)
         //   .take(take);

         //if (where != null)
         //   query.where_and(where);

         //if (order != null)
         //   query.order_by_add(order).order_by_add(cr.CrosourceId.Asc);
         //else
         //   query.order_by_add(cr.CrosourceId.Asc);

         //if (skip != -1)
         //{
         //   query.skip(skip);
         //   total = db.ExecuteSizeOfSelect(query);
         //}
         //else
         //{
         //   total = 0;
         //}

         //return db.Query(query, reader =>
         //{
         //   var des = cr.Description.GetValue(reader);
         //   if (des.Length > 100)
         //      des = des.Substring(0, 100);
         //   return new MicroCourseRanking()
         //   {
         //      CourseId = mc.CourseId.GetValue(reader),
         //      CrosourceId = cr.CrosourceId.GetValue(reader),
         //      ResourceTitle = cr.Title.GetValue(reader),
         //      Title = mc.CourseTitle.GetValue(reader),
         //      Author = cr.Author.GetValue(reader),
         //      CoverPath = cf.FilePath.GetValue(reader),
         //      AuthorCompany = cr.AuthorCompany.GetValue(reader),
         //      CreatedTime = cr.CreatedTime.GetValue(reader),
         //      FavoriteCount = cr.FavoriteCount.GetValue(reader),
         //      CompanyPath = rc.Path.GetValue(reader),
         //      ProvinceId = cr.ProvinceId.GetValue(reader),
         //      AreaId = cr.AreaId.GetValue(reader),
         //      SchoolId = cr.CompanyId.GetValue(reader),
         //      ViewCount = cr.ViewCount.GetValue(reader),
         //      CommentCount = cr.CommentCount.GetValue(reader),
         //      DownCount = cr.DownCount.GetValue(reader),
         //      //FileExtName = cr.FileExtName.GetValue(reader),
         //      Description = des,
         //   };
         //}).ToList();

         total = 0;
         return null;
      }

      #endregion

      #region [我的论文]

      public List<CroMyResource> MyCroResource(long id, out int total, int take, int skip = 0)
      {
         var t = APDBDef.CroResource;
         var userid = id;
         var query = APQuery.select(t.CrosourceId, t.Title, t.Author, //t.CoverPath, t.FileExtName, 
             t.Description, t.CreatedTime, t.AuditOpinion, t.StatePKID,t.ActiveId)
            .from(t)
            .where(t.Creator == userid)

            .order_by(t.CreatedTime.Desc)
            .primary(t.CrosourceId)
            .take(take)
            .skip(skip);

         total = db.ExecuteSizeOfSelect(query);

         return db.Query(query, reader =>
         {
            var des = t.Description.GetValue(reader);
            if (des.Length > 100)
               des = des.Substring(0, 100);

            return new CroMyResource()
            {
               CrosourceId = t.CrosourceId.GetValue(reader),
               Title = t.Title.GetValue(reader),
               Author = t.Author.GetValue(reader),
               Description = des,
               OccurTime = t.CreatedTime.GetValue(reader),
               StatePKID = t.StatePKID.GetValue(reader),
               AuditOpinion = t.AuditOpinion.GetValue(reader),
			   IsCurrentActive=t.ActiveId.GetValue(reader)==ThisApp.CurrentActiveId
            };
         }).ToList();
      }
      #endregion

      #region [我的奖章]

      public List<CroResourceMedal> MyMedals(long id, out int total, int take, int skip = 0)
      {
         var cr = APDBDef.CroResource;
         var m = APDBDef.CroResourceMedal;
         var f = APDBDef.Files;

         var currentActive = ResSettings.SettingsInSession.Actives[0];
         var userid = id;
         var query = APQuery.select(m.ResourceMedalId, m.CreateDate,
                                   cr.CrosourceId.As("CrosourceId"), cr.Title, cr.Author, cr.WinLevelPKID, 
                                   f.FilePath)
            .from(m,
                  cr.JoinInner(m.CrosourceId == cr.CrosourceId),
                  f.JoinInner(f.FileId == m.FileId))
            .where(cr.Creator == userid & m.ActiveId == currentActive.ActiveId & cr.WinLevelPKID > 0)
            .order_by(cr.CreatedTime.Desc)
            .primary(m.ResourceMedalId)
            .take(take)
            .skip(skip);

         total = db.ExecuteSizeOfSelect(query);

         return db.Query(query, r => new CroResourceMedal
         {
            ResourceMedalId = m.ResourceMedalId.GetValue(r),
            CrosourceId = m.CrosourceId.GetValue(r, "CrosourceId"),
            Title = cr.Title.GetValue(r),
            Author = cr.Author.GetValue(r),
            FilePath = f.FilePath.GetValue(r),
            ActiveName = currentActive.ActiveName,
            CreateDate = m.CreateDate.GetValue(r),
            WinLevel = CroResourceHelper.DictWinLevel[cr.WinLevelPKID.GetValue(r)]
         }).ToList();
      }

      #endregion
 

   }

}
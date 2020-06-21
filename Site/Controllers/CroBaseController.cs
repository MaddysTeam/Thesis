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
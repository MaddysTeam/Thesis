using Res.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Res.Business
{

   #region [ ResPickList ]


   public partial class ResPickList : ResPickListBase
   {

      #region [ Constructors ]


      public ResPickList(long pickListId, string innerKey, string name, string description)
         : base(pickListId, innerKey, name, false, false, description, 0, DateTime.MinValue, 0, DateTime.MinValue)
      {
      }


      #endregion

   }


   #endregion


   #region [ ResPickListItem ]


   public partial class ResPickListItem : ResPickListItemBase
   {

      #region [ Constructors ]


      public ResPickListItem(string name)
         : base(0, 0, name, 0, "", false, 0, DateTime.MinValue, 0, DateTime.MinValue)
      {
      }


      public ResPickListItem(string name, bool isDefault)
         : base(0, 0, name, 0, "", isDefault, 0, DateTime.MinValue, 0, DateTime.MinValue)
      {
      }


      public ResPickListItem(string name, long strengthenValue)
         : base(0, 0, name, strengthenValue, "", false, 0, DateTime.MinValue, 0, DateTime.MinValue)
      {
      }


      public ResPickListItem(string name, long strengthenValue, bool isDefault)
         : base(0, 0, name, strengthenValue, "", isDefault, 0, DateTime.MinValue, 0, DateTime.MinValue)
      {
      }


      #endregion

   }


   #endregion


   #region [ ResUser ]


   public partial class ResUser : ResUserBase
   {

      #region [ Properties ]


      public string Gender { get { return ResUserHelper.Gender.GetName(GenderPKID); } }
      public string UserType { get { return ResUserHelper.UserType.GetName(UserTypePKID); } }

      public string CompanyName { get; set; }
      public string RoleName { get; set; }

      public int FavoriteCount { get; set; }
      public int DownCount { get; set; }
      public int CommentCount { get; set; }


      #endregion


   }


   #endregion



   #region [ ResCompany ]


   public partial class ResCompany : ResCompanyBase
   {

      #region [ Properties ]


      public List<ResCompany> Children { get; set; }


      #endregion

   }


   #endregion


   #region [ ResReportSummary ]


   public class ResReportSummary
   {

      public int TotalResource { get; set; }
      public long TotalResourceSize { get; set; }
      public int CreateThisWeek { get; set; }
      public int CreateThisMonth { get; set; }
      public int CreateThisYear { get; set; }

      public int TotalUser { get; set; }
      public int TotalComment { get; set; }
      public int TotalView { get; set; }
      public int TotalDownload { get; set; }
      public int TotalFavorite { get; set; }
      public int TotalStar { get; set; }

   }




   #endregion


   #region [ CroResource ]


   public partial class CroResource : CroResourceBase
   {

      #region [ Properties ]

      public string State { get { return CroResourceHelper.State.GetName(StatePKID); } }

      public string WinLevel { get { return CroResourceHelper.WinLevel.GetName(WinLevelPKID); } }

      public string Province { get { return GetCompanyName(ProvinceId); } }

      public string Area { get { return GetCompanyName(AreaId); } }

      public string Theme { get { return CroResourceHelper.Theme.GetName(ThemeId); } }

      public bool IsFirstTrailDone { get; set; }

      [Display(Name = "资源名称")]
      [Required]
      public string AttachmentName { get; set; }

      [Display(Name = "资源路径")]
      public string AttachmentPath { get; set; }

      [RegularExpression("[1](([3][0-9])|([4][5-9])|([5][0-3,5-9])|([6][5,6])|([7][0-8])|([8][0-9])|([9][1,8,9]))[0-9]{8}", ErrorMessage = "请输入正确的手机号码")]
      [Required]
      public override string AuthorPhone
      {
         get
         {
            return base.AuthorPhone;
         }
         set
         {
            base.AuthorPhone = value;
         }
      }

      [EmailAddress]
      [Required]
      public override string AuthorEmail
      {
         get
         {
            return base.AuthorEmail;
         }

         set
         {
            base.AuthorEmail = value;
         }
      }

      #endregion


      #region [ private methods ]

      private string GetCompanyName(long companyId)
      {
         var company = ResSettings.SettingsInSession.Companies.Find(x => x.CompanyId == companyId);
         return company != null ? company.CompanyName : string.Empty;
      }

      #endregion
   }


   #endregion



   #region [EvalGroup]


   public partial class EvalGroup : EvalGroupBase
   {
      public string Level { get { return EvalGroupHelper.Level.GetName(LevelPKID); } }

      public string ActiveName { get; set; }
   }

   #endregion


   #region [Indication]

   public partial class Indication : IndicationBase
   {
      public string Level { get { return IndicationHelper.Level.GetName(LevelPKID); } }

      public string ActiveName { get; set; }

      public string Type { get { return IndicationHelper.Type.GetName(TypePKID); } }

      public double EvalScore { get; set; }
   }


   #endregion


   #region [Eval]

   public partial class EvalResult : EvalResultBase
   {
      public List<EvalResultItem> Items { get; set; }

      public bool IsFirstEval => this.EvalType == EvalGroupHelper.FirstTrial;//初审

      public bool IsFinalEval => this.EvalType == EvalGroupHelper.LastTrial; //终审
   }

   #endregion


   #region [ CroBulletin ]


   public partial class CroBulletin : CroBulletinBase
   {

      #region [ Properties ]


      [Display(Name = "文件路径")]
      [Required]
      public string GhostFileName { get; set; }


      #endregion


   }


   #endregion


   #region [ ResBulletin ]


   //public partial class ResBulletin : ResBulletinBase
   //{

   //	#region [ Properties ]


   //	[Display(Name = "文件路径")]
   //	[Required]
   //	public string GhostFileName { get; set; }


   //	#endregion


   //}


   #endregion


}
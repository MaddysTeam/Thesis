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


		public string Province { get { return ResCompanyHelper.GetCompanyName(ProvinceId); } }

		public string Area { get { return ResCompanyHelper.GetCompanyName(AreaId); } }

		[Required(ErrorMessage = "必须填写电子邮箱")]
		[Display(Name = "电子邮箱")]
		[RegularExpression(@"[A-Za-z0-9]+([_\.][A-Za-z0-9]+)*@([A-Za-z0-9\-]+\.)+[A-Za-z]{2,6}", ErrorMessage = "请填写正确的邮箱地址！")]
		public override string Email
		{
			get
			{
				return base.Email;
			}

			set
			{
				base.Email = value;
			}
		}

		[Required(ErrorMessage = "必须填写手机号码")]
		[RegularExpression(@"[1][3,4,5,7,8][0-9]{9}", ErrorMessage = "请输入正确的手机号码")]
		[Display(Name = "手机号码")]
		public override string Phone
		{
			get
			{
				return base.Phone;
			}

			set
			{
				base.Phone = value;
			}
		}

		[Required(ErrorMessage ="必须选择省份")]
		[Display(Name = "省份")]
		public override long ProvinceId
		{
			get
			{
				return base.ProvinceId;
			}

			set
			{
				base.ProvinceId = value;
			}
		}

		[Required(ErrorMessage = "必须填写证件号码")]
		[RegularExpression(@"[1-9]\d{5}(18|19|20|(3\d))\d{2}((0[1-9])|(1[0-2]))(([0-2][1-9])|10|20|30|31)\d{3}[0-9Xx]", ErrorMessage = "请输入正确的身份证号码")]
		[Display(Name = "证件号")]
		public override string IDCard
		{
			get
			{
				return base.IDCard;
			}

			set
			{
				base.IDCard = value;
			}
		}

		[Required(ErrorMessage = "必须填写姓名")]
		[MaxLength(50, ErrorMessage = "姓名过长")]
      [RegularExpression(@"[\u4E00-\u9FA5A-Za-z0-9_]+", ErrorMessage = "真实姓名不能包含特殊符号！")]
      public override string RealName
		{
			get
			{
				return base.RealName;
			}

			set
			{
				base.RealName = value;
			}
		}

		[Required(ErrorMessage = "必须填写单位全称")]
		[MaxLength(200, ErrorMessage = "单位全称过长")]
		public override string Company
		{
			get
			{
				return base.Company;
			}

			set
			{
				base.Company = value;
			}
		}



		#endregion

		public override string ToString()
		{
			return this.Email + this.Password + this.UserId;
		}
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

		public string ResourceType { get { return CroResourceHelper.ResourceType.GetName(ResourceTypePKID); } }

		public string State { get { return CroResourceHelper.State.GetName(StatePKID); } }

		public string Province { get { return ResCompanyHelper.GetCompanyName(ProvinceId); } }

		public string Area { get { return ResCompanyHelper.GetCompanyName(AreaId); } }

		public string WinLevel { get { return CroResourceHelper.WinLevel.GetName(WinLevelPKID); } }

		public string Theme { get { return CroResourceHelper.Theme.GetName(ThemeId); } }

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

      [RegularExpression(@"[\u4E00-\u9FA5A-Za-z0-9_]+", ErrorMessage = "作者姓名不能包含特殊符号！")]
      public override string Author
      {
         get
         {
            return base.Author;
         }

         set
         {
            base.Author = value;
         }
      }

		[Required]
		[MaxLength(100, ErrorMessage ="论文标题太长")]
		public override string Title
		{
			get
			{
				return base.Title;
			}

			set
			{
				base.Title = value;
			}
		}

		#endregion


	}


	#endregion


	#region [ Active ]

	public partial class Active : ActiveBase
	{
		public bool IsInUploadPeriod => DateTime.UtcNow >= this.UploadStartDate && DateTime.UtcNow <= this.UploadEndDate;
	}

	#endregion

	#region [Mendals]

	public partial class CroResourceMedal
	{
		public string Author { get; set; }
		public string Title { get; set; }
		public string FilePath { get; set; }
		public string ActiveName { get; set; }
		public string WinLevel { get; set; }
	}

	#endregion

}
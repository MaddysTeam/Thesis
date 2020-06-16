using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Business
{
	public static class PKIDExtensions
	{
		public static string ShowPKID(this HtmlHelper html, string str)
		{
			return str == "" ? "全体" : str;
		}
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Business.Security
{

   public class SecurityScenario
   {

      public static class SQLInjection
      {
         public static string FilterSqlString(string param)
         {
            if (string.IsNullOrEmpty(param))
               return string.Empty;

            param = param.Trim().ToLower();
            //param = param.Replace("=", "");
            param = param.Replace("'", "");
            param = param.Replace(";", "");
            param = param.Replace(" or ", "");
            param = param.Replace(" and ", "");
            param = param.Replace("paramelect", "");
            param = param.Replace("update", "");
            param = param.Replace("inparamert", "");
            param = param.Replace("delete", "");
            param = param.Replace("declare", "");
            param = param.Replace("exec", "");
            param = param.Replace("drop", "");
            param = param.Replace("create", "");
			param = param.Replace("where", "");
            param = param.Replace("%", "");
            param = param.Replace("--", "");
            param = param.Replace("@", "");
            param = param.Replace("CR", "");
            param = param.Replace("LF", "");
            param = param.Replace(",", "");
            param = param.Replace("*", "");
            param = param.Replace(".", "");
            param = param.Replace("?", "");
            param = param.Replace("+", "");
            param = param.Replace("$", "");
            param = param.Replace("^", "");
            param = param.Replace("{", "");
            param = param.Replace("}", "");
            param = param.Replace("|", "");
            return param;
         }

      }

      public static class SpecialCharChecker
      {

         public static bool HasSpecialChar(string input)
         {
            var r = new Regex("[\\*\\.\\'\\?\\+\\$\\^\\{\\}\\|\\/]");

            return r.Matches(input).Count > 0;
         }

      }

      public static class ScriptInjection
      {
         public static string FilterScript(string input)
         {
            var str = new Regex("<script[^>]*?>.*?</script>").Replace(input, string.Empty)
               .Replace("<script>", "")
               .Replace("</script>", "")
               .Replace("<iframe>", "")
               .Replace("</iframe>", "")
               //.Replace("<", "& lt;")
               //.Replace(">", "& gt;")
               .Replace("'", "& #39;")
               .Replace("javascript:", "");

            str = Regex.Replace(str, "\\(", "& #40;");
            str = Regex.Replace(str, "\\)", "& #41;");
            str = Regex.Replace(str, "eval\\((.*)\\)", "");
            str = Regex.Replace(str, "[\\\"\\\'][\\s]*javascript:(.*)[\\\"\\\']", "\"\"");
            str = Regex.Replace(str, "\\(", "& #40;");

            return str;
         }
      }

      public static bool VerifyStrongPassword(string password)
      {
         var PasswordPattern = "(?=.*[0-9].*)(?=.*[A-Z].*)(?=.*[a-z].*).{8,20}";
         return new Regex(PasswordPattern).IsMatch(password);
      }

   }


	public class SecurityAttribute : FilterAttribute, IActionFilter
	{

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var parms = filterContext.ActionDescriptor.GetParameters();

			//avoid sql and script injection
			foreach (var item in parms)
			{
				if (item.ParameterType == typeof(string))
				{
					if (filterContext.ActionParameters.ContainsKey(item.ParameterName))
					{
						var parm = filterContext.ActionParameters[item.ParameterName] ?? string.Empty;
						parm = SecurityScenario.SQLInjection.FilterSqlString(parm.ToString());
						parm = SecurityScenario.ScriptInjection.FilterScript(parm.ToString());
						filterContext.ActionParameters[item.ParameterName] = parm;
					}
				}
			}
		}

	}


	public class AvoidScriptAttribute : ValidationAttribute
	{
		static string regex = "<script[^>]*?>.*?</script>";

		public override bool IsValid(object value)
		{
			if (value is string && Regex.Match(value.ToString(), regex).Success)
			{
				ErrorMessage = "可疑的脚本注入";
				return false;
			}

			return true;
		}

	}

}

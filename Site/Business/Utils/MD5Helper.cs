using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Res.Business
{

	public class MD5Helper
	{

		/// <summary>
		/// 这是最基础的MD5 加密
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string MD5Str(string str)
		{
			return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");


		}

	}

}
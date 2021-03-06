﻿using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public static class EvalGroupHelper
   {
      public static PickListAPRptColumn Level;

      static EvalGroupHelper()
      {
         Level = new PickListAPRptColumn(APDBDef.EvalGroup.LevelPKID, ThisApp.PLKey_Level);
      }


      // 评审级别
      public static long CityLevel = 5004;
      public static long ProvinceLevel = 5005;
      public static long UnionLevel = 5006;

      /// <summary>
      /// 初审
      /// </summary>
      public static long FirstTrial = 9999; 

      /// <summary>
      ///  评审
      /// </summary>
      public static long LastTrial = 9998;

   }

}
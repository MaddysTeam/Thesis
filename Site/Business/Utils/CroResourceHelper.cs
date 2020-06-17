using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public static class CroResourceHelper
   {
      public static PickListAPRptColumn Domain;
      public static PickListAPRptColumn Deformity;
      public static PickListAPRptColumn LearnFrom;
      public static PickListAPRptColumn SchoolType;
      public static PickListAPRptColumn Stage;
      public static PickListAPRptColumn Grade;
      public static PickListAPRptColumn ImportSource;
      public static PickListAPRptColumn MediumType;
      public static PickListAPRptColumn ResourceType;
      public static PickListAPRptColumn Subject;
      public static PickListAPRptColumn State;
      public static PickListAPRptColumn CourseType;
      public static PickListAPRptColumn WinLevel;
      public static PickListAPRptColumn PublicState;
      public static PickListAPRptColumn DownloadState;

      static CroResourceHelper()
      {
         ResourceType = new PickListAPRptColumn(APDBDef.CroResource.ResourceTypePKID, ThisApp.PLKey_ResourceType);
         WinLevel = new PickListAPRptColumn(APDBDef.CroResource.WinLevelPKID, ThisApp.PLKey_WinLevel);
      }


      // 作品状态
      public static long StateWait = 10351;
      public static long StateAllow = 10352;
      public static long StateDeny = 10353;
      public static long StateDelete = 10359;


      // 作品省份
      public static long Zhejiang = 1312;
      public static long Jiangsu = 1181;
      public static long Shanghai = 1161;
      public static long Anhui = 1425;

      // 获奖级别
      public static Dictionary<long, string> DictWinLevel = new Dictionary<long, string> {
         { 208, "特等奖"}, { 205, "一等奖"}, { 206, "二等奖"}, { 207, "三等奖"}
      };


      private static Dictionary<string, long> dictMediumType;
      public static long GetMediumType(string ext)
      {
         if (dictMediumType == null)
         {
            dictMediumType = new Dictionary<string, long>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var item in MediumType.GetItems())
            {
               foreach (var s in item.Code.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
               {
                  dictMediumType[s] = item.PickListItemId;
               }
            }
         }

         if (dictMediumType.ContainsKey(ext))
            return dictMediumType[ext];
         return 10216;
      }
   }

}
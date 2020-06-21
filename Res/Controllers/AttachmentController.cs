using Res.Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Util.ThirdParty.WangsuCloud;

namespace Res.Controllers
{

   public class AttachmentController : BaseController
   {

      static APDBDef.FilesTableDef f = APDBDef.Files;

      //
      //	文件 - 上传其他文件到 CDN
      // POST:		/Attachment/UploadResource
      //
      [HttpPost]
      public ActionResult UploadResource()
      {
         if (Request.Files.Count != 1)
            return Content("Error");

         HttpPostedFileBase hpf = Request.Files[0];
         var md5 = FileHelper.ConvertToMD5(hpf.InputStream);
         var file = Files.ConditionQuery(f.Md5 == md5, null).FirstOrDefault();
         if (file == null)
         {

            var ext = Path.GetExtension(hpf.FileName);
            var anotherName = md5 + ext;
            // upload file to CDN Server
            var uploadFile = new UploadFile { Stream = hpf.InputStream, FileName = $"2019/files/{DateTime.Today.ToString("yyyyMMdd")}/{anotherName}" };
            var result = FileUploader.SliceUpload(uploadFile);

            if (null == result || null == result.FileUrl) return Content("上传失败");

            if (ext.ToLowerInvariant() == ".doc" || ext.ToLowerInvariant() == ".docx")
            {
               Stream docStream = null;
               try
               {
                  docStream = Util.ThirdParty.Aspose.WordConverter.ConvertoPdf(hpf.InputStream);
                  var docFile = new UploadFile
                  {
                     Stream = docStream,
                     FileName = $"2019/files/{DateTime.Today.ToString("yyyyMMdd")}/{anotherName}{FileHelper.PdfExtName}"
                  };
                  var docResult = FileUploader.SliceUpload(docFile);
                  if (null == docResult || null == docResult.FileUrl || !docResult.IsSuccess) return Content("word 转pdf失败");
               }
               catch { }
               finally
               {
                  if (docStream != null)
                  {
                     docStream.Close();
                     docStream.Dispose();
                  }
               }
            }

            file = new Files { Md5 = md5, FileName = hpf.FileName, FilePath = result.FileUrl, ExtName = ext, FileSize = hpf.ContentLength };
            db.FilesDal.Insert(file);
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               fileId = file.FileId,
               name = file.FileName,
               path = file.FilePath,
               size = file.FileSize,
               ext = file.ExtName
            });
         }
         else
         {
            return Content("upload ok");
         }
      }

   }

}
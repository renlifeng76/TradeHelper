using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TradeHelper.Core;
using TradeHelper.CustomException;

namespace TradeHelper.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class UploadController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;

        public UploadController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

         [HttpPost, Route("upload")]
        public JsonResult Upload(IFormFile file)
        {

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                //NLog.LogManager.GetLogger("rlf").Debug("参数:");
                
                if (file == null)
                {
                    var files = Request.Form.Files;
                    if(files == null || files.Count <=0)
                    {
                        throw new BusinessException("参数为空！");
                    }
                    else
                    {
                        file = files[0];
                    }
                    
                }
                //if (jsonObj == null)
                //{
                //    throw new BusinessException("参数为空！");
                //}

                var fileDir = @"D:\uploadspace\image";

                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }

                //文件名称
                string strExt = Path.GetExtension(file.FileName).ToLower();
                string projectFileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + System.DateTime.Now.ToString("yyyyMMddhhmmss") + strExt;

                //上传的文件的路径
                string filePath = fileDir + $@"\{projectFileName}";
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                string strImgPath = $"http://image.rlf99.com/image/{projectFileName}";
                dictRtn.Add("FileUrl", strImgPath);

                //文件类型
                string strFileType = string.Empty;
                if (strExt.IndexOf("jpg") > -1
                || strExt.IndexOf("jpeg") > -1
                || strExt.IndexOf("png") > -1)
                {
                    strFileType = "image";
                }
                if (strExt.IndexOf("mp4") > -1
                || strExt.IndexOf("avi") > -1
                || strExt.IndexOf("mpeg") > -1)
                {
                    strFileType = "video";
                }
                if (strExt.IndexOf("mp3") > -1
                || strExt.IndexOf("wav") > -1
                || strExt.IndexOf("m4a") > -1)
                {
                    strFileType = "audio";
                }
                dictRtn.Add("FileType", strFileType);

                return dictRtn;

            });

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

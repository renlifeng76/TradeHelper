using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TradeHelper.Core;
using TradeHelper.CustomException;
using TradeHelper.DbTools;
using TradeHelper.Model;
using TradeHelper.Model.RlfConfig;
using TradeHelper.Model.RlfStock;

namespace TradeHelper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class TradeLogController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;

        public TradeLogController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("search")]
        public JsonResult Search(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string strPerPageNum = jsonObj["PerPageNum"].ToString();
                string strCurPage = jsonObj["CurPage"].ToString();

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //var model = fsql.Select<TradeLog>()
                //  .Where(t => t.Id == 1)
                //  .ToOne();

                var lstModel = fsql.Select<TradeLog>()
                  //.Where(t => t.Id == 1)
                  .Count(out var total) //总记录数量
                  .OrderBy("CompanyCode asc,AgentType asc,Id asc")
                  .Page(int.Parse(strCurPage), int.Parse(strPerPageNum)).ToList();

                dictRtn.Add("gridData", lstModel);

                dictRtn.Add("count", total);

                return dictRtn;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("uploadone")]
        public JsonResult UploadOne(IFormFile file)
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

        [HttpPost, Route("savememo")]
        public JsonResult SaveMemo(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                //保存备注
                string strId = jsonObj["Id"].ToString();
                string strMemo = jsonObj["Memo"].ToString();

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                TradeLog source = fsql.Select<TradeLog>().Where(t => t.Id == int.Parse(strId)).ToOne();

                source.Memo = strMemo;

                if (source != null)
                {
                    fsql.Update<TradeLog>().SetSource(source).UpdateColumns(a => a.Memo).ExecuteAffrows();
                }
                

                return null;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

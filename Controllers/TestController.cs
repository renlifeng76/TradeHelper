using System.Collections.Generic;
using System.Text.Json;
using FreeSqlDB.Model.RlfConfig;
using FreeSqlDB.Model.RlfStock;
using FreeSqlDB.Model.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace TradeHelper.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class TestController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;


        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("get")]
        public string Get()
        {
            string strRtn = "Get";

            //测试数据
            IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

            var model = fsql.Select<TradeLog>()
              .Where(t => t.Id == 1)
              .ToOne();

            return strRtn;
        }

        [HttpGet, Route("getTest")]
        public JsonResult GetTest([FromQuery] Dictionary<string, string> value)
        {
            //AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(jsonObj, (pjsonObj) =>
            //{
            //    Dictionary<string, object> dict = new Dictionary<string, object>();

            //    var strRtn = "" + jsonObj["a"] + jsonObj["b"];
            //    dict.Add("Content", strRtn);

            //    return dict;

            //});

            return new JsonResult(value);

        }

        [AllowAnonymous]
        [HttpPost, Route("postTest")]
        public JsonResult PostTest(JObject jsonObj)
        //public JsonResult PostTest([FromQuery] Dictionary<string, string> value)
        {
            //var jsontstr = JsonConvert.SerializeObject(jobject);
            //NLog.LogManager.GetLogger("rlf").Debug("参数:" + jobject["a"] + jobject["b"]);

            //var strRtn = "" + jsonObj["a"] + jsonObj["b"];

            //Dictionary<string, object> dict = new Dictionary<string, object>();
            //dict.Add("content",strRtn);

            //AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(jsonObj, (pjsonObj) =>
            //{
            //    Dictionary<string, object> dict = new Dictionary<string, object>();

            //    var strRtn = "" + jsonObj["a"] + jsonObj["b"];
            //    dict.Add("Content", strRtn);

            //    return dict;

            //});

            //return new JsonResult(ajaxRtnJsonData);

            //Dictionary<string, object> dict = new Dictionary<string, object>();
            //dict.Add("a","gvf");

            //_logger.LogInformation(value.ToString());

            return new JsonResult(jsonObj);

        }

        [HttpGet, Route("AppConfig")]
        public string AppConfig()
        {
            string strRtn = "Get:处理完成";

            #region //添加数据库配置

            //sqlite3
            IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfconfig", FreeSql.DataType.Sqlite);
            fsql.Insert<AppConfig>(
                new AppConfig()
                {
                    AppKey = "rlfstock",
                    AppValue = @"Data Source=D:\sqlitedb\rlfstock.db;Initial Catalog=sqlite;Integrated Security=True;Pooling=true;Max Pool Size=10",
                    DataType = (int)FreeSql.DataType.Sqlite

                }).ExecuteAffrows();

            //SqlServer
            //IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfconfig", FreeSql.DataType.SqlServer);

            //fsql.Insert<AppConfig>(
            //    new AppConfig()
            //    { 
            //        AppKey = "rlfstock",
            //        //AppValue = @"Data Source=D:\sqlitedb\rlfstock.db;Initial Catalog=sqlite;Integrated Security=True;Pooling=true;Max Pool Size=10",
            //        AppValue = @"Data Source=localhost,1433;Initial Catalog=rlfstock;User ID=sa;Password=Abc!@#123456;Connect Timeout=1200",
            //        DataType = (int)FreeSql.DataType.SqlServer

            //    }).ExecuteAffrows();

            #endregion

            return strRtn;
        }
    }
}

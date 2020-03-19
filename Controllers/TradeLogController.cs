using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using FreeSqlDB.Model.RlfStock;
using FreeSqlDB.Model.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TradeHelper.Core;
using TradeHelper.CustomException;

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

        /// <summary>
        /// 交易日志:检索
        /// </summary>
        /// <param name="jsonObj">{PerPageNum,CurPage,CompanyCode,AgentType}</param>
        /// <returns>日志数据列表</returns>
        [HttpPost, Route("search")]
        public JsonResult Search(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string PerPageNum = HandlerHelper.GetValue(jsonObj, "PerPageNum");
                string CurPage = HandlerHelper.GetValue(jsonObj, "CurPage");
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode"); 
                string AgentType = HandlerHelper.GetValue(jsonObj, "AgentType");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //where 条件
                Expression<Func<TradeLog, bool>> where = x=>true;

                //证券代码
                if(!string.IsNullOrEmpty(CompanyCode))
                {
                    where = where.And(x => x.CompanyCode == CompanyCode);
                }
                //委托方向
                if(!string.IsNullOrEmpty(AgentType))
                {
                    where = where.And(x => x.AgentType == AgentType);
                }
                //标签
                if (!string.IsNullOrEmpty(Tag))
                {
                    string[] array = Tag.Split(' ');

                    Expression<Func<TradeLog, bool>> whereSub = x => x.Tag.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(one))
                        {
                            continue;
                        }
                        whereSub = whereSub.Or(x => x.Tag.Contains(one));
                    }
                    where = where.And(whereSub);
                }

                var lstModel = fsql.Select<TradeLog>()
                  .Where(where)
                  .Count(out var total) //总记录数量
                  .OrderBy("tradetime desc")
                  .Page(int.Parse(CurPage), int.Parse(PerPageNum)).ToList();

                dictRtn.Add("gridData", lstModel);

                dictRtn.Add("count", total);

                return dictRtn;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        /// <summary>
        /// 保存分析内容
        /// </summary>
        /// <param name="jsonObj">Id,Memo</param>
        /// <returns></returns>
        [HttpPost, Route("saveanalysiscontent")]
        public JsonResult SaveAnalysisContent(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                //参数
                string Id = HandlerHelper.GetValue(jsonObj, "Id"); 
                string AnalysisContent = HandlerHelper.GetValue(jsonObj, "AnalysisContent");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                TradeLog source = fsql.Select<TradeLog>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();

                source.AnalysisContent = AnalysisContent;

                if (source != null)
                {
                    fsql.Update<TradeLog>().SetSource(source).UpdateColumns(a => a.AnalysisContent).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("saverow")]
        public JsonResult SaveRow(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {
                //参数
                string Id = HandlerHelper.GetValue(jsonObj, "Id");

                string TradeTime = HandlerHelper.GetValue(jsonObj, "TradeTime");
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string AgentType = HandlerHelper.GetValue(jsonObj, "AgentType");
                string TradeVol = HandlerHelper.GetValue(jsonObj, "TradeVol");
                string TradePriceAverage = HandlerHelper.GetValue(jsonObj, "TradePriceAverage");
                string TradePrice = HandlerHelper.GetValue(jsonObj, "TradePrice");
                string TradeMkPlace = HandlerHelper.GetValue(jsonObj, "TradeMkPlace");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                //更新/插入
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                TradeLog tradelog = null;

                if (string.IsNullOrEmpty(Id))
                {
                    tradelog = new TradeLog();
                }
                else
                {
                    tradelog = fsql.Select<TradeLog>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();
                }

                tradelog.TradeTime = Convert.ToDateTime(TradeTime, CultureInfo.CurrentCulture);
                tradelog.CompanyCode = CompanyCode;
                tradelog.CompanyName = CompanyName;
                tradelog.AgentType = AgentType;
                tradelog.TradeVol = int.Parse(TradeVol, CultureInfo.CurrentCulture);
                tradelog.TradePrice = float.Parse(TradePrice, CultureInfo.CurrentCulture);
                tradelog.TradePriceAverage = float.Parse(TradePriceAverage, CultureInfo.CurrentCulture);
                tradelog.TradeMkPlace = TradeMkPlace;
                tradelog.Tag = Tag;

                if (!string.IsNullOrEmpty(Id))
                {
                    fsql.Update<TradeLog>().SetSource(tradelog).ExecuteAffrows();
                }
                else
                {
                    fsql.Insert<TradeLog>(tradelog).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

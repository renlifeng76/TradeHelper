using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using FreeSqlDB.Model.RlfStock;
using FreeSqlDB.Model.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TradeHelper.Core;

namespace TradeHelper.Controllers
{
    /// <summary>
    /// 持仓分析
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class PositionAnalysisController : ControllerBase
    {

        private readonly ILogger<TestController> _logger;

        public PositionAnalysisController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 持仓分析:检索
        /// </summary>
        /// <param name="jsonObj">{PerPageNum,CurPage,CompanyCode,PositionStatus}</param>
        /// <returns>日志数据列表</returns>
        [HttpPost, Route("search")]
        public JsonResult Search(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string strPerPageNum = HandlerHelper.GetValue(jsonObj, "PerPageNum"); 
                string strCurPage = HandlerHelper.GetValue(jsonObj, "CurPage"); 
                string strCompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode"); 
                string strPositionStatus = HandlerHelper.GetValue(jsonObj, "PositionStatus"); 

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //where 条件
                Expression<Func<PositionAnalysis, bool>> where = x=>true;

                if(!string.IsNullOrEmpty(strCompanyCode))
                {
                    where = where.And(x => x.CompanyCode == strCompanyCode);
                }

                if(!string.IsNullOrEmpty(strPositionStatus))
                {
                    where = where.And(x => x.PositionStatus == int.Parse(strPositionStatus));
                }

                var lstModel = fsql.Select<PositionAnalysis>()
                  .Where(where)
                  .Count(out var total) //总记录数量
                  .OrderBy("PositionStartTime desc")
                  .Page(int.Parse(strCurPage), int.Parse(strPerPageNum)).ToList();

                dictRtn.Add("GridData", lstModel);

                dictRtn.Add("Count", total);

                return dictRtn;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("add")]
        public JsonResult Add(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {
                //参数
                string PositionStatus = HandlerHelper.GetValue(jsonObj, "PositionStatus"); 
                string PositionStartTime = HandlerHelper.GetValue(jsonObj, "PositionStartTime"); 
                string PositionEndTime = HandlerHelper.GetValue(jsonObj, "PositionEndTime"); 
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode"); 
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName"); 
                string PositionVol = HandlerHelper.GetValue(jsonObj, "PositionVol");
                string CostPrice = HandlerHelper.GetValue(jsonObj, "CostPrice"); 
                string CurrentPrice = HandlerHelper.GetValue(jsonObj, "CurrentPrice"); 
                string TradeMkPlace = HandlerHelper.GetValue(jsonObj, "TradeMkPlace"); 
                string AnalysisContent = HandlerHelper.GetValue(jsonObj, "AnalysisContent"); 

                //添加
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                PositionAnalysis positionAnalysis = new PositionAnalysis();

                positionAnalysis.PositionStatus = int.Parse(PositionStatus, CultureInfo.CurrentCulture);
                positionAnalysis.PositionStartTime = string.IsNullOrEmpty(PositionStartTime) ? (DateTime?)null : Convert.ToDateTime(PositionStartTime, CultureInfo.CurrentCulture);
                positionAnalysis.PositionEndTime = string.IsNullOrEmpty(PositionEndTime) ? (DateTime?)null : Convert.ToDateTime(PositionEndTime, CultureInfo.CurrentCulture);
                positionAnalysis.CompanyCode = CompanyCode;
                positionAnalysis.CompanyName = CompanyName;
                positionAnalysis.PositionVol = int.Parse(PositionVol, CultureInfo.CurrentCulture);
                positionAnalysis.CostPrice = float.Parse(CostPrice, CultureInfo.CurrentCulture);
                positionAnalysis.CurrentPrice = float.Parse(CurrentPrice, CultureInfo.CurrentCulture);
                positionAnalysis.TradeMkPlace = TradeMkPlace;
                positionAnalysis.AnalysisContent = AnalysisContent;

                fsql.Insert(positionAnalysis).ExecuteAffrows();

                return null;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("update")]
        public JsonResult Update(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {
                //参数
                string Id = HandlerHelper.GetValue(jsonObj, "Id");

                string PositionStatus = HandlerHelper.GetValue(jsonObj, "PositionStatus");
                string PositionStartTime = HandlerHelper.GetValue(jsonObj, "PositionStartTime");
                string PositionEndTime = HandlerHelper.GetValue(jsonObj, "PositionEndTime");
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string PositionVol = HandlerHelper.GetValue(jsonObj, "PositionVol");
                string CostPrice = HandlerHelper.GetValue(jsonObj, "CostPrice");
                string CurrentPrice = HandlerHelper.GetValue(jsonObj, "CurrentPrice");
                string TradeMkPlace = HandlerHelper.GetValue(jsonObj, "TradeMkPlace");
                //string AnalysisContent = HandlerHelper.GetValue(jsonObj, "AnalysisContent");

                //更新
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                PositionAnalysis positionAnalysis = fsql.Select<PositionAnalysis>().Where(t => t.Id == int.Parse(Id)).ToOne();

                if (positionAnalysis != null)
                {
                    positionAnalysis.PositionStatus = int.Parse(PositionStatus, CultureInfo.CurrentCulture);
                    positionAnalysis.PositionStartTime = string.IsNullOrEmpty(PositionStartTime)? (DateTime?)null : Convert.ToDateTime(PositionStartTime, CultureInfo.CurrentCulture);
                    positionAnalysis.PositionEndTime = string.IsNullOrEmpty(PositionEndTime) ? (DateTime?)null : Convert.ToDateTime(PositionEndTime, CultureInfo.CurrentCulture);
                    positionAnalysis.CompanyCode = CompanyCode;
                    positionAnalysis.CompanyName = CompanyName;
                    positionAnalysis.PositionVol = int.Parse(PositionVol, CultureInfo.CurrentCulture);
                    positionAnalysis.CostPrice = float.Parse(CostPrice, CultureInfo.CurrentCulture);
                    positionAnalysis.CurrentPrice = float.Parse(CurrentPrice, CultureInfo.CurrentCulture);
                    positionAnalysis.TradeMkPlace = TradeMkPlace;
                    //positionAnalysis.AnalysisContent = AnalysisContent;

                    fsql.Update<PositionAnalysis>().SetSource(positionAnalysis).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("saveanlysiscontent")]
        public JsonResult SaveAnalysisContent(JObject jsonObj)
        {
            //_logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                //保存备注
                string strId = jsonObj["Id"].ToString();
                string strAnalysisContent = jsonObj["AnalysisContent"].ToString();

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                PositionAnalysis source = fsql.Select<PositionAnalysis>().Where(t => t.Id == int.Parse(strId, CultureInfo.CurrentCulture)).ToOne();

                source.AnalysisContent = strAnalysisContent;

                if (source != null)
                {
                    fsql.Update<PositionAnalysis>().SetSource(source).UpdateColumns(a => a.AnalysisContent).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

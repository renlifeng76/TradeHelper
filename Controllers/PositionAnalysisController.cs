using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using FreeSqlDB.Model.RlfStock;
using FreeSqlDB.Model.Tools;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    //[EnableCors("any")]
    public class PositionAnalysisController : ControllerBase
    {

        private readonly ILogger<PositionAnalysisController> _logger;

        public PositionAnalysisController(ILogger<PositionAnalysisController> logger)
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

                string PerPageNum = HandlerHelper.GetValue(jsonObj, "PerPageNum"); 
                string CurPage = HandlerHelper.GetValue(jsonObj, "CurPage"); 
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag"); 

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //where 条件
                Expression<Func<PositionAnalysis, bool>> where = x=>true;

                if(!string.IsNullOrEmpty(CompanyCode))
                {
                    where = where.And(x => x.CompanyCode == CompanyCode);
                }

                //证券名称
                if (!string.IsNullOrEmpty(CompanyName))
                {
                    string[] array = CompanyName.Split(' ');

                    Expression<Func<PositionAnalysis, bool>> whereSub = x => x.CompanyName.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(one))
                        {
                            continue;
                        }
                        whereSub = whereSub.Or(x => x.CompanyName.Contains(one));
                    }
                    where = where.And(whereSub);
                }

                //标签
                if (!string.IsNullOrEmpty(Tag))
                {
                    string[] array = Tag.Split(' ');

                    Expression<Func<PositionAnalysis, bool>> whereSub = x => x.Tag.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(one))
                        {
                            continue;
                        }
                        whereSub = whereSub.And(x => x.Tag.Contains(one));
                    }
                    where = where.And(whereSub);
                }

                var lstModel = fsql.Select<PositionAnalysis>()
                  .Where(where)
                  .Count(out var total) //总记录数量
                  .OrderBy("PositionStartTime asc,CreateTime asc")
                  .Page(int.Parse(CurPage), int.Parse(PerPageNum)).ToList();

                dictRtn.Add("GridData", lstModel);

                dictRtn.Add("Count", total);

                return dictRtn;

            });

            //_logger.LogInformation("Search 结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        /// <summary>
        /// 保存分析内容
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 保存行
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        [HttpPost, Route("saverow")]
        public JsonResult SaveRow(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {
                //参数
                string Id = HandlerHelper.GetValue(jsonObj, "Id");

                string PositionStartTime = HandlerHelper.GetValue(jsonObj, "PositionStartTime");
                string PositionEndTime = HandlerHelper.GetValue(jsonObj, "PositionEndTime");
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string PositionVol = HandlerHelper.GetValue(jsonObj, "PositionVol");
                string CostPrice = HandlerHelper.GetValue(jsonObj, "CostPrice");
                string CurrentPrice = HandlerHelper.GetValue(jsonObj, "CurrentPrice");
                string TradeMkPlace = HandlerHelper.GetValue(jsonObj, "TradeMkPlace");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                //更新/插入
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                PositionAnalysis model = null;

                if (string.IsNullOrEmpty(Id))
                {
                    model = new PositionAnalysis();
                }
                else
                {
                    model = fsql.Select<PositionAnalysis>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();

                    //插入更新日志
                    if (model.Tag.IndexOf("更新日志") == -1)
                    {

                        model.Tag = "更新日志";
                        fsql.Insert<PositionAnalysis>(model).ExecuteAffrows();

                    }
                }

                model.PositionStartTime = string.IsNullOrEmpty(PositionStartTime) ? (DateTime?)null : Convert.ToDateTime(PositionStartTime, CultureInfo.CurrentCulture);
                model.PositionEndTime = string.IsNullOrEmpty(PositionEndTime) ? (DateTime?)null : Convert.ToDateTime(PositionEndTime, CultureInfo.CurrentCulture);
                model.CompanyCode = CompanyCode;
                model.CompanyName = CompanyName;
                model.PositionVol = int.Parse(PositionVol, CultureInfo.CurrentCulture);
                model.CostPrice = float.Parse(CostPrice, CultureInfo.CurrentCulture);
                model.CurrentPrice = float.Parse(CurrentPrice, CultureInfo.CurrentCulture);
                model.TradeMkPlace = TradeMkPlace;
                model.Tag = Tag;

                if (!string.IsNullOrEmpty(Id))
                {
                    fsql.Update<PositionAnalysis>().SetSource(model).ExecuteAffrows();
                }
                else
                {
                    fsql.Insert<PositionAnalysis>(model).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

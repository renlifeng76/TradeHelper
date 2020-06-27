using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    /// 上市公司
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class CompanyController : ControllerBase
    {

        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 上市公司:检索
        /// </summary>
        /// <param name="jsonObj">{PerPageNum,CurPage,CompanyCode,PositionStatus,Tag}</param>
        /// <returns>日志数据列表</returns>
        [HttpPost, Route("search")]
        public JsonResult Search(JObject jsonObj)
        {
            _logger.LogInformation("Search 开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string PerPageNum = HandlerHelper.GetValue(jsonObj, "PerPageNum");
                string CurPage = HandlerHelper.GetValue(jsonObj, "CurPage");
                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");
                string HoldCompanyName = HandlerHelper.GetValue(jsonObj, "HoldCompanyName");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //where 条件
                Expression<Func<Company, bool>> where = x=>true;

                //证券代码
                if(!string.IsNullOrEmpty(CompanyCode))
                {
                    where = where.And(x => x.CompanyCode == CompanyCode);
                }
                //证券名称
                if (!string.IsNullOrEmpty(CompanyName))
                {
                    string[] array = CompanyName.Split(' ');

                    Expression<Func<Company, bool>> whereSub = x => x.CompanyName.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(HoldCompanyName))
                        {
                            continue;
                        }
                        whereSub = whereSub.Or(x => x.CompanyName.Contains(one));
                    }
                    where = where.And(whereSub);
                }
                //概念/标签
                if (!string.IsNullOrEmpty(Tag))
                {
                    string[] arrayTag = Tag.Split(' ');

                    Expression<Func<Company, bool>> whereSub = x => x.Tag.Contains(arrayTag[0]);
                    foreach(string tag in arrayTag)
                    {
                        if(arrayTag[0].Equals(tag))
                        {
                            continue;
                        }
                        whereSub = whereSub.And(x => x.Tag.Contains(tag));
                    }
                    where = where.And(whereSub);
                }
                //控股公司
                if (!string.IsNullOrEmpty(HoldCompanyName))
                {
                    string[] array= HoldCompanyName.Split(' ');

                    Expression<Func<Company, bool>> whereSub = x => x.HoldCompanyName.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(HoldCompanyName))
                        {
                            continue;
                        }
                        whereSub = whereSub.Or(x => x.HoldCompanyName.Contains(one));
                    }
                    where = where.And(whereSub);
                }

                //var sql = fsql.Select<Company>()
                //  .Where(where)
                //  .OrderBy("Id asc")
                //  .Page(int.Parse(CurPage, CultureInfo.CurrentCulture), int.Parse(PerPageNum, CultureInfo.CurrentCulture)).ToSql();

                var lstModel = fsql.Select<Company>()
                  .Where(where)
                  .Count(out var total) //总记录数量
                  .OrderBy("Id asc")
                  .Page(int.Parse(CurPage,CultureInfo.CurrentCulture), int.Parse(PerPageNum, CultureInfo.CurrentCulture)).ToList();

                dictRtn.Add("GridData", lstModel);

                dictRtn.Add("Count", total);

                return dictRtn;

            });

            //_logger.LogInformation("Search 结束运行");

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

                string CompanyCode = HandlerHelper.GetValue(jsonObj, "CompanyCode");
                string CompanyName = HandlerHelper.GetValue(jsonObj, "CompanyName");
                string CompanyType = HandlerHelper.GetValue(jsonObj, "CompanyType");
                string HoldCompanyName = HandlerHelper.GetValue(jsonObj, "HoldCompanyName");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                //更新
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                Company model = null;

                if (string.IsNullOrEmpty(Id))
                {
                    model = new Company();
                }
                else
                {
                    model = fsql.Select<Company>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();
                }

                model.CompanyCode = CompanyCode;
                model.CompanyName = CompanyName;
                model.CompanyType = CompanyType;
                model.HoldCompanyName = HoldCompanyName;
                model.Tag = Tag;

                if (!string.IsNullOrEmpty(Id))
                {
                    fsql.Update<Company>().SetSource(model).ExecuteAffrows();
                }
                else
                {
                    fsql.Insert<Company>(model).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        [HttpPost, Route("savedescription")]
        public JsonResult SaveDescription(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                //保存备注
                string Id = HandlerHelper.GetValue(jsonObj, "Id");
                string Description = HandlerHelper.GetValue(jsonObj, "Description");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                Company source = fsql.Select<Company>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();

                source.Description = Description;

                if (source != null)
                {
                    fsql.Update<Company>().SetSource(source).UpdateColumns(a => a.Description).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

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
    /// <summary>
    /// 证券文章
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    //[EnableCors("any")]
    public class StockArtController : ControllerBase
    {

        private readonly ILogger<StockArtController> _logger;

        public StockArtController(ILogger<StockArtController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        [HttpPost, Route("search")]
        public JsonResult Search(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string PerPageNum = HandlerHelper.GetValue(jsonObj, "PerPageNum");
                string CurPage = HandlerHelper.GetValue(jsonObj, "CurPage");
                string ArtTitle = HandlerHelper.GetValue(jsonObj, "ArtTitle"); 
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                //where 条件
                Expression<Func<StockArt, bool>> where = x=>true;

                //标题
                if (!string.IsNullOrEmpty(ArtTitle))
                {
                    string[] array = ArtTitle.Split(' ');

                    Expression<Func<StockArt, bool>> whereSub = x => x.ArtTitle.Contains(array[0]);
                    foreach (string one in array)
                    {
                        if (array[0].Equals(one))
                        {
                            continue;
                        }
                        whereSub = whereSub.And(x => x.ArtTitle.Contains(one));
                    }
                    where = where.And(whereSub);
                }
                //标签
                if (!string.IsNullOrEmpty(Tag))
                {
                    string[] array = Tag.Split(' ');

                    Expression<Func<StockArt, bool>> whereSub = x => x.Tag.Contains(array[0]);
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

                var lstModel = fsql.Select<StockArt>()
                  .Where(where)
                  .Count(out var total) //总记录数量
                  .OrderBy("Id desc")
                  .Page(int.Parse(CurPage), int.Parse(PerPageNum)).ToList();

                dictRtn.Add("gridData", lstModel);

                dictRtn.Add("count", total);

                return dictRtn;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

        /// <summary>
        /// 保存文章内容
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <returns></returns>
        [HttpPost, Route("saveartcontent")]
        public JsonResult SaveArtContent(JObject jsonObj)
        {
            //_logger.LogInformation("开始运行");

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(() =>
            {

                //参数
                string Id = HandlerHelper.GetValue(jsonObj, "Id"); 
                string ArtContent = HandlerHelper.GetValue(jsonObj, "ArtContent");

                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                StockArt source = fsql.Select<StockArt>().Where(t => t.Id == int.Parse(Id)).ToOne();

                source.ArtContent = ArtContent;

                if (source != null)
                {
                    fsql.Update<StockArt>().SetSource(source).UpdateColumns(a => a.ArtContent).ExecuteAffrows();
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

                string ArtTitle = HandlerHelper.GetValue(jsonObj, "ArtTitle");
                string Tag = HandlerHelper.GetValue(jsonObj, "Tag");

                //更新/插入
                IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfstock", FreeSql.DataType.Sqlite);

                StockArt model = null;

                if (string.IsNullOrEmpty(Id))
                {
                    model = new StockArt();
                }
                else
                {
                    model = fsql.Select<StockArt>().Where(t => t.Id == int.Parse(Id, CultureInfo.CurrentCulture)).ToOne();
                }

                model.ArtTitle = ArtTitle;
                model.Tag = Tag;

                if (!string.IsNullOrEmpty(Id))
                {
                    fsql.Update<StockArt>().SetSource(model).ExecuteAffrows();
                }
                else
                {
                    fsql.Insert<StockArt>(model).ExecuteAffrows();
                }

                return null;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);
        }

    }
}

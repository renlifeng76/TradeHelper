using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using TradeHelper.Model.RlfConfig;

namespace TradeHelper.DbTools
{
    public static class FreeSqlFactory
    {
        static IConfiguration Configuration;

        static ConcurrentDictionary<string,IFreeSql> dictIFreeSql = new ConcurrentDictionary<string, IFreeSql>();
        static ConcurrentDictionary<string, IFreeSql> dictIFreeSqlForSqlite = new ConcurrentDictionary<string, IFreeSql>();

        // Lock对象，线程安全所用
        private static object syncRoot = new Object();

        /// <summary>
        /// 取得IFreeSql
        /// </summary>
        /// <param name="strDbName">库名</param>
        /// <param name="dateType">库类型</param>
        /// <param name="strSegmentId">分库Id</param>
        /// <returns></returns>
        public static IFreeSql GetIFreeSql(string strDbName, FreeSql.DataType dateType,string strSegmentId = "0")
        {
            //是否有效库名,不是抛异常


            //是否需要分库

            lock (syncRoot)
            {
                if(Configuration == null)
                {
                    var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json");

                    Configuration = builder.Build();
                }

                //已经存在,直接返回
                //否则创建并添加FreeSqlBuilder
                ConcurrentDictionary<string, IFreeSql> dict = dictIFreeSql;

                if (dateType == FreeSql.DataType.Sqlite)
                {
                    dict = dictIFreeSqlForSqlite;
                }

                if (dict.ContainsKey(strDbName))
                {
                    IFreeSql fsql = null;
                    dict.TryGetValue(strDbName, out fsql);
                    return fsql;
                }
                else
                {

                    ////创建并添加IFreeSql
                    IFreeSql fsql = null;
                    string strConn = string.Empty;

                    if (!dict.ContainsKey("rlfconfig"))
                    {
                        strConn = Configuration.GetConnectionString("DefaultConnection");

                        if(dateType == FreeSql.DataType.Sqlite)
                        {
                            strConn = Configuration["ConnectionStrings:SqliteConnection"];
                        }

                        fsql = new FreeSql.FreeSqlBuilder()
                       .UseConnectionString(dateType, strConn)
                       .UseAutoSyncStructure(true) //自动同步实体结构到数据库
                       .Build(); //请务必定义成 Singleton 单例模式

                        dict.TryAdd("rlfconfig", fsql);
                    }

                    //读取appconfig数据库配置
                    dict.TryGetValue("rlfconfig",out fsql);

                    if(strDbName.Equals("rlfconfig"))
                    {
                        return fsql;
                    }

                    var model = fsql.Select<AppConfig>()
                      .Where(t => t.AppKey == strDbName && t.DataType == (int)dateType)
                      .ToOne();

                    if(model == null)
                    {
                        throw new Exception("AppConfig没有读取到DbName数据!");
                    }

                    //todo:1.解密 2.分库计算库名处理strDbName?
                    strConn = model.AppValue;

                    //创建IFreeSql
                    fsql = new FreeSql.FreeSqlBuilder()
                        .UseConnectionString(dateType, strConn)
                        .UseAutoSyncStructure(true) //自动同步实体结构到数据库
                        .Build(); //请务必定义成 Singleton 单例模式

                    //添加IFreeSql
                    dict.TryAdd(strDbName, fsql);

                    return fsql;

                }

            }

        }

    }
}

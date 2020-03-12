//------------------------------------------------------------------------------
//1.关键字partial:允许一个类多个cs文件,编译合并成一个类
//2.特性:[JsonObject(MemberSerialization.OptIn)] , JsonProperty 标记可Json序列化属性
//3.StringLength = -1:sqlserver:varchar(max),sqlite:text
//4.sqlite索引名,同库不能重复
//------------------------------------------------------------------------------
using System;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
namespace TradeHelper.Model.RlfStock
{
    /// <summary>
    /// 交易日志
    /// </summary>
    [Index("uk_tradelog_id", "Id", true)]
    [Index("idx_tradelog_companycode", "CompanyCode", false)]
    [JsonObject(MemberSerialization.OptIn)]
    [Table(Name = "TradeLog")]
    public partial class TradeLog
    {
        [JsonProperty, Column(IsIdentity = true)]
        public int Id { get; set; }


        //字段-------------------------------------------------------------------------

        ////交易日
        //[JsonProperty, Column(Name = "TradeDay")]
        //public DateTime TradeDay { get; set; }

        //交易时间
        [JsonProperty, Column(Name = "TradeTime")]
        public DateTime TradeTime { get; set; }

        //证券代码
        [JsonProperty, Column(Name = "CompanyCode", DbType = "varchar(50)")]
        public string CompanyCode { get; set; } = string.Empty;

        //证券名称
        [JsonProperty, Column(Name = "CompanyName", DbType = "nvarchar(50)")]
        public string CompanyName { get; set; } = string.Empty;

        //委托方向
        [JsonProperty, Column(Name = "AgentType", DbType = "varchar(10)")]
        public string AgentType { get; set; } = string.Empty;

        //委托数量
        [JsonProperty, Column(Name = "AgentVol", DbType = "int")]
        public int AgentVol { get; set; }

        //委托状态
        [JsonProperty, Column(Name = "AgentStatus", DbType = "varchar(10)")]
        public string AgentStatus { get; set; } = string.Empty;

        //委托价格
        [JsonProperty, Column(Name = "AgentPrice")]
        public float AgentPrice { get; set; }

        //成交数量
        [JsonProperty, Column(Name = "TradeVol")]
        public int TradeVol { get; set; }

        //成交金额
        [JsonProperty, Column(Name = "TradePrice")]
        public float TradePrice { get; set; }

        //成交均价
        [JsonProperty, Column(Name = "TradePriceAverage")]
        public float TradePriceAverage { get; set; }

        //交易市场
        [JsonProperty, Column(Name = "TradeMkPlace", DbType = "varchar(10)")]
        public string TradeMkPlace { get; set; }

        //委托编号
        [JsonProperty, Column(Name = "AgentNo", DbType = "varchar(10)")]
        public string AgentNo { get; set; }

        //股东账号
        [JsonProperty, Column(Name = "HolderNo", DbType = "varchar(10)")]
        public string HolderNo { get; set; }

        //币种
        [JsonProperty, Column(Name = "CurrencyType", DbType = "varchar(10)")]
        public string CurrencyType { get; set; }

        //证券名称
        [JsonProperty, Column(Name = "Memo", DbType = "nvarchar(2000)",StringLength = -1)]
        public string Memo { get; set; } = string.Empty;


        //所有表默认字段-------------------------------------------------------------------------
        [JsonProperty, Column(DbType = "varchar(20)")]
        public string CreateUser { get; set; } = string.Empty;

        [JsonProperty, Column(DbType = "varchar(20)")]
        public string UpdateUser { get; set; } = string.Empty;


        //所有表默认字段:FreeSql插入自动填充-----------------------------------------------------
        [JsonProperty, Column(ServerTime = DateTimeKind.Utc, CanUpdate = false)]
        public DateTime CreateTime { get; set; }

        [JsonProperty, Column(ServerTime = DateTimeKind.Utc)]
        public DateTime UpdateTime { get; set; }

        [Column(IsVersion = true)]
        public int Version { get; set; }

    }

}

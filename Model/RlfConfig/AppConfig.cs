//------------------------------------------------------------------------------
//1.关键字partial:允许一个类多个cs文件,编译合并成一个类
//2.特性:[JsonObject(MemberSerialization.OptIn)] , JsonProperty 标记可Json序列化属性
//3.StringLength = -1:sqlserver:varchar(max),sqlite:text
//4.sqlite索引名,同库不能重复
//------------------------------------------------------------------------------
using System;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
namespace TradeHelper.Model.RlfConfig
{
    /// <summary>
    /// 项目配置数据
    /// </summary>
    [Index("uk_appconfig_id", "Id", true)]
    [Index("idx_appconfig_appkey", "AppKey", false)]
    [JsonObject(MemberSerialization.OptIn)]
    [Table(Name = "AppConfig")]
    public partial class AppConfig
    {
        [JsonProperty, Column(IsIdentity = true)]
        public int Id { get; set; }


        //字段-------------------------------------------------------------------------
        [JsonProperty, Column(Name = "DataType", DbType = "int")]
        public int? DataType { get; set; }

        [JsonProperty, Column(Name = "AppKey", DbType = "varchar(200)")]
        public string AppKey { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "AppValue", DbType = "varchar(2000)", StringLength = -1)]
        public string AppValue { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "EnAppValue", DbType = "varchar(2000)", StringLength = -1)]
        public string EnAppValue { get; set; } = string.Empty;


        //所有表默认字段-------------------------------------------------------------------------
        [JsonProperty, Column(DbType = "varchar(20)")]
        public string CreateUser { get; set; } = string.Empty;

        [JsonProperty, Column(DbType = "varchar(20)")]
        public string UpdateUser { get; set; } = string.Empty;

        [JsonProperty, Column(ServerTime = DateTimeKind.Utc, CanUpdate = false)]
        public DateTime CreateTime { get; set; }

        [JsonProperty, Column(ServerTime = DateTimeKind.Utc)]
        public DateTime UpdateTime { get; set; }

        [Column(IsVersion = true)]
        public int Version { get; set; }

    }

}

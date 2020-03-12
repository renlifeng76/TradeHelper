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
    /// 公司
    /// </summary>
    [Index("uk_company_id", "Id", true)]
    [Index("idx_company_companycode", "CompanyCode", false)]
    [JsonObject(MemberSerialization.OptIn)]
    [Table(Name = "Company")]
    public partial class Company
    {
        [JsonProperty, Column(IsIdentity = true)]
        public int Id { get; set; }


        //字段-------------------------------------------------------------------------
        [JsonProperty, Column(Name = "CompanyCode", DbType = "varchar(50)")]
        public string Companycode { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "CompanyName", DbType = "nvarchar(50)")]
        public string Companyname { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "CompanyType", DbType = "varchar(10)")]
        public string Companytype { get; set; } = string.Empty;

        [JsonProperty, Column(DbType = "nvarchar(2000)", StringLength = -1)]
        public string Description { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "HoldCompanyDesc", DbType = "nvarchar(2000)", StringLength = -1)]
        public string Holdcompanydesc { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "HoldCompanyName", DbType = "nvarchar(50)")]
        public string Holdcompanyname { get; set; } = string.Empty;

        [JsonProperty, Column(Name = "LaunchDate")]
        public DateTime? Launchdate { get; set; }

        [JsonProperty, Column(DbType = "nvarchar(500)")]
        public string Tag { get; set; } = string.Empty;


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

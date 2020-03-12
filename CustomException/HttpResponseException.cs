using System;

namespace TradeHelper.CustomException
{
    /// <summary>
    /// 自定义异常类型
    /// </summary>
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }
    }
}

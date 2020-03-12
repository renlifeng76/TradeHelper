using System;

namespace TradeHelper.CustomException
{
    public class DBOperatorException : Exception
    {
        public int Status { get; set; } = 500;

        public object Value { get; set; }

    }
}

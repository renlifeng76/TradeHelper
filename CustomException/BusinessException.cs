using System;

namespace TradeHelper.CustomException
{
    public class BusinessException:Exception
    {

        public int Status { get; set; } = 500;

        public object Value { get; set; }

        public BusinessException(string strMsg )
        {

        }

    }
}

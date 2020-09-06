using System;

namespace TradeHelper.CustomException
{
    public class BusinessException: BaseAppException
    {

        public BusinessException()
    :       base()
        {

        }
        public BusinessException(string message)
            : base(message)
        {

        }
        public BusinessException(string message,Exception innerException)
            : base(message, innerException)
        {

        }

    }
}

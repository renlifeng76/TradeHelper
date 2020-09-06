using System;

namespace TradeHelper.CustomException
{
    public class BaseAppException : Exception
    {
        //Exception ex;
        string errMessage = string.Empty;
        string errCode = string.Empty;

        public BaseAppException()
        : base()
        {
            errCode = "0";
        }

        public BaseAppException(string message)
        : base(message)
        {
            errCode = "0";
            errMessage = message;
        }

        //public BaseAppException(string message,Exception innerException)
        //    : base(message, innerException)
        //{
        //    errCode = "0";
        //    errMessage = message;
        //    ex = innerException;
        //}

        public override string Message
        {
            get
            {
                return errMessage;
            }
        }

        public BaseAppException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

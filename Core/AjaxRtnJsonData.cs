using System.Collections.Generic;

namespace TradeHelper.Core
{
    public class AjaxRtnJsonData
    {
        private string _errCode;

        public string ErrCode
        {
            get { return _errCode; }
            set { _errCode = value; }
        }
        private string _errMsg;

        public string ErrMsg
        {
            get { return _errMsg; }
            set { _errMsg = value; }
        }
        private string _status;

        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private Dictionary<string, object> _resultData;

        public Dictionary<string, object> ResultData
        {
            get { return _resultData; }
            set { _resultData = value; }
        }

        public AjaxRtnJsonData()
        {
            ErrCode = "0";
            Status = "true";
            ResultData = new Dictionary<string, object>();
        }
    }
}

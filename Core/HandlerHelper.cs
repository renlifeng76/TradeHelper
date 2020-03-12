using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TradeHelper.CustomException;

namespace TradeHelper.Core
{
    public class HandlerHelper
    {

        public delegate Dictionary<string, object> dlgMethod(JObject jsonObj);

        public delegate Dictionary<string, object> dlgMethod1();

        public static AjaxRtnJsonData ActionWrap(JObject jsonObj, dlgMethod dlg)
        {
            //string strRtn = string.Empty;

            AjaxRtnJsonData RtnJsonData = new AjaxRtnJsonData();

            try
            {
                //WebLogHelper.WriteLog("ActionWrap start ");
                //WebLogHelper.WriteLog("debug", context.Request.Cookies["UserIDFileCheck"].Value.ToString());

                //if (context.Request.Cookies["UserIDFileCheck"] == null
                //|| string.IsNullOrEmpty(context.Request.Cookies["UserIDFileCheck"]))
                //{

                //    throw new BusinessExcption() { Value = "Token已过期！" };

                //}

                RtnJsonData.ResultData = dlg(jsonObj);

                //WebLogHelper.WriteLog("ActionWrap end ");

            }
            catch (BusinessException ex)
            {
                RtnJsonData.ErrCode = "1";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("","BusinessExcption=>" + ex.ToString());
                //WebLogHelper.WriteLog("","ex.Source=>" + ex.Source);
            }
            catch (DBOperatorException ex)
            {
                RtnJsonData.ErrCode = "2";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("HandlerDBOperatorException", "DBOperatorException=>" + ex.ToString());
                //WebLogHelper.WriteLog("HandlerDBOperatorException", "ex.Source=>" + ex.Source);
            }
            catch (Exception ex)
            {
                RtnJsonData.ErrCode = "3";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = "系统异常!" + ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("HandlerException", "Exception=>" + ex.ToString());
                //WebLogHelper.WriteLog("HandlerException", "ex.Source=>" + ex.Source);
            }

            return RtnJsonData;

        }

        public static AjaxRtnJsonData ActionWrap(dlgMethod1 dlg)
        {
            //string strRtn = string.Empty;

            AjaxRtnJsonData RtnJsonData = new AjaxRtnJsonData();

            try
            {
                //WebLogHelper.WriteLog("ActionWrap start ");
                //WebLogHelper.WriteLog("debug", context.Request.Cookies["UserIDFileCheck"].Value.ToString());

                //if (context.Request.Cookies["UserIDFileCheck"] == null
                //|| string.IsNullOrEmpty(context.Request.Cookies["UserIDFileCheck"]))
                //{

                //    throw new BusinessExcption() { Value = "Token已过期！" };

                //}

                RtnJsonData.ResultData = dlg();

                //WebLogHelper.WriteLog("ActionWrap end ");

            }
            catch (BusinessException ex)
            {
                RtnJsonData.ErrCode = "1";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("","BusinessExcption=>" + ex.ToString());
                //WebLogHelper.WriteLog("","ex.Source=>" + ex.Source);
            }
            catch (DBOperatorException ex)
            {
                RtnJsonData.ErrCode = "2";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("HandlerDBOperatorException", "DBOperatorException=>" + ex.ToString());
                //WebLogHelper.WriteLog("HandlerDBOperatorException", "ex.Source=>" + ex.Source);
            }
            catch (Exception ex)
            {
                RtnJsonData.ErrCode = "3";
                RtnJsonData.Status = "false";
                RtnJsonData.ErrMsg = "系统异常!" + ex.Message;
                //string typeName = this.GetType().ToString();//当类名用
                //string methodName = context.Request.Form["Action"];
                //ex.Source = typeName + "." + methodName + "->" + ex.Source;
                //WebLogHelper.WriteLog("HandlerException", "Exception=>" + ex.ToString());
                //WebLogHelper.WriteLog("HandlerException", "ex.Source=>" + ex.Source);
            }

            return RtnJsonData;

        }

    }
}

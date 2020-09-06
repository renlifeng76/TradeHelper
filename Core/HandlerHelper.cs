using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TradeHelper.CustomException;

namespace TradeHelper.Core
{
    public class HandlerHelper
    {

        public delegate Dictionary<string, object> dlgMethod(string action,JObject jsonObj);

        public delegate Dictionary<string, object> dlgMethod1();

        public static AjaxRtnJsonData ActionWrap(string action, JObject jsonObj, dlgMethod dlg)
        {
            //string strRtn = string.Empty;

            AjaxRtnJsonData RtnJsonData = new AjaxRtnJsonData();

            try
            {
                //WebLogHelper.WriteLog("ActionWrap start ");
                if(string.IsNullOrEmpty(action) || action.IndexOf("RequestToken") == -1)
                {
                    string UserId = HandlerHelper.GetValue(jsonObj, "UserId");
                    if (string.IsNullOrEmpty(UserId))
                    {
                        throw new BusinessException("UserId为空！");
                    }
                }

                RtnJsonData.ResultData = dlg(action, jsonObj);

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

        /// <summary>
        /// 取得JObject属性值
        /// </summary>
        /// <param name="jsonObj"></param>
        /// <param name="strPropName"></param>
        /// <returns></returns>
        public static string GetValue(JObject jsonObj,string strPropName)
        {

            string strRtn = string.Empty;

            if(jsonObj != null && jsonObj[strPropName] != null)
            {
                strRtn = jsonObj[strPropName].ToString().Trim();
            }

            return strRtn;

        }

    }
}

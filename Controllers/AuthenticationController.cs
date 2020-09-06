using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeHelper.Core;
using TradeHelper.CustomException;
using TradeHelper.Dto;
using TradeHelper.IService;

namespace TradeHelper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Route("[controller]")]
    //[EnableCors("any")]  此行需要注释掉:否则阿里云服务器报错:500
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        private readonly IAuthenticateService _authService;
        public AuthenticationController(IAuthenticateService authService, ILogger<AuthenticationController> logger)
        {
            this._authService = authService;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost, Route("requestToken")]
        public ActionResult RequestToken([FromBody] LoginRequestDTO request)
        {
            _logger.LogInformation("RequestToken 开始运行");

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            string token;
            string userid;
            if (_authService.IsAuthenticated(request, out token,out userid))
            {
                return Ok(token + "|" + userid);
            }

            return BadRequest("Invalid Request");

        }

        [AllowAnonymous]
        [HttpPost, Route("requestToken1")]
        public JsonResult RequestToken1(JObject jsonObj)
        {
            //_logger.LogInformation("RequestToken 开始运行");

            string strAction = "RequestToken1";

            AjaxRtnJsonData ajaxRtnJsonData = HandlerHelper.ActionWrap(strAction, jsonObj, (strAction, jsonObj) =>
            {

                Dictionary<string, object> dictRtn = new Dictionary<string, object>();

                string username = HandlerHelper.GetValue(jsonObj, "username");
                string password = HandlerHelper.GetValue(jsonObj, "password");

                LoginRequestDTO loginRequestDTO = new LoginRequestDTO() { Username = username,Password = password };

                if (!ModelState.IsValid)
                {
                    throw new BusinessException("无效请求！");
                }

                string token;
                string userid;

                bool isAuth = _authService.IsAuthenticated(loginRequestDTO, out token, out userid);
                if(isAuth == false)
                {
                    throw new BusinessException("用户名或密码错误！");
                }

                dictRtn.Add("Token", token);
                dictRtn.Add("UserId", userid);

                return dictRtn;

            });

            //_logger.LogInformation("结束运行");

            return new JsonResult(ajaxRtnJsonData);

        }
    }
}

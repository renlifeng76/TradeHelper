using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            if (_authService.IsAuthenticated(request, out token))
            {
                return Ok(token);
            }

            return BadRequest("Invalid Request");

        }
    }
}

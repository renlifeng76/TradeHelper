﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeHelper.IService;
using TradeHelper.Model;

namespace TradeHelper.Service
{
    public class UserService : IUserService
    {
        //模拟测试，默认都是人为验证有效
        public bool IsValid(LoginRequestDTO req)
        {
            return true;
        }
    }
}

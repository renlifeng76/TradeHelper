using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeHelper.Model;

namespace TradeHelper.IService
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);
    }
}

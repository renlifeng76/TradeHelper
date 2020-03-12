using TradeHelper.Dto;
using TradeHelper.IService;

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

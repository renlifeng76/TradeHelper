using TradeHelper.Dto;

namespace TradeHelper.IService
{
    public interface IUserService
    {
        bool IsValid(LoginRequestDTO req);
    }
}

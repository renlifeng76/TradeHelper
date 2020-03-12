using TradeHelper.Dto;

namespace TradeHelper.IService
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(LoginRequestDTO request, out string token);
    }
}

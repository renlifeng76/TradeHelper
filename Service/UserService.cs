using FreeSqlDB.Model.RlfMain;
using FreeSqlDB.Model.Tools;
using TradeHelper.Dto;
using TradeHelper.IService;

namespace TradeHelper.Service
{
    public class UserService : IUserService
    {
        //登录用户名，密码验证
        public bool IsValid(LoginRequestDTO req)
        {
            //var logger = NLog.LogManager.GetLogger("rlf");

            if(req == null || string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
            {
                return false;
            }

            //logger.Debug(req.Username);

            IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfmain", FreeSql.DataType.Sqlite);

            Users users = fsql.Select<Users>().Where(t => t.UserName == req.Username && t.Password== req.Password).ToOne();

            if(users == null)
            {
                return false;
            }

            //更新登录时间
            fsql.Update<Users>().Set(x => x.LoginTime , System.DateTime.Now).Where(x => x.Id == users.Id).ExecuteAffrows();

            return true;
        }

        //取得用户Id
        public int GetUserId(string strUserName)
        {
            //var logger = NLog.LogManager.GetLogger("rlf");

            IFreeSql fsql = FreeSqlFactory.GetIFreeSql("rlfmain", FreeSql.DataType.Sqlite);

            Users users = fsql.Select<Users>().Where(t => t.UserName == strUserName).ToOne();

            return users.Id;

        }
    }
}

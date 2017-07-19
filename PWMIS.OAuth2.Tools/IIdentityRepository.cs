using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.AuthorizationCenter.Repository
{
    /// <summary>
    /// 身份认证持久化接口
    /// </summary>
    public interface IIdentityRepository
    {
        /// <summary>
        /// 客户ID是否存在
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task<bool> ExistsClientId(string clientId);
        /// <summary>
        /// 校验客户标识
        /// </summary>
        /// <param name="clientId">客户ID</param>
        /// <param name="clientSecret">客户秘钥</param>
        /// <returns></returns>
        Task<bool> ValidateClient(string clientId, string clientSecret);
        /// <summary>
        /// 校验用户名密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> ValidatedUserPassword(string userName, string password);
    }
}

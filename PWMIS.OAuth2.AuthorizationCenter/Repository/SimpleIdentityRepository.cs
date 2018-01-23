using PWMIS.DataMap.Entity;
using PWMIS.OAuth2.AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Repository
{
    public class SimpleIdentityRepository : IIdentityRepository
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, string> dictClient = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        public async Task<bool> ExistsClientId(string clientId)
        {
            return await Task.Run<bool>(() =>
            {
                AuthClientInfoEntity entity = new AuthClientInfoEntity();
                entity.ClientId = clientId;

                OQL q = OQL.From(entity)
                    .Select(entity.ClientId)
                    .Where(entity.ClientId)
                    .END;
                AuthDbContext context = new AuthDbContext();
                AuthClientInfoEntity dbEntity = context.QueryObject<AuthClientInfoEntity>(q);
                return dbEntity != null;
            });
        }

        public async Task<bool> ValidateClient(string clientId, string clientSecret)
        {
            string dict_clientSecret;
            if (dictClient.TryGetValue(clientId, out dict_clientSecret) && dict_clientSecret== clientSecret)
            {
                return true;
            }
            else
            {
                return await Task.Run<bool>(() => {
                    AuthClientInfoEntity entity = new AuthClientInfoEntity();
                    entity.ClientId = clientId;
                    entity.ClientSecret = clientSecret;
                    OQL q = OQL.From(entity)
                        .Select(entity.ClientId)
                        .Where(entity.ClientId, entity.ClientSecret)
                        .END;
                    AuthDbContext context = new AuthDbContext();
                    AuthClientInfoEntity dbEntity = context.QueryObject<AuthClientInfoEntity>(q);
                    if (dbEntity != null)
                    {
                        dictClient.TryAdd(clientId, clientSecret);
                        return true;
                    }
                    else
                        return false;
                });
            }
            
        }

        public async Task<bool> ValidatedUserPassword(string userName, string password)
        {
            return await Task.Run<bool>(() =>
            {
                UserInfoEntity user = new UserInfoEntity();
                user.UserName = userName;
                user.Password = password;
                OQL q = OQL.From(user)
                   .Select()
                   .Where(user.UserName, user.Password)
                   .END;
                AuthDbContext context = new AuthDbContext();
                AuthClientInfoEntity dbEntity = context.QueryObject<AuthClientInfoEntity>(q);
                return dbEntity != null;
            });
        }
    }
}
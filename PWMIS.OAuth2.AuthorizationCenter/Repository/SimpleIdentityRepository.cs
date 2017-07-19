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
        public async Task<bool> ExistsClientId(string clientId)
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
        }

        public async Task<bool> ValidateClient(string clientId, string clientSecret)
        {
            AuthClientInfoEntity entity = new AuthClientInfoEntity();
            entity.ClientId = clientId;
            entity.ClientSecret = clientSecret;
            OQL q = OQL.From(entity)
                .Select(entity.ClientId)
                .Where(entity.ClientId, entity.ClientSecret)
                .END;
            AuthDbContext context = new AuthDbContext();
            AuthClientInfoEntity dbEntity = context.QueryObject<AuthClientInfoEntity>(q);
            return dbEntity != null;
        }

        public async Task<bool> ValidatedUserPassword(string userName, string password)
        {
            UserInfoEntity user = new UserInfoEntity();
            user.UserName = userName;
            user.Password = password;
            OQL q = OQL.From(user)
               .Select()
               .Where(user.UserName,user.Password)
               .END;
            AuthDbContext context = new AuthDbContext();
            AuthClientInfoEntity dbEntity = context.QueryObject<AuthClientInfoEntity>(q);
            return dbEntity != null;
        }
    }
}
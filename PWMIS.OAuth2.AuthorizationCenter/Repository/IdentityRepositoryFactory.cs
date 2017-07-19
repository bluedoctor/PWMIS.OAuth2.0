using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWMIS.OAuth2.AuthorizationCenter.Repository
{
    class IdentityRepositoryFactory
    {
       public static IIdentityRepository CreateInstance()
        {
            string cnfig = System.Configuration.ConfigurationManager.AppSettings["IdentityRepository"];
            if (string.IsNullOrEmpty(cnfig))
                throw new Exception("请在appSettings配置名称为 IdentityRepository 的IIdentityRepository 接口实现类，例如：\"PWMIS.OAuth2.AuthorizationCenter.Repository.SimpleIdentityRepository,PWMIS.OAuth2.AuthorizationCenter\"");
            string[] arrTemp = cnfig.Split(',');
            string providerAssembly = arrTemp[1];
            string providerType = arrTemp[0];
            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(providerAssembly);
            object provider = assembly.CreateInstance(providerType);
            if (provider is IIdentityRepository)
            {
                IIdentityRepository result = provider as IIdentityRepository;
                return result;
            }
            else
            {
                throw new InvalidOperationException("当前指定的的提供程序不是 IIdentityRepository 接口实现类，请确保应用程序进行了正确的配置（在appSettings配置名称为 IdentityRepository 的配置）。");
            }
        }
    }
}
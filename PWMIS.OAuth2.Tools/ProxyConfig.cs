using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWMIS.OAuth2.Tools
{
    /// <summary>
    /// 代理配置
    /// </summary>
    public class ProxyConfig
    {
        public ProxyConfig()
        {
            this.LogFilePath = System.IO.Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProxyLog");
            this.RouteMaps = new List<ProxyRouteMap>();
            this.ServerName = "PWMIS-ProxyServer";
        }

        /// <summary>
        /// 是否开启缓存，如果开启，那么来自客户端的HTTP请求的响应结果可能会被缓存
        /// </summary>
        public bool EnableCache { get; set; }
        /// <summary>
        /// 是否开启请求日志，如果开启，将记录请求日志和请求的响应时间
        /// </summary>
        public bool EnableRequestLog { get; set; }
        /// <summary>
        /// 代理日志的文件路径，不带文件名，默认为我的文档目录 ProxyLog
        /// </summary>
        public string LogFilePath { get; set; }
        /// <summary>
        /// 路由映射表
        /// </summary>
        public List<ProxyRouteMap> RouteMaps { get; set; }
        /// <summary>
        /// 代理服务器名字
        /// </summary>
        public string ServerName { get; set; }
        /// <summary>
        /// 在使用OAuth 2.0授权服务的时候，如果用户没有登录就调用资源服务器，指示客户端跳转的URL地址。如果未设置此属性，会继续调用资源服务器。
        /// </summary>
        public string OAuthRedirUrl { get; set; }

        /// <summary>
        /// 目标API地址访问未授权，是否跳转OAuthRedirUrl。如果跳转，将跳转到OAuthRedirUrl 指定的页面，如果不跳转，会直接抛出 HTTP Statue Unauthorized
        /// </summary>
        public bool UnauthorizedRedir { get; set; }

    }

    /// <summary>
    /// 代理服务路由映射
    /// </summary>
    public class ProxyRouteMap
    {
        /// <summary>
        /// 代理转发时候要匹配的源URL请求前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 要转发到的服务器地址，例如 localhost:8001
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 转发请求的地址中需要匹配的词
        /// </summary>
        public string Match { get; set; }
        /// <summary>
        /// 转发请求的地址中被匹配的词要映射的新词
        /// </summary>
        public string Map { get; set; }
        /// <summary>
        /// 是否需要会话支持
        /// </summary>
        public bool SessionRequired { get; set; }

    }
}

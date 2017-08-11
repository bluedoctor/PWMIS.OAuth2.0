using PWMIS.OAuth2.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Forms;

namespace Demo.OAuth2.WinFormTest
{
    public partial class Form1 : Form
    {
        private OAuthClient oAuthCenterClient;
        private HttpClient client;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = this.txtUseName.Text.Trim();
            string password = this.txtPassword.Text;
           
            try
            {
                await oAuthCenterClient.WebLogin(userName, password, result =>
                {
                    if (result.LogonMessage == "OK")
                    {
                        MessageBox.Show("登录成功！");
                        this.txtUrl.Text = this.oAuthCenterClient.ResourceServerClient.BaseAddress.ToString() + "api/values";
                        btnGo.Enabled = true;
                        //有关 cookie，可以参考：
                        // string[] strCookies = (string[])response.Headers.GetValues("Set-Cookie");
                        // http://www.cnblogs.com/leeairw/p/3754913.html
                        // http://www.cnblogs.com/sjns/p/5331723.html
                    }
                    else
                    {
                        MessageBox.Show(result.LogonMessage);
                        btnGo.Enabled = false;
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     

        private async void btnGo_Click(object sender, EventArgs e)
        {
            //使用全局的HttpClient，将使用登录时候获取的Cookie,服务器会认为这是同一个用户的请求
            HttpClient client = this.client;
            if (!this.ckbUserLogin.Checked)
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["Host_Webapi"];
                client = new HttpClient();
                client.BaseAddress = new Uri(url);
            }
            await RequestResouceServer(client);
        }

        private async Task RequestResouceServer(HttpClient client)
        {
            var tokenResponse = oAuthCenterClient.GetToken("client_credentials").Result;
            if (tokenResponse == null)
            {
                MessageBox.Show("获取客户端令牌失败");
                return;
            }
            oAuthCenterClient.SetAuthorizationRequest(client, tokenResponse);

            var response = await client.GetAsync(this.txtUrl.Text);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    string errMsg = string.Format("HTTP响应码：{0}，错误信息：{1}", response.StatusCode, (await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                    MessageBox.Show(errMsg);
                }
                catch
                {
                    MessageBox.Show(response.StatusCode.ToString());
                }

            }
            else
            {
                this.txtPage.Text = await response.Content.ReadAsStringAsync();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.btnGo.Enabled = false;
            this.oAuthCenterClient = new OAuthClient();
            this.client = oAuthCenterClient.ResourceServerClient;
            this.txtUrl.Text = this.client.BaseAddress.ToString();


        }

        private async void txtOpenIE_Click(object sender, EventArgs e)
        {
            await oAuthCenterClient.OpenUrlByBrowser(this.txtUseName.Text, this.txtUrl.Text);
        }

       
    }
}

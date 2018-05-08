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
        private bool HasError;
        private string ErrorMessages;
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

        private  void btnBatchTest_Click(object sender, EventArgs e)
        {
            int paraNum;
            int ReqCount;
            bool checkLogin = ckbLogin.Checked;
            if (!int.TryParse(this.txtParaNum.Text, out paraNum))
            {
                MessageBox.Show("请输入正确的数字！");
                return;
            }
            if (!int.TryParse(this.txtReqCount.Text, out ReqCount))
            {
                MessageBox.Show("请输入正确的数字！");
                return;
            }
            if (paraNum < 1 || ReqCount < 1)
            {
                MessageBox.Show("请输入正确的数字！");
                return;
            }
            HasError = false;
            ErrorMessages = "";
            txtPage.Text = "正在测试...";
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int j = 0; j < ReqCount; j++)
            {
                List<Task<HttpStatusCode>> list = new List<Task<HttpStatusCode>>();
                for (int i = 0; i < paraNum; i++)
                {
                    Task<HttpStatusCode> t = Task<HttpStatusCode>.Run(() => TestAPI());
                    if (checkLogin && i % 10 == 0)
                    {
                        Task<HttpStatusCode> t2 = Task<HttpStatusCode>.Run(() => TestLogin());
                        list.Add(t2);
                    }
                    list.Add(t);

                }
                //测试API访问期间重登录
               
                Task.WaitAll(list.ToArray());
            }
           
            sw.Stop();
            long tps = 1000 * (paraNum * ReqCount) / sw.ElapsedMilliseconds;
            txtPage.Text = ErrorMessages;
            MessageBox.Show("测试完成，耗时(ms)："+sw.ElapsedMilliseconds+",TPS="+tps);

        }

        private async Task<HttpStatusCode> TestAPI()
        {
            var response = await this.client.GetAsync(this.txtUrl.Text);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    string errMsg = string.Format("HTTP响应码：{0}，错误信息：{1}", response.StatusCode, (await response.Content.ReadAsAsync<HttpError>()).ExceptionMessage);
                    //MessageBox.Show(errMsg);
                    ErrorMessages = "\r\n"+errMsg;
                }
                catch
                {
                    //MessageBox.Show("HTTP响应码："+response.StatusCode.ToString());
                    ErrorMessages = "\r\nHTTP响应码：" + response.StatusCode.ToString();
                }

            }
            return response.StatusCode;
        }

        private async Task<HttpStatusCode> TestLogin()
        {
            string userName = this.txtUseName.Text.Trim();
            string password = this.txtPassword.Text;

            var code= await oAuthCenterClient.WebLogin(userName, password, result =>
            {
                if (result.LogonMessage == "OK")
                {
                    //MessageBox.Show("登录成功！");
                 
                }
                else
                {
                    MessageBox.Show("登录失败："+result.LogonMessage);
                }
            });
            return code;
        }

    }
}

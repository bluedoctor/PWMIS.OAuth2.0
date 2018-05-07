namespace Demo.OAuth2.WinFormTest
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUseName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.txtOpenIE = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ckbUserLogin = new System.Windows.Forms.CheckBox();
            this.btnBatchTest = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtParaNum = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtReqCount = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "密码：";
            // 
            // txtUseName
            // 
            this.txtUseName.Location = new System.Drawing.Point(94, 38);
            this.txtUseName.Name = "txtUseName";
            this.txtUseName.Size = new System.Drawing.Size(140, 21);
            this.txtUseName.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(92, 74);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(142, 21);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(92, 113);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(110, 27);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "登录";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(284, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "访问资源服务器：";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(286, 74);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(433, 21);
            this.txtUrl.TabIndex = 6;
            this.txtUrl.Text = "http://";
            // 
            // btnGo
            // 
            this.btnGo.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnGo.Location = new System.Drawing.Point(725, 63);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(57, 34);
            this.btnGo.TabIndex = 7;
            this.btnGo.Text = "Go";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtPage
            // 
            this.txtPage.Location = new System.Drawing.Point(286, 115);
            this.txtPage.Multiline = true;
            this.txtPage.Name = "txtPage";
            this.txtPage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPage.Size = new System.Drawing.Size(495, 243);
            this.txtPage.TabIndex = 8;
            // 
            // txtOpenIE
            // 
            this.txtOpenIE.Location = new System.Drawing.Point(287, 382);
            this.txtOpenIE.Name = "txtOpenIE";
            this.txtOpenIE.Size = new System.Drawing.Size(129, 34);
            this.txtOpenIE.TabIndex = 9;
            this.txtOpenIE.Text = "使用浏览器打开";
            this.txtOpenIE.UseVisualStyleBackColor = true;
            this.txtOpenIE.Click += new System.EventHandler(this.txtOpenIE_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(42, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 73);
            this.label4.TabIndex = 10;
            this.label4.Text = "注意：登录只是调用Web接口登录，缺省情况下，访问资源服务器将使用客户端授权模式，客户端ID和秘钥请看应用程序配置。\r\n也可以使用登录时候的凭据来访问资源服务器，" +
    "这等同于使用密码授权模式。";
            // 
            // ckbUserLogin
            // 
            this.ckbUserLogin.AutoSize = true;
            this.ckbUserLogin.Location = new System.Drawing.Point(406, 45);
            this.ckbUserLogin.Name = "ckbUserLogin";
            this.ckbUserLogin.Size = new System.Drawing.Size(120, 16);
            this.ckbUserLogin.TabIndex = 11;
            this.ckbUserLogin.Text = "使用用户登录凭据";
            this.ckbUserLogin.UseVisualStyleBackColor = true;
            // 
            // btnBatchTest
            // 
            this.btnBatchTest.Location = new System.Drawing.Point(455, 382);
            this.btnBatchTest.Name = "btnBatchTest";
            this.btnBatchTest.Size = new System.Drawing.Size(95, 33);
            this.btnBatchTest.TabIndex = 12;
            this.btnBatchTest.Text = "批量测试";
            this.btnBatchTest.UseVisualStyleBackColor = true;
            this.btnBatchTest.Click += new System.EventHandler(this.btnBatchTest_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(459, 446);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "并发请求数：";
            // 
            // txtParaNum
            // 
            this.txtParaNum.Location = new System.Drawing.Point(542, 442);
            this.txtParaNum.Name = "txtParaNum";
            this.txtParaNum.Size = new System.Drawing.Size(55, 21);
            this.txtParaNum.TabIndex = 14;
            this.txtParaNum.Text = "1000";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(626, 446);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "请求次数：";
            // 
            // txtReqCount
            // 
            this.txtReqCount.Location = new System.Drawing.Point(697, 442);
            this.txtReqCount.Name = "txtReqCount";
            this.txtReqCount.Size = new System.Drawing.Size(55, 21);
            this.txtReqCount.TabIndex = 16;
            this.txtReqCount.Text = "10";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 507);
            this.Controls.Add(this.txtReqCount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtParaNum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnBatchTest);
            this.Controls.Add(this.ckbUserLogin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtOpenIE);
            this.Controls.Add(this.txtPage);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUseName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "PWMIS OAuth 2.0 Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUseName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPage;
        private System.Windows.Forms.Button txtOpenIE;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox ckbUserLogin;
        private System.Windows.Forms.Button btnBatchTest;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtParaNum;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtReqCount;
    }
}


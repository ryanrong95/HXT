using System;

namespace Yahv.Web.Forms
{
    /// <summary>
    /// 登录用户页面基类
    /// </summary>
    abstract public class UserPage : ClientPage
    {

        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected UserPage()
        {
            this.Init += UserPage_Init_Authenticate;
        }

        private void UserPage_Init_Authenticate(object sender, EventArgs e)
        {
            if (this.Authenticate())
            {
                this.OnSucess();
            }
            else
            {
                this.OnDenied();
            }
        }
        /// <summary>
        /// 验证调用
        /// </summary>
        /// <returns>是否通过</returns>
        abstract protected bool Authenticate();
        /// <summary>
        /// 验证成功调用
        /// </summary>
        abstract protected void OnSucess();
        /// <summary>
        /// 验证失败调用
        /// </summary>
        abstract protected void OnDenied();


    }
}

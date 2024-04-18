using System;

namespace Needs.Web.Forms
{
    /// <summary>
    /// 原先私有的
    /// </summary>
    abstract public class UserPage : ClientPage
    {
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
        /// 身份验证
        /// </summary>
        /// <returns></returns>
        abstract protected bool Authenticate();
        abstract protected void OnSucess();
        abstract protected void OnDenied();
    }
}

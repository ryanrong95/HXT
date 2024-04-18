using System;
using Yahv.Web.Forms.Sso;

namespace Yahv.Web.Erp
{
    /// <summary>
    ///  Erp 颗粒化 页面基类
    /// </summary>
    public class ErpSsoPage : ErpPage
    {
        public ErpSsoPage()
        {
            //废弃
            //this.Load += ErpSsoPage_Load;
        }

        protected override void OnDenied()
        {
            Response.Clear();
            Response.Write(Properties.Resource.Return);
            Response.End();
        }

        private void ErpSsoPage_Load(object sender, EventArgs e)
        {
            //判断菜单访问权限
            string roleID = Yahv.Erp.Current.Role.ID;

            if (Yahv.Erp.Current.IsSuper)
            {
                return;
            }

            string url = Request.Url.OriginalString.ToLower();
            if (url.EndsWith("/panels.aspx"))
            {
                return;
            }


            if (!Roles.Contains(roleID, Request.Url.AbsoluteUri))
            {
                throw new Exception("恶意访问！犯罪严重！");
            }
        }

        protected override bool Authenticate()
        {
            return Yahv.Erp.Current != null;
        }
    }
}

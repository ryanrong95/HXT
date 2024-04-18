namespace Yahv.Web.Forms.Sso
{
    /// <summary>
    /// Erp 管理员页面基类
    /// </summary>
    abstract public class ErpPage : UserPage
    {
        protected override bool Authenticate()
        {
            return false;
        }

        protected override void OnDenied()
        {
            Response.Redirect("/");
        }

        protected override void OnSucess()
        {

        }
    }
}

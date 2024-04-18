using System;

namespace WebApp
{
    public partial class Boots : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();

            Yahv.Plats.Services.Boot.Current.EnterSuccess += Boot_EnterSuccess;
            Yahv.Plats.Services.Boot.Current.EnterError += Boot_EnterError;
            Yahv.Plats.Services.Boot.Current.Execute();

            Response.Write("初始化结束!");
            Response.End();
        }

        private void Boot_EnterSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            try
            {
                Response.Write($"{sender.ToString()}<br>");
            }
            catch (Exception)
            {
            }
        }

        private void Boot_EnterError(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            try
            {
                Response.Write($"<font color='red'>{sender}</font><br>");
            }
            catch (Exception)
            {
            }
        }
    }
}
using System;
using System.Linq;

namespace WebApp.Tests
{
    public partial class Postest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Clear();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int[] arry = null;

            arry.Select(item =>
            {

                //处理人民币折扣
                //item.Prices

                return item;
            });
        }
    }
}
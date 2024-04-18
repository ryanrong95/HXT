using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class help : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //Session
            this.AbortTransaction += Help_AbortTransaction; ;
        }

        private void Help_AbortTransaction(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
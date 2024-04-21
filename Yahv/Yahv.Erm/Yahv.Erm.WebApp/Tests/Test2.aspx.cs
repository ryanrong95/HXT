using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Yahv.Erm.WebApp.Tests
{
    public partial class Test2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string connectString1 = "Data Source=172.30.10.199,5311;Initial Catalog=PvCenter;Persist Security Info=True;User ID=udata;Password=Turing2019";
            string connectString2 = "Data Source=172.30.10.51,6522;Initial Catalog=PvCenter;Persist Security Info=True;User ID=u_v0;Password=V6e1W9MiA84t6FJ05TXCU9uRY9THg3EpT60v9X1qwMbLg895Oc";

            SqlConnection sqlCnt1 = new SqlConnection(connectString1);
            sqlCnt1.Open();
            sqlCnt1.Close();

            SqlConnection sqlCnt2 = new SqlConnection(connectString2);
            sqlCnt2.Open();
            sqlCnt2.Close();
        }
    }
}
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Settings;

namespace WebApp
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {




            var check = JsonsManager<Test.Models.Customs>.Current.IsCheckUser;
            this.lblMessage.Text = check + "";


            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var topN = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>().Take(1);
                Console.WriteLine(topN.ToArray().Length);
            }

            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                var topN = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>().Take(1);
                Console.WriteLine(topN.ToArray().Length);
            }

            if (true)
            {

                var reponsitory = LinqFactory<PvbCrmReponsitory>.Create();

                for (int index = 0; index < 10; index++)
                {
                    var topN = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Clients>().Take(1);
                    Console.WriteLine(topN.ToArray().Length);
                }

                reponsitory.Dispose();
            }
        }
    }
}
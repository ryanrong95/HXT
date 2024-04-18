using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Report
{
    public partial class PaymentReport : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var datas = new Needs.Ccs.Services.Views.PayExchangeToYahvReceivableView().GetData("PEA20200624014");



        }

        protected void data()
        {



            List<object> list = new List<object>();

            list.Add(new
            {
                PayDate = "2020-07-02",
                PayExchangeAmount = "1214134",
                ExchangeRate = "7.012",
                CNYAmount = "",
            });

            for (int i = 0; i < 19; i++)
            {
                list.Add(new
                {

                });
            }       

            Response.Write(new
            {
                rows = list.ToArray(),
                total = 20,
            }.Json());
        }

    }
}
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;

namespace Yahv.PvOms.WebApp.Test
{
    public partial class CouponTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //TestCouponCost();
            TestCouponStatisticsTopView();
        }

        void TestCouponCost()
        {
            List<UsedMap> maps = new List<UsedMap>()
            {
                new UsedMap
                {
                    ReceivableID = "Receb20191018000001",
                    CouponID = "Coupon201912050001",
                    Quantity = 1
                },

                new UsedMap
                {
                    ReceivableID = "Receb20191018000003",
                    CouponID = "Coupon201912050002",
                    Quantity = 1
                }
            };

            //管理端调用
            CouponManager.Current["DBAEAB43B47EB4299DD1D62F764E6B6A", "013EEC9B29F8749B2E4E87F707C952E2"].Confirm("Admin00057", maps.ToArray());

            //会员中心调用
            //CouponManager.Current["DBAEAB43B47EB4299DD1D62F764E6B6A", "013EEC9B29F8749B2E4E87F707C952E2"].Pay("UserID", maps.ToArray());
        }

        void TestCouponStatisticsTopView()
        {
            var linq = new Yahv.Services.Views.CouponStatisticsTopView<PvWsOrderReponsitory>()
                       .Where(c => c.Payer == "DBAEAB43B47EB4299DD1D62F764E6B6A" && c.Payee == "013EEC9B29F8749B2E4E87F707C952E2").ToArray();

            var linq2 = CouponManager.Current["DBAEAB43B47EB4299DD1D62F764E6B6A", "013EEC9B29F8749B2E4E87F707C952E2"].ToArray();
        }
    }
}
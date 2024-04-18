using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.WebApp.Test;
using Yahv.Services.Models;

namespace Yahv.PvOms.WebApp.Test
{
    public partial class SortTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitSorted();
        }
        private void SubmitSorted()
        {
            //调用代仓储 库房分拣更新数据接口
            var apisetting = new PvWsOrderApiSetting2();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitSorted;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, new StoreChange
            {
                List = {
                        new ChangeItem() { orderid = "NL02020200410011" },
                }
            });
        }
    }
}
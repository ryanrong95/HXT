using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvWsOrder.WebApp.Test
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        private void LoadData()
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ID = Request.QueryString["id"].Trim();

            //调用代仓储接口
            var apisetting = new PvWsOrderApiSetting2();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitSorted;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, new StoreChange
            {
                List = {
                        new ChangeItem() { orderid = "Order201911140003" }
                }
            });
        }
    }

    /// <summary>
    /// 模拟库房入库调用wsorder接口
    /// </summary>
    public class PvWsOrderApiSetting2
    {
        /// <summary>
        /// 调用系统地址
        /// </summary>
        public string ApiName { get; private set; }

        /// <summary>
        /// 分拣更新数据
        /// </summary>
        public string SubmitSorted { get; private set; }

        /// <summary>
        /// 芯达通变革订单数据
        /// </summary>
        public string SubmitChanged { get; private set; }

        public PvWsOrderApiSetting2()
        {
            ApiName = "PvWsOrderApiUrl";
            SubmitSorted = "Sorted/SubmitSorted";
            SubmitChanged = "Sorted/SubmitChanged";
        }
    }
}
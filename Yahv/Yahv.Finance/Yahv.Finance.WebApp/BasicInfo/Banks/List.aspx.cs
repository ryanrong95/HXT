using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Banks
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> dic_isAccountCost = new Dictionary<string, string>();
            dic_isAccountCost.Add("0", "全部");
            dic_isAccountCost.Add("1", "是");
            dic_isAccountCost.Add("2", "否");
            this.Model.IsAccountCost = dic_isAccountCost.Select(item => new { value = item.Key, text = item.Value });

            Dictionary<string, string> dic_status = new Dictionary<string, string>();
            dic_status.Add("0", "全部");
            dic_status.Add(Underly.GeneralStatus.Normal.GetHashCode().ToString(), "正常");
            dic_status.Add(Underly.GeneralStatus.Closed.GetHashCode().ToString(), "停用");
            this.Model.Statuses = dic_status.Select(item => new { value = item.Key, text = item.Value });
        }

        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string name = Request.QueryString["s_name"];
            string isAccountCost = Request.QueryString["s_isAccountCost"];
            string status = Request.QueryString["s_status"];

            using (var query1 = Erp.Current.Finance.Banks)
            {
                var view = query1;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    view = view.SearchByName(name);
                }

                if (!string.IsNullOrWhiteSpace(isAccountCost))
                {
                    if (isAccountCost == "1" || isAccountCost == "2")
                    {
                        view = view.SearchByIsAccountCost(isAccountCost == "1");
                    }
                }

                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == Underly.GeneralStatus.Normal.GetHashCode().ToString() || status == Underly.GeneralStatus.Closed.GetHashCode().ToString())
                    {
                        view = view.SearchByStatus((Underly.GeneralStatus)(Convert.ToInt32(status)));
                    }
                }

                return view.ToMyPage(page, rows).Json();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void enable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Banks.Enable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "启用", Request.Form["items"]);
            Response.Write((new { success = true, message = "启用成功", }).Json());
        }

        /// <summary>
        /// 停用
        /// </summary>
        protected void disable()
        {
            var array = Request.Form["items"].Split(',');
            Erp.Current.Finance.Banks.Disable(array);
            Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), "停用", Request.Form["items"]);
            Response.Write((new { success = true, message = "停用成功", }).Json());
        }

    }
}
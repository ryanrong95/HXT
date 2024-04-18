using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Vault
{
    /// <summary>
    /// 财务金库列表界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            string Leader = Request.QueryString["Leader"];
            var vaultlist = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Name.Trim();
                vaultlist = vaultlist.Where(t => t.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Leader))
            {
                Leader = Leader.Trim();
                vaultlist = vaultlist.Where(t => t.Leader.Contains(Leader));
            }
            Func<Needs.Ccs.Services.Models.FinanceVault, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                Leader = item.Admin.ByName,
                Summary = item.Summary
            };
            this.Paging(vaultlist, convert);
        }
        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault[id];
            if (del != null)
            {
                del.AbandonSuccess += Del_AbandonSuccess;
                del.Abandon();
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Del_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}

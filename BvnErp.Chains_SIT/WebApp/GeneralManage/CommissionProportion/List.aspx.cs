using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.CommissionProportion
{
    /// <summary>
    /// 提成比例
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.CommissionProportions.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            Func<Needs.Ccs.Services.Models.CommissionProportion, object> convert = item => new
            {
                ID = item.ID,
                RegeisterMonth = item.RegeisterMonth,
                CommissionProportion = item.Proportion,
                Summary = item.Summary
            };
            this.Paging(list, convert);
        }
        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.CommissionProportions[id];
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
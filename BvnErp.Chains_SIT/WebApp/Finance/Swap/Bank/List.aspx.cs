using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap.Bank
{
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
            string Name = Request.QueryString["Name"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(Name))
            {
                Name = Name.Trim();
                list = list.Where(t => t.Name.Contains(Name));
            }
            Func<Needs.Ccs.Services.Models.SwapBank, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                Code = item.Code,
                CreateDate = item.CreateDate.ToShortDateString(),
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
            var del = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks[id];
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
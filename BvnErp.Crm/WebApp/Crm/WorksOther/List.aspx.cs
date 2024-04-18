using Needs.Utils.Serializers;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksOther
{
    /// <summary>
    /// 工作计划展示列表页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var work = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => new { item.ID, item.RealName });
                this.Model.AdminData = work.Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            var works = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther.Where(item => true);
            //对象转化
            Func<NtErp.Crm.Services.Models.WorksOther, object> linq = c => new
            {
                c.ID,
                c.StartDate,
                c.Subject,
                c.Context,
                AdminName = c.Admin.RealName,
                isShow = Needs.Erp.ErpPlot.Current.ID == c.Admin.ID ? true : false,
                StatusName = c.Status.GetDescription(),
            };
            if (!string.IsNullOrWhiteSpace(Name))
            {
                works = works.Where(c => c.Admin.ID == Name);
            }
            this.Paging(works,linq);
        }

        /// <summary>
        /// 数据作废
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var del = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksOther[id];
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
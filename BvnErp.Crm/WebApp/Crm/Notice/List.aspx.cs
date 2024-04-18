using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Notice
{
    public partial class List : Uc.PageBase
    {
        /// <summary>
        /// 公告展示列表页面
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.CurrentAdmin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID).Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            var notices = Needs.Erp.ErpPlot.Current.ClientSolutions.MyNotice;
            //对象转化
            Func<NtErp.Crm.Services.Models.Notice, object> linq = item => new
            {
                item.ID,
                item.Name,
                AdminName = item.Admin.RealName,
                item.Context,
                item.CreateDate,
                IsOwner = item.Admin.ID == Needs.Erp.ErpPlot.Current.ID,
                StatusName = item.Status.GetDescription(),
            };
            this.Paging(notices, linq);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var notice = Needs.Erp.ErpPlot.Current.ClientSolutions.MyNotice[id];
            if(notice != null)
            {
                notice.Abandon();
            }
        }
    }
}
using Needs.Ccs.Services;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class UserCurrentPayApplyList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.ContrNo = Request.QueryString["ContrNo"];
                this.Model.EntryID = Request.QueryString["EntryID"];
                this.Model.OrderID = Request.QueryString["OrderID"];
            }
        }

        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];

            UserCurrentPayApplyListView view = new UserCurrentPayApplyListView(OrderID);
            var userCurrentPayApplyList = view.ToList();

            //在最后增加一个合计行
            //求当前和
            decimal currentPayApplySum = userCurrentPayApplyList.Sum(t => t.CurrentPayApplyAmount);

            userCurrentPayApplyList.Add(new UserCurrentPayApplyListModel
            {
                CurrentPayApplyAmount = currentPayApplySum,
                ApplyTime = new DateTime(1, 1, 1),
            });

            Func<Needs.Ccs.Services.Views.UserCurrentPayApplyListModel, object> convert = item => new
            {
                CurrentPayApplyAmount = item.CurrentPayApplyAmount.ToRound(2),
                ApplyTime = item.ApplyTime < new DateTime(1900, 1, 1) ? string.Empty : item.ApplyTime.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            Response.Write(new
            {
                rows = userCurrentPayApplyList.Select(convert).ToArray(),
            }.Json());
        }


    }
}
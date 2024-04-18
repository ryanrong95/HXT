using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.AttachApproval.Applicant
{
    public partial class UnApprovedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 跟单员待审批列表
        /// </summary>
        protected void UnApprovedListData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.AttachApprovalUnApprovedListViewModel, bool>>)(t => t.ApplicantID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
            
            if (!string.IsNullOrEmpty(orderID))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.AttachApprovalUnApprovedListViewModel, bool>>)(
                    t => t.MainOrderID.Contains(orderID) || t.TinyOrderID.Contains(orderID)));
            }

            int totalCount = 0;
            var view = new Needs.Ccs.Services.Views.AttachApprovalUnApprovedListView();
            var unApproved = view.GetResult(out totalCount, page, rows, lamdas.ToArray());

            Response.Write(new
            {
                rows = unApproved.Select(
                        item => new
                        {
                            OrderControlStepID = item.OrderControlStepID,
                            OrderControlID = item.OrderControlID,
                            ShowOrderID = ShowOrderID(item),
                            ControlTypeInt = item.ControlType.GetHashCode(),
                            ControlTypeDes = item.ControlType.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            ApplicantName = item.ApplicantName,
                            ClientName = item.ClientName,
                            Currency = item.Currency,
                            DeclarePrice = item.DeclarePrice,
                        }
                     ).ToArray(),
                total = totalCount,
            }.Json());
        }

        private string ShowOrderID(Needs.Ccs.Services.Views.AttachApprovalUnApprovedListViewModel item)
        {
            string showOrderID = string.Empty;

            switch (item.ControlType)
            {
                case Needs.Ccs.Services.Enums.OrderControlType.GenerateBillApproval:
                    showOrderID = item.MainOrderID;
                    break;
                case Needs.Ccs.Services.Enums.OrderControlType.DeleteModelApproval:
                    showOrderID = item.TinyOrderID;
                    break;
                case Needs.Ccs.Services.Enums.OrderControlType.ChangeQuantityApproval:
                    showOrderID = item.TinyOrderID;
                    break;
                case Needs.Ccs.Services.Enums.OrderControlType.SplitOrderApproval:
                    showOrderID = item.TinyOrderID;
                    break;
                default:
                    showOrderID = item.TinyOrderID;
                    break;
            }

            return showOrderID;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Invoice
{
    public partial class InvoicedList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            //开票类型选项
            List<object> invoiceTypeOptionList = new List<object>();
            invoiceTypeOptionList.Add(new { value = InvoiceType.Normal, text = InvoiceType.Normal.GetDescription(), });
            invoiceTypeOptionList.Add(new { value = InvoiceType.VAT, text = InvoiceType.VAT.GetDescription(), });
            invoiceTypeOptionList.Add(new { value = InvoiceType.None, text = InvoiceType.None.GetDescription(), });
            this.Model.InvoiceTypeOption = invoiceTypeOptionList;

            //申请人选项
            this.Model.ApplyOption = new Yahv.PvWsOrder.Services.Views.AdminsAll().OrderBy(t => t.RealName)
                .Select(item => new { value = item.ID, text = item.RealName, }).ToArray();
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string CompanyName = Request.QueryString["CompanyName"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string AdminID = Request.QueryString["AdminID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            using (var query = new Yahv.PvWsOrder.Services.Views.InvoicedListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    ClientCode = ClientCode.Trim();
                    view = view.SearchByEnterCode(ClientCode);
                }

                if (!string.IsNullOrEmpty(CompanyName))
                {
                    CompanyName = CompanyName.Trim();
                    view = view.SearchByCompanyName(CompanyName);
                }

                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    view = view.SearchByInvoiceType((Yahv.Underly.InvoiceType)int.Parse(InvoiceType));
                }

                if (!string.IsNullOrEmpty(AdminID))
                {
                    view = view.SearchByAdminID(AdminID);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime begin = DateTime.Parse(StartDate);
                    view = view.SearchByInvoiceNoticeCreateDateBegin(begin);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = DateTime.Parse(EndDate);
                    end = end.AddDays(1);
                    view = view.SearchByInvoiceNoticeCreateDateEnd(end);
                }

                Func<Yahv.PvWsOrder.Services.Views.InvoicedListViewModel, InvoicedListDataModel> convert = item => new InvoicedListDataModel
                {
                    InvoiceNoticeID = item.InvoiceNoticeID,
                    EnterCode = item.EnterCode,
                    CompanyName = item.CompanyName,
                    InvoiceTypeDes = item.InvoiceType.GetDescription(),
                    Amount = item.Amount,
                    InvoiceDeliveryTypeDes = item.InvoiceDeliveryType.GetDescription(),
                    InvoiceNoticeStatusDes = item.InvoiceNoticeStatus.GetDescription(),
                    AdminID = item.AdminID,
                    AdminName = item.AdminName,
                    InvoiceNoticeCreateDateDes = item.InvoiceNoticeCreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    WaybillCode = item.WaybillCode,
                };

                var viewData = view.ToMyPage(convert, page, rows);

                Response.Write(new { total = viewData.Item2, rows = viewData.Item1, }.Json());
            }
        }

        public class InvoicedListDataModel
        {
            /// <summary>
            /// 开票编号
            /// </summary>
            public string InvoiceNoticeID { get; set; }

            /// <summary>
            /// 客户编号
            /// </summary>
            public string EnterCode { get; set; }

            /// <summary>
            /// 公司名称
            /// </summary>
            public string CompanyName { get; set; }

            /// <summary>
            /// 开票类型
            /// </summary>
            public string InvoiceTypeDes { get; set; }

            /// <summary>
            /// 含税金额
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// 发票交付方式
            /// </summary>
            public string InvoiceDeliveryTypeDes { get; set; }

            /// <summary>
            /// 开票状态
            /// </summary>
            public string InvoiceNoticeStatusDes { get; set; }

            /// <summary>
            /// 申请人ID
            /// </summary>
            public string AdminID { get; set; }

            /// <summary>
            /// 申请人姓名
            /// </summary>
            public string AdminName { get; set; }

            /// <summary>
            /// 申请日期
            /// </summary>
            public string InvoiceNoticeCreateDateDes { get; set; }

            /// <summary>
            /// 发票运单
            /// </summary>
            public string WaybillCode { get; set; }
        }
    }
}
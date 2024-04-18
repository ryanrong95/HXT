using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Invoices
{
    public partial class List : ErpParticlePage
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
            //开票状态选项
            this.Model.InvoiceStatusOption = ExtendsEnum.ToArray<InvoiceEnum>()
                .Select(item => new { value = (int)item, text = item.GetDescription() });
            //开票类型选项
            this.Model.InvoiceTypeOption = ExtendsEnum.ToArray(InvoiceType.Unkonwn)
                .Select(item => new { value = (int)item, text = item.GetDescription() });
            //交付方式选项
            this.Model.InvoiceDeliveryOption = ExtendsEnum.ToArray(Underly.InvoiceDeliveryType.UnKnown)
                .Select(item => new { value = (int)item, text = item.GetDescription() });
            //申请人选项
            this.Model.ApplyOption = new { value = "", text = "", };
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
            string Title = Request.QueryString["Title"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string InvoiceStatus = Request.QueryString["InvoiceStatus"];
            string InvoiceDeliveryType = Request.QueryString["DeliveryType"];
            string AdminID = Request.QueryString["AdminID"];

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string InvoiceStartDate = Request.QueryString["InvoiceStartDate"];
            string InvoiceEndDate = Request.QueryString["InvoiceEndDate"];

            using (var query = new SzMvc.Services.Views.Invoices_Show_View())
            {
                var view = query;

                #region 条件查询
                if (!string.IsNullOrEmpty(Title))
                {
                    view = view.SearchByTitle(Title.Trim());
                }
                if (!string.IsNullOrEmpty(InvoiceStatus))
                {
                    view = view.SearchByStatus((InvoiceEnum)int.Parse(InvoiceStatus));
                }
                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    view = view.SearchByInvoiceType((InvoiceType)int.Parse(InvoiceType));
                }
                if (!string.IsNullOrEmpty(InvoiceDeliveryType))
                {
                    view = view.SearchByInvoiceDeliveryType((Underly.InvoiceDeliveryType)int.Parse(InvoiceDeliveryType));
                }
                if (!string.IsNullOrEmpty(AdminID))
                {
                    view = view.SearchByAdminID(AdminID);
                }
                if (!(string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate)))
                {
                    DateTime? begin = null;
                    DateTime? end = null;
                    if (!string.IsNullOrEmpty(StartDate))
                    {
                        begin = DateTime.Parse(StartDate);
                    }
                    if (!string.IsNullOrEmpty(EndDate))
                    {
                        end = DateTime.Parse(EndDate);
                    }
                    view = view.SearchByCreateDate(begin, end);
                }
                if (!(string.IsNullOrEmpty(InvoiceStartDate) && string.IsNullOrEmpty(InvoiceEndDate)))
                {
                    DateTime? begin = null;
                    DateTime? end = null;
                    if (!string.IsNullOrEmpty(InvoiceStartDate))
                    {
                        begin = DateTime.Parse(InvoiceStartDate);
                    }
                    if (!string.IsNullOrEmpty(InvoiceEndDate))
                    {
                        end = DateTime.Parse(InvoiceEndDate);
                    }
                    view = view.SearchByInvoiceDate(begin, end);
                }

                #endregion

                var result = view.ToMyPage(page, rows);
                var linq = result.Item1.Select(t => new
                {
                    t.ID,
                    t.Title,
                    t.EnterCode,
                    t.InvoiceNos,
                    t.AdminName,
                    Amount = t.Amount.ToString("f2"),
                    Difference = t.Difference?.ToString("f2"),
                    InvoiceTypeDec = t.InvoiceType.GetDescription(),
                    InvoiceDeliveryTypeDec = t.InvoiceDeliveryType.GetDescription(),
                    InvoiceNoticeStatusDec = t.InvoiceNoticeStatus.GetDescription(),
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    InvoiceDate = t.InvoiceDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                });

                Response.Write(new { rows = linq, total = result.Item2 }.Json());
            }
        }
    }
}

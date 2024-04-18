using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.PvWsOrder.Services.Views.Alls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders.Clients
{
    public partial class ListClientBill : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            this.Model.ClientData = Erp.Current.WsOrder.MyWsClients.Select(t => new
            {
                value = t.Name,
                text = t.Name,
            });
            //币种数据
            this.Model.currencyData = ExtendsEnum.ToArray(Currency.Unknown)
                .Select(item => new { value = (int)item, text = item.GetCurrency().ShortName + " " + item.GetDescription() })
                .Where(t => t.value == (int)Currency.CNY || t.value == (int)Currency.HKD);
            //发票状态
            this.Model.isInvoiceData = new List<object>() {
                new
                {
                    value = "未开票",
                    text = "未开票",
                },
                new
                {
                    value = "已开票",
                    text = "已开票",
                }
            };
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            //查询参数
            var ClientName = Request.QueryString["ClientName"];
            var ClientCode = Request.QueryString["ClientCode"];
            var CurrencyStr = Request.QueryString["Currency"];
            var IsInvoiced = Request.QueryString["IsInvoiced"];

            var bills = new Bills_Show_View(Erp.Current);

            if (!string.IsNullOrWhiteSpace(ClientName))
            {
                bills = bills.SearchByClientName(ClientName);
            }
            if (!string.IsNullOrWhiteSpace(ClientCode))
            {
                ClientCode = ClientCode.Trim();
                bills = bills.SearchByClientCode(ClientCode);
            }
            if (!string.IsNullOrWhiteSpace(CurrencyStr))
            {
                var currency = (Currency)Enum.Parse(typeof(Currency), CurrencyStr);
                bills = bills.SearchByCurrency(currency);
            }
            if (!string.IsNullOrWhiteSpace(IsInvoiced))
            {
                IsInvoiced = IsInvoiced.Trim();
                if (IsInvoiced == "未开票")
                {
                    bills = bills.SearchByIsInvoice(false);
                }
                else
                {
                    bills = bills.SearchByIsInvoice(true);
                }
            }
            var linq = bills.ToMyPage(page, rows);
            var query = linq.Item1.OrderByDescending(t => t.CreateDate).Select(t => new
            {
                t.ID,
                t.ClientID,
                t.ClientName,
                t.EnterCode,
                Currency = t.Currency.GetDescription(),
                Price = t.Currency == Currency.CNY ? t.CnyPrice.ToString("f2") : t.HkdPrice.ToString("f2"),
                IsInvoice = t.IsInvoice ? "已开票" : "未开票",
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                Creater = t.Creator,
            });

            return new
            {
                rows = query.ToArray(),
                total = linq.Item2
            };
        }
    }
}
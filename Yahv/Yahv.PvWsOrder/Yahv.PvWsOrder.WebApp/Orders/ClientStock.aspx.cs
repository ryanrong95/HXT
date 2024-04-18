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
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders
{
    public partial class ClientStock : ErpParticlePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadComboboxData();
            }
        }

        private void LoadComboboxData()
        {
            //币种数据
            this.Model.currencyData = ExtendsEnum.ToArray(Currency.Unknown)
                .Select(item => new { Value = (int)item, Text = item.GetCurrency().ShortName + " " + item.GetDescription() });
        }

        protected object data()
        {
            var ClientID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.HKStorages.Where(item => item.ClientID == ClientID).Where(Predicate()).ToArray();

            var linq = query.Select(item => new
            {
                ID = item.ID,
                CreateDate = item.EnterDate,
                Manufacturer = item.Manufacturer,
                PartNumber = item.PartNumber,
                DateCode = item.DateCode,
                Origin = item.Origin,
                Quantity = item.Quantity,
                Currency = item.Currency,
                TotalPrice = item.TotalPriceDec,
                GrossWeight = 0.00M,
                Volume = 0.00M,
                Supplier = item.Supplier,

                CreateDateStr = item.EnterDate.ToString("yyyy-MM-dd"),
                OriginDec = item.OriginDec,
                CurrencyDec = item.CurrencyDec,
            });
            return linq.Where(item => item.Quantity != 0);
        }

        Expression<Func<Storage, bool>> Predicate()
        {
            Expression<Func<Storage, bool>> predicate = item => true;

            //查询参数
            var Manufacturer = Request.QueryString["Manufacturer"];
            var PartNumber = Request.QueryString["PartNumber"];
            var DateCode = Request.QueryString["DateCode"];
            var Currency = Request.QueryString["Currency"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrWhiteSpace(Manufacturer))
            {
                Manufacturer = Manufacturer.Trim();
                predicate = predicate.And(item => item.Manufacturer.Contains(Manufacturer));
            }
            if (!string.IsNullOrWhiteSpace(PartNumber))
            {
                PartNumber = PartNumber.Trim();
                predicate = predicate.And(item => item.PartNumber.Contains(PartNumber));
            }
            if (!string.IsNullOrWhiteSpace(DateCode))
            {
                predicate = predicate.And(item => item.DateCode.Contains(DateCode));
            }
            if (!string.IsNullOrWhiteSpace(Currency))
            {
                var currency = (Currency)int.Parse(Currency);
                predicate = predicate.And(item => item.Currency == currency);
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.EnterDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.EnterDate < end);
            }
            return predicate;
        }
    }
}
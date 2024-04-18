using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Stocks.Temporary
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected object data()
        {
            var query = Erp.Current.WsOrder.TempStorages.GetWaybills()
                .OrderByDescending(item => item.CreateDate).AsQueryable();
            //查询表达式
            Expression<Func<Yahv.Services.Models.Waybill, bool>> predicate = Predicate();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return this.Paging(query, t => new
            {
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                ID = t.ID,
                Code = t.Code,
                Subcodes = t.Subcodes,
                EnterCode = t.EnterCode,
                Supplier = t.Supplier,
                Place = t.Consignor.Place,
                TotalParts = t.TotalParts,
                Address = t.Consignor.Address,
            });
        }

        Expression<Func<Yahv.Services.Models.Waybill, bool>> Predicate()
        {
            Expression<Func<Yahv.Services.Models.Waybill, bool>> predicate = item => true;

            //查询参数
            var EnterCode = Request.QueryString["EnterCode"];
            var WaybillCode = Request.QueryString["WaybillCode"];
            var Supplier = Request.QueryString["Supplier"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            if (!string.IsNullOrWhiteSpace(EnterCode))
            {
                EnterCode = EnterCode.Trim();
                predicate = predicate.And(item => item.EnterCode.Contains(EnterCode));
            }
            if (!string.IsNullOrWhiteSpace(WaybillCode))
            {
                WaybillCode = WaybillCode.Trim();
                predicate = predicate.And(item => item.Code.Contains(WaybillCode)|| item.Subcodes.Contains(WaybillCode));
            }
            if (!string.IsNullOrWhiteSpace(Supplier))
            {
                Supplier = Supplier.Trim();
                predicate = predicate.And(item => item.Supplier.Contains(Supplier));
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }

            return predicate;
        }
    }
}
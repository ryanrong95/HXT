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

namespace Yahv.PvOms.WebApp.Stocks.Adopt
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected object data()
        {
            var query =  Erp.Current.WsOrder.MyTempStorage.Where(item => true);

            //var query = Erp.Current.WsOrder.TempStorages.GetWaybills()
            //    .OrderByDescending(item => item.CreateDate).AsQueryable();
            //查询表达式
            Expression<Func<Yahv.PvWsOrder.Services.Models.AdoptTmepStock, bool>> predicate = Predicate();
            if (predicate != null)
            {
                query = query.Where(predicate).OrderByDescending(t=>t.CreateDate);
            }
            DateTime dtTommorow = DateTime.Now.AddDays(1);
            return this.Paging(query, t => new
            {
                ID = t.ID,
                EnterCode = t.EnterCode,
                CompanyName = t.CompanyName,
                ShelveID = t.ShelveID,
                WaybillCode = t.WaybillCode,
                Quantity = t.Quantity,
                Summary = t.Summary,
                TempStatus = t.TempStatus.GetDescription(),
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                StockDays = (dtTommorow - t.CreateDate.Date).Days

            });
        }

        Expression<Func<Yahv.PvWsOrder.Services.Models.AdoptTmepStock, bool>> Predicate()
        {
            Expression<Func<Yahv.PvWsOrder.Services.Models.AdoptTmepStock, bool>> predicate = item => true;

            //查询参数
            var EnterCode = Request.QueryString["EnterCode"];
            var WaybillCode = Request.QueryString["WaybillCode"];
            var CompanyName = Request.QueryString["CompanyName"];
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
                predicate = predicate.And(item => item.WaybillCode.Contains(WaybillCode));
            }
            if (!string.IsNullOrWhiteSpace(CompanyName))
            {
                CompanyName = CompanyName.Trim();
                predicate = predicate.And(item => item.CompanyName.Contains(CompanyName));
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
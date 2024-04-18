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
using Yahv.Services;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Stocks
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
        }
        private void LoadComboboxData()
        {
            List<WhSettings> warehouses = new List<WhSettings>();
            warehouses.Add(WhSettings.HK);
            warehouses.Add(WhSettings.SZ);
            this.Model.Warehouse = warehouses.Select(t => new
            {
                value = t.ID,
                text = t.Name,
            });
        }
        protected object data()
        {
            var query = Erp.Current.WsOrder.Storages.AsQueryable();
            //查询表达式
            Expression<Func<Storage, bool>> predicate = Predicate();
            if (predicate != null)
            {
                query = query.Where(predicate).Where(t => t.Quantity > 0).OrderByDescending(item => item.EnterDate);
            }
            return this.Paging(query, t => new
            {
                ID = t.ID,
                ClientName = t.ClientName,
                EnterCode = t.EnterCode,
                Manufacturer = t.Manufacturer,
                PartNumber = t.PartNumber,
                DateCode = t.DateCode,
                Origin = t.OriginDec,
                Total = t.Total, //总库存数量
                Quantity = t.Quantity,//可用数量 
                Currency = t.Currency?.GetDescription(),
                UnitPrice = t.UnitPrice,
                CreateDate = t.EnterDate.ToString("yyyy-MM-dd"),
                Supplier = t.Supplier,
                Warehouse = t.WareHouseID.Contains("HK") ? "香港" : "深圳",
            });
        }

        Expression<Func<Storage, bool>> Predicate()
        {
            Expression<Func<Storage, bool>> predicate = item => true;

            //查询参数
            var clientName = Request.QueryString["ClientName"];
            var entryCode = Request.QueryString["EntryCode"];
            var manufacturer = Request.QueryString["Manufacturer"];
            var partNumber = Request.QueryString["PartNumber"];
            var dateCode = Request.QueryString["DateCode"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            var Warehouse = Request.QueryString["Warehouse"];

            if (!string.IsNullOrWhiteSpace(clientName))
            {
                clientName = clientName.Trim();
                predicate = predicate.And(item => item.ClientName.Contains(clientName));
            }
            if (!string.IsNullOrWhiteSpace(entryCode))
            {
                entryCode = entryCode.Trim();
                predicate = predicate.And(item => item.EnterCode.Contains(entryCode));
            }
            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                manufacturer = manufacturer.Trim();
                predicate = predicate.And(item => item.Manufacturer.Contains(manufacturer));
            }
            if (!string.IsNullOrWhiteSpace(partNumber))
            {
                partNumber = partNumber.Trim();
                predicate = predicate.And(item => item.PartNumber.Contains(partNumber));
            }
            if (!string.IsNullOrWhiteSpace(dateCode))
            {
                dateCode = dateCode.Trim();
                predicate = predicate.And(item => item.DateCode.Contains(dateCode));
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
            if (!string.IsNullOrWhiteSpace(Warehouse))
            {
                predicate = predicate.And(item => item.WareHouseID.Contains(Warehouse));
            }

            return predicate;
        }
    }
}
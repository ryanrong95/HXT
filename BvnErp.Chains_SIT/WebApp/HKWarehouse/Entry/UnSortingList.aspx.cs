using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq.Generic;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Entry
{
    /// <summary>
    /// 入库通知--香港库房
    /// 入库通知列表查询界面
    /// </summary>
    public partial class UnSortingList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string Supplier = Request.QueryString["Supplier"];
            string EntryNumber = Request.QueryString["EntryNumber"];
            string ClientName = Request.QueryString["ClientName"];
            string Model = Request.QueryString["Model"];

            #region 旧视图
            /*
            IQueryable<HKEntryNotice> entryNoticeView;
            if (string.IsNullOrEmpty(Model) == false)
            {
                entryNoticeView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNotice;
            }
            else
            {
                entryNoticeView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticeSimple;
            }

            var data = entryNoticeView.Where(entryEntry => entryEntry.EntryNoticeStatus != EntryNoticeStatus.Sealed);
            if (!string.IsNullOrEmpty(Supplier))
            {
                data = data.Where(entryEntry => entryEntry.Order.OrderConsignee.ClientSupplier.ChineseName.Contains(Supplier));
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                data = data.Where(entryEntry => entryEntry.Order.Client.Company.Name.Contains(ClientName));
            }
            if (!string.IsNullOrEmpty(EntryNumber))
            {
                data = data.Where(entryEntry => entryEntry.ClientCode == EntryNumber);
            }
            if (!string.IsNullOrEmpty(Model))
            {
                data = data.Where(entryEntry => entryEntry.HKItems.Select(item => item.OrderItem.Product.Model).Contains(Model));
            }

            data = data.OrderByDescending(entryEntry => entryEntry.CreateDate);

            Func<EntryNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,
                EntryNumber = item.ClientCode,
                ClientName = item.Order.Client.Company.Name,
                SupplierName = item.Order.OrderConsignee.ClientSupplier.ChineseName,
                Type = item.Order.OrderConsignee.Type.GetDescription(),
                Status = item.EntryNoticeStatus.GetDescription()
            };
            this.Paging(data, convert);
            */
            #endregion

            #region 新视图

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<HKEntryNotice, bool>> expression = item => item.EntryNoticeStatus != EntryNoticeStatus.Sealed;

            if (!string.IsNullOrEmpty(Supplier))
            {
                Expression<Func<HKEntryNotice, bool>> lambda = item => item.Order.OrderConsignee.ClientSupplier.ChineseName.Contains(Supplier);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                Expression<Func<HKEntryNotice, bool>> lambda = item => item.Order.Client.Company.Name.Contains(ClientName);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(EntryNumber))
            {
                Expression<Func<HKEntryNotice, bool>> lambda = item => item.ClientCode == EntryNumber;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(Model))
            {
                Expression<Func<HKEntryNotice, bool>> lambda = item => item.HKItems.Select(i => i.OrderItem.Model).Contains(Model);
                lambdas.Add(lambda);
            }

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            PageList<HKEntryNotice> entryNoticeList;
            if (!string.IsNullOrEmpty(Model))
                entryNoticeList = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticesWithItemsAll.GetPageList(page, rows, expression, lambdas.ToArray());
            else
                entryNoticeList = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKEntryNoticesAll.GetPageList(page, rows, expression, lambdas.ToArray());

            Response.Write(new
            {
                rows = entryNoticeList.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.Order.ID,
                            EntryNumber = item.ClientCode,
                            ClientName = item.Order.Client.Company.Name,
                            SupplierName = item.Order.OrderConsignee.ClientSupplier.ChineseName,
                            Type = item.Order.OrderConsignee.Type.GetDescription(),
                            Status = item.EntryNoticeStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm")
                        }
                     ).ToArray(),
                total = entryNoticeList.Total,
            }.Json());

            #endregion
        }
    }
}
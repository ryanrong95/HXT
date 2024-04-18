using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Linq;

namespace WebApp.SZWarehouse.Entry
{
    public partial class EntryedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 香港出库通知
        /// </summary>
        protected void data()
        {
            //新写法
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.QueryString["OrderID"];
            string EntryNumber = Request.QueryString["EntryNumber"];

            var view = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.NewSZEntryNotice;
            var predicate = PredicateBuilder.Create<Needs.Wl.Warehouse.Services.PageModels.SZWarehouseEntryListModel>();
            predicate = predicate.And(item => item.EntryNoticeStatus == Needs.Wl.Models.Enums.EntryNoticeStatus.Boxed);

            if (string.IsNullOrEmpty(OrderID) == false)
            {
                predicate = predicate.And(x => x.OrderID.Contains(OrderID));
            }
            if (string.IsNullOrEmpty(EntryNumber) == false)
            {
                predicate = predicate.And(x => x.ClientCode.Contains(EntryNumber));
            }

            view.Predicate = predicate;
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;

            int recordCount = view.RecordCount;

            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                DecHeadID = item.DecHeadID,
                ClientCode = item.ClientCode,
                ClientName = item.CompanyName,
                PackNo = item.PackNo,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                NoticeStatus = item.EntryNoticeStatus == Needs.Wl.Models.Enums.EntryNoticeStatus.Boxed ? "已入库" : item.EntryNoticeStatus.GetDescription(),
            });

            Response.Write(new
            {
                rows = list.ToArray(),
                total = recordCount,
            }.Json());
        }
    }
}
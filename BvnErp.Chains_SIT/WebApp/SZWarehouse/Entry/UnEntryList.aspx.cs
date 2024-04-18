using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
//using Needs.Utils.Npoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WebApp.Ccs.Utils;
namespace WebApp.SZWarehouse.Entry
{
    public partial class UnEntryList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 香港出库通知
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string EntryNumber = Request.QueryString["EntryNumber"];

            var entryNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZEntryNotice
                .Where(item => item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed).AsQueryable();

            if (string.IsNullOrEmpty(OrderID) == false)
            {
                entryNotices = entryNotices.Where(x => x.Order.ID.Contains(OrderID));
            }
            if (string.IsNullOrEmpty(EntryNumber) == false)
            {
                entryNotices = entryNotices.Where(x => x.Order.Client.ClientCode.Contains(EntryNumber));
            }

            entryNotices = entryNotices.OrderByDescending(x => x.CreateDate);

            Func<SZEntryNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,
                DecHeadID = item.DecHead.ID,
                ClientCode = item.ClientCode,
                ClientName = item.Order.Client.Company.Name,
                PackNo = item.DecHead.PackNo,
                CreateDate = item.CreateDate.ToShortDateString(),
                NoticeStatus = item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed ? "待入库" : item.EntryNoticeStatus.GetDescription(),
            };
            this.Paging(entryNotices, convert);
        }

        /// <summary>
        /// 导出
        /// </summary>
        protected void Export()
        {

            string OrderID = Request.Form["OrderID"];
            string EntryNumber = Request.Form["EntryNumber"];



            var entryNotices = new SZEntryNoticeItemExportView()
                .Where(item => item.EntryNoticeStatus == EntryNoticeStatus.UnBoxed).AsQueryable();

            if (string.IsNullOrEmpty(OrderID) == false)
            {
                entryNotices = entryNotices.Where(x => x.EntryNotice.Order.ID.Contains(OrderID));
            }
            if (string.IsNullOrEmpty(EntryNumber) == false)
            {
                entryNotices = entryNotices.Where(x => x.EntryNotice.Order.Client.ClientCode.Contains(EntryNumber));
            }

            var lstentryNotices = entryNotices.ToList();
            var result = from item in lstentryNotices
                         orderby item.DecList.CaseNo
                         group item by new { item.DecList.CaseNo,item.EntryNotice.ClientCode} into g
                         select new
                         {
                             箱号=g.Key.CaseNo,
                             客户编号 = g.Key.ClientCode,
                             客户名称 = g.FirstOrDefault().EntryNotice.Order.Client.Company.Name,
                             毛重 = g.Sum(item => item.DecList.GrossWt).Value.ToString("0.#####"),
                             净重 = g.Sum(item => item.DecList.NetWt).Value.ToString("0.#####"),
                             创建时间 = g.FirstOrDefault().CreateDate.ToShortDateString()
                         };

            IWorkbook workbook = ExcelFactory.Create();
            Needs.Utils.Npoi.NPOIHelper npoi = new Needs.Utils.Npoi.NPOIHelper(workbook);
            int[] columnsWidth = { 20, 10, 30, 10, 10, 10 };
            npoi.EnumrableToExcel(result,0, columnsWidth);

            var fileName = DateTime.Now.Ticks + ".xlsx";
            FileDirectory file = new FileDirectory(fileName);
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            file.CreateDataDirectory();
            //保存文件
            npoi.SaveAs(file.FilePath);

            Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
          
        }

    }
}
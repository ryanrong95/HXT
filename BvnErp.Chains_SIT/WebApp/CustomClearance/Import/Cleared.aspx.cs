using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.OrderWaybill
{
    public partial class Cleared : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string StartTime = Request.QueryString["StartTime"];
            string EndTime = Request.QueryString["EndTime"];

            //var data = new Needs.Ccs.Services.Views.OrderWaybillView().OrderByDescending(x => x.ArrivalDate).ThenBy(x => x.Carrier.Code)
            //    .Where(item => item.HKDeclareStatus == HKDeclareStatus.Declared);
            var data = new Needs.Ccs.Services.Views.HKClearedDataNew().OrderByDescending(x => x.ArrivalDate).ThenBy(x => x.Carrier.Code).AsQueryable();
            if (!string.IsNullOrEmpty(StartTime))
            {
                data = data.Where(item => item.ArrivalDate.CompareTo(StartTime) >= 0);
            }
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime dtEnd = Convert.ToDateTime(EndTime).AddDays(1);
                data = data.Where(item => item.ArrivalDate.CompareTo(dtEnd) <= 0);
            }

            Func<Needs.Ccs.Services.Models.OrderWaybill, object> linq = item => new
            {
                ID = item.ID,
                WaybillCode = item.WaybillCode,
                CarrierName = item.Carrier.Code,
                ArrivalDate = item.ArrivalDate.ToString("yyyy-MM-dd"),
                TotalCount = item.TotalCount,
                TotalPrice = item.TotalPrice,
                Currency = item.Currency,
                Status = item.HKDeclareStatus.GetDescription(),
            };
            this.Paging(data, linq);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void Export()
        {
            try
            {
                //写入数据
                string StartTime = Request.Form["StartTime"];
                string EndTime = Request.Form["EndTime"];
                //var data = new Needs.Ccs.Services.Views.OrderWaybillView().OrderByDescending(x => x.ArrivalDate).ThenBy(x => x.Carrier.Code)
                //    .Where(item => item.HKDeclareStatus == HKDeclareStatus.Declared);
                var data = new Needs.Ccs.Services.Views.HKClearedDataNew().OrderByDescending(x => x.ArrivalDate).ThenBy(x => x.Carrier.Code).AsQueryable();
                if (!string.IsNullOrEmpty(StartTime))
                {
                    data = data.Where(item => item.ArrivalDate.CompareTo(StartTime) >= 0);
                }
                if (!string.IsNullOrEmpty(EndTime))
                {
                    DateTime dtEnd = Convert.ToDateTime(EndTime).AddDays(1);
                    data = data.Where(item => item.ArrivalDate.CompareTo(dtEnd) <= 0);
                }

                Func<Needs.Ccs.Services.Models.OrderWaybill, object> linq = item => new
                {
                    //ID = item.ID,
                    ArrivalDate = item.ArrivalDate.ToString("yyyy-MM-dd"),
                    WaybillCode = item.WaybillCode,
                    CarrierName = item.Carrier.Code,
                    TotalCount = item.TotalCount,
                    TotalPrice = item.TotalPrice,
                    Currency = item.Currency,
                    Status = item.HKDeclareStatus.GetDescription(),
                };

                //数据源
                List<string> columns = new List<string>();
                columns.Add("到港日期");
                columns.Add("运单号");
                columns.Add("快递公司");
                columns.Add("产品总数量");
                columns.Add("产品总金额");
                columns.Add("币种");
                columns.Add("状态");

                DataTable table = NPOIHelper.JsonToDataTable(data.Select(linq).ToArray().Json());
                var NPOI = NPOIContext.Current;
                NPOI.SetColumns(columns);
                NPOI.DataSource = table;
                NPOI.WriteToExcel();
                string fileName = DateTime.Now.Ticks.ToString() + ".xls";//以时间戳进行导出文件的命名

                //创建文件夹
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                NPOI.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
    }
}
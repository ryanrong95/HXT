using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NPOI.Util;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance
{
    public partial class UnSwapList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();
        }

        /// <summary>
        /// 加载换汇通知
        /// </summary>
        protected void data()
        {
            string BankName = Request.QueryString["BankName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.AsQueryable()
                .Where(item => item.SwapStatus == SwapStatus.ApprovedAudit ||  item.SwapStatus == SwapStatus.Auditing || item.SwapStatus == SwapStatus.RefuseAudit);//SwapStatus.Auditing

            if (!string.IsNullOrEmpty(BankName))
            {
                BankName = BankName.Trim();
                notices = notices.Where(t => t.BankName == BankName);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                notices = notices.Where(t => t.CreateDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                notices = notices.Where(t => t.CreateDate < end);
            }

            Func<SwapNotice, object> convert = item => new
            {
                ID = item.ID,
                Creator = item.Admin.RealName,
                item.Currency,
                item.TotalAmount,
                item.BankName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                SwapStatus = item.SwapStatus.GetDescription(),
                SwapStatusInt = item.SwapStatus,
                item.ConsignorCode,
                item.uid
            };
            this.Paging(notices, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            try
            {
                string SwapNoitceID = Request.Form["ID"];
                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.AsQueryable()
                    .Where(item => item.ID == SwapNoitceID).FirstOrDefault();
                notice.Cancel();
                Response.Write((new { success = true, message = "取消成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "取消失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出换汇文件
        /// </summary>
        protected void ExportSwapFiles()
        {
            try
            {
                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipSwapFile);
                file.CreateDataDirectory();

                string ID = Request.Form["ID"];
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];
                var zipFileName = "SwapDownload.zip";

                var files = new List<string>();
                //2.返回PDF文件
                if (apply.BankName.Contains("星展"))
                {
                    files = apply.ToXingzhanPDf();
                }

                if (apply.BankName.Contains("汇丰"))
                {
                    files = apply.ToHuiFengPDf();
                }

                if (apply.BankName.Contains("渣打"))
                {
                    files = apply.ToSCBPDf();
                }
                else
                {
                    files = apply.ToNongYePDf();
                }
                //3.压缩文件并下载
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

    }
}
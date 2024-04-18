using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.Notice
{
    public partial class ExcelList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

       

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            Response.Write(new
            {
                rows = 0,
                total = 0
            }.Json());
        }

        protected void DownloadExcel()
        {
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string Origin = Request.Form["Origin"];
            try
            {
                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                file.CreateDataDirectory();

                var heads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(t=>t.IsSuccess).OrderByDescending(item => item.ContrNo).ToList();

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    heads = heads.Where(t => t.DDate >= from).ToList();
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate).AddDays(1);
                    heads = heads.Where(t => t.DDate <= to).ToList();
                }

                IcgooCheckExcelList excel = new IcgooCheckExcelList(heads, Origin);
                excel.OrderType = OrderType.Icgoo;
                string FileName = DateTime.Now.ToString("yyyyMMddHHmmsss") + ".xlsx";
                string DomainUrl = System.Configuration.ConfigurationManager.AppSettings["DomainUrl"];
                string FileAddress = file.FileUrl.Replace(DomainUrl, "");
                excel.setFilePath(FileAddress.Substring(1, FileAddress.Length - 1));
                excel.SaveAs(FileName);
                Response.Write(new { result = true, info = "导出成功", url = file.FileUrl + FileName }.Json());
            }
            catch (Exception ex)
            {
                Response.Write(new { result = false, info = "保存错误" + ex.ToString() }.Json());
            }
        }
    }
}
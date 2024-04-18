using Needs.Ccs.Services;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.GeneralManage.DeclarationStatistics
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化数据加载
        /// </summary>
        protected void LoadData()
        {
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员" && manager.Admin.RealName != "张庆永").
                Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();
        }
        protected void data()
        {
            string SaleManID = Request.QueryString["SaleManID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            var from = DateTime.Parse(StartDate);
            var to = DateTime.Parse(EndDate);

            var lists = new Needs.Ccs.Services.Views.DeclarationStatisticsView().ToArray();
            if (!string.IsNullOrEmpty(SaleManID))
            {
                lists = lists.Where(x => x.ID == SaleManID).ToArray();
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                lists = lists.Where(x => x.OrderDate >= from).ToArray();
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                lists = lists.Where(x => x.OrderDate < to.AddDays(1)).ToArray();
            }
            var listItems = from detail in lists
                            group detail by new
                            {
                                detail.ID,
                                detail.Name,
                                detail.Currency
                            } into Item
                            select new Needs.Ccs.Services.Models.DeclarationStatistics
                            {
                                ID = Item.Key.ID,
                                Name = Item.Key.Name,
                                Currency = Item.Key.Currency,
                                TotalDeclarePrice = Item.Sum(t => t.DeclarePrice)
                            };

            Func<Needs.Ccs.Services.Models.DeclarationStatistics, object> convert = t => new
            {
                t.ID,//业务员ID
                t.Name,
                t.Currency,
                TotalDeclarePrice = t.TotalDeclarePrice.ToRound(2),

            };

            //合计信息
            var total = from cur in lists
                        group cur by new { cur.Currency } into item
                        select new
                        {
                            Currency = item.Key.Currency,
                            TotalDeclarePrice = item.Sum(t => t.DeclarePrice)
                        };

            var message = "合计：";
            foreach (var r in total)
            {
                message += "&nbsp;&nbsp;&nbsp;&nbsp;" + r.TotalDeclarePrice.ToString("0.00") + "&nbsp;" + r.Currency;
            }

            Response.Write(new { rows = listItems.Select(convert).ToArray(), arr = message }.Json());
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string SaleManID = Request.Form["SaleManID"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                var from = DateTime.Parse(StartDate);
                var to = DateTime.Parse(EndDate);

                var lists = new Needs.Ccs.Services.Views.DeclarationStatisticsView().ToArray();
                if (!string.IsNullOrEmpty(SaleManID))
                {
                    lists = lists.Where(x => x.ID == SaleManID).ToArray();
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    lists = lists.Where(x => x.OrderDate >= from).ToArray();
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    lists = lists.Where(x => x.OrderDate < to.AddDays(1)).ToArray();
                }
                var listItems = from detail in lists
                                group detail by new
                                {
                                    detail.ID,
                                    detail.Name,
                                    detail.Currency
                                } into Item
                                select new Needs.Ccs.Services.Models.DeclarationStatistics
                                {
                                    ID = Item.Key.ID,
                                    Name = Item.Key.Name,
                                    Currency = Item.Key.Currency,
                                    TotalDeclarePrice = Item.Sum(t => t.DeclarePrice)
                                };

                #region 业务量统计
                var detailData = listItems.Select(d => new
                {
                    业务员 = d.Name,
                    币种 = d.Currency,
                    订单总金额 = d.TotalDeclarePrice.ToRound(2),

                });
                #endregion

                //文件
                string filename = "业务量统计表" + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDic5 = new FileDirectory(filename);
                fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                fileDic5.CreateDataDirectory();

                var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportSalesDeclareTotal);
                using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(file);
                    NPOIHelper npoi = new NPOIHelper(workbook);

                    npoi.SetSheet("业务量统计");
                    npoi.GenerateExcelByTemplate(detailData, 1);
                    npoi.SaveAs(fileDic5.FilePath);
                }

                Response.Write((new { success = true, message = "导出成功", url = fileDic5.FileUrl }).Json());

            }
            catch (Exception ex)
            {

            }

        }
    }
}
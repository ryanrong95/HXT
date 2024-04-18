using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.HKWarehouse.Report
{
    public partial class FeeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.FeeType = EnumUtils.ToDictionary<WarehousePremiumType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string clientName = Request.QueryString["ClientName"];
            string feeType = Request.QueryString["FeeType"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            var fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumsAll;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> expression = item => item.WarehouseType == WarehouseType.HongKong;

            #region 查询条件
            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.OrderID.Contains(orderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.Company.Name.Contains(clientName)).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(feeType))
            {
                int intFeeType = Int32.Parse(feeType);
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => (int)item.WarehousePremiumType == intFeeType;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var feelist = fees.GetPageList(page, rows, expression, lambdas.ToArray());

            Response.Write(new
            {
                rows = feelist.Select(
                        item => new
                        {
                            ID = item.ID,
                            OrderID = item.OrderID,
                            ClientCode = item.Client.ClientCode,
                            ClientName = item.Client.Company.Name,
                            WarehousePremiumType = item.WarehousePremiumType.GetDescription(),
                            item.UnitPrice,
                            item.Count,
                            TotalPrice = item.UnitPrice * item.Count,
                            Currency = item.Currency,
                            CreateDate = item.CreateDate.ToString(),
                            AdminName = item.Creater.RealName,
                            PremiumsStatus = item.WarehousePremiumsStatus,
                        }
                     ).ToArray(),
                total = feelist.Total,
            }.Json());
            #endregion
        }


        /// <summary>
        /// 删除费用
        /// </summary>
        protected void Delete()
        {
            try
            {
                string ID = Request.Form["ID"];
                var Fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.AsQueryable();
                var fee = Fees.Where(t => t.ID == ID).FirstOrDefault();
                //if (fee.WarehousePremiumsStatus == Needs.Ccs.Services.Enums.WarehousePremiumsStatus.Audited)
                //{
                //    Response.Write((new { success = false, message = "删除失败：费用已审批或已付款" }).Json());
                //    return;
                //}
                fee.SetOperator(Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID));
                fee.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string orderID = Request.Form["OrderID"];
                string clientCode = Request.Form["ClientCode"];
                string clientName = Request.Form["ClientName"];
                string feeType = Request.Form["FeeType"];
                string startDate = Request.Form["StartDate"];
                string endDate = Request.Form["EndDate"];

                var fees = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremiumsAll;
                List<LambdaExpression> lambdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> expression = item => item.WarehouseType == WarehouseType.HongKong;

                #region 查询条件
                if (!string.IsNullOrEmpty(orderID))
                {
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.OrderID.Contains(orderID.Trim());
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(clientCode))
                {
                    var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(clientName))
                {
                    var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(item => item.Company.Name.Contains(clientName)).Select(item => item.ID).ToArray();
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => clientIds.Contains(item.ClientID);
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(feeType))
                {
                    int intFeeType = Int32.Parse(feeType);
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => (int)item.WarehousePremiumType == intFeeType;
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.CreateDate >= from;
                    lambdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate);
                    Expression<Func<Needs.Ccs.Services.Models.OrderWhesPremium, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                    lambdas.Add(lambda);
                }
                #endregion

                var feelist = fees.GetAlls(expression, lambdas.ToArray());

                Func<Needs.Ccs.Services.Models.OrderWhesPremium, object> convert = item => new
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    ClientCode = item.Client.ClientCode,
                    ClientName = item.Client.Company.Name,
                    WarehousePremiumType = item.WarehousePremiumType.GetDescription(),
                    item.UnitPrice,
                    item.Count,
                    TotalPrice = item.UnitPrice * item.Count,
                    Currency = item.Currency,
                    CreateDate = item.CreateDate.ToString(),
                    AdminName = item.Creater.RealName,
                    PremiumsStatus = item.WarehousePremiumsStatus,
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(feelist.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xls";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式

                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "库房费用";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientCode", ExcelColumn = "客户编号", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ClientName", ExcelColumn = "客户名称", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "WarehousePremiumType", ExcelColumn = "费用类型", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UnitPrice", ExcelColumn = "单价", Alignment = "center" });

                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Count", ExcelColumn = "数量", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TotalPrice", ExcelColumn = "总价", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "添加时间", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AdminName", ExcelColumn = "添加人", Alignment = "center" });

                #endregion

                //调用导出方法
                NPOIHelper.ExcelDownload(dt, excelconfig);

                Response.Write((new
                {
                    success = true,
                    message = "导出成功",
                    url = fileDic.FileUrl
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "导出失败：" + ex.Message,
                }).Json());
            }
        }


    }
}
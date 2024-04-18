using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.Classify
{
    /// <summary>
    /// 产品归类已完成列表界面
    /// </summary>
    public partial class DoneList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }

        void init()
        {
            this.Model.Admin = new
            {
                ID = Yahv.Erp.Current.ID,
                UserName = Yahv.Erp.Current.UserName,
                RealName = Yahv.Erp.Current.RealName
            }.Json();

            var pvdataApi = new PvDataApiSetting();
            var pvwsorderApi = new PvWsOrderApiSetting();

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.SubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.GetNext,
            }.Json();
        }

        /// <summary>
        /// 获取已归类产品
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<OrderItem, bool>> expression = item => true;
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = new Yahv.PvWsOrder.Services.Views.Alls.ClassifyProductsAll(ClassifyStep.Done).GetPageList(page, rows, expression, Lamdas().ToArray());

            return new
            {
                rows = products.Select(
                        item => new
                        {
                            item.ID,
                            ItemID = item.InputID,
                            item.OrderID,
                            MainID = item.OrderID,
                            OrderedDate = item.OrderedDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            ClientCode = item.WsClient.EnterCode,
                            ClientName = item.WsClient.Name,

                            item.Product.PartNumber,
                            item.Product.Manufacturer,
                            item.CustomName,
                            //Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin)).GetOrigin().Code,
                            Origin = item.Origin.GetOrigin().Code,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            item.Quantity,
                            Unit = item.Unit.GetUnit().Code,
                            Currency = item.Currency?.GetCurrency().ShortName,
                            TotalPrice = item.TotalPrice.ToString("0.0000"),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                            item.ClassifiedPartNumber.HSCode,
                            item.ClassifiedPartNumber.TariffName,
                            ImportPreferentialTaxRate = item.ClassifiedPartNumber.ImportPreferentialTaxRate.ToString("0.0000"),
                            VATRate = item.ClassifiedPartNumber.VATRate.ToString("0.0000"),
                            ExciseTaxRate = item.ClassifiedPartNumber.ExciseTaxRate.ToString("0.0000"),
                            item.ClassifiedPartNumber.TaxCode,
                            item.ClassifiedPartNumber.TaxName,
                            item.ClassifiedPartNumber.LegalUnit1,
                            item.ClassifiedPartNumber.LegalUnit2,
                            item.ClassifiedPartNumber.CIQCode,
                            item.ClassifiedPartNumber.Elements,

                            OriginATRate = item.OrderItemsTerm.OriginRate.ToString("0.0000"),
                            item.OrderItemsTerm.CIQ,
                            item.OrderItemsTerm.CIQprice,
                            item.OrderItemsTerm.Ccc,
                            item.OrderItemsTerm.Embargo,
                            item.OrderItemsTerm.HkControl,
                            item.OrderItemsTerm.IsHighPrice,
                            item.OrderItemsTerm.Coo,

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.RealName ?? "--",
                            LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Yahv.Erp.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Yahv.Erp.Current.ID,

                            CompleteDate = item.OrderItemsChcd.SecondDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            ClassifyFirstOperatorName = item.OrderItemsChcd.FirstAdmin?.RealName ?? "Npc系统",
                            ClassifySecondOperatorName = item.OrderItemsChcd?.SecondAdmin?.RealName ?? "Npc系统",
                            OrderStatus = item.OrderStatus.GetDescription()
                        }
                     ).ToArray(),
                total = products.Total,
            }.Json();
        }

        List<LambdaExpression> Lamdas()
        {
            //查询参数
            string orderId = Request.QueryString["orderId"];
            string partNumber = Request.QueryString["partNumber"];
            string name = Request.QueryString["name"];
            string hsCode = Request.QueryString["hsCode"];
            string startDate = Request.QueryString["startDate"];
            string endDate = Request.QueryString["endDate"];
            string isShowLocked = Request.QueryString["isShowLocked"];
            bool showLocked = false;

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrWhiteSpace(orderId))
            {
                Expression<Func<OrderItem, bool>> lambda1 = item => item.OrderID.Contains(orderId.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(partNumber))
            {
                Expression<Func<OrderItem, bool>> lambda1 = item => item.Product.PartNumber.Contains(partNumber.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                Expression<Func<OrderItem, bool>> lambda1 = item => item.ClassifiedPartNumber.TariffName.Contains(name.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(hsCode))
            {
                Expression<Func<OrderItem, bool>> lambda1 = item => item.ClassifiedPartNumber.HSCode.Contains(hsCode.Trim());
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                DateTime dt;
                if (DateTime.TryParse(startDate, out dt))
                {
                    lamdas.Add((Expression<Func<OrderItem, bool>>)(item => item.ClassifiedPartNumber.OrderDate >= dt));
                }
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                DateTime dt;
                if (DateTime.TryParse(endDate, out dt))
                {
                    dt = dt.AddDays(1);
                    lamdas.Add((Expression<Func<OrderItem, bool>>)(item => item.ClassifiedPartNumber.OrderDate < dt));
                }
            }
            if (!string.IsNullOrWhiteSpace(isShowLocked))
            {
                showLocked = Convert.ToBoolean(isShowLocked);
            }
            if (!showLocked)
            {
                Expression<Func<OrderItem, bool>> lambda1 = item => !item.IsLocked || item.Locker.ID == Yahv.Erp.Current.ID;
                lamdas.Add(lambda1);
            }

            return lamdas;
        }

        /// <summary>
        /// 获取订单信息：合同发票、特殊类型
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string orderId = Request.Form["orderId"];

            #region 合同发票
            string pis = new PvWsOrder.Services.Views.OrderFilesRoll(orderId).Where(item => item.Type == (int)FileType.Invoice)
                    .ToList().Select(item => new
                    {
                        ID = item.ID,
                        FileName = item.CustomName,
                        FileFormat = "",
                        Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + item.Url,
                    }).Json();
            #endregion

            #region 特殊类型
            //TODO: 待设计实现
            #endregion

            return new
            {
                PIs = pis,
                SpecialType = "--",
            };
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void Export()
        {
            //查询参数
            string orderId = Request.QueryString["orderId"];
            string partNumber = Request.QueryString["partNumber"];
            string name = Request.QueryString["name"];
            string hsCode = Request.QueryString["hsCode"];
            string startDate = Request.QueryString["startDate"];
            string endDate = Request.QueryString["endDate"];

            try
            {
                #region 查询条件

                var view = new Yahv.PvWsOrder.Services.Views.Alls.ClassifyProductsExcel();

                if (!string.IsNullOrWhiteSpace(orderId))
                {
                    view.Where(item => item.OrderID.Contains(orderId.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(partNumber))
                {
                    view.Where(item => item.PartNumber.Contains(partNumber.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(name))
                {
                    view.Where(item => item.TariffName.Contains(name.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(hsCode))
                {
                    view.Where(item => item.HSCode.Contains(hsCode.Trim()));
                }
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(startDate, out dt))
                    {
                        view.Where(item => item.CompleteDate >= dt);
                    }
                }
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(endDate, out dt))
                    {
                        view.Where(item => item.CompleteDate < dt);
                    }
                }

                #endregion

                #region 导出Excel

                IWorkbook workBook = ExcelFactory.Create(ExcelFactory.ExcelVersion.Excel07);
                NPOIHelper npoi = new NPOIHelper(workBook);
                StyleConfig config = new StyleConfig()
                {
                    Title = "归类已完成产品",
                    TitleFont = "微软雅黑",
                    TitlePoint = 16
                };

                var data = view.ToList().Select(item => new
                {
                    品牌 = item.Manufacturer,
                    产品型号 = item.PartNumber,
                    报关品名 = item.TariffName,
                    HS编码 = item.HSCode,
                    申报要素 = item.Elements,
                    关税率 = item.ImportPreferentialTaxRate + item.OriginRate,
                    单价 = item.UnitPrice,
                    客户名称 = item.ClientName,
                    创建时间 = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    归类完成时间 = item.CompleteDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    预处理一人员 = item.ClassifyFirstOperatorName,
                    预处理二人员 = item.ClassifySecondOperatorName,
                    税务编码 = item.TaxCode,
                    税务名称 = item.TaxName,
                    订单编号 = item.OrderID,
                    归类日志 = string.Join(",", item.ClassifyLogs),
                });
                int[] columnWidth = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

                npoi.EnumrableToExcel(data, config, null);

                #endregion

                #region 文件保存

                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.Test);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                npoi.SaveAs(filePath);

                #endregion

                Response.Write((new { success = true, message = "申请成功", fileUrl = @"../Files/Download/" + fileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }
    }
}
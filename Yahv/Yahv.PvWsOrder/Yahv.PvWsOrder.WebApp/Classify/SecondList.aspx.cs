using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.Classify
{
    /// <summary>
    /// 产品归类预处理二列表界面
    /// </summary>
    public partial class SecondList : ErpParticlePage
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
        /// 获取待归类产品
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<OrderItem, bool>> expression = item => true;
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = new Yahv.PvWsOrder.Services.Views.Alls.ClassifyProductsAll(ClassifyStep.Step2).GetPageList(page, rows, expression, Lamdas().ToArray());

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

                            ClassifyFirstOperatorName = item.OrderItemsChcd.FirstAdmin?.RealName ?? "Npc系统",
                            SpecialType = item.OrderItemsTerm.GetSpecialType()
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
        /// 一键归类
        /// </summary>
        protected void QuickClassify()
        {
            try
            {
                string[] ids = Request.Form["ids"].Split(',');

                var orderItems = new Yahv.PvWsOrder.Services.Views.Alls.ClassifyProductsAll(ClassifyStep.Step2).GetTop(ids.Length, item => ids.Contains(item.ID)).ToArray();
                var classifiedResults = GetClassifiedResults(orderItems);

                //TODO: 子系统业务处理
                orderItems.QuickClassify(Yahv.Erp.Current.ID);
                Response.Write((new { success = true, message = "归类成功", data = classifiedResults }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "归类失败：" + ex.Message }).Json());
            }
        }

        private string GetClassifiedResults(OrderItem[] orderItems)
        {
            var orderIds = orderItems.Select(item => item.OrderID).Distinct().ToArray();
            var orderFiles = new Services.Views.CenterFilesTopView().Where(item => orderIds.Contains(item.WsOrderID) && item.Type == (int)FileType.Invoice).ToList();

            var pvdataApi = new PvDataApiSetting();
            var pvwsorderApi = new PvWsOrderApiSetting();

            return orderItems.Select(item => new
            {
                ItemID = item.InputID,
                MainID = item.OrderID,
                ClientName = item.WsClient.Name,
                ClientCode = item.WsClient.EnterCode,
                OrderedDate = item.OrderedDate,
                PIs = GetPIs(orderFiles.Where(pi => pi.WsOrderID == item.OrderID)),
                SpecialType = "--",//TODO: 待设计实现
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
                CallBackUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.SubmitClassified,
                NextUrl = ConfigurationManager.AppSettings[pvwsorderApi.ApiName] + pvwsorderApi.GetNext,

                PartNumber = item.Product.PartNumber,
                Manufacturer = item.Product.Manufacturer,
                Currency = item.Currency.GetDescription(),
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,

                HSCode = item.ClassifiedPartNumber.HSCode,
                TariffName = item.ClassifiedPartNumber.TariffName,
                TaxCode = item.ClassifiedPartNumber.TaxCode,
                TaxName = item.ClassifiedPartNumber.TaxName,
                Unit = item.Unit.GetUnit().Code,
                LegalUnit1 = item.ClassifiedPartNumber.LegalUnit1,
                LegalUnit2 = item.ClassifiedPartNumber.LegalUnit2,
                VATRate = item.ClassifiedPartNumber.VATRate,
                ImportPreferentialTaxRate = item.ClassifiedPartNumber.ImportPreferentialTaxRate,
                ExciseTaxRate = item.ClassifiedPartNumber.ExciseTaxRate,
                CIQCode = item.ClassifiedPartNumber.CIQCode,
                Elements = item.ClassifiedPartNumber.Elements,

                OriginATRate = item.OrderItemsTerm.OriginRate,
                Ccc = item.OrderItemsTerm.Ccc,
                Embargo = item.OrderItemsTerm.Embargo,
                Coo = item.OrderItemsTerm.Coo,
                CIQ = item.OrderItemsTerm.CIQ,
                CIQprice = item.OrderItemsTerm.CIQprice,
                IsHighPrice = item.OrderItemsTerm.IsHighPrice,
                IsDisinfected = item.OrderItemsTerm.IsDisinfected,
                Step = ClassifyStep.Step2.GetHashCode(),
                CreatorID = Yahv.Erp.Current.ID,
                CreatorName = Yahv.Erp.Current.RealName

            }).Json();
        }

        /// <summary>
        /// 获取订单信息：合同发票、特殊类型
        /// </summary>
        /// <returns></returns>
        protected object GetOrderInfos()
        {
            string orderId = Request.Form["orderId"];
            var orderFiles = new PvWsOrder.Services.Views.OrderFilesRoll(orderId).Where(item => item.Type == (int)FileType.Invoice).ToList();

            return new
            {
                PIs = GetPIs(orderFiles),
                SpecialType = GetSpecialType(orderId),
            };
        }

        /// <summary>
        /// 获取合同发票
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private string GetPIs(IEnumerable<Yahv.Services.Models.CenterFileDescription> orderFiles)
        {
            return orderFiles.Select(item => new
            {
                ID = item.ID,
                FileName = item.CustomName,
                FileFormat = "",
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + item.Url,
            }).Json();
        }

        /// <summary>
        /// 获取订单特殊类型
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private string GetSpecialType(string orderId)
        {
            //TODO: 待设计实现
            return "--";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvData.WebApp.Classify
{
    /// <summary>
    /// 产品归类预处理一列表界面
    /// </summary>
    public partial class FirstList : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }

            var order = new PvWsOrder.Services.Views.OrderAlls().FirstOrDefault(item => item.ID == "Order201910290009");
            var client = new PvWsOrder.Services.Views.WsClientsAlls().FirstOrDefault(item=>item.ID == "013EEC9B29F8749B2E4E87F707C952E2");
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

            var products = new Yahv.PvWsOrder.Services.Views.Alls.ClassifyProductsAll(ClassifyStep.Step1).GetPageList(page, rows, expression, Lamdas().ToArray());

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
                            TariffName = item.CustomName,
                            //Origin = ((Origin)Enum.Parse(typeof(Origin), item.Origin)).GetOrigin().Code,
                            Origin = item.Origin.GetOrigin().Code,
                            UnitPrice = item.UnitPrice.ToString("0.0000"),
                            item.Quantity,
                            Unit = item.Unit.GetUnit().Code,
                            Currency = item.Currency?.GetCurrency().ShortName,
                            TotalPrice = item.TotalPrice.ToString("0.0000"),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),

                            //产品归类锁定
                            LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                            Locker = item.Locker?.RealName ?? "--",
                            LockTime = item.LockDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "--",
                            IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Yahv.Erp.Current.ID),
                            IsCanUnlock = item.IsLocked && item.Locker.ID == Yahv.Erp.Current.ID
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
    }
}
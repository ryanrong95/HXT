using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WebApp.Declaration.OrderChange
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var orderChange = new OrderChangeDetalView().Where(item => item.OrderID == ID).FirstOrDefault();
            var DecHead = new DecHeadsView().Where(item => item.OrderID == orderChange.OrderID).FirstOrDefault();

            this.Model.OrderChange = new
            {
                ID = ID,
                OrderID = orderChange.OrderID,
                orderChange.ContrNo,
                orderChange.EntryId,
                DDate = orderChange.DDate?.ToShortDateString(),
                CreateDate = orderChange.CreateDate.ToShortDateString(),//下单日期
                orderChange.Currency,
                orderChange.DecAmount,//报关总价
                orderChange.CustomsExchangeRate,
                TariffValue = orderChange.TariffValue ?? 0M,
                AddedValue = orderChange.AddedValue ?? 0M,
                ExciseTaxValue = orderChange.ExciseTaxValue ?? 0M,
                TransPremiumInsurance = ConstConfig.TransPremiumInsurance,//运保杂
                IsTwoStep = DecHead.isTwoStep,
            }.Json();
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void ProductData()
        {
            //var OrderID = Request.QueryString["ID"];
            //var orderItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(x => x.OrderID == OrderID).AsQueryable();
            ////前台显示
            //Func<OrderItem, object> convert = item => new
            //{
            //    item.ID,
            //    item.Category.HSCode,
            //    ProductName = item.Category.Name,  //归类后品名
            //    ProductModel = item.Product.Model,//型号
            //    Origin = item.Origin,
            //    item.TotalPrice,
            //    ImportTax = item.ImportTax.Value,
            //    ImportRate = item.ImportTax.Rate,
            //    AddedValueRate = item.AddedValueTax.Rate,
            //    AddedValueTax = item.AddedValueTax.Value,

            //};
            //Response.Write(new { rows = orderItems.Select(convert).ToArray(), }.Json());
            var OrderID = Request.QueryString["ID"];
            var products = new OrderChangeProductView().Where(x => x.OrderID == OrderID).OrderBy(item => item.GNo).AsQueryable();
            int i = 0;
            Func<OrderChangeProduct, object> convert = item => new
            {
                item.ID,
                item.OrderItemID,
                item.HSCode,
                item.ProductName,  //归类后品名
                item.ProductModel,//型号
                item.Origin,
                item.TotalPrice,
                item.ImportRate,
                item.ExciseTaxRate,
                item.AddedValueRate,
                item.CustomsExchangeRate,
                item.ImportTax,
                item.ExciseTax,
                item.AddedValueTax,
            };
            Response.Write(new { rows = products.Select(convert).ToArray(), }.Json());


        }

        protected void Submit()
        {
            try
            {

                string ImportData = Request.Form["ArrImport"].Replace("&quot;", "'");
                string ExciseTaxData = Request.Form["ArrExciseTax"].Replace("&quot;", "'");
                string AddedValueData = Request.Form["ArrAddedValue"].Replace("&quot;", "'");
                string CusTariffValue = Request.Form["CusTariffValue"].Replace("&quot;", "'");
                //string CusAddedValue = Request.Form["CusAddedValue"].Replace("&quot;", "'");
                string OrderId = Request.Form["OrderID"];
                IEnumerable<ParamsOrderChange> importData = ImportData.JsonTo<IEnumerable<ParamsOrderChange>>();
                IEnumerable<ParamsOrderChange> exciseTaxData = ExciseTaxData.JsonTo<IEnumerable<ParamsOrderChange>>();
                IEnumerable<ParamsOrderChange> addedValueData = AddedValueData.JsonTo<IEnumerable<ParamsOrderChange>>();
                var orderChange = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, x => x.OrderID == OrderId).FirstOrDefault();
                var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                orderChange.Admin = Needs.Underly.FkoFactory<Admin>.Create(adminID);
                orderChange.UpdateOrderTax(importData, exciseTaxData, addedValueData, CusTariffValue);

                var cpsIDs = importData.Select(item => item.OrderItemID).Union(exciseTaxData.Select(item => item.OrderItemID)).Union(addedValueData.Select(item => item.OrderItemID));
                if (cpsIDs.Count() > 1)
                {
                    var cps = cpsIDs.Select(id => new ClassifyProduct { ID = id }).ToArray();
                    SyncManager.Current.TaxChange.For(cps).DoSync();
                }

                NoticeLog noticeLog = new NoticeLog();
                noticeLog.MainID = OrderId;
                noticeLog.NoticeType = Needs.Ccs.Services.Enums.SendNoticeType.TaxError;
                noticeLog.Readed = true;
                noticeLog.SendNotice();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 获取税率修改记录
        /// </summary>
        /// <returns></returns>
        protected object GetTaxRateChangeLogs()
        {
            string OrderID = Request.Form["OrderID"];

            if (string.IsNullOrEmpty(OrderID))
            {
                OrderID = string.Empty;
            }

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.OrderChangeNoticeLogsListView.OrderChangeNoticeLogsListModel, bool>>)(t => t.OrderID == OrderID));


            var logs = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderChangeNoticeLogsList.GetCommon(lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.OrderChangeNoticeLogsListView.OrderChangeNoticeLogsListModel, object> convert = log => new
            {
                log.ID,
                log.CreateDate,
                log.Summary,
            };

            return logs.Select(convert).ToArray();
        }


    }
}
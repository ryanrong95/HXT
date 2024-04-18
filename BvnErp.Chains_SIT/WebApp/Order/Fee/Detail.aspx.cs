using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Fee
{
    /// <summary>
    /// 订单杂费详情界面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化订单基本信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var fee = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderPremiums[id];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[fee.OrderID];
            var taxPoint = 1 + order.ClientAgreement.InvoiceTaxRate;

            StandardRemark stdRemark = null;
            if (!string.IsNullOrEmpty(fee.StandardRemark))
            {
                stdRemark = JsonConvert.DeserializeObject<StandardRemark>(fee.StandardRemark);
            }

            // 收费项目
            string feeName = stdRemark?.SelectedStd?.Name;

            // 收费明细
            List<object> feeItems = new List<object>();
            if (stdRemark != null && stdRemark.ChargeInputs != null && stdRemark.ChargeInputs.Length > 0)
            {
                foreach (var chargeInput in stdRemark.ChargeInputs)
                {
                    string stdID = chargeInput.StdID;
                    string feeItemName = "";
                    string unitPrice = "";
                    if (stdRemark.SelectedStd.Children == null)
                    {
                        feeItemName = stdRemark.SelectedStd.Name;
                        unitPrice = stdRemark.SelectedStd.Price;
                    }
                    else
                    {
                        var child = stdRemark.SelectedStd.Children.Where(t => t.ID == stdID).FirstOrDefault();
                        if (child != null)
                        {
                            feeItemName = child.Name;
                            unitPrice = child.Price;
                        }
                    }

                    feeItems.Add(new
                    {
                        Name = feeItemName,
                        UnitPrice = unitPrice,
                        Currency = chargeInput.Currency,
                        CurrencyCN = chargeInput.CurrencyCN,
                        Units = chargeInput.Values.Select(item => new
                        {
                            Unit = item.Unit,
                            Value = item.Value,
                        }),
                    });
                }
            }

            this.Model.FeeData = new
            {
                fee.ID,
                fee.OrderID,
                Name = fee.Type == OrderPremiumType.OtherFee ? fee.Name : fee.Type.GetDescription(),
                fee.Count,
                fee.UnitPrice,
                fee.Currency,
                fee.Rate,
                IsPaid = fee.GetPremiumStatus(taxPoint).GetDescription(),
                PaymentDate = fee.PaymentDate?.ToShortDateString(),

                FeeName = feeName,
                StandardPrice = fee.StandardPrice,
                FeeItems = feeItems,
            }.Json();

        }

        /// <summary>
        /// 费用附件
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            var files = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderFiles.Where(t => t.OrderPremiumID == id);

            Func<OrderFile, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                FileFormat = item.FileFormat,
                Url = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl()
            };

            Response.Write(new
            {
                rows = files.Select(convert).ToArray(),
                total = files.Count()
            }.Json());
        }
    }
}
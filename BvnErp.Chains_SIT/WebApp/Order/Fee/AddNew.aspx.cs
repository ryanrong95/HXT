using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Fee
{
    public partial class AddNew : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            this.Model.timestamp = GetTimeStamp();
        }

        private string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Key = item.Code, Value = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 获取币种的实时汇率
        /// </summary>
        protected decimal? GetExchangeRate()
        {
            decimal? exchangeRate = null;
            var currency = Request.Form["Currency"];
            if (currency == MultiEnumUtils.ToCode<Currency>(Currency.CNY))
            {
                exchangeRate = 1;
            }
            else
            {
                var realExchange = Needs.Wl.Admin.Plat.AdminPlat.RealTimeRates.Where(rate => rate.Code == currency).FirstOrDefault();
                exchangeRate = realExchange?.Rate;
            }

            return exchangeRate;
        }

        /// <summary>
        /// 保存费用
        /// </summary>
        protected void SaveFee()
        {
            try
            {
                //前台数据
                var orderID = Request.Form["OrderID"];
                var type = OrderPremiumType.OtherFee; // (OrderPremiumType)Enum.Parse(typeof(OrderPremiumType), Request.Form["Type"]);
                var name = Request.Form["Name"];
                var count = Convert.ToInt32(Request.Form["Count"]);
                var unitPrice = Convert.ToDecimal(Request.Form["UnitPrice"]);
                var currency = Request.Form["Currency"];
                var rate = Convert.ToDecimal(Request.Form["Rate"]);
                var file = Request.Files["File"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var standardID = Request.Form["StandardID"];
                var standardPrice = Convert.ToDecimal(Request.Form["StandardPrice"]);
                var standardCurrency = Request.Form["StandardCurrency"];
                var standardRemark = Request.Form["StandardRemark"].Replace("&quot;", "\"");

                //新增费用
                var fee = new Needs.Ccs.Services.Models.OrderPremium();
                fee.OrderID = orderID;
                fee.Type = type;
                fee.Name = name;
                fee.Count = count;
                fee.UnitPrice = unitPrice;
                fee.Currency = currency;
                fee.Rate = rate;
                fee.Admin = admin;

                fee.StandardID = standardID;
                fee.StandardPrice = standardPrice;
                fee.StandardCurrency = standardCurrency;
                fee.StandardRemark = standardRemark;

                //费用附件
                if (file != null && file.ContentLength != 0)
                {
                    //文件保存
                    string fileName = file.FileName.ReName();
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                    fileDic.CreateDataDirectory();
                    file.SaveAs(fileDic.FilePath);

                    fee.Files.Add(new Needs.Ccs.Services.Models.OrderFile()
                    {
                        OrderID = orderID,
                        OrderPremiumID = fee.ID,
                        Admin = admin,
                        Name = file.FileName,
                        FileType = FileType.OrderFeeFile,
                        FileFormat = file.ContentType,
                        Url = fileDic.VirtualPath,
                        FileStatus = OrderFileStatus.Audited
                    });
                }

                fee.Enter();

                //重新传这个订单的杂费给 Yahv
                Needs.Ccs.Services.Models.PaymentToYahvOtherFee paymentToYahvOtherFee = new Needs.Ccs.Services.Models.PaymentToYahvOtherFee(orderID, admin.ID);
                paymentToYahvOtherFee.Execute();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}
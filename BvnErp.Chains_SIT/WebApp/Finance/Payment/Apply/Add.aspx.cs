using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment
{
    public partial class Add : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadData();
            }
        }

        protected void LoadData()
        {
            //付款费用类型
            this.Model.FeeTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>()
                .Select(item => new { item.Key, item.Value }).Json();
            //付款方式
            this.Model.PaymentTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>()
                .Select(item => new { item.Key, item.Value }).Json();
            //币种
            this.Model.CurrencyData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.Currency>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        protected void Submit()
        {
            try
            {
                var OrderID = Request.Form["OrderID"];
                var PayeeName = Request.Form["PayeeName"];
                var BankAccount = Request.Form["BankAccount"];
                var BankName = Request.Form["BankName"];
                var FeeType = Request.Form["FeeType"];
                var FeeDesc = Request.Form["FeeDesc"];
                var Amount = Request.Form["Amount"];
                var Currency = Request.Form["Currency"];
                var PayType = Request.Form["PayType"];
                var PayDate = Request.Form["PayDate"];
                //初始化申请数据
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                Needs.Ccs.Services.Models.PaymentApply apply = new Needs.Ccs.Services.Models.PaymentApply();
                apply.OrderID = OrderID;
                apply.Applier = admin;
                apply.PayeeName = PayeeName;
                apply.BankAccount = BankAccount;
                apply.BankName = BankName;
                apply.PayFeeType = (Needs.Ccs.Services.Enums.FinanceFeeType)int.Parse(FeeType);
                apply.FeeDesc = FeeDesc;
                apply.Amount = decimal.Parse(Amount);
                apply.Currency = Currency;
                apply.PayDate = Convert.ToDateTime(PayDate);
                apply.PayType = (Needs.Ccs.Services.Enums.PaymentType)int.Parse(PayType);
                apply.SetOperator(admin);
                apply.Enter();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}
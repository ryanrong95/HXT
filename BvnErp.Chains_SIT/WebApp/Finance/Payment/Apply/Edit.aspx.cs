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
    public partial class Edit : Uc.PageBase
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

            //查询订单付款申请
            string ApplyID = Request.QueryString["ApplyID"];
            var Apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentApply.Where(item => item.ID == ApplyID).FirstOrDefault();
            this.Model.ApplyData = new
            {
                PayeeName = Apply.PayeeName,
                BankAccount = Apply.BankAccount,
                BankName = Apply.BankName,
                FeeType = Apply.PayFeeType,
                FeeDesc = Apply.FeeDesc,
                Amount = Apply.Amount,
                Currency = Apply.Currency,
                PayType = Apply.PayType,
                PayDate = Apply.PayDate,
            }.Json();
        }

        protected void Submit()
        {
            try
            {
                var ApplyID = Request.Form["ApplyID"];
                var PayeeName = Request.Form["PayeeName"];
                var BankAccount = Request.Form["BankAccount"];
                var BankName = Request.Form["BankName"];
                var FeeType = Request.Form["FeeType"];
                var FeeDesc = Request.Form["FeeDesc"];
                var Amount = Request.Form["Amount"];
                var Currency = Request.Form["Currency"];
                var PayType = Request.Form["PayType"];
                var PayDate = Request.Form["PayDate"];
                //更新申请数据
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentApply.Where(item => item.ID == ApplyID).FirstOrDefault();
                apply.Applier = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
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

                Response.Write((new { success = true, message = "提交成功"}).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}
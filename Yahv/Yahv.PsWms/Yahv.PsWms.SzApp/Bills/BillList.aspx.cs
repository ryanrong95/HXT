using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PsWms.SzMvc.Services;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.PsWms.SzMvc.Services.Views.Roll;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PsWms.SzApp.Bills
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns></returns>
        protected object data()
        {
            Expression<Func<VoucherShow, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var query = new Vouchers_Show_View().GetPageList(page, rows, expression).Where(v=>!v.Isinvoiced);
            return this.Paging(query, t => new
            {
                t.ID,
                t.PayerID,
                t.PayerName,
                Total = t.Total.ToString("f2"),
                CutDateIndex = t.CutDateIndex,
                ReceiptTotal = t.ReceiptTotal,
            });
        }

        Expression<Func<VoucherShow, bool>> Predicate()
        {
            Expression<Func<VoucherShow, bool>> predicate = item => true;
            //查询参数
            var Name = Request.QueryString["Name"];
            var DateIndex = Request.QueryString["DateIndex"];

            if (!string.IsNullOrWhiteSpace(Name))
            {
                predicate = predicate.And(item => item.PayerName.Contains(Name.Trim()));
            }
            if (!string.IsNullOrWhiteSpace(DateIndex))
            {
                var index = int.Parse(DateIndex);
                predicate = predicate.And(item => item.CutDateIndex == index);
            }
            return predicate;
        }

        /// <summary>
        /// 提交开票申请
        /// </summary>
        protected void Submit()
        {
            #region 界面数据
            try
            {
                //发票信息  邮寄信息

                //产品信息
                var vouchers = Request.Form["vouchers"].Replace("&quot;", "'").Replace("amp;", "");
                var vvouchers = vouchers.JsonTo<List<VoucherShow>>();
                #endregion

                #region 数据
                var voucher = vvouchers.FirstOrDefault();
                var invoice = new SzMvc.Services.Views.Origins.InvoicesOrigin().SingleOrDefault(t => t.ClientID == voucher.PayerID);
                var invoiceNotice =  new
                {
                    ClientID = voucher.PayerID,
                    Title = voucher.PayerName,
                    TaxNumber = invoice.TaxNumber,
                    RegAddress = invoice.RegAddress,
                    Tel = invoice.Tel,
                    BankName = invoice.BankName,
                    BankAccount = invoice.BankAccount,
                    PostAddress = invoice.RevAddress,
                    PostRecipient = invoice.Name,
                    PostTel = invoice.Phone,
                    AdminID = Erp.Current.ID,
                    Price = voucher.Total,
                    VourcherID = voucher.ID,
                    DeliveryType=invoice.DeliveryType,
                };

                var apiSetting = new PvWsOrderApiSetting();
                var url = ConfigurationManager.AppSettings[apiSetting.ApiName] + apiSetting.ApplyInvoice;
                var res = Yahv.Utils.Http.ApiHelper.Current.JPost(url, invoiceNotice);
                //提交成功修改为已开票。
                var re = res.JsonTo<JObject>();
                var success = re["success"].Value<bool>();
                if (success)
                {
                    var vourcher = new Voucher() { ID = voucher.ID };
                    vourcher.UpdateInvoiceStatus();
                    Response.Write((new { success = true, message = "提交成功" }).Json());
                }
               

               
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
            #endregion
        }
    }

}

using Needs.Erp.Generic;
using Needs.Underly;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Order
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }

        void PageInit()
        {
            SetDropDownList();
            string id = Request.QueryString["id"];
            var order = Needs.Erp.ErpPlot.Current.ClientSolutions.Order[id];
            var jsondata = new object();
            if (order != null)
            {
                jsondata = new { Address = order.Address, DeliveryAddress = order.DeliveryAddress, Currency = order.Currency, ClientID = order.ClientID, BeneficiaryID = order.BeneficiaryID, ConsigneeID = order.ConsigneeID };
            }
             
            this.Model.AllData = jsondata.Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            string consigneeid = Request.Form["ConsigneeID"];
            string beneficiaryid = Request.Form["BeneficiaryID"];

            var order = Needs.Erp.ErpPlot.Current.ClientSolutions.Order[id] as NtErp.Crm.Services.Models.Order ?? new NtErp.Crm.Services.Models.Order();

            order.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            order.Client = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Client>.Create(Request.Form["ClientID"]);
            order.Currency = (CurrencyType)int.Parse(Request.Form["Currency"]);

            order.Beneficiaries = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Beneficiaries>.Create(beneficiaryid);
            order.DeliveryAddress = Request.Form["DeliveryAddress"];
            order.Address = Request.Form["Address"];
            order.Contact = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Contact>.Create(consigneeid);
            order.EnterSuccess += Contact_EnterSuccess;
            order.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        void SetDropDownList()
        {
           // this.Model.CurrencyData = ToCurrencyDictionary<Currency>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.CurrencyData = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            var client = Needs.Erp.ErpPlot.Current.ClientSolutions.MyClientsBase;
            this.Model.ClientData = client.Select(c => new { value = c.ID, text = c.Name }).Json();
            this.Model.BeneficiaryData = Needs.Erp.ErpPlot.Current.ClientSolutions.Beneficiaries.Select(c => new { value = c.ID, bank = c.Bank, bankcode = c.BankCode }).Json();
            this.Model.ConsigneeData = Needs.Erp.ErpPlot.Current.ClientSolutions.Contacts.Select(c => new { value = c.ID, text = c.Name }).Json();
        }

        /// <summary>
        /// 获取货币键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        private Dictionary<string, string> ToCurrencyDictionary<T>()
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var key = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)).ToString();
                var value = ((Currency)item).GetICurrency().ShortName;
                dic.Add(key, value);
            }
            return dic;
        }
    }
}
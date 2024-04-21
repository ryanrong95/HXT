//using System;
//using System.Linq;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;


//namespace Yahv.Csrm.WebApp.Crm.WsBeneficiaries
//{
//    public partial class Edit : BasePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//                var supplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//                this.Model.Entity = supplier.Beneficiaries[Request.QueryString["id"]];
//                this.Model.Enterprise = new EnterprisesRoll()[Request.QueryString["supplierid"]];
//                // 1.币种
//                this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                //2.支付方式
//                this.Model.Methord = ExtendsEnum.ToArray<Methord>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                //3.所在地区
//                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });
//                //4.发票
//                this.Model.InvoiceType = ExtendsEnum.ToArray(InvoiceType.Unkonwn).Select(item => new
//                {
//                    value = (int)item,
//                    text = item.GetDescription()
//                });

//            }
//        }
//        protected void btnSubmit_Click(object sender, EventArgs e)
//        {
//            var id = Request.QueryString["id"];
//            var wsclient = Erp.Current.Whs.WsClients[Request.QueryString["clientid"]];
//            var supplier = wsclient.WsSuppliers[Request.QueryString["supplierid"]];
//            var entity = supplier.Beneficiaries[id] ?? new WsBeneficiary();

//            entity.WsClient = wsclient.Enterprise;
//            entity.EnterpriseID = supplier.Enterprise.ID;
//            entity.Enterprise = supplier.Enterprise;
//            entity.Currency = (Currency)int.Parse(Request["Currency"]);
//            entity.District = (District)int.Parse(Request["selDistrict"]);
//            entity.Methord = (Methord)int.Parse(Request["Methord"]);
//            entity.Bank = Request.Form["Bank"].Trim();
//            entity.BankAddress = Request.Form["BankAddress"].Trim();
//            entity.RealName = Request.Form["RealName"].Trim();
//            entity.Account = Request.Form["Account"].Trim();
//            entity.SwiftCode = Request.Form["SwiftCode"].Trim();
//            entity.InvoiceType = (InvoiceType)int.Parse(Request["InvoiceType"]);
//            entity.Name = Request.Form["Name"].Trim();
//            entity.Tel = Request.Form["Tel"].Trim();
//            entity.Mobile = Request.Form["Mobile"].Trim();
//            entity.Email = Request.Form["Email"].Trim();
//            entity.IsDefault = Request["IsDefault"] == null ? false : true;
//            if (string.IsNullOrWhiteSpace(id))
//            {
//                entity.CreatorID = Yahv.Erp.Current.ID;
//            }
//            entity.EnterSuccess += Entity_EnterSuccess;
//            entity.Enter();


//        }

//        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            var entity = sender as WsBeneficiary;
//            //调用接口
//            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(api))
//            {
//                Unify(api,entity);
//            }
//            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
//        }
//        //调用接口
//        object Unify(string api,WsBeneficiary data)
//        {
//            var json = new
//            {
//                Enterprise = data.WsClient,
//                SupplierName = data.Enterprise.Name,
//                IsDefault = data.IsDefault,
//                RealName = data.RealName,
//                Bank = data.Bank,
//                BankAddress = data.BankAddress,
//                Account = data.Account,
//                SwiftCode = data.SwiftCode,
//                Methord = data.Methord,
//                Currency = data.Currency,
//                District = data.District,
//                Name = data.Name,
//                Tel = data.Tel,
//                Mobile = data.Mobile,
//                Email = data.Email
//            }.Json();
//            var response = HttpClientHelp.HttpPostRaw(api + "/suppliers/banks", json);
//            return response;
//        }
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Web.Erp;
//using Yahv.Web.Forms;
//using YaHv.Csrm.Services.Extends;
//using YaHv.Csrm.Services.Models.Origins;
//using YaHv.Csrm.Services.Views.Rolls;

//namespace Yahv.Csrm.WebApp.Crm.WsBeneficiaries
//{
//    public partial class List : ErpParticlePage
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                init();
//                this.Model.ClientID = Request.QueryString["clientid"];
//                this.Model.SupplierID = Request.QueryString["supplierid"];

//            }
//        }
//        void init()
//        {
//            //状态
//            Dictionary<string, string> statusdic = new Dictionary<string, string>() { { "0", "全部" } };
//            statusdic.Add(ApprovalStatus.Normal.ToString(), ApprovalStatus.Normal.GetDescription());
//            statusdic.Add(ApprovalStatus.Black.ToString(), ApprovalStatus.Black.GetDescription());
//            statusdic.Add(ApprovalStatus.Closed.ToString(), ApprovalStatus.Closed.GetDescription());
//            statusdic.Add(ApprovalStatus.Deleted.ToString(), ApprovalStatus.Deleted.GetDescription());
//            //状态
//            this.Model.Status = statusdic.Select(item => new
//            {
//                value = item.Key,
//                text = item.Value
//            });
//            Dictionary<string, string> dic = new Dictionary<string, string>() { { "-100", "全部" } };
//            //币种
//            this.Model.Currency = dic.Concat(ExtendsEnum.ToDictionary<Currency>()).Select(item => new
//            {
//                value = int.Parse(item.Key),
//                text = item.Value.ToString()
//            });
//            //支付方式
//            this.Model.Methord = dic.Concat(ExtendsEnum.ToDictionary<Methord>()).Select(item => new
//            {
//                value = int.Parse(item.Key),
//                text = item.Value.ToString()
//            });
//        }
//        protected object data()
//        {
//            string clientid = Request.QueryString["clientid"];
//            string supplierid = Request.QueryString["supplierid"];
//            var beneficiary = Erp.Current.Whs.WsClients[clientid].WsSuppliers[supplierid].Beneficiaries.Where(Predicate());
//            return new
//            {
//                rows = beneficiary.ToArray().Select(item => new
//                {
//                    item.ID,
//                    item.RealName,
//                    item.Bank,
//                    item.BankAddress,
//                    item.SwiftCode,
//                    item.Account,
//                    District = item.District.GetDescription(),
//                    Currency = item.Currency.GetDescription(),
//                    Methord = item.Methord.GetDescription(),
//                    Name = item.Name,
//                    item.Mobile,
//                    item.Tel,
//                    item.Status,
//                    StatusName = item.Status.GetDescription(),
//                    TaxType = item.InvoiceType.GetDescription(),
//                    Admin = item.Creator == null ? null : item.Creator.RealName,
//                    IsDefault = item.IsDefault ? "是" : "否"
//                })
//            };
//        }
//        Expression<Func<WsBeneficiary, bool>> Predicate()
//        {
//            Expression<Func<WsBeneficiary, bool>> predicate = item => true;
//            var name = Request["name"];
//            var method = Request["method"];
//            var currency = Request["currency"];
//            if (!string.IsNullOrWhiteSpace(name))
//            {
//                predicate = predicate.And(item => item.Bank.Contains(name));
//            }
          
//            if (method != "-100")
//            {
//                predicate = predicate.And(item => item.Methord == (Methord)int.Parse(method));
//            }
//            if (currency != "-100")
//            {
//                predicate = predicate.And(item => item.Currency == (Currency)int.Parse(currency));
//            }

//            return predicate;
//        }
//        protected void del()
//        {
//            var id = Request["id"];
//            string clientid = Request["clientid"];
//            string supplierid = Request["supplierid"];
//            var beneficiary = Erp.Current.Whs.WsClients[clientid].WsSuppliers[supplierid].Beneficiaries[id];
//            beneficiary.AbandonSuccess += Beneficiary_AbandonSuccess;
//            beneficiary.Abandon();

//        }

//        private void Beneficiary_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
//        {
//            var beneficiary = sender as WsBeneficiary;
//            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(api))
//            {
//                var wssupplier = new WsSuppliersRoll()[beneficiary.Enterprise.ID];
//                Unify(api, beneficiary.WsClient.Name, wssupplier.ChineseName, beneficiary.Account);
//            }

//        }
//        //同步接口
//        object Unify(string api, string clientname, string suppliername, string account)
//        {
//            var response = HttpClientHelp.CommonHttpRequest(api + "/suppliers/banks?name=" + clientname + "&supplierName=" + suppliername + "&account=" + account, "DELETE");
//            return response;
//        }
//    }
//}
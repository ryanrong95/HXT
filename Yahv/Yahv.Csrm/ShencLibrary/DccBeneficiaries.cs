using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace ShencLibrary
{


    public class SyncBeneficiary
    {
        public Currency Currency { get; set; }
        // public District District { get; set; }
        public Methord Methord { get; set; }
        public string Bank { get; set; }
        public string BankAddress { get; set; }
        public string RealName { get; set; }

        public string Account { get; set; }
        public string SwiftCode { get; set; }
        public string Contact { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
        public string Place { set; get; }
    }

    public class DccBeneficiary
    {
        public DccBeneficiary()
        {
            //dynamic entity = new System.Dynamic.ExpandoObject();
            //var kkk = (SyncBeneficiary)entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientid">客户ID</param>
        /// <param name="nsupplierid">供应商ID</param>
        /// <param name="entity"></param>
        /// <param name="npayeeid">收款人ID：修改时要赋值</param>
        /// <returns></returns>
        /// <remarks>
        //NullReferenceException: 未将对象引用设置到对象的实例。] 
        //Yahv.Utils.Extends.StringExtend.ToHalfAngle(String input) +3 
        //YaHv.Csrm.Services.Models.Origins.Enterprise.set_Name(String value) +65 
        //ShencLibrary.DccBeneficiary.Enter(String clientid, String nsupplierid, SyncBeneficiary entity, String npayeeid) +295 
        //Yahv.PvWsClient.WebAppNew.Controllers.AccountController.SupplierSubmit(AddSupplierViewModel model) +736 
        //lambda_method(Closure , ControllerBase , Object[] ) +138 
        //System.Web.Mvc.ControllerActionInvoker.InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary`2 parameters) +34 System.Web.Mvc.Async.<>c.<BeginInvokeSynchronousActionMethod>b__9_0(IAsyncResult asyncResult, ActionInvocation innerInvokeState) +38 
        //System.Web.Mvc.Async.WrappedAsyncResult`2.CallEndDelegate(IAsyncResult asyncResult) +76 System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeActionMethod(IAsyncResult asyncResult) +41 System.Web.Mvc.Async.AsyncInvocationWithFilters.<InvokeActionMethodFilterAsynchronouslyRecursive>b__11_0() +71 System.Web.Mvc.Async.<>c__DisplayClass11_1.<InvokeActionMethodFilterAsynchronouslyRecursive>b__2() +396 
        //System.Web.Mvc.Async.<>c__DisplayClass11_1.<InvokeActionMethodFilterAsynchronouslyRecursive>b__2() +396 
        //System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeActionMethodWithFilters(IAsyncResult asyncResult) +42 
        //System.Web.Mvc.Async.<>c__DisplayClass3_6.<BeginInvokeAction>b__3() +50 
        //System.Web.Mvc.Async.<>c__DisplayClass3_1.<BeginInvokeAction>b__5(IAsyncResult asyncResult) +188 
        //System.Web.Mvc.Async.AsyncControllerActionInvoker.EndInvokeAction(IAsyncResult asyncResult) +38 
        //System.Web.Mvc.<>c.<BeginExecuteCore>b__152_1(IAsyncResult asyncResult, ExecuteCoreState innerState) +29 
        //System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +73 
        //System.Web.Mvc.Controller.EndExecuteCore(IAsyncResult asyncResult) +52 
        //System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +39 
        //System.Web.Mvc.Controller.EndExecute(IAsyncResult asyncResult) +38 
        //System.Web.Mvc.<>c.<BeginProcessRequest>b__20_1(IAsyncResult asyncResult, ProcessRequestState innerState) +43 
        //System.Web.Mvc.Async.WrappedAsyncVoid`1.CallEndDelegate(IAsyncResult asyncResult) +73 
        //System.Web.Mvc.MvcHandler.EndProcessRequest(IAsyncResult asyncResult) +38 
        //System.Web.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute() +431 
        //System.Web.HttpApplication.ExecuteStepImpl(IExecutionStep step) +75 
        //System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously) +158
        /// </remarks>
        public string Enter(string clientid, string nsupplierid, SyncBeneficiary entity, string npayeeid = null)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                var nsupplier = client.nSuppliers[nsupplierid];

                nPayee data;
                (data = new nPayee
                {
                    ID = npayeeid,
                    nSupplierID = nsupplierid,
                    EnterpriseID = clientid,
                    RealID = nsupplier.RealID,
                    RealEnterprise = new Enterprise
                    {
                        ID = nsupplier.ID,
                        Name = entity.RealName
                    },
                    Bank = entity.Bank,
                    BankAddress = entity.BankAddress,
                    Account = entity.Account,
                    SwiftCode = entity.SwiftCode,
                    Methord = entity.Methord,
                    Currency = entity.Currency,
                    Contact = entity.Contact ?? "",
                    Tel = entity.Tel,
                    Mobile = entity.Mobile,
                    Email = entity.Email,
                    Creator = "",
                    IsDefault = entity.IsDefault,
                    Place = entity.Place

                }).Enter();
                this.Synchro(data);//向芯达通同步

                return data.ID;
            }
        }
        public void Abandon(string clientid, string nsupplierid, string npayeeid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var supplier = client.nSuppliers[nsupplierid];
                var entity = new nPayeesRoll()[npayeeid];
                entity.Abandon();
                AbandonSynchro(client.Enterprise.Name, supplier.EnglishName, entity.Account);//向芯达通同步
            }
        }
        public void SetDefault(string clientid, string nsupplierid, string napyeeid)
        {
            using (var roll = new WsClientsRoll())
            {
                var client = roll[clientid];
                var supplier = client.nSuppliers[nsupplierid];
                var entity = supplier.nPayees[napyeeid];
                entity.IsDefault = true;
                entity.Enter();
                Synchro(entity);//向芯达通同步
            }
        }
        public void Synchro(nPayee data)
        {
            string url = Commons.UnifyApiUrl + "/suppliers/banks";
            Enterprise client = new EnterprisesRoll()[data.EnterpriseID];
            Enterprise supplier = new EnterprisesRoll()[data.RealID];
            var param = new
            {
                Enterprise = client,
                Place = data.Place,
                SupplierName = supplier.Name,
                IsDefault = data.IsDefault,
                Bank = data.Bank,
                BankAddress = data.BankAddress,
                Account = data.Account,
                SwiftCode = data.SwiftCode,
                Methord = data.Methord,
                Currency = data.Currency,
                Name = data.Contact,
                Tel = data.Tel,
                Mobile = data.Mobile,
                Email = data.Email,
            }.Json();
            var response = Commons.HttpPostRaw(url, param);
        }
        public void AbandonSynchro(string clientname, string suppliername, string account)
        {
            string url = Commons.UnifyApiUrl + "/suppliers/banks";
            Commons.CommonHttpRequest(url + "?name=" + clientname + "&supplierName=" + suppliername + "&account=" + account, "DELETE");
        }
    }

}

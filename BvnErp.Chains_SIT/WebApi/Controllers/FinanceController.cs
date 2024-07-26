using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using Needs.Utils;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    /// <summary>
    /// 财务相关接口
    /// </summary>
    public class FinanceController : MyApiController
    {
        /// <summary>
        /// 新增发票文件数据
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public ActionResult InsertInvoiceNoticeFiles(JPost files)
        {
            var requestModel = files.ToObject<InvoiceNoticeFileRequest>();
            var normalInvoiceNoticeFiles = requestModel.ToNormalServiceModel();


            string batchID = Guid.NewGuid().ToString("N");
            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                RequestContent = requestModel.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = "/Finance/InsertInvoiceNoticeFiles",
            };
            apiLog.Enter();


            foreach (var invoiceNoticeFile in normalInvoiceNoticeFiles)
            {
                invoiceNoticeFile.Enter();
            }

            var json = new JMessage()
            {
                code = 200,
                success = true,
                data = "提交成功"
            };

            apiLog.ResponseContent = json.Json();
            apiLog.Enter();

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除发票文件数据
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public ActionResult DeleteInvoiceNoticeFiles(JPost files)
        {
            var requestModel = files.ToObject<InvoiceNoticeFileRequest>();
            var deleteInvoiceNoticeFiles = requestModel.ToDeleteServiceModel();


            string batchID = Guid.NewGuid().ToString("N");
            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                RequestContent = requestModel.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = "/Finance/DeleteInvoiceNoticeFiles",
            };
            apiLog.Enter();


            foreach (var invoiceNoticeFile in deleteInvoiceNoticeFiles)
            {
                invoiceNoticeFile.Abandon();
            }

            var json = new JMessage()
            {
                code = 200,
                success = true,
                data = "提交成功"
            };

            apiLog.ResponseContent = json.Json();
            apiLog.Enter();

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 香港库房费用接口
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult InsertHKWarehouseFee(JPost requestData)
        {
            try
            {
                var requestModel = requestData.ToObject<WhesPremium>();

                if (requestModel.Premiums.Count < 1)
                {
                    return Json(new JMessage()
                    {
                        code = 300,
                        success = false,
                        data = "参数为空!"
                    }, JsonRequestBehavior.AllowGet);
                }

                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = "/Finance/InsertHKWarehouseFee",
                    RequestContent = requestModel.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "库房费用",
                };

                //转换操作人
                var OriginAdminID = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.Premiums[0].AdminID)?.OriginID;
                if (string.IsNullOrEmpty(OriginAdminID))
                {
                    OriginAdminID = "XDTAdmin";
                }

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    foreach (var premium in requestModel.Premiums)
                    {
                        //验证是否有重复
                        if (!new OrderWhesPremiumView().Any(t => t.ID == premium.ID && t.Status == Needs.Ccs.Services.Enums.Status.Normal))
                        {

                            var intStatus = premium.PaymentType == WhsePaymentType.Cash ? (int)WarehousePremiumsStatus.Payed : (int)WarehousePremiumsStatus.Auditing;

                            //20220118 大陆来货 相关额外收 100元入仓费
                            if (premium.WhesFeeType == WarehousePremiumType.MainlandClearance) 
                            {
                                #region 新增杂费

                                //var feeClearance = new Layer.Data.Sqls.ScCustoms.OrderPremiums();
                                ////ID status createdate updatedate
                                //feeClearance.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                                //feeClearance.OrderID = premium.TinyOrderID;
                                //feeClearance.Type = (int)OrderPremiumType.CustomClearanceFee;
                                //feeClearance.Name = "大陆来货清关费";
                                //feeClearance.Count = premium.Count;
                                //feeClearance.UnitPrice = premium.UnitPrice;
                                //feeClearance.Currency = premium.Currency;
                                //feeClearance.Rate = 1;
                                //feeClearance.Summary = "库房录入(大陆来货清关费)";
                                //feeClearance.AdminID = OriginAdminID;
                                //feeClearance.CreateDate = premium.CreateDate;
                                //feeClearance.UpdateDate = premium.CreateDate;
                                //feeClearance.Status = (int)Status.Normal;

                                //reponsitory.Insert(feeClearance);


                                //新增费用
                                var fee = new Needs.Ccs.Services.Models.OrderPremium();
                                fee.OrderID = premium.TinyOrderID; ;
                                fee.Type = OrderPremiumType.CustomClearanceFee;
                                fee.Name = "大陆来货清关费";
                                fee.Count = premium.Count;
                                fee.UnitPrice = premium.UnitPrice;
                                fee.Currency = premium.Currency;
                                fee.Rate = 1;
                                fee.Admin = new Admin { ID = OriginAdminID }; //Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(OriginAdminID);
                                fee.Summary = "库房录入(大陆来货清关费)";

                                fee.Enter();
                                #endregion

                                intStatus = (int)WarehousePremiumsStatus.Audited;
                            }


                            //使用实时汇率
                            var customRate = new RealTimeExchangeRatesView().Where(item => item.Code == premium.Currency).SingleOrDefault();

                            //生成库房费用
                            var entity = new Layer.Data.Sqls.ScCustoms.OrderWhesPremiums();
                            entity.ID = premium.ID;
                            entity.OrderID = premium.TinyOrderID;
                            entity.CreaterID = OriginAdminID;
                            entity.WarehouseType = (int)Needs.Ccs.Services.Enums.WarehouseType.HongKong;
                            entity.WhesFeeType = (int)premium.WhesFeeType;
                            entity.PaymentType = (int)premium.PaymentType;
                            entity.Count = premium.Count;
                            entity.UnitPrice = premium.UnitPrice;
                            entity.UnitName = "笔";
                            entity.Currency = premium.Currency;
                            entity.ExchangeRate = customRate.Rate;
                            entity.ApprovalPrice = premium.Count * premium.UnitPrice * customRate.Rate;
                            entity.PremiumsStatus = intStatus;
                            entity.Status = (int)Needs.Ccs.Services.Enums.Status.Normal;
                            entity.CreateDate = premium.CreateDate;
                            entity.UpdateDate = premium.CreateDate;
                            entity.Summary = premium.Summary;

                            reponsitory.Insert(entity);
                        }
                    }
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };

                apiLog.ResponseContent = json.Json();
                apiLog.Enter();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("库房费用新增失败");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 香港库房重构 费用接口
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult InsertHKWarehouseFeeNew(JPost requestData)
        {
            try
            {
                var requestModel = requestData.ToObject<HKWhesPremium>();

                if (requestModel.Premiums.Count < 1)
                {
                    return Json(new JMessage()
                    {
                        code = 300,
                        success = false,
                        data = "参数为空!"
                    }, JsonRequestBehavior.AllowGet);
                }

                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = "/Finance/InsertHKWarehouseFeeNew",
                    RequestContent = requestModel.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "库房费用",
                };

                //转换操作人
                var OriginAdminID = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.Premiums[0].AdminID)?.OriginID;
                if (string.IsNullOrEmpty(OriginAdminID))
                {
                    OriginAdminID = "XDTAdmin";
                }

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    foreach (var premium in requestModel.Premiums)
                    {
                        //验证是否有重复
                        if (!new OrderWhesPremiumView().Any(t => t.ID == premium.ID && t.Status == Needs.Ccs.Services.Enums.Status.Normal))
                        {

                            var intStatus = premium.PaymentType == WhsePaymentType.Cash ? (int)WarehousePremiumsStatus.Payed : (int)WarehousePremiumsStatus.Auditing;

                            //20220118 大陆来货 相关额外收 100元入仓费
                            if (premium.WhesFeeType == WarehousePremiumType.MainlandClearance)
                            {
                                #region 新增杂费
                             
                                //新增费用
                                var fee = new Needs.Ccs.Services.Models.OrderPremium();
                                fee.OrderID = premium.TinyOrderID; ;
                                fee.Type = OrderPremiumType.CustomClearanceFee;
                                fee.Name = "大陆来货清关费";
                                fee.Count = premium.Count;
                                fee.UnitPrice = premium.UnitPrice;
                                fee.Currency = premium.Currency;
                                fee.Rate = 1;
                                fee.Admin = new Admin { ID = OriginAdminID }; //Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(OriginAdminID);
                                fee.Summary = "库房录入(大陆来货清关费)";

                                fee.Enter();
                                #endregion

                                intStatus = (int)WarehousePremiumsStatus.Audited;
                            }


                            //使用实时汇率
                            var customRate = new RealTimeExchangeRatesView().Where(item => item.Code == premium.Currency).SingleOrDefault();

                            //生成库房费用
                            var entity = new Layer.Data.Sqls.ScCustoms.OrderWhesPremiums();
                            entity.ID = ChainsGuid.NewGuidUp();
                            entity.OrderID = premium.TinyOrderID;
                            entity.CreaterID = OriginAdminID;
                            entity.WarehouseType = (int)Needs.Ccs.Services.Enums.WarehouseType.HongKong;
                            entity.WhesFeeType = (int)premium.HKWhesFeeType;
                            entity.PaymentType = (int)premium.PaymentType;
                            entity.Count = premium.Count;
                            entity.UnitPrice = premium.UnitPrice/premium.Count;
                            entity.UnitName = "笔";
                            entity.Currency = premium.Currency;
                            entity.ExchangeRate = customRate.Rate;
                            entity.ApprovalPrice = premium.UnitPrice * customRate.Rate;
                            entity.PremiumsStatus = intStatus;
                            entity.Status = (int)Needs.Ccs.Services.Enums.Status.Normal;
                            entity.CreateDate = premium.CreateDate;
                            entity.UpdateDate = premium.CreateDate;
                            entity.Summary = premium.Summary;

                            reponsitory.Insert(entity);
                        }
                    }
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };

                apiLog.ResponseContent = json.Json();
                apiLog.Enter();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("库房费用新增失败");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 中心修改FinanceVaults
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult CenterVaultUpdate(JPost requestData)
        {
            try
            {
                var requestModel = requestData.ToObject<SendStrcut>();
                var centerVault = JsonConvert.DeserializeObject<CenterVault>(requestModel.model.ToString());
                var XDTLeaderID = new AdminsTopView2().FirstOrDefault(t => t.ID == centerVault.OwnerID)?.OriginID;

                if (requestModel.option.ToLower().Equals(CenterConstant.Enter))
                {
                    var XDTCreateID = new AdminsTopView2().FirstOrDefault(t => t.ID == centerVault.CreatorID)?.OriginID;

                    var financeVault = new Needs.Ccs.Services.Views.FinanceVaultsView().Where(t => t.Name == centerVault.Name).FirstOrDefault();
                    if (financeVault != null)
                    {
                        var nojson = new JMessage() { code = 400, success = false, data = "该金库已存在" };
                        return Json(nojson, JsonRequestBehavior.AllowGet);
                    }

                    var newFinanceVault = new Needs.Ccs.Services.Models.FinanceVault();
                    newFinanceVault.Name = centerVault.Name;
                    newFinanceVault.Leader = XDTLeaderID;
                    newFinanceVault.Summary = centerVault.Summary;
                    newFinanceVault.Admin = new Admin
                    {
                        ID = XDTCreateID
                    };

                    newFinanceVault.Enter();
                }
                else
                {
                    var financeVault = new Needs.Ccs.Services.Views.FinanceVaultsView().Where(t => t.Name == centerVault.OriginName).FirstOrDefault();
                    if (financeVault != null)
                    {
                        financeVault.Name = centerVault.Name;
                        financeVault.Leader = XDTLeaderID;
                        financeVault.Summary = centerVault.Summary;
                        if (requestModel.option.ToLower().Equals(CenterConstant.Delete))
                        {
                            financeVault.Status = Needs.Ccs.Services.Enums.Status.Delete;
                        }
                        financeVault.Enter();
                    }
                    else
                    {
                        var nojson = new JMessage() { code = 400, success = false, data = "该金库不存在" };
                        return Json(nojson, JsonRequestBehavior.AllowGet);
                    }
                }


                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心修改FinanceVaults失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>

        public ActionResult CenterAccountUpdate(JPost requestData)
        {
            try
            {
                var requestModel = requestData.ToObject<SendStrcut>();
                var centerAccount = JsonConvert.DeserializeObject<CenterAccount>(requestModel.model.ToString());
                var XDTOwnerID = new AdminsTopView2().FirstOrDefault(t => t.ID == centerAccount.Owner)?.OriginID;
                var XDTCreatorID = new AdminsTopView2().FirstOrDefault(t => t.ID == centerAccount.CreatorID)?.OriginID;

                //简易账户
                if (centerAccount.AccountSource == (int)AccountSource.easy)
                {
                    var financeVault = new Needs.Ccs.Services.Models.FinanceAccount();
                    financeVault.AccountName = centerAccount.AccountName;
                    financeVault.BankName = centerAccount.BankName;
                    financeVault.BankAccount = centerAccount.BankAccount;
                    financeVault.Currency = "CNY";
                    financeVault.Balance = 0;
                    financeVault.Summary = centerAccount.Summary;
                    financeVault.Admin = Needs.Underly.FkoFactory<Admin>.Create(XDTCreatorID);
                    financeVault.AccountType = AccountType.basic;
                    financeVault.AdminInchargeID = XDTOwnerID;
                    financeVault.Balance = centerAccount.Balance == null ? 0 : centerAccount.Balance.Value;
                    financeVault.AccountSource = AccountSource.easy;
                    financeVault.Enter();

                }
                else
                {
                    if (centerAccount.CompanyName.Equals("深圳市华芯通供应链管理有限公司") || centerAccount.CompanyName.Equals("香港万路通国际物流有限公司") || centerAccount.CompanyName.Equals("香港畅运国际物流有限公司"))
                    {
                        var vault = new Needs.Ccs.Services.Views.FinanceVaultsView().Where(t => t.Name == centerAccount.VaultName).FirstOrDefault()?.ID;
                        if (vault == null)
                        {
                            var errorjson = new JMessage() { code = 400, success = false, data = "金库不存在,请先同步金库!" };
                            return Json(errorjson, JsonRequestBehavior.AllowGet);
                        }

                        AccountType accountType;
                        if (centerAccount.AccountType == "")
                        {
                            accountType = AccountType.basic;
                        }
                        else
                        {
                            AccountTypeTransfer transfer = new AccountTypeTransfer();
                            accountType = transfer.Combine(centerAccount.AccountType);
                        }


                        if (requestModel.option.ToLower().Equals(CenterConstant.Enter))
                        {
                            var oldfinanceVault = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == centerAccount.BankAccount).FirstOrDefault();
                            if (oldfinanceVault != null)
                            {
                                var errorjson = new JMessage() { code = 400, success = false, data = "该账号已存在" };
                                return Json(errorjson, JsonRequestBehavior.AllowGet);
                            }


                            var financeVault = new Needs.Ccs.Services.Models.FinanceAccount();
                            financeVault.FinanceVaultID = vault;
                            financeVault.AccountName = centerAccount.AccountName;
                            financeVault.BankName = centerAccount.BankName;
                            financeVault.BankAddress = centerAccount.BankAddress;
                            financeVault.BankAccount = centerAccount.BankAccount;
                            financeVault.SwiftCode = centerAccount.SwiftCode;
                            financeVault.Currency = centerAccount.Currency;
                            financeVault.Balance = 0;
                            financeVault.Summary = centerAccount.Summary;
                            financeVault.Admin = new Admin { ID = XDTOwnerID };
                            financeVault.AccountType = accountType;
                            financeVault.AdminInchargeID = XDTOwnerID;
                            financeVault.CompanyName = centerAccount.CompanyName;
                            financeVault.Region = centerAccount.Region;
                            financeVault.Balance = centerAccount.Balance == null ? 0 : centerAccount.Balance.Value;
                            financeVault.AccountSource = (AccountSource)centerAccount.AccountSource;
                            financeVault.Enter();
                        }
                        else
                        {
                            var financeVault = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == centerAccount.BankAccount).FirstOrDefault();
                            if (financeVault != null)
                            {
                                financeVault.FinanceVaultID = vault;
                                financeVault.AccountName = centerAccount.AccountName;
                                financeVault.BankName = centerAccount.BankName;
                                financeVault.BankAddress = centerAccount.BankAddress;
                                financeVault.SwiftCode = centerAccount.SwiftCode;
                                financeVault.Currency = centerAccount.Currency;
                                financeVault.Summary = centerAccount.Summary;
                                financeVault.AccountType = accountType;
                                financeVault.AdminInchargeID = XDTOwnerID;
                                financeVault.CompanyName = centerAccount.CompanyName;
                                financeVault.Region = centerAccount.Region;
                                if (requestModel.option.ToLower().Equals(CenterConstant.Delete))
                                {
                                    financeVault.Status = Needs.Ccs.Services.Enums.Status.Delete;
                                }
                                financeVault.Balance = centerAccount.Balance == null ? 0 : centerAccount.Balance.Value;
                                financeVault.AccountSource = (AccountSource)centerAccount.AccountSource;
                                financeVault.Enter();
                            }
                            else
                            {
                                var nojson = new JMessage() { code = 400, success = false, data = "该账号不存在" };
                                return Json(nojson, JsonRequestBehavior.AllowGet);
                            }
                        }


                    }
                    else
                    {
                        var accountjson = new JMessage() { code = 400, success = false, data = "只同步华芯通、万路通、畅运账户信息" };
                        return Json(accountjson, JsonRequestBehavior.AllowGet);
                    }
                }

                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ex.CcsLog("中心修改FinanceAccount失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 中心同步收款
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult FinanceReceipt(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                CenterFinanceReceipt requestModel = JsonConvert.DeserializeObject<CenterFinanceReceipt>(outModel.model.ToString());
                var financeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.Account).FirstOrDefault();
                if (financeAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }
                if (requestModel.Rate == 1)
                {
                    requestModel.Currency = "CNY";
                }
                var XDTCreateID = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID)?.OriginID;

                FinanceFeeType feeType = FeeTypeTransfer.Current.C2LInTransfer(requestModel.FeeType);
                int paymentType = PaymentTypeTransfer.Current.C2LTransfer((CenterPaymentType)requestModel.ReceiptType);

                var financeReceipt = new Needs.Ccs.Services.Models.FinanceReceipt();
                financeReceipt.Payer = requestModel.Payer;
                financeReceipt.SeqNo = requestModel.SeqNo;
                financeReceipt.FeeType = feeType;
                financeReceipt.ReceiptType = (PaymentType)paymentType;
                financeReceipt.ReceiptDate = requestModel.ReceiptDate;
                financeReceipt.Currency = requestModel.Currency;
                financeReceipt.Rate = requestModel.Rate;
                financeReceipt.Vault = new FinanceVault { ID = financeAccount.FinanceVaultID };
                financeReceipt.Amount = (requestModel.Amount).ToRound(2);
                financeReceipt.Account = financeAccount;
                financeReceipt.Admin = new Admin { ID = XDTCreateID };
                financeReceipt.Summary = requestModel.Summary;
                financeReceipt.AccountProperty = (AccountProperty)requestModel.AccountSource;


                financeReceipt.ApiEnter();

                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心修改FinanceReceipt失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 中心批量同步收款
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult FinanceReceiptMulti(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                List<CenterFinanceReceipt> requestModels = JsonConvert.DeserializeObject<List<CenterFinanceReceipt>>(outModel.model.ToString());

                foreach (var requestModel in requestModels)
                {
                    var financeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.Account).FirstOrDefault();
                    if (financeAccount == null)
                    {
                        var accountJson = new JMessage() { code = 400, success = false, data = "账号不存在" };
                        return Json(accountJson, JsonRequestBehavior.AllowGet);
                    }
                    if (requestModel.Rate == 1)
                    {
                        requestModel.Currency = "CNY";
                    }
                    var XDTCreateID = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID)?.OriginID;

                    FinanceFeeType feeType = FeeTypeTransfer.Current.C2LInTransfer(requestModel.FeeType);
                    int paymentType = PaymentTypeTransfer.Current.C2LTransfer((CenterPaymentType)requestModel.ReceiptType);

                    var financeReceipt = new Needs.Ccs.Services.Models.FinanceReceipt();
                    financeReceipt.Payer = requestModel.Payer;
                    financeReceipt.SeqNo = requestModel.SeqNo;
                    financeReceipt.FeeType = feeType;
                    financeReceipt.ReceiptType = (PaymentType)paymentType;
                    financeReceipt.ReceiptDate = requestModel.ReceiptDate;
                    financeReceipt.Currency = requestModel.Currency;
                    financeReceipt.Rate = requestModel.Rate;
                    financeReceipt.Vault = new FinanceVault { ID = financeAccount.FinanceVaultID };
                    financeReceipt.Amount = (requestModel.Amount).ToRound(2);
                    financeReceipt.Account = financeAccount;
                    financeReceipt.Admin = new Admin { ID = XDTCreateID };
                    financeReceipt.Summary = requestModel.Summary;
                    financeReceipt.AccountProperty = (AccountProperty)requestModel.AccountSource;


                    financeReceipt.ApiEnter();
                }

                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心修改FinanceReceipt失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 中心同步付款
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult FinancePayment(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                CenterFinancePayment requestModel = (CenterFinancePayment)outModel.model;


                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心修改FinancePayments失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 中心同步费用
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult FinanceFee(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                CenterFee requestModel = JsonConvert.DeserializeObject<CenterFee>(outModel.model.ToString());

                var PayerAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.AccountNo).FirstOrDefault();
                if (PayerAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "付款方账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }

                var XDTCreator = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID);
                var XDTCreateID = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID)?.OriginID;

                CostApply costApply = new CostApply();
                costApply.ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayApplicant);

                foreach (var item in requestModel.FeeItems)
                {
                    FinanceFeeType feeType = FeeTypeTransfer.Current.C2LOutTransfer(item.FeeType);

                    CostApplyItem applyItem = new CostApplyItem();
                    applyItem.ID = ChainsGuid.NewGuidUp();
                    applyItem.CostApplyID = costApply.ID;
                    applyItem.FeeType = feeType;
                    applyItem.Amount = item.Amount;
                    applyItem.FeeDesc = item.FeeDesc;

                    costApply.Items.Add(applyItem);
                }

                var PayeeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.ReceiveAccountNo).FirstOrDefault();
                if (PayeeAccount == null)
                {
                    if (requestModel.MoneyType == (int)MoneyTypeEnum.BankAutoApply)
                    {
                        PayeeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.AccountName == "未知").FirstOrDefault();
                    }
                    else
                    {
                        if (!costApply.Items.Any(t => t.FeeType == FinanceFeeType.PaySalary))
                        {
                            var accountJson = new JMessage() { code = 400, success = false, data = "收款方账号不存在" };
                            return Json(accountJson, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            PayeeAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.AccountName == "工资账户").FirstOrDefault();
                        }
                    }
                }

                costApply.PayeeAccountID = PayeeAccount.ID;
                costApply.PayeeName = PayeeAccount.AccountName;
                costApply.PayeeAccount = PayeeAccount.BankAccount;
                costApply.PayeeBank = PayeeAccount.BankName;
                costApply.Amount = requestModel.Amount;
                costApply.Currency = requestModel.Currency;
                costApply.CostStatus = CostStatusEnum.PaySuccess;
                costApply.AdminID = XDTCreateID;
                costApply.PayTime = requestModel.PaymentDate;

                costApply.MoneyType = (MoneyTypeEnum)requestModel.MoneyType;
                costApply.CashType = CashTypeEnum.Common;

                costApply.Enter();

                #region 数据写入PaymentNotices
                PaymentNotice paymentNotice = new PaymentNotice();
                paymentNotice.SeqNo = requestModel.SeqNo;
                paymentNotice.Admin = new Admin { ID = XDTCreateID };
                //paymentNotice.Payer = new Admin { ID = "Admin0000000361" }; //默认付款人是郝红梅
                paymentNotice.Payer = new Admin { ID = "Admin0000009365" }; //默认付款人是郝红梅
                paymentNotice.FinanceVault = new FinanceVault { ID = PayerAccount.FinanceVaultID };
                paymentNotice.FinanceAccount = PayerAccount;
                paymentNotice.PayFeeType = costApply.Items.FirstOrDefault().FeeType; //默认是费用里的第一个
                paymentNotice.PayeeName = PayeeAccount.AccountName;
                paymentNotice.BankAccount = PayeeAccount.BankAccount;
                paymentNotice.BankName = PayeeAccount.BankName;
                paymentNotice.Amount = costApply.Amount;
                paymentNotice.Currency = requestModel.Currency;
                paymentNotice.ExchangeRate = requestModel.Rate;
                paymentNotice.PayDate = requestModel.PaymentDate;
                int paymentType = PaymentTypeTransfer.Current.C2LTransfer((CenterPaymentType)requestModel.PaymentType);
                paymentNotice.PayType = (PaymentType)paymentType; ; //要转换一下
                paymentNotice.Status = PaymentNoticeStatus.Paid;
                paymentNotice.CostApplyID = costApply.ID;
                paymentNotice.Enter();
                string paymentNoticeID = paymentNotice.ID;
                #endregion

                #region 补充CostApplyLogs,下载文件，增加付款记录扣除账户余额
                CostApplyLog(costApply.ID, XDTCreator);
                PaymentLog(paymentNotice);
                uploadFiles(requestModel, costApply.ID, XDTCreator.OriginID, paymentNoticeID);
                #endregion

                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心提交FinanceFee失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void uploadFiles(CenterFee requestModel, string costApplyID, string XDTCreateID, string paymentNoticeID)
        {
            string newurl = "";
            try
            {

                List<Needs.Ccs.Services.Models.CostApplyFile> costApplyFiles = new List<Needs.Ccs.Services.Models.CostApplyFile>();
                List<Needs.Ccs.Services.Models.PaymentNoticeFile> noticeFiles = new List<Needs.Ccs.Services.Models.PaymentNoticeFile>();
                foreach (var file in requestModel.Files)
                {
                    FileDirectory fileDic = new FileDirectory(file.FileName);
                    string newFilePath = fileDic.FilePath.Replace("api", "foricadmin").Replace("foric_0", "v8_0");
                    fileDic.SetFilePathFormDifferentSite(newFilePath);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Cost);
                    fileDic.CreateDataDirectory();

                    var webClient = new WebClient();
                    webClient.DownloadFile(file.Url, newurl);

                    if (file.FileType == (int)CenterFeeFileType.FeeFile)
                    {
                        costApplyFiles.Add(new Needs.Ccs.Services.Models.CostApplyFile
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            CostApplyID = costApplyID,
                            AdminID = XDTCreateID,
                            Name = file.FileName,
                            FileType = CostApplyFileTypeEnum.Inovice,
                            FileFormat = file.FileFormat,
                            URL = fileDic.VirtualPath.Replace(@"/", @"\"),
                            Status = Status.Normal,
                            CreateDate = DateTime.Now,
                        });
                    }

                    if (file.FileType == (int)CenterFeeFileType.PayFile)
                    {
                        PaymentNoticeFile NoticeFile = new PaymentNoticeFile()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            PaymentNoticeID = paymentNoticeID,
                            FileName = file.FileName,
                            FileFormat = file.FileFormat,
                            Url = fileDic.VirtualPath.Replace(@"/", @"\"),
                            FileType = FileType.PaymentVoucher,
                            AdminID = XDTCreateID,
                            CreateDate = DateTime.Now,
                            Status = Status.Normal,
                        };
                        noticeFiles.Add(NoticeFile);
                    }

                }

                foreach (var costApplyFile in costApplyFiles)
                {
                    costApplyFile.Enter();
                }

                foreach (var noticeFile in noticeFiles)
                {
                    noticeFile.Enter();
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog(newurl);
            }

        }

        private void CostApplyLog(string costApplyID, Admin Creator)
        {
            CostApplyLog costApplyLog1 = new CostApplyLog();
            costApplyLog1.ID = ChainsGuid.NewGuidUp();
            costApplyLog1.CostApplyID = costApplyID;
            costApplyLog1.AdminID = Creator.ID;
            costApplyLog1.CurrentCostStatus = CostStatusEnum.UnSubmit;
            costApplyLog1.NextCostStatus = CostStatusEnum.FinanceStaffUnApprove;
            costApplyLog1.CreateDate = DateTime.Now;
            costApplyLog1.Summary = Creator.RealName + "提交了费用申请";
            costApplyLog1.Enter();

            CostApplyLog costApplyLog2 = new CostApplyLog();
            costApplyLog2.ID = ChainsGuid.NewGuidUp();
            costApplyLog2.CostApplyID = costApplyID;
            //costApplyLog2.AdminID = "Admin0000009426";//测试ID
            costApplyLog2.AdminID = "Admin0000009426";//正式ID
            costApplyLog2.CurrentCostStatus = CostStatusEnum.FinanceStaffUnApprove;
            costApplyLog2.NextCostStatus = CostStatusEnum.ManagerUnApprove;
            costApplyLog2.CreateDate = DateTime.Now;
            costApplyLog2.Summary = "施思静通过了费用申请";
            costApplyLog2.Enter();

            CostApplyLog costApplyLog3 = new CostApplyLog();
            costApplyLog3.ID = ChainsGuid.NewGuidUp();
            costApplyLog3.CostApplyID = costApplyID;
            //costApplyLog3.AdminID = "Admin0000000282";//测试ID
            costApplyLog3.AdminID = "Admin0000000282";//正式ID
            costApplyLog3.CurrentCostStatus = CostStatusEnum.ManagerUnApprove;
            costApplyLog3.NextCostStatus = CostStatusEnum.UnPay;
            costApplyLog3.CreateDate = DateTime.Now;
            costApplyLog3.Summary = "张庆永通过了费用申请..";
            costApplyLog3.Enter();

            CostApplyLog costApplyLog4 = new CostApplyLog();
            costApplyLog4.ID = ChainsGuid.NewGuidUp();
            costApplyLog4.CostApplyID = costApplyID;
            //costApplyLog4.AdminID = "Admin0000000361";//测试ID
            costApplyLog4.AdminID = "Admin0000009365"; //正式ID
            costApplyLog4.CurrentCostStatus = CostStatusEnum.UnPay;
            costApplyLog4.NextCostStatus = CostStatusEnum.PaySuccess;
            costApplyLog4.CreateDate = DateTime.Now;
            costApplyLog4.Summary = "财务[郝红梅]完成付款..";
            costApplyLog4.Enter();
        }

        private void PaymentLog(PaymentNotice paymentNotice)
        {
            var financePayment = new FinancePayment(paymentNotice);
            financePayment.Enter();
        }

        /// <summary>
        /// 获取费用Json 临时用
        /// </summary>
        /// <returns></returns>
        public ActionResult FinanceFeeJson()
        {
            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";
            sendStrcut.option = CenterConstant.Enter;

            CenterFee fee = new CenterFee();
            fee.ReceiveAccountNo = "258258";
            fee.AccountNo = "2021-0118-1457-0000";
            fee.Amount = 5;
            fee.Currency = "CNY";
            fee.Rate = 1;
            fee.PaymentDate = Convert.ToDateTime("2021-02-19");
            fee.PaymentType = 1;
            fee.CreatorID = "Admin00530";
            fee.SeqNo = "56987456";
            fee.MoneyType = 2;

            fee.FeeItems = new List<CenterFeeItem>();

            CenterFeeItem centerFeeItem0 = new CenterFeeItem();
            centerFeeItem0.FeeType = "AccCatType0367";
            centerFeeItem0.Amount = 2;
            fee.FeeItems.Add(centerFeeItem0);

            CenterFeeItem centerFeeItem1 = new CenterFeeItem();
            centerFeeItem1.FeeType = "AccCatType0368";
            centerFeeItem1.Amount = 3;
            fee.FeeItems.Add(centerFeeItem1);

            sendStrcut.model = fee.Json();
            var json = sendStrcut.Json();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 中心同步 资金挑拨
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult FundTransfer(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                CenterFundTransfer requestModel = JsonConvert.DeserializeObject<CenterFundTransfer>(outModel.model.ToString());
                var OutAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.OutAccountNo).FirstOrDefault();
                if (OutAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "调出账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }
                var InAccount = new Needs.Ccs.Services.Views.FinanceAccountsView().Where(t => t.BankAccount == requestModel.InAccountNo).FirstOrDefault();
                if (InAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "调入账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }
                var XDTCreator = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID);

                FundTransferApplies fundTransfer = new FundTransferApplies();
                fundTransfer.OutAccount = OutAccount;
                fundTransfer.OutAmount = requestModel.OutAmount;
                fundTransfer.OutCurrency = requestModel.OutCurrency;
                fundTransfer.OutSeqNo = requestModel.OutSeqNo;
                fundTransfer.InAccount = InAccount;
                fundTransfer.InAmount = requestModel.InAmount;
                fundTransfer.InCurrency = requestModel.InCurrency;
                fundTransfer.InSeqNo = requestModel.InSeqNo;
                fundTransfer.Rate = requestModel.Rate;
                fundTransfer.DiscountInterest = requestModel.DiscountInterest;
                fundTransfer.FromSeqNo = requestModel.DiscountSeqNo;
                fundTransfer.Poundage = requestModel.Poundage;
                fundTransfer.PoundageSeqNo = requestModel.PoundageSeqNo;

                fundTransfer.FeeType = (FundTransferType)Convert.ToInt16(requestModel.FeeType);

                int paymentType = PaymentTypeTransfer.Current.C2LTransfer((CenterPaymentType)requestModel.PaymentType);
                fundTransfer.PaymentType = (PaymentType)paymentType;
                fundTransfer.PaymentDate = requestModel.PaymentDate;
                fundTransfer.Admin = new Admin { ID = XDTCreator.OriginID };
                fundTransfer.ApplyStatus = FundTransferApplyStatus.Done;

                fundTransfer.Enter();
                FundTransferLog(fundTransfer);
                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心提交FinanceFee失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void FundTransferLog(FundTransferApplies apply)
        {
            try
            {
                //付款
                FinancePayment payOut = new FinancePayment();
                payOut.SeqNo = apply.OutSeqNo;
                payOut.PayeeName = apply.OutAccount.AccountName;
                payOut.BankName = apply.OutAccount.BankName;
                payOut.BankAccount = apply.OutAccount.BankAccount;
                payOut.Payer = apply.Admin;
                payOut.PayFeeType = FinanceFeeType.FundTransfer;
                payOut.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                payOut.FinanceAccount = apply.OutAccount;
                payOut.Amount = apply.OutAmount;
                payOut.Currency = "CNY";
                payOut.ExchangeRate = 1.0M;
                payOut.PayType = apply.PaymentType;
                payOut.PayDate = DateTime.Now;
                payOut.Enter();

                //手续费
                if (apply.Poundage != 0 && apply.Poundage != null)
                {
                    FinancePayment Poundage = new FinancePayment();
                    Poundage.SeqNo = apply.PoundageSeqNo;
                    Poundage.PayeeName = apply.OutAccount.AccountName;
                    Poundage.Payer = apply.Admin;
                    Poundage.PayFeeType = FinanceFeeType.Poundage;
                    Poundage.FinanceVault = new FinanceVault { ID = apply.OutAccount.FinanceVaultID };
                    apply.OutAccount.Balance -= apply.OutAmount;
                    Poundage.FinanceAccount = apply.OutAccount;
                    Poundage.BankName = apply.OutAccount.BankName;
                    Poundage.BankAccount = apply.OutAccount.BankAccount;
                    Poundage.Amount = apply.Poundage.Value;
                    Poundage.Currency = "CNY";
                    Poundage.ExchangeRate = 1.0M;
                    Poundage.PayType = apply.PaymentType;
                    Poundage.PayDate = DateTime.Now;
                    Poundage.Enter();
                }

                //收款
                FinanceReceipt recIn = new FinanceReceipt();
                recIn.SeqNo = apply.InSeqNo;
                recIn.Payer = apply.OutAccount.AccountName;
                recIn.FeeType = FinanceFeeType.FundTransfer;
                recIn.ReceiptType = apply.PaymentType;
                recIn.ReceiptDate = DateTime.Now;
                recIn.Currency = "CNY";
                recIn.Rate = 1.0M;
                recIn.Amount = apply.InAmount;
                recIn.Vault = new FinanceVault { ID = apply.InAccount.FinanceVaultID };
                recIn.Account = apply.InAccount;
                recIn.Admin = apply.Admin;
                recIn.Enter();
            }
            catch (Exception EX)
            {

            }
        }

        /// <summary>
        /// 获取资金调拨Json 临时用
        /// </summary>
        /// <returns></returns>
        public ActionResult FundTransferJson()
        {
            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";
            sendStrcut.option = CenterConstant.Enter;

            CenterFundTransfer fundTransfer = new CenterFundTransfer();
            fundTransfer.OutAccountNo = "2021-0118-1457-0000";
            fundTransfer.OutAmount = 500;
            fundTransfer.OutCurrency = "CNY";
            fundTransfer.OutSeqNo = "Out20210208-001";

            fundTransfer.InAccountNo = "2021-0118-1634-0000";
            fundTransfer.InAmount = 500;
            fundTransfer.InCurrency = "CNY";
            fundTransfer.InSeqNo = "In20210208-001";

            fundTransfer.PaymentType = 1;
            fundTransfer.Rate = 1;
            fundTransfer.FeeType = (int)FundTransferType.ProductFee;
            fundTransfer.PaymentDate = Convert.ToDateTime("2021-02-08");
            fundTransfer.CreatorID = "Admin00530";

            fundTransfer.DiscountInterest = 50;
            fundTransfer.DiscountSeqNo = "130152300001720200629668590789";
            fundTransfer.Poundage = 60;
            fundTransfer.PoundageSeqNo = "125629937165";

            sendStrcut.model = fundTransfer.Json();
            var json = sendStrcut.Json();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 货款Json
        /// </summary>
        /// <returns></returns>
        public ActionResult FinancePayementJson()
        {
            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";
            sendStrcut.option = CenterConstant.Enter;

            string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(Needs.Ccs.Services.Enums.FinanceFeeType.PayGoods);
            int paymentType = PaymentTypeTransfer.Current.L2CTransfer(Needs.Ccs.Services.Enums.PaymentType.TransferAccount);

            CenterFinancePayment fee = new CenterFinancePayment();
            fee.FeeType = ceterFeetype;
            fee.InAccountNo = "2021-0118-1457-0000";
            fee.MidAccountNo = "6224915186970311";
            fee.OutAccountNo = "40551285303272534";
            fee.RMBAmount = 6521;
            fee.Currency = "USD";
            fee.Amount = 1000;
            fee.Rate = 6.521m;
            fee.InSeqNo = "In2021-02-25-001";
            fee.OutSeqNo = "Out2021-02-25-001";
            fee.MidInSeqNo = "MidIn2021-02-25-001";
            fee.MidOutSeqNo = "MidOut2021-02-25-001";
            fee.Poundage = 10;
            fee.PoundageSeqNo = "P2021-02-25-001";
            fee.PaymentType = paymentType;
            fee.PaymentDate = DateTime.Now;
            fee.CreatorID = "Admin00530";

            sendStrcut.model = fee.Json();
            var json = sendStrcut.Json();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 测试文件保存
        /// </summary>
        /// <returns></returns>
        public ActionResult FileSave()
        {
            string newurl = "";
            string oldUrl = "";
            try
            {
                //网络文件地址
                string file_url = @"http://uuws.b1b.com/2021/3/2/FilesDesc00304.pdf";
                //string file_url = @"http://api0.for-ic.net/Files/Cost/202103/02/DBSTest.pdf";

                FileDirectory fileDic = new FileDirectory("FilesDesc00304.pdf");
                string newFilePath = fileDic.FilePath.Replace("api", "foricadmin").Replace("foric_0", "v8_0");
                fileDic.SetFilePathFormDifferentSite(newFilePath);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Cost);
                fileDic.CreateDataDirectory();

                var webClient = new WebClient();
                oldUrl = fileDic.FilePath;

                webClient.DownloadFile(file_url, oldUrl);

                var json = new JMessage() { code = 200, success = true, data = oldUrl };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog(newurl);
                var json = new JMessage() { code = 400, success = false, data = newurl + "&" + oldUrl };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult FinanceReceiptMultiJson()
        {
            SendStrcut sendStrcut = new SendStrcut();
            sendStrcut.sender = "FSender001";
            sendStrcut.option = CenterConstant.Enter;

            List<CenterFinanceReceipt> centerFinanceReceipts = new List<CenterFinanceReceipt>();

            CenterFinanceReceipt centerFinanceReceipt = new CenterFinanceReceipt();
            centerFinanceReceipt.SeqNo = "FR2021-0324-001";
            centerFinanceReceipt.Payer = "李传浩";
            centerFinanceReceipt.FeeType = "AccCatType0038";
            centerFinanceReceipt.ReceiptType = 1;
            centerFinanceReceipt.ReceiptDate = DateTime.Now;
            centerFinanceReceipt.Currency = "CNY";
            centerFinanceReceipt.Rate = 1;
            centerFinanceReceipt.Amount = 100;
            centerFinanceReceipt.Account = "6224920783483395";
            centerFinanceReceipt.CreatorID = "Admin00530";
            centerFinanceReceipt.AccountSource = 2;

            centerFinanceReceipts.Add(centerFinanceReceipt);

            CenterFinanceReceipt centerFinanceReceipt1 = new CenterFinanceReceipt();
            centerFinanceReceipt1.SeqNo = "FR2021-0324-002";
            centerFinanceReceipt1.Payer = "曹丽萍";
            centerFinanceReceipt1.FeeType = "AccCatType0038";
            centerFinanceReceipt1.ReceiptType = 1;
            centerFinanceReceipt1.ReceiptDate = DateTime.Now;
            centerFinanceReceipt1.Currency = "CNY";
            centerFinanceReceipt1.Rate = 1;
            centerFinanceReceipt1.Amount = 100;
            centerFinanceReceipt1.Account = "6224920783483395";
            centerFinanceReceipt1.CreatorID = "Admin00530";
            centerFinanceReceipt1.AccountSource = 2;

            centerFinanceReceipts.Add(centerFinanceReceipt1);

            sendStrcut.model = centerFinanceReceipts;
            var json = sendStrcut.Json();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 中心同步费用
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public ActionResult AcceptanceBill(JPost requestData)
        {
            try
            {
                var outModel = requestData.ToObject<SendStrcut>();
                CenterAcceptanceBill requestModel = JsonConvert.DeserializeObject<CenterAcceptanceBill>(outModel.model.ToString());
                var XDTCreator = new AdminsTopView2().FirstOrDefault(t => t.ID == requestModel.CreatorID);

                var OutAccount = new Needs.Ccs.Services.Views.FinanceAccountsOriginView().Where(t => t.BankAccount == requestModel.PayerAccountNo).FirstOrDefault();
                if (OutAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "调出账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }
                var InAccount = new Needs.Ccs.Services.Views.FinanceAccountsOriginView().Where(t => t.BankAccount == requestModel.PayeeAccountNo).FirstOrDefault();
                if (InAccount == null)
                {
                    var accountJson = new JMessage() { code = 400, success = false, data = "调入账号不存在" };
                    return Json(accountJson, JsonRequestBehavior.AllowGet);
                }

                AcceptanceBill acceptance = new AcceptanceBill();
                acceptance.ID = ChainsGuid.NewGuidUp();
                acceptance.Code = requestModel.Code;
                acceptance.Name = requestModel.Name;
                acceptance.BankCode = requestModel.BankCode;
                acceptance.BankName = requestModel.BankName;
                acceptance.BankNo = requestModel.BankNo;
                acceptance.Price = requestModel.Price;
                acceptance.IsMoney = requestModel.IsMoney;
                acceptance.IsTransfer = requestModel.IsTransfer;
                acceptance.PayerAccount = OutAccount;
                acceptance.PayeeAccount = InAccount;
                acceptance.StartDate = requestModel.StartDate;
                acceptance.EndDate = requestModel.EndDate;
                acceptance.Nature = requestModel.Nature;
                acceptance.Creator = new Admin { ID = XDTCreator.OriginID};
                acceptance.Endorser = requestModel.Endorser;
                if (requestModel.ExchangeDate != null)
                {
                    acceptance.ExchangeDate = requestModel.ExchangeDate;
                }
                if (requestModel.ExchangePrice != null)
                {
                    acceptance.ExchangePrice = requestModel.ExchangePrice;
                }

                acceptance.Enter();
                uploadAcceptanceFiles(requestModel, acceptance.ID, XDTCreator.ID);
                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("中心提交AcceptanceBill失败");
                var json = new JMessage() { code = 400, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void uploadAcceptanceFiles(CenterAcceptanceBill requestModel, string costApplyID, string XDTCreateID)
        {
            string newurl = "";
            try
            {

                List<Needs.Ccs.Services.Models.CostApplyFile> costApplyFiles = new List<Needs.Ccs.Services.Models.CostApplyFile>();

                foreach (var file in requestModel.Files)
                {
                    FileDirectory fileDic = new FileDirectory(file.FileName);
                    string newFilePath = fileDic.FilePath.Replace("api", "foricadmin").Replace("foric_0", "v8_0");
                    fileDic.SetFilePathFormDifferentSite(newFilePath);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Cost);
                    fileDic.CreateDataDirectory();
                    newurl = fileDic.FilePath;

                    var webClient = new WebClient();
                    webClient.DownloadFile(file.Url, newurl);

                    costApplyFiles.Add(new Needs.Ccs.Services.Models.CostApplyFile
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        CostApplyID = costApplyID,
                        AdminID = XDTCreateID,
                        Name = file.FileName,
                        FileType = CostApplyFileTypeEnum.Inovice,
                        FileFormat = file.FileFormat,
                        URL = fileDic.VirtualPath.Replace(@"/", @"\"),
                        Status = Status.Normal,
                        CreateDate = DateTime.Now,
                    });

                }

                foreach (var costApplyFile in costApplyFiles)
                {
                    costApplyFile.Enter();
                }


            }
            catch (Exception ex)
            {
                ex.CcsLog(newurl);
            }

        }
    }
}

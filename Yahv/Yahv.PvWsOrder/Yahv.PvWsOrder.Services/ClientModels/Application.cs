using Layers.Data.Sqls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    /// <summary>
    /// 代收代付货款申请
    /// </summary>
    public class Application : Models.Application
    {
        /// <summary>
        /// 申请
        /// </summary>
        public Application()
        {
            this.DelivaryOpportunity = Yahv.PvWsOrder.Services.Enums.DelivaryOpportunity.CashOn;
        }

        public Application(string id, ApplicationType type)
        {
            this.IsEntry = false;
            this.DelivaryOpportunity = Yahv.PvWsOrder.Services.Enums.DelivaryOpportunity.CashOn;
            if (!string.IsNullOrEmpty(id))
            {
                var entity = new ApplicationsOrigin().FirstOrDefault(m => m.ID == id && m.Type == type);
                if (entity != null)
                {
                    var reponsitory = new PvWsOrderReponsitory();
                    this.ID = id;
                    this.ClientID = entity.ClientID;
                    this.Type = type;
                    this.TotalPrice = entity.TotalPrice;
                    this.Currency = entity.Currency;
                    this.RateToUSD = entity.RateToUSD;
                    this.InCompanyName = entity.InCompanyName;
                    this.InBankName = entity.InBankName;
                    this.InBankAccount = entity.InBankAccount;
                    this.OutCompanyName = entity.OutCompanyName;
                    this.OutBankName = entity.OutBankName;
                    this.OutBankAccount = entity.OutBankAccount;
                    this.Status = entity.Status;
                    this.ApplicationStatus = entity.ApplicationStatus;
                    this.ReceiveStatus = entity.ReceiveStatus;
                    this.PaymentStatus = entity.PaymentStatus;
                    this.IsEntry = entity.IsEntry;
                    this.DelivaryOpportunity = entity.DelivaryOpportunity;
                    this.CheckDelivery = entity.CheckDelivery;
                    this.CheckCarrier = entity.CheckCarrier;
                    this.CheckWaybillCode = entity.CheckWaybillCode;
                    this.CreateDate = entity.CreateDate;
                    this.ReceiveDate = entity.ReceiveDate;
                    this.PaymentDate = entity.PaymentDate;
                    this.UserID = entity.UserID;
                    this.CheckPayeeAccount = entity.CheckPayeeAccount;
                    this.CheckConsignee = entity.CheckConsignee;
                    this.CheckTitle = entity.CheckTitle;
                    this.HandlingFeePayerType = entity.HandlingFeePayerType;
                    this.HandlingFee = entity.HandlingFee;
                    this.USDRate = entity.USDRate;

                    var items = from item in new ApplicationItemsOrigin(reponsitory).Where(m =>
                          m.ApplicationID == id)
                                join order in
                                    reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrdersTopView>() on item
                                        .OrderID equals order.ID
                                select new
                                {
                                    item.ID,
                                    item.ApplicationID,
                                    item.OrderID,
                                    item.Amount,
                                    inCurrency = (Currency)order.inCurrency,
                                    outCurrency = (Currency)order.outCurrency,
                                    order.SupplierID
                                };
                    if (type == ApplicationType.Payment)
                    {
                        this.Items = items.Select(item => new ApplicationItem
                        {
                            ID = item.ID,
                            ApplicationID = item.ApplicationID,
                            OrderID = item.OrderID,
                            Amount = item.Amount,
                            SettlementCurrency = item.inCurrency,
                            SupplierID = item.SupplierID
                        });
                    }
                    else
                    {
                        this.Items = items.Select(item => new ApplicationItem
                        {
                            ID = item.ID,
                            ApplicationID = item.ApplicationID,
                            OrderID = item.OrderID,
                            Amount = item.Amount,
                            SettlementCurrency = item.outCurrency,
                            SupplierID = item.SupplierID
                        });
                    }

                    if (type == ApplicationType.Payment)
                    {
                        this.PayPayee = new ApplicationPayeesOrigin(reponsitory).FirstOrDefault(m => m.ApplicationID == id);
                        this.PayPayer = new ApplicationPayersOrigin(reponsitory).FirstOrDefault(m => m.ApplicationID == id);
                    }
                    else
                    {
                        this.ReceivePayer = new ApplicationPayersOrigin(reponsitory).FirstOrDefault(m => m.ApplicationID == id);
                    }
                    this.FileItems = new CenterFilesView().SearchByApplicationID(id);
                }
            }
        }

        #region 拓展属性

        /// <summary>
        /// 代付货款收款人(供应商收款信息)
        /// </summary>
        public Models.ApplicationPayee PayPayee { get; set; }

        /// <summary>
        /// 代付货款付款人(客户或者客户的客户-付款信息)
        /// </summary>
        public Models.ApplicationPayer PayPayer { get; set; }

        /// <summary>
        /// 代收货款付款人(客户的客户- 付款信息)
        /// </summary>
        public Models.ApplicationPayer ReceivePayer { get; set; }

        #endregion

        /// <summary>
        /// 申请数据提交
        /// </summary>
        public void Sumbit()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                //主表持久化
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Application);
                this.ApplicationEnter(reponsitory);
                //收付款信息持久化
                if (PayPayee != null)
                {
                    this.PayeeEnter(reponsitory);
                }
                if (PayPayer != null || ReceivePayer != null)
                {
                    this.PayerEnter(reponsitory);
                }
                //订单项持久化
                this.ItemsEnter(reponsitory);
                //文件持久化
                if (FileItems != null && FileItems.Any())
                {
                    Task.Run(() =>
                    {
                        var fileitem = this.FileItems.Select(item => new CenterFileDescription
                        {
                            CustomName = item.CustomName,
                            Url = item.Url,
                            AdminID = item.AdminID,
                            ApplicationID = this.ID,
                            Type = item.Type,
                        }).ToArray();

                        new CenterFilesView().Upload(fileitem);
                    });
                }
            }
        }

        /// <summary>
        /// 申请主表新增
        /// </summary>
        private void ApplicationEnter(PvWsOrderReponsitory reponsitory)
        {
            int? handlingFeePayerType = null;
            if (int.TryParse(this.HandlingFeePayerType, out var handlingFeePayerType_1))
            {
                handlingFeePayerType = handlingFeePayerType_1;
            }

            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Applications
            {
                ID = this.ID,
                ClientID = this.ClientID,
                Type = (int)this.Type,
                TotalPrice = this.TotalPrice,
                Currency = (int)this.Currency,
                Status = (int)this.Status,
                ApplicationStatus = (int)this.ApplicationStatus,
                ReceiveStatus = (int)this.ReceiveStatus,
                PaymentStatus = (int)this.PaymentStatus,
                IsEntry = this.IsEntry,
                DelivaryOpportunity = (int)this.DelivaryOpportunity.GetValueOrDefault(),
                InCompanyName = this.InCompanyName,
                InBankAccount = this.InBankAccount,
                InBankName = this.InBankName,
                OutBankAccount = this.OutBankAccount,
                OutBankName = this.OutBankName,
                OutCompanyName = this.OutCompanyName,
                CheckDelivery = (int)this.CheckDelivery.GetValueOrDefault(),
                CheckCarrier = this.CheckCarrier ?? string.Empty,
                CheckWaybillCode = this.CheckWaybillCode,
                CreateDate = this.CreateDate,
                PaymentDate = this.PaymentDate,
                ReceiveDate = this.ReceiveDate,
                UserID = this.UserID,
                CheckPayeeAccount = this.CheckPayeeAccount,
                CheckConsignee = this.CheckConsignee,
                CheckTitle = this.CheckTitle,
                HandlingFeePayerType = handlingFeePayerType,
                HandlingFee = this.HandlingFee,
                USDRate = this.USDRate,
            });
        }


        /// <summary>
        /// 付款人持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void PayerEnter(PvWsOrderReponsitory reponsitory)
        {
            var entity = PayPayer;
            if (this.Type == ApplicationType.Receival)
            {
                entity = ReceivePayer;
            }

            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ApplicationPayers
            {
                ID = Guid.NewGuid().ToString(),
                ApplicationID = this.ID,
                PayerID = entity.PayerID,
                EnterpriseID = entity.EnterpriseID,
                EnterpriseName = entity.EnterpriseName,
                BankName = entity.BankName ?? "",
                BankAccount = entity.BankAccount ?? "",
                Method = (int)entity.Method,
                Currency = (int)entity.Currency,
                Amount = entity.Amount
            });
        }

        /// <summary>
        /// 收款人持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void PayeeEnter(PvWsOrderReponsitory reponsitory)
        {
            //收款人
            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ApplicationPayees
            {
                ID = Guid.NewGuid().ToString(),
                ApplicationID = this.ID,
                PayeeID = PayPayee.PayeeID,
                EnterpriseID = PayPayee.EnterpriseID,
                EnterpriseName = PayPayee.EnterpriseName,
                BankName = PayPayee.BankName,
                BankAccount = PayPayee.BankAccount,
                Method = (int)PayPayee.Method,
                Currency = (int)PayPayee.Currency,
                Amount = PayPayee.Amount,
            });
        }

        /// <summary>
        /// 申请项持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void ItemsEnter(PvWsOrderReponsitory reponsitory)
        {
            var linq = this.Items.Select(item => new Layers.Data.Sqls.PvWsOrder.ApplicationItems
            {
                ID = Guid.NewGuid().ToString(),
                ApplicationID = this.ID,
                OrderID = item.OrderID,
                Amount = item.Amount,
                Status = (int)item.Status,
            });

            reponsitory.Insert(linq);
        }
    }
}

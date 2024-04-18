using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders
{
    public partial class AddTurnDeclare : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadComboboxData();
                this.LoadData();
            }
        }

        private void LoadComboboxData()
        {
            //币种数据
            this.Model.currencyData = ExtendsEnum.ToArray(Currency.Unknown)
                .Select(item => new { Value = (int)item, Text = item.GetCurrency().ShortName + " " + item.GetDescription() });
            //原产地数据
            this.Model.originData = ExtendsEnum.ToArray<Origin>()
                .Select(item => new { Value = (int)item, Text = item + " " + item.GetDescription() });
            //包装类型数据
            this.Model.packageData = ExtendsEnum.ToArray<Package>()
                .Select(item => new { Value = (int)item, Text = item.GetPackage().Code + " " + item });
            //支付方式
            this.Model.paymentType = ExtendsEnum.ToArray<Methord>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
            //文件类型
            this.Model.fileType = ExtendsEnum.ToArray<FileType>()
                .Select(item => new { Value = item, Text = item.GetDescription() }).Where(item => new int[] { 5, 24, 25 }.Contains((int)item.Value));
            //证件类型
            this.Model.idType = ExtendsEnum.ToArray<IDType>(IDType.PickSeal)
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        private void LoadData()
        {
            //客户ID
            string ClientID = Request.QueryString["ClientID"];
            //内部公司
            var company = Erp.Current.WsOrder.Companys;
            this.Model.companyData = company.Select(item => new { Value = item.ID, Text = item.Name });
            //客户收货人
            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.EnterpriseID == ClientID);
            this.Model.consigeeData = consigee.Select(item => new
            {
                Value = item.ID,
                Text = string.IsNullOrEmpty(item.Title) ? item.Enterprise.Name : item.Title,
            });

            //客户的供应商
            var supplier = Erp.Current.WsOrder.Suppliers.Where(item => item.OwnID == ClientID);
            this.Model.supplierData = supplier.Select(item => new { Value = item.ID, Text = item.Name });
            //客户供应商的收款人
            string[] supplierIDs = supplier.Select(item => item.ID).ToArray();
            var beneficiary = Erp.Current.WsOrder.SupplierPayees.Where(item => supplierIDs.Contains(item.nSupplierID));
            this.Model.beneficiaryData = beneficiary.Select(item => new
            {
                Value = item.ID,
                Text = item.Account,
                Name = item.RealEnterpriseName,
                BankName = item.Bank,
                Currency = item.CurrencyDes,
                Method = item.MethordDes,
            });
        }

        protected object data()
        {
            string[] Ids = Request.QueryString["Ids"].Split(',');
            var query = Erp.Current.WsOrder.Storages.Where(item => Ids.Contains(item.ID)).ToArray();
            var linq = query.Select(t => new
            {
                StorageID = t.ID,
                DateCode = t.DateCode,
                CustomName = "***",
                PartNumber = t.PartNumber,
                Manufacturer = t.Manufacturer,
                Origin = t.Origin,
                TotalQuantity = t.Quantity,//库存数量
                Quantity = t.Quantity,//申报数量
                Unit = LegalUnit.个,
                Currency = t.Currency,
                TotalPrice = t.TotalPriceDec,
                GrossWeight = 0.02M,
                Volume = 0.00M,
                CreateDate = t.EnterDate.ToString("yyyy-MM-dd"),

                OriginDec = t.OriginDec,
                UnitDec = LegalUnit.个.GetDescription(),
                CurrencyDec = t.CurrencyDec,
            });
            return linq;
        }

        /// <summary>
        /// 选择客户供应商
        /// </summary>
        protected void SelectSupplier()
        {
            string SupplierID = Request.Form["SupplierID"];
            string ClientID = Request.Form["ClientID"];
            Currency Currency = (Currency)(int.Parse(Request.Form["Currency"]));

            var beneficiary = Erp.Current.WsOrder.SupplierPayees
                .Where(item => item.nSupplierID == SupplierID && item.OwnID == ClientID)
                .Where(item => item.Currency == Currency);
            var data = beneficiary.Select(item => new
            {
                Value = item.ID,
                Text = item.Account,
                Name = item.RealEnterpriseName,
                BankName = item.Bank,
                Currency = item.CurrencyDes,
                Method = item.MethordDes,
            });
            Response.Write((new { success = true, data = data }).Json());
        }

        /// <summary>
        /// 选择客户联系人
        /// </summary>
        protected void SelectConsigee()
        {
            string consigeeID = Request.Form["ConsigeeID"];
            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.ID == consigeeID);
            var data = consigee.Select(item => new
            {
                Address = item.Address,
                Name = item.Name,
                Mobile = string.IsNullOrEmpty(item.Mobile) ? item.Tel : item.Mobile,
                Postzip = item.Postzip
            });
            Response.Write((new { success = true, data = data }).Json());
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                FileType type = (FileType)(int.Parse(Request.Form["fileType"]));

                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadFile");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            Services.Common.FileDirectory dic = new Services.Common.FileDirectory(file.FileName, type);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
                                FileType = dic.FileType,
                                FileTypeDec = dic.FileType.GetDescription(),
                                Url = dic.uploadResult.Url,
                            });
                        }
                    }
                }
                Response.Write((new { success = true, data = fileList }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        protected object LoadFile()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        protected void SubmitOrder()
        {
            try
            {
                #region 界面数据

                //基本信息
                string clientID = Request.Form["clientID"];
                string enterCode = Request.Form["enterCode"];
                string currency = Request.Form["currency"];
                string supplier = Request.Form["supplier"];
                string isPayForGoods = Request.Form["isPayForGoods"];
                string beneficiary = Request.Form["beneficiary"];

                string paySupplier = Request.Form["paySupplier"];
                string[] paymentSupplier = paySupplier.Split(',');
                if (!paymentSupplier.Contains(supplier))
                {
                    //将供应商添加到付汇供应商里面
                    paymentSupplier = paymentSupplier.Concat(new string[] { supplier }).ToArray();
                }
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                string totalPrice = Request.Form["totalPrice"];

                //国内交货信息
                string sz_DeliveryType = Request.Form["sz_DeliveryType"];
                string s_pickUpTime = Request.Form["s_pickUpTime"];
                string s_pickUpAddress = Request.Form["s_pickUpAddress"];
                string pickUpName = Request.Form["pickUpName"];
                string pickUpTel = Request.Form["pickUpTel"];
                string IDCardType = Request.Form["IDCardType"];
                string IDCardNumber = Request.Form["IDCardNumber"];

                string companyName = Request.Form["companyName"];
                string address = Request.Form["address"];
                string contacts = Request.Form["contacts"];
                string phone = Request.Form["phone"];
                //其它交货信息
                string package = Request.Form["package"];
                string TotalPackages = Request.Form["TotalPackages"];
                string TotalWeight = Request.Form["TotalWeight"];
                string TotalVolume = Request.Form["TotalVolume"];
                string summary = Request.Form["summary"];

                string isUnBox = Request.Form["isUnBox"];
                string isDetection = Request.Form["isDetection"];
                string isCustomLabel = Request.Form["isCustomLabel"];
                string isRepackaging = Request.Form["isRepackaging"];
                string isVacuumPackaging = Request.Form["isVacuumPackaging"];
                string isWaterproofPackaging = Request.Form["isWaterproofPackaging"];
                string isCharterBus = Request.Form["isCharterBus"];
                //文件信息
                var files = Request.Form["files"].Replace("&quot;", "'").Replace("amp;", "");
                var orderFiles = files.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                #endregion

                #region 客户的发票信息获取
                var client = Erp.Current.WsOrder.WsClients.Single(t => t.ID == clientID);
                var invoiceID = client.StorageType != WsIdentity.Personal ?
                    Erp.Current.WsOrder.Invoices.Where(item => item.EnterpriseID == clientID).FirstOrDefault()?.ID :
                    Erp.Current.WsOrder.vInvoices.Where(t => t.EnterpriseID == clientID && t.IsDefault).FirstOrDefault()?.ID;
                if (string.IsNullOrEmpty(invoiceID))
                {
                    throw new Exception("客户的开票信息为Null");
                }
                #endregion

                Order order = new Order();
                order.ClientID = clientID;
                order.Type = OrderType.TransferDeclare;
                order.InvoiceID = invoiceID;
                order.PayeeID = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A";
                order.Summary = summary;
                order.CreatorID = order.OperatorID = Erp.Current.ID;
                order.SupplierID = supplier;
                order.PaymentSuppliers = paymentSupplier;
                order.EnterCode = enterCode;

                #region 订单的收货扩展
                order.OrderInput = new OrderInput();
                order.OrderInput.BeneficiaryID = beneficiary;
                order.OrderInput.IsPayCharge = isPayForGoods == "true" ? true : false;
                order.OrderInput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                order.OrderInput.Conditions = new OrderCondition().Json();

                #region 添加运单数据
                if (order.OrderInput.Waybill == null)
                {
                    order.OrderInput.Waybill = new Waybill();
                    order.OrderInput.Waybill.IsClearance = false;
                    order.OrderInput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderInput.Waybill.Code = "";
                order.OrderInput.Waybill.Type = WaybillType.DeliveryToWarehouse;
                order.OrderInput.Waybill.Subcodes = "";
                order.OrderInput.Waybill.CarrierID = "";
                order.OrderInput.Waybill.FreightPayer = WaybillPayer.Consignee;
                order.OrderInput.Waybill.TotalParts = int.Parse(string.IsNullOrWhiteSpace(TotalPackages) ? "1" : TotalPackages);
                order.OrderInput.Waybill.TotalWeight = decimal.Parse(string.IsNullOrWhiteSpace(TotalWeight) ? "0" : TotalWeight);
                order.OrderInput.Waybill.TotalVolume = decimal.Parse(string.IsNullOrWhiteSpace(TotalVolume) ? "0" : TotalVolume);
                order.OrderInput.Waybill.CarrierAccount = "";
                order.OrderInput.Waybill.EnterCode = enterCode;
                order.OrderInput.Waybill.VoyageNumber = "";
                order.OrderInput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderInput.Waybill.TransferID = "";
                order.OrderInput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderInput.Waybill.Source = CgNoticeSource.AgentCustomsFromStorage;
                order.OrderInput.Waybill.NoticeType = CgNoticeType.Enter;

                #region 交货人、收货人

                var Company = Erp.Current.WsOrder.Companys
                    .Where(item => item.Name.Contains(Helper.GetWarehouseName(enterCode))).FirstOrDefault();
                var Contact = Company?.Contacts.FirstOrDefault();
                //收货人(香港万路通)
                Yahv.Services.Models.WayParter inConsignee = new Yahv.Services.Models.WayParter();
                inConsignee.Company = Company.Name;
                inConsignee.Address = Company.RegAddress;
                inConsignee.Contact = Contact?.Name;
                inConsignee.Phone = Contact?.Tel ?? Contact?.Mobile;
                inConsignee.Email = Contact?.Email;
                inConsignee.Place = Origin.HKG.GetOrigin().Code;
                order.OrderInput.Waybill.Consignee = inConsignee;
                //交货人(香港万路通)
                Yahv.Services.Models.WayParter outConsignor = new Yahv.Services.Models.WayParter();
                outConsignor.Company = Company.Name;
                outConsignor.Address = Company.RegAddress;
                outConsignor.Contact = Contact?.Name;
                outConsignor.Phone = Contact?.Mobile;
                outConsignor.Email = Contact?.Email;
                outConsignor.Place = Origin.HKG.GetOrigin().Code;
                order.OrderInput.Waybill.Consignor = outConsignor;
                #endregion

                #region 提货信息
                order.OrderInput.Waybill.ExcuteStatus = (int)CgSortingExcuteStatus.Completed;
                #endregion

                #endregion

                #endregion

                #region 订单的发货扩展
                order.OrderOutput = new OrderOutput();
                order.OrderOutput.BeneficiaryID = clientID;
                order.OrderOutput.IsReciveCharge = false;
                order.OrderOutput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);

                OrderCondition orderCondition = new OrderCondition();//出库条件
                orderCondition.IsDetection = isDetection == "true" ? true : false;
                orderCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                orderCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                orderCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                orderCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                orderCondition.IsCharterBus = isCharterBus == "true" ? true : false;
                order.OrderOutput.Conditions = orderCondition.Json();

                #region 添加运单数据
                order.OrderOutput.Waybill = new Waybill();
                order.OrderOutput.Waybill.IsClearance = false;
                order.OrderOutput.Waybill.CreatorID = Erp.Current.ID;
                order.OrderOutput.Waybill.Code = "";
                order.OrderOutput.Waybill.Type = (WaybillType)int.Parse(sz_DeliveryType);
                order.OrderOutput.Waybill.Subcodes = "";
                order.OrderOutput.Waybill.CarrierID = "";
                order.OrderOutput.Waybill.FreightPayer = WaybillPayer.Consignee;
                order.OrderOutput.Waybill.TotalParts = int.Parse(string.IsNullOrWhiteSpace(TotalPackages) ? "1" : TotalPackages);
                order.OrderOutput.Waybill.TotalWeight = decimal.Parse(string.IsNullOrWhiteSpace(TotalWeight) ? "0" : TotalWeight);
                order.OrderOutput.Waybill.TotalVolume = decimal.Parse(string.IsNullOrWhiteSpace(TotalVolume) ? "0" : TotalVolume);
                order.OrderOutput.Waybill.CarrierAccount = "";
                order.OrderOutput.Waybill.EnterCode = enterCode;
                order.OrderOutput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderOutput.Waybill.TransferID = "";
                order.OrderOutput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderOutput.Waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                order.OrderOutput.Waybill.Source = CgNoticeSource.AgentCustomsFromStorage;
                order.OrderOutput.Waybill.NoticeType = CgNoticeType.Boxing;

                #region 交货人（默认香港库房）、收货人
                Yahv.Services.Models.WayParter Consignor = new Yahv.Services.Models.WayParter();
                Consignor.Company = Company.Name;
                Consignor.Address = Company.RegAddress;
                Consignor.Contact = Contact?.Name;
                Consignor.Phone = Contact?.Tel ?? Contact?.Mobile;
                Consignor.Email = Contact?.Email;
                Consignor.Place = Origin.HKG.GetOrigin().Code;
                order.OrderOutput.Waybill.Consignor = Consignor;
                //收货人
                Yahv.Services.Models.WayParter Consignee = new Yahv.Services.Models.WayParter();
                if (order.OrderOutput.Waybill.Type == WaybillType.PickUp)
                {
                    Consignee.Company = pickUpName;
                    Consignee.Contact = pickUpName;
                    Consignee.Phone = pickUpTel;
                    Consignee.IDType = string.IsNullOrEmpty(IDCardType) ? IDType.IDCard : (IDType)int.Parse(IDCardType);
                    Consignee.IDNumber = IDCardNumber;
                    Consignee.Place = Origin.CHN.GetOrigin().Code;

                    var wayLoading = new Yahv.Services.Models.WayLoading();
                    wayLoading.TakingDate = Convert.ToDateTime(s_pickUpTime);
                    wayLoading.TakingAddress = s_pickUpAddress;
                    wayLoading.TakingContact = pickUpName;
                    wayLoading.TakingPhone = pickUpTel;
                    wayLoading.CreatorID = Erp.Current.ID;
                    wayLoading.ModifierID = Erp.Current.ID;
                    order.OrderOutput.Waybill.WayLoading = wayLoading;
                }
                else
                {
                    Consignee.Company = companyName;
                    Consignee.Address = address;
                    Consignee.Contact = contacts;
                    Consignee.Phone = phone;
                    Consignee.Zipcode = "";
                    Consignee.Place = Origin.CHN.GetOrigin().Code;
                }
                order.OrderOutput.Waybill.Consignee = Consignee;
                #endregion

                #region 运单条件
                Yahv.Services.Models.WayCondition condition = new Yahv.Services.Models.WayCondition();
                condition.PayForFreight = false;
                condition.UnBoxed = isUnBox == "true" ? true : false;
                condition.LableServices = isCustomLabel == "true" ? true : false;
                condition.Repackaging = isRepackaging == "true" ? true : false;
                condition.VacuumPackaging = isVacuumPackaging == "true" ? true : false;
                condition.WaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                condition.IsCharterBus = isCharterBus == "true" ? true : false;
                order.OrderOutput.Waybill.Condition = condition.Json();
                #endregion

                #endregion

                #endregion

                #region 订单文件数据
                List<Yahv.Services.Models.CenterFileDescription> fileList = new List<Yahv.Services.Models.CenterFileDescription>();
                foreach (var item in orderFiles)
                {
                    fileList.Add(item);
                }
                order.Fileitems = fileList;
                #endregion

                #region 订单项数据
                OrderItemCondition itemCondition = new OrderItemCondition();
                itemCondition.IsDetection = isDetection == "true" ? true : false;
                itemCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                itemCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                itemCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                itemCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                itemCondition.IsCharterBus = isCharterBus == "true" ? true : false;
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var item in productList)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.ID = item.ID;
                    orderItem.OrderID = item.OrderID;
                    orderItem.InputID = item.InputID;
                    orderItem.Product = new Yahv.Services.Models.CenterProduct { Manufacturer = item.Manufacturer, PartNumber = item.PartNumber };
                    orderItem.CustomName = item.CustomName;
                    string originStr = item.Origin;
                    orderItem.Origin = string.IsNullOrEmpty(originStr) ? Origin.Unknown : (Origin)Enum.Parse(typeof(Origin), originStr);
                    orderItem.DateCode = item.DateCode;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Currency = (Currency)int.Parse(currency);
                    orderItem.Unit = item.Unit == null ? LegalUnit.个 : item.Unit;
                    orderItem.TotalPrice = item.TotalPrice;
                    orderItem.UnitPrice = orderItem.TotalPrice / orderItem.Quantity;
                    string gwStr = item.GrossWeight;
                    orderItem.GrossWeight = string.IsNullOrEmpty(gwStr) ? 0.02M : decimal.Parse(gwStr);
                    string volStr = item.Volume;
                    orderItem.Volume = string.IsNullOrEmpty(volStr) ? 0.02M : decimal.Parse(volStr);
                    orderItem.Conditions = itemCondition.Json();
                    orderItem.StorageID = item.StorageID;

                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                //新增订单
                order.Enter();
                //芯达通订单
                JMessage Message = order.OrderToXDT().JsonTo<JMessage>();
                if (!Message.success)
                {
                    order.Abandon();
                    throw new Exception(Message.data + Message.code);
                }
                order.OperateLog("转报关订单保存成功");
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}
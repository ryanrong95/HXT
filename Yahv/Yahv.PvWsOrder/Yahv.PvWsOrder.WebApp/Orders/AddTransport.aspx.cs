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
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders
{
    public partial class AddTransport : ErpParticlePage
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
            //海关单位数据
            this.Model.unitData = ExtendsEnum.ToArray<LegalUnit>()
                .Select(item => new { UnitValue = item.GetUnit().Code, UnitText = item.GetUnit().Name });
            //支付方式
            this.Model.paymentType = ExtendsEnum.ToArray<Methord>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
            //文件类型
            this.Model.fileType = ExtendsEnum.ToArray<FileType>()
                .Select(item => new { Value = item, Text = item.GetDescription() }).Where(item => new int[] { 5, 10, 24, 25, 26, 27 }.Contains((int)item.Value));
            //证件类型
            this.Model.idType = ExtendsEnum.ToArray<Underly.IDType>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        private void LoadData()
        {
            //客户ID
            string ClientID = Request.QueryString["ID"];
            //内部公司
            var company = Erp.Current.WsOrder.Companys;
            this.Model.companyData = company.Select(item => new { Value = item.ID, Text = item.Name });

            //客户的供应商
            var supplier = Erp.Current.WsOrder.Suppliers.Where(item => item.OwnID == ClientID);
            this.Model.supplierData = supplier.Select(item => new { Value = item.ID, Text = item.Name });
            //客户供应商的收款人
            string[] supplierIDs = supplier.Select(item => item.ID).ToArray();
            var beneficiary = Erp.Current.WsOrder.SupplierPayees.Where(item => supplierIDs.Contains(item.nSupplierID));
            this.Model.beneficiaryData = beneficiary.Where(item => item.Methord != Methord.Cash).Select(item => new
            {
                Value = item.ID,
                Text = item.Account,
                Name = item.RealEnterpriseName,
                BankName = item.Bank,
                Currency = item.CurrencyDes,
                Method = item.MethordDes,
            });
            //公司承运商
            this.Model.carrierData = Erp.Current.WsOrder.Carriers.Where(item => item.Place == Origin.HKG.ToString()).Select(item => new { Value = item.ID, Text = item.Name });
            //客户收货人信息
            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.EnterpriseID == ClientID);
            this.Model.consigeeData = consigee.Select(item => new
            {
                Value = item.ID,
                Text = string.IsNullOrEmpty(item.Title) ? item.Enterprise.Name : item.Title,
            });
        }

        protected object data()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

        /// <summary>
        /// 选择客户供应商
        /// </summary>
        protected void SelectSupplier()
        {
            string SupplierID = Request.Form["SupplierID"];
            string ClientID = Request.Form["ClientID"];
            Currency Currency = (Currency)(int.Parse(Request.Form["Currency"]));
            //客户供应商的收款人
            var beneficiary = Erp.Current.WsOrder.SupplierPayees
                .Where(item => item.nSupplierID == SupplierID && item.OwnID == ClientID)
                .Where(item => item.Currency == Currency);
            //客户供应商的收货地址
            var consignees = Erp.Current.WsOrder.SupplierConsignors
                .Where(item => item.nSupplierID == SupplierID)
                .Where(item => item.Status == GeneralStatus.Normal)
                .Select(item => new
                {
                    Value = item.ID,
                    Text = item.Address,
                    Tel = item.Tel ?? item.Mobile,
                    Contact = item.Contact,
                });
            var data = beneficiary.Where(item => item.Methord != Methord.Cash).Select(item => new
            {
                Value = item.ID,
                Text = item.Account,
                Name = item.RealEnterpriseName,
                BankName = item.Bank,
                Currency = item.CurrencyDes,
                Method = item.MethordDes,
            });
            Response.Write((new { success = true, data = data, consignee = consignees }).Json());
        }

        /// <summary>
        /// 选择客户联系人
        /// </summary>
        protected void SelectConsigee()
        {
            string consigeeID = Request.Form["ConsigeeID"];
            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.ID == consigeeID).Distinct();
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
        /// 导入产品信息
        /// </summary>
        /// <returns></returns>
        protected void ImportProductInfo()
        {
            try
            {
                HttpPostedFile file = Request.Files["btnUpload"];
                string ext = System.IO.Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }

                var products = Services.Common.Helper.ProductImport(file);
                Response.Write((new { success = true, data = products }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        protected object LoadFile()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

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
                string SettlementCurrency = Request.Form["SettlementCurrency"];
                string totalPrice = Request.Form["totalPrice"];
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                //香港交货信息
                string sourceDistrict = Request.Form["sourceDistrict"];
                string isPayForGoods = Request.Form["isPayForGoods"];
                string Supplier = Request.Form["supplier"];
                string SupplierName = Request.Form["SupplierName"];
                string Beneficiary = Request.Form["beneficiary"];
                string hk_ReceiveType = Request.Form["hk_ReceiveType"];
                string pickUpTime = Request.Form["pickUpTime"];
                string pickUpAddress = Request.Form["pickUpAddress"];
                string pickUpName = Request.Form["pickUpName"];
                string pickUpTel = Request.Form["pickUpTel"];
                string WaybillCode = Request.Form["waybillCode"];
                string isPayForFreight = Request.Form["isPayForFreight"];
                string Carrier = Request.Form["carrier"];
                string VoyageNumber = Request.Form["voyageNumber"];
                //香港发货信息
                string hk_SendType = Request.Form["hk_SendType"];
                string s_pickUpTime = Request.Form["s_pickUpTime"];
                string s_pickUpName = Request.Form["s_pickUpName"];
                string s_pickUpTel = Request.Form["s_pickUpTel"];
                string idCardType = Request.Form["idCardType"];
                string idCardNumber = Request.Form["idCardNumber"];
                string companyName = Request.Form["companyName"];
                string address = Request.Form["address"];
                string contacts = Request.Form["contacts"];
                string phone = Request.Form["phone"];
                string zipCode = Request.Form["zipCode"];
                string s_isPayForFreight = Request.Form["s_isPayForFreight"];
                string isReciveCharge = Request.Form["isReciveCharge"];
                string targetDistrict = Request.Form["targetDistrict"];
                //其它交货信息
                string package = Request.Form["package"];

                int TotalPackages = int.Parse(string.IsNullOrWhiteSpace(Request.Form["TotalPackages"].ToString()) ? "1" : Request.Form["TotalPackages"].ToString());
                decimal TotalWeight = decimal.Parse(string.IsNullOrWhiteSpace(Request.Form["TotalWeight"]) ? "0.00" : Request.Form["TotalWeight"]);
                decimal TotalVolume = decimal.Parse(string.IsNullOrWhiteSpace(Request.Form["TotalVolume"]) ? "0.00" : Request.Form["TotalVolume"]);
                string Summary = Request.Form["Summary"];

                string isUnBox = Request.Form["isUnBox"];
                string isDetection = Request.Form["isDetection"];
                string isCustomLabel = Request.Form["isCustomLabel"];
                string isRepackaging = Request.Form["isRepackaging"];
                string isVacuumPackaging = Request.Form["isVacuumPackaging"];
                string isWaterproofPackaging = Request.Form["isWaterproofPackaging"];
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
                order.Type = OrderType.Transport;
                order.InvoiceID = invoiceID;
                order.PayeeID = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A";
                order.Summary = Summary;
                order.CreatorID = order.OperatorID = Erp.Current.ID;
                order.SupplierID = Supplier;
                order.EnterCode = enterCode;
                order.SettlementCurrency = (Currency)Enum.Parse(typeof(Currency), SettlementCurrency);

                #region 订单的收货扩展
                order.OrderInput = new OrderInput();
                order.OrderInput.BeneficiaryID = Beneficiary;
                order.OrderInput.IsPayCharge = isPayForGoods == "true" ? true : false;
                order.OrderInput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);

                //入库条件
                OrderCondition orderCondition = new OrderCondition();
                orderCondition.IsDetection = isDetection == "true" ? true : false;
                orderCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                orderCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                orderCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                orderCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                order.OrderInput.Conditions = orderCondition.Json();

                #region 添加运单数据
                if (order.OrderInput.Waybill == null)
                {
                    order.OrderInput.Waybill = new Waybill();
                    order.OrderInput.Waybill.IsClearance = false;
                    order.OrderInput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderInput.Waybill.Code = WaybillCode;
                order.OrderInput.Waybill.Type = (WaybillType)int.Parse(hk_ReceiveType);
                order.OrderInput.Waybill.Subcodes = "";
                order.OrderInput.Waybill.CarrierID = Carrier;
                order.OrderInput.Waybill.FreightPayer = isPayForFreight == "true" ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                order.OrderInput.Waybill.TotalParts = TotalPackages;
                order.OrderInput.Waybill.TotalWeight = TotalWeight;
                order.OrderInput.Waybill.TotalVolume = TotalVolume;
                order.OrderInput.Waybill.CarrierAccount = "";
                order.OrderInput.Waybill.EnterCode = enterCode;
                order.OrderInput.Waybill.VoyageNumber = VoyageNumber;
                order.OrderInput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderInput.Waybill.Supplier = SupplierName;
                order.OrderInput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderInput.Waybill.Source = CgNoticeSource.Transfer;
                order.OrderInput.Waybill.NoticeType = CgNoticeType.Enter;

                #region 交货人、收货人（默认供应商和香港库房）
                var supplier = new WsSupplierAlls().Where(item => item.ID == Supplier).FirstOrDefault();
                //交货人
                Yahv.Services.Models.WayParter Consignor = new Yahv.Services.Models.WayParter();
                Consignor.Company = supplier?.Name;
                Consignor.Address = supplier?.RegAddress;
                Consignor.Contact = supplier?.Contact?.Name;
                Consignor.Phone = supplier?.Contact?.Mobile;
                Consignor.Email = supplier?.Contact?.Email;
                Consignor.Place = ((Origin)int.Parse(sourceDistrict)).GetOrigin().Code;
                order.OrderInput.Waybill.Consignor = Consignor;
                //收货人(香港万路通)
                var Company = Erp.Current.WsOrder.Companys
                    .Where(item => item.Name.Contains(Helper.GetWarehouseName(enterCode))).FirstOrDefault();
                var Contact = Company?.Contacts.FirstOrDefault();
                Yahv.Services.Models.WayParter Consignee = new Yahv.Services.Models.WayParter();
                Consignee.Company = Company.Name;
                Consignee.Address = Company.RegAddress;
                Consignee.Contact = Contact?.Name;
                Consignee.Phone = Contact?.Tel ?? Contact?.Mobile;
                Consignee.Email = Contact?.Email;
                Consignee.Place = Origin.HKG.GetOrigin().Code;
                order.OrderInput.Waybill.Consignee = Consignee;
                #endregion

                #region 提货信息
                if ((WaybillType)int.Parse(hk_ReceiveType) == WaybillType.PickUp)
                {
                    var wayLoading = new Yahv.Services.Models.WayLoading();
                    wayLoading.TakingDate = Convert.ToDateTime(pickUpTime);
                    wayLoading.TakingAddress = pickUpAddress;
                    wayLoading.TakingContact = pickUpName;
                    wayLoading.TakingPhone = pickUpTel;
                    wayLoading.CreatorID = Erp.Current.ID;
                    wayLoading.ModifierID = Erp.Current.ID;

                    order.OrderInput.Waybill.WayLoading = wayLoading;
                    order.OrderInput.Waybill.ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting;
                }
                else
                {
                    order.OrderInput.Waybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                }
                #endregion

                #region 货物条款
                if (isPayForGoods == "true" ? true : false)
                {
                    var WayCharge = new Yahv.Services.Models.WayCharge();
                    WayCharge.Payer = WayChargeType.PayCharge;
                    WayCharge.Currency = (Currency)int.Parse(currency);
                    WayCharge.TotalPrice = decimal.Parse(totalPrice);

                    order.OrderInput.Waybill.WayCharge = WayCharge;
                }
                #endregion

                #region 运单条件
                Yahv.Services.Models.WayCondition condition = new Yahv.Services.Models.WayCondition();
                condition.PayForFreight = isPayForFreight == "true" ? true : false;
                condition.UnBoxed = isUnBox == "true" ? true : false;
                condition.IsDetection = isDetection == "true" ? true : false;
                order.OrderInput.Waybill.Condition = condition.Json();
                #endregion

                #endregion

                #endregion

                #region 订单的发货扩展
                order.OrderOutput = new OrderOutput();
                order.OrderOutput.BeneficiaryID = clientID;
                order.OrderOutput.IsReciveCharge = isReciveCharge == "true" ? true : false;
                order.OrderOutput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                OrderCondition outCondition = new OrderCondition();//出库条件
                outCondition.IsDetection = isDetection == "true" ? true : false;
                outCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                outCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                outCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                outCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;

                order.OrderOutput.Conditions = outCondition.Json();

                #region 添加运单数据
                if (order.OrderOutput.Waybill == null)
                {
                    order.OrderOutput.Waybill = new Waybill();
                    order.OrderOutput.Waybill.IsClearance = false;
                    order.OrderOutput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderOutput.Waybill.Code = "";
                order.OrderOutput.Waybill.Type = (WaybillType)int.Parse(hk_SendType);
                order.OrderOutput.Waybill.Subcodes = "";
                order.OrderOutput.Waybill.CarrierID = "";
                order.OrderOutput.Waybill.FreightPayer = isPayForFreight == "true" ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                order.OrderOutput.Waybill.TotalParts = TotalPackages;
                order.OrderOutput.Waybill.TotalWeight = TotalWeight;
                order.OrderOutput.Waybill.TotalVolume = TotalVolume;
                order.OrderOutput.Waybill.CarrierAccount = "";
                order.OrderOutput.Waybill.EnterCode = enterCode;
                order.OrderOutput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderOutput.Waybill.TransferID = "";
                order.OrderOutput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderOutput.Waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                order.OrderOutput.Waybill.Source = CgNoticeSource.Transfer;
                order.OrderOutput.Waybill.NoticeType = CgNoticeType.Out;

                #region 交货人（默认香港库房）、收货人
                Yahv.Services.Models.WayParter outConsignor = new Yahv.Services.Models.WayParter();
                Yahv.Services.Models.WayParter outConsignee = new Yahv.Services.Models.WayParter();

                outConsignor.Company = Company.Name;
                outConsignor.Address = Company.RegAddress;
                outConsignor.Contact = Contact?.Name;
                outConsignor.Phone = Contact?.Tel ?? Contact?.Mobile;
                outConsignor.Email = Contact?.Email;
                outConsignor.Place = Origin.HKG.GetOrigin().Code;
                order.OrderOutput.Waybill.Consignor = outConsignor;

                if (order.OrderOutput.Waybill.Type == WaybillType.PickUp)
                {
                    outConsignee.Company = s_pickUpName;
                    outConsignee.Address = Company.RegAddress;
                    outConsignee.Contact = s_pickUpName;
                    outConsignee.Phone = s_pickUpTel;
                    outConsignee.IDType = string.IsNullOrEmpty(idCardType) ? IDType.IDCard : (IDType)int.Parse(idCardType);
                    outConsignee.IDNumber = idCardNumber;
                    outConsignee.Place = ((Origin)int.Parse(targetDistrict)).GetOrigin().Code;
                }
                else
                {
                    outConsignee.Company = companyName;
                    outConsignee.Address = address;
                    outConsignee.Contact = contacts;
                    outConsignee.Phone = phone;
                    outConsignee.Zipcode = zipCode;
                    outConsignee.Place = ((Origin)int.Parse(targetDistrict)).GetOrigin().Code;
                }
                order.OrderOutput.Waybill.Consignee = outConsignee;
                #endregion

                #region 提货信息
                if (order.OrderOutput.Waybill.Type == WaybillType.PickUp)
                {
                    var wayload = order.OrderOutput.Waybill.WayLoading;
                    if (string.IsNullOrEmpty(wayload?.ID))
                    {
                        wayload = new Yahv.Services.Models.WayLoading();
                        wayload.CreatorID = Erp.Current.ID;
                    }
                    wayload.TakingDate = Convert.ToDateTime(s_pickUpTime);
                    wayload.TakingAddress = Company?.RegAddress;
                    wayload.TakingContact = pickUpName;
                    wayload.TakingPhone = pickUpTel;
                    wayload.ModifierID = Erp.Current.ID;

                    order.OrderOutput.Waybill.WayLoading = wayload;
                }
                else
                {
                    order.OrderOutput.Waybill.WayLoading = null;
                }
                #endregion

                #region 货物条款
                if (isReciveCharge == "true" ? true : false)
                {
                    var WayCharge = new Yahv.Services.Models.WayCharge();
                    WayCharge.Payer = WayChargeType.ReciveCharge;
                    WayCharge.Currency = (Currency)int.Parse(currency);
                    WayCharge.TotalPrice = decimal.Parse(totalPrice);

                    order.OrderOutput.Waybill.WayCharge = WayCharge;
                }
                #endregion

                #region 运单条件
                Yahv.Services.Models.WayCondition outWayCondition = new Yahv.Services.Models.WayCondition();
                outWayCondition.PayForFreight = s_isPayForFreight == "true" ? true : false;
                outWayCondition.UnBoxed = isUnBox == "true" ? true : false;
                outWayCondition.LableServices = isCustomLabel == "true" ? true : false;
                outWayCondition.Repackaging = isRepackaging == "true" ? true : false;
                outWayCondition.VacuumPackaging = isVacuumPackaging == "true" ? true : false;
                outWayCondition.WaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                order.OrderOutput.Waybill.Condition = outWayCondition.Json();
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
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var item in productList)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.ID = item.ID;
                    orderItem.OrderID = item.OrderID;
                    orderItem.InputID = item.InputID;
                    orderItem.Product = new Yahv.Services.Models.CenterProduct { Manufacturer = item.Manufacturer, PartNumber = item.PartNumber };
                    string originStr = item.Origin;
                    orderItem.Origin = string.IsNullOrEmpty(originStr) ? Origin.Unknown : (Origin)Enum.Parse(typeof(Origin), originStr);
                    orderItem.DateCode = item.DateCode;
                    orderItem.Quantity = item.Qty;
                    orderItem.Currency = (Currency)int.Parse(currency);
                    orderItem.Unit = item.Unit == null ? LegalUnit.个 : item.Unit;
                    orderItem.TotalPrice = item.TotalPrice;
                    orderItem.UnitPrice = orderItem.TotalPrice / orderItem.Quantity;
                    string gwStr = item.GrossWeight;
                    orderItem.GrossWeight = string.IsNullOrEmpty(gwStr) ? 0.02M : decimal.Parse(gwStr);
                    string volStr = item.Volume;
                    orderItem.Volume = string.IsNullOrEmpty(volStr) ? 0.02M : decimal.Parse(volStr);
                    orderItem.Conditions = itemCondition.Json();

                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                order.Enter();
                order.OperateLog("即收即发订单保存成功");
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}
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

namespace Yahv.PvWsOrder.WebApp.Orders.Declare
{
    public partial class Edit : ErpParticlePage
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
            //证件类型
            this.Model.idType = ExtendsEnum.ToArray<IDType>(IDType.PickSeal)
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        private void LoadData()
        {
            string OrderID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.Orders.Where(item => item.ID == OrderID).FirstOrDefault();
            this.Model.orderData = new
            {
                //基本信息
                ClientID = query.ClientID,
                Company = query.PayeeID,
                CompanyBeneficiary = query.BeneficiaryID,
                //香港交货信息
                SourceDistrict = ExtendsEnum.ToArray<Origin>().Where(item => item.GetOrigin().Code == query.OrderInput.Waybill.Consignor.Place).FirstOrDefault(),
                SupplierID = query.SupplierID,
                SupplierIDs = query.PaymentSuppliers,
                BeneficiaryID = query.OrderInput.BeneficiaryID,
                IsPayCharge = query.OrderInput.IsPayCharge,
                PaymentType = query.OrderInput.Waybill.WayCharge?.PayMethod,
                WaybillType = query.OrderInput.Waybill.Type,

                TakingDate = query.OrderInput.Waybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd hh:mm:ss"),
                TakingAddress = query.OrderInput.Waybill.WayLoading?.TakingAddress,
                TakingContact = query.OrderInput.Waybill.WayLoading?.TakingContact,
                TakingPhone = query.OrderInput.Waybill.WayLoading?.TakingPhone,

                Code = query.OrderInput.Waybill.Code,
                IsPayForFreight = query.OrderInput.Waybill.WayCondition.PayForFreight,
                CarrierID = query.OrderInput.Waybill.CarrierID,
                VoyageNumber = query.OrderInput.Waybill.VoyageNumber,
                //香港发货信息
                targetDistrict = ExtendsEnum.ToArray<Origin>().Where(item => item.GetOrigin().Code == query.OrderOutput.Waybill.Consignee.Place).FirstOrDefault(),
                IsReciveCharge = query.OrderOutput.IsReciveCharge,
                OutPaymentType = query.OrderOutput.Waybill.WayCharge?.PayMethod,
                OutWaybillType = query.OrderOutput.Waybill.Type,

                PickUpTime = query.OrderOutput.Waybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd hh:mm:ss"),
                PickUpAddress = query.OrderOutput.Waybill.WayLoading.TakingAddress,

                ceeCompany = query.OrderOutput.Waybill.Consignee.Company,
                ceeContact = query.OrderOutput.Waybill.Consignee.Contact,
                ceePhone = query.OrderOutput.Waybill.Consignee.Phone,
                ceeIDType = query.OrderOutput.Waybill.Consignee.IDType,
                ceeIDNumber = query.OrderOutput.Waybill.Consignee.IDNumber,
                ceeAddress = query.OrderOutput.Waybill.Consignee.Address,
                ceeZipcode = query.OrderOutput.Waybill.Consignee.Zipcode,
                ceeIsPayForFreight = query.OrderOutput.Waybill.WayCondition.PayForFreight,
                //其它信息
                Packaging = query.OrderInput.Waybill.Packaging,
                Summary = query.Summary,
                TotalParts = query.OrderInput.Waybill.TotalParts,
                TotalWeight = query.OrderInput.Waybill.TotalWeight,
                TotalVolume = query.OrderInput.Waybill.TotalVolume,

                IsUnBoxed = query.OrderInput.Waybill.WayCondition.UnBoxed,
                IsDetection = query.OrderInput.OrderCondition.IsDetection,
                IsRepackaging = query.OrderInput.OrderCondition.IsRepackaging,
                IsVacuumPackaging = query.OrderInput.OrderCondition.IsVacuumPackaging,
                IsWaterproofPackaging = query.OrderInput.OrderCondition.IsWaterproofPackaging,
                IsCustomLabel = query.OrderInput.OrderCondition.IsCustomLabel,
                IsCharterBus = query.OrderInput.OrderCondition.IsCharterBus,
            };

            var ordetItem = query.Orderitems.Where(t => t.Status == Services.Enums.OrderItemStatus.Returned).FirstOrDefault();
            this.Model.TinyOrderID = ordetItem != null ? ordetItem.TinyOrderID : OrderID + "-01";

            //内部公司
            var company = Erp.Current.WsOrder.Companys;
            this.Model.companyData = company.Select(item => new { Value = item.ID, Text = item.Name });

            //客户的供应商
            var supplier = Erp.Current.WsOrder.Suppliers.Where(item => item.OwnID == query.ClientID);
            this.Model.supplierData = supplier.Select(item => new { Value = item.ID, Text = item.Name });

            //客户供应商的收款人
            var beneficiary = Erp.Current.WsOrder.SupplierPayees
                .Where(item => item.nSupplierID == query.SupplierID && item.OwnID == query.ClientID)
                .Where(item => item.Currency == query.Orderitems.FirstOrDefault().Currency);
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
            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.EnterpriseID == query.ClientID);
            this.Model.consigeeData = consigee.Select(item => new
            {
                Value = item.ID,
                Text = string.IsNullOrEmpty(item.Title) ? item.Enterprise.Name : item.Title,
            });
        }

        protected object data()
        {
            string ID = Request.QueryString["ID"];
            var query = Erp.Current.WsOrder.OrderItems
                .Where(item => item.OrderID == ID && item.Type == Services.Enums.OrderItemType.Normal).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                OrderID = t.OrderID,
                TinyOrderID = t.TinyOrderID,
                InputID = t.InputID,
                CustomName = t.CustomName,
                DateCode = t.DateCode,
                PartNumber = t.Product?.PartNumber,
                Manufacturer = t.Product?.Manufacturer,
                Origin = t.Origin,
                Qty = t.Quantity,
                Unit = t.Unit,
                TotalPrice = t.TotalPrice,
                GrossWeight = t.GrossWeight,
                Volume = t.Volume,
                CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                Currency = t.Currency,
                IsAuto = t.IsAuto,
                WayBillID = t.WayBillID,
                StorageID = t.StorageID,
            });

            return linq.OrderBy(item => item.PartNumber);
        }

        /// <summary>
        /// 加载发票文件
        /// </summary>
        protected object LoadInvoice()
        {
            string ID = Request.QueryString["ID"];
            var query = new OrderFilesRoll(ID).Where(item => item.Type == (int)FileType.Invoice).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
            });

            return linq;
        }

        /// <summary>
        /// 加载提货文件
        /// </summary>
        protected object LoadDelivery()
        {
            string ID = Request.QueryString["ID"];
            var query = new OrderFilesRoll(ID).Where(item => item.Type == (int)FileType.Delivery).AsEnumerable();
            var linq = query.Select(t => new
            {
                ID = t.ID,
                CustomName = t.CustomName,
                Url = PvWsOrder.Services.Common.FileDirectory.ServiceRoot + t.Url,
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

        /// <summary>
        /// 导入合同发票(PI)
        /// </summary>
        protected void UploadInvoice()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadInvoice");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            Services.Common.FileDirectory dic = new Services.Common.FileDirectory(file.FileName, FileType.Invoice);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
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
        /// 导入提货文件
        /// </summary>
        protected void UploadDelivery()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadDelivery");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            Services.Common.FileDirectory dic = new Services.Common.FileDirectory(file.FileName, FileType.Delivery);
                            dic.AdminID = Erp.Current.ID;
                            dic.Save(file);
                            fileList.Add(new
                            {
                                ID = dic.uploadResult.FileID,
                                CustomName = dic.FileName,
                                FileName = dic.uploadResult.FileName,
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
                string enterCode = Request.Form["enterCode"];
                string currency = Request.Form["currency"];
                string sourceDistrict = Request.Form["sourceDistrict"];
                string Supplier = Request.Form["supplier"];
                string SupplierName = Request.Form["SupplierName"];
                string isPayForGoods = Request.Form["isPayForGoods"];
                string beneficiary = Request.Form["beneficiary"];
                string paySupplier = Request.Form["paySupplier"];
                string[] paymentSupplier = paySupplier.Split(',');
                if (!paymentSupplier.Contains(Supplier))
                {
                    //将供应商添加到付汇供应商里面
                    paymentSupplier = paymentSupplier.Concat(new string[] { Supplier }).ToArray();
                }
                //产品信息
                string totalPrice = Request.Form["totalPrice"];
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                //香港交货信息
                string hk_DeliveryType = Request.Form["hk_DeliveryType"];
                string pickUpTime = Request.Form["pickUpTime"];
                string pickUpAddress = Request.Form["pickUpAddress"];
                string pickUpName = Request.Form["pickUpName"];
                string pickUpTel = Request.Form["pickUpTel"];
                string WaybillCode = Request.Form["WaybillCode"];
                string isPayForFreight = Request.Form["isPayForFreight"];
                string Carrier = Request.Form["Carrier"];
                string VoyageNumber = string.Empty;
                //国内交货信息
                string sz_DeliveryType = Request.Form["sz_DeliveryType"];
                string s_pickUpTime = Request.Form["s_pickUpTime"];
                string s_pickUpAddress = Request.Form["s_pickUpAddress"];
                string s_pickUpName = Request.Form["s_pickUpName"];
                string s_pickUpTel = Request.Form["s_pickUpTel"];
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
                var invoices = Request.Form["invoices"].Replace("&quot;", "'").Replace("amp;", "");
                var deliverys = Request.Form["deliverys"].Replace("&quot;", "'").Replace("amp;", "");
                var invoiceList = invoices.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                var deliveryList = deliverys.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

                #endregion
                string orderID = Request.Form["orderID"];
                Order order = Erp.Current.WsOrder.Orders.Where(item => item.ID == orderID).FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("订单" + orderID + "不存在");
                }
                order.Summary = summary;
                order.OperatorID = Erp.Current.ID;
                order.MainStatus = CgOrderStatus.待审核;
                order.SupplierID = Supplier;
                order.PaymentSuppliers = paymentSupplier;

                #region 订单的收货扩展
                //order.OrderInput = new OrderInput();
                order.OrderInput.BeneficiaryID = beneficiary;
                order.OrderInput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                order.OrderInput.IsPayCharge = isPayForGoods == "true" ? true : false;

                //入库条件
                OrderCondition orderCondition = new OrderCondition();
                orderCondition.IsDetection = isDetection == "true" ? true : false;
                orderCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                orderCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                orderCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                orderCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                orderCondition.IsCharterBus = isCharterBus == "true" ? true : false;
                order.OrderInput.Conditions = orderCondition.Json();

                #region 添加运单数据
                if (order.OrderInput.Waybill == null)
                {
                    order.OrderInput.Waybill = new Waybill();
                    order.OrderInput.Waybill.IsClearance = false;
                    order.OrderInput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderInput.Waybill.Code = WaybillCode;
                order.OrderInput.Waybill.Type = (WaybillType)int.Parse(hk_DeliveryType);
                order.OrderInput.Waybill.Subcodes = "";
                order.OrderInput.Waybill.CarrierID = Carrier;
                order.OrderInput.Waybill.FreightPayer = isPayForFreight == "true" ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                order.OrderInput.Waybill.TotalParts = int.Parse(string.IsNullOrWhiteSpace(TotalPackages) ? "1" : TotalPackages);
                order.OrderInput.Waybill.TotalWeight = decimal.Parse(string.IsNullOrWhiteSpace(TotalWeight) ? "0" : TotalWeight);
                order.OrderInput.Waybill.TotalVolume = decimal.Parse(string.IsNullOrWhiteSpace(TotalVolume) ? "0" : TotalVolume);
                order.OrderInput.Waybill.CarrierAccount = "";
                order.OrderInput.Waybill.EnterCode = enterCode;
                order.OrderInput.Waybill.VoyageNumber = VoyageNumber;
                order.OrderInput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderInput.Waybill.Supplier = SupplierName;
                order.OrderInput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderInput.Waybill.Source = CgNoticeSource.AgentBreakCustoms;
                order.OrderInput.Waybill.NoticeType = CgNoticeType.Enter;

                #region 交货人、收货人
                var supplier = new WsSupplierAlls().Where(item => item.ID == Supplier).FirstOrDefault();
                if (supplier != null)
                {
                    //交货人(默认供应商)
                    Yahv.Services.Models.WayParter Consignor = new Yahv.Services.Models.WayParter();
                    Consignor.Company = supplier.Name;
                    Consignor.Address = supplier.RegAddress;
                    Consignor.Contact = supplier.Contact?.Name;
                    Consignor.Phone = supplier.Contact?.Mobile;
                    Consignor.Email = supplier.Contact?.Email;
                    Consignor.Place = ((Origin)int.Parse(sourceDistrict)).GetOrigin().Code;
                    order.OrderInput.Waybill.Consignor = Consignor;
                }
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
                if (int.Parse(hk_DeliveryType) == (int)WaybillType.PickUp)
                {
                    var wayLoading = new Yahv.Services.Models.WayLoading();
                    wayLoading.CreatorID = Erp.Current.ID;
                    wayLoading.TakingDate = Convert.ToDateTime(pickUpTime);
                    wayLoading.TakingAddress = pickUpAddress;
                    wayLoading.TakingContact = pickUpName;
                    wayLoading.TakingPhone = pickUpTel;
                    wayLoading.ModifierID = Erp.Current.ID;

                    order.OrderInput.Waybill.WayLoading = wayLoading;
                    order.OrderInput.Waybill.ExcuteStatus = (int)CgLoadingExcuteStauts.Waiting;
                }
                else
                {
                    order.OrderInput.Waybill.WayLoading = null;
                    order.OrderInput.Waybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                }
                #endregion

                #region 货物条款
                if (isPayForGoods == "true" ? true : false)
                {
                    var WayCharge = order.OrderInput.Waybill.WayCharge ?? new Yahv.Services.Models.WayCharge();
                    WayCharge.Payer = WayChargeType.PayCharge;
                    WayCharge.Currency = (Currency)int.Parse(currency);
                    WayCharge.PayMethod = Methord.TT;
                    WayCharge.TotalPrice = decimal.Parse(totalPrice);

                    order.OrderInput.Waybill.WayCharge = WayCharge;
                }
                else
                {
                    order.OrderInput.Waybill.WayCharge = null;
                }
                #endregion

                #region 运单条件
                var condition = new Yahv.Services.Models.WayCondition();
                condition.PayForFreight = isPayForFreight == "true" ? true : false;
                condition.UnBoxed = isUnBox == "true" ? true : false;
                condition.IsDetection = isDetection == "true" ? true : false;
                order.OrderInput.Waybill.Condition = condition.Json();
                #endregion

                #endregion

                #endregion

                #region 订单的发货扩展
                order.OrderOutput.BeneficiaryID = order.ClientID;
                order.OrderOutput.IsReciveCharge = false;
                order.OrderOutput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                order.OrderOutput.Conditions = orderCondition.Json();

                #region 添加运单数据
                if (order.OrderOutput.Waybill == null)
                {
                    order.OrderOutput.Waybill = new Waybill();
                    order.OrderOutput.Waybill.IsClearance = false;
                    order.OrderOutput.Waybill.CreatorID = Erp.Current.ID;
                }
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
                order.OrderOutput.Waybill.VoyageNumber = VoyageNumber;
                order.OrderOutput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderOutput.Waybill.TransferID = "";
                order.OrderOutput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderOutput.Waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                order.OrderOutput.Waybill.Source = CgNoticeSource.AgentBreakCustoms;
                order.OrderOutput.Waybill.NoticeType = CgNoticeType.Out;
                order.OrderOutput.Waybill.WayCharge = null;

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
                    outConsignee.Contact = s_pickUpName;
                    outConsignee.Phone = s_pickUpTel;
                    outConsignee.IDType = string.IsNullOrEmpty(IDCardType) ? IDType.IDCard : (IDType)int.Parse(IDCardType);
                    outConsignee.IDNumber = IDCardNumber;
                    outConsignee.Place = Origin.CHN.GetOrigin().Code;

                    //提货信息
                    var wayLoading = new Yahv.Services.Models.WayLoading();
                    wayLoading.TakingDate = Convert.ToDateTime(s_pickUpTime);
                    wayLoading.TakingAddress = s_pickUpAddress;
                    wayLoading.TakingContact = s_pickUpName;
                    wayLoading.TakingPhone = s_pickUpTel;
                    wayLoading.CreatorID = Erp.Current.ID;
                    wayLoading.ModifierID = Erp.Current.ID;
                    order.OrderOutput.Waybill.WayLoading = wayLoading;
                }
                else
                {
                    outConsignee.Company = companyName;
                    outConsignee.Address = address;
                    outConsignee.Contact = contacts;
                    outConsignee.Phone = phone;
                    outConsignee.Zipcode = "";
                    outConsignee.Place = Origin.CHN.GetOrigin().Code;
                    //提货信息
                    order.OrderOutput.Waybill.WayLoading = null;
                }
                order.OrderOutput.Waybill.Consignee = outConsignee;
                #endregion

                #region 运单条件
                Yahv.Services.Models.WayCondition wayCondition = new Yahv.Services.Models.WayCondition();
                wayCondition.PayForFreight = false;
                wayCondition.UnBoxed = isUnBox == "true" ? true : false;
                wayCondition.LableServices = isCustomLabel == "true" ? true : false;
                wayCondition.Repackaging = isRepackaging == "true" ? true : false;
                wayCondition.VacuumPackaging = isVacuumPackaging == "true" ? true : false;
                wayCondition.WaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                wayCondition.IsCharterBus = isCharterBus == "true" ? true : false;
                order.OrderOutput.Waybill.Condition = wayCondition.Json();
                #endregion

                #endregion

                #endregion

                #region 订单文件数据
                List<Yahv.Services.Models.CenterFileDescription> fileList = new List<Yahv.Services.Models.CenterFileDescription>();
                foreach (var item in invoiceList)
                {
                    fileList.Add(item);
                }
                foreach (var item in deliveryList)
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
                    orderItem.TinyOrderID = item.TinyOrderID;
                    orderItem.InputID = item.InputID;
                    orderItem.Product = new Yahv.Services.Models.CenterProduct { Manufacturer = item.Manufacturer, PartNumber = item.PartNumber };
                    string originStr = item.Origin;
                    orderItem.Origin = string.IsNullOrEmpty(originStr) ? Origin.Unknown : (Origin)Enum.Parse(typeof(Origin), originStr);
                    orderItem.CustomName = item.CustomName;
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
                    orderItem.StorageID = item.StorageID;
                    orderItem.ProductUniqueCode = item.ProductUniqueCode;
                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                order.Enter();
                //芯达通订单
                JMessage Message = order.OrderToXDT(true).JsonTo<JMessage>();
                if (!Message.success)
                {
                    //order.Abandon();
                    throw new Exception(Message.data + Message.code);
                }
                else
                {
                    //更新订单状态
                    order.MainStatus = CgOrderStatus.待审核;
                    order.StatusLogUpdate();
                }
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}
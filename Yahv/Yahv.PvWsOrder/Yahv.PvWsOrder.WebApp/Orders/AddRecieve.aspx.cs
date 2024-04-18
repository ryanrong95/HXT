using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders
{
    public partial class AddRecieve : ErpParticlePage
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
        }

        /// <summary>
        /// 加载订单信息
        /// </summary>
        private void LoadData()
        {
            //客户ID
            string ClientID = Request.QueryString["ID"];

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
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadDelivery");
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
                                FileType = dic.FileType,
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
        /// 加载发票文件
        /// </summary>
        protected object LoadInvoice()
        {
            return new
            {
                rows = new List<object>().ToArray(),
                total = 0
            };
        }

        /// <summary>
        /// 加载提货文件
        /// </summary>
        protected object LoadDelivery()
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
                string SettlementCurrency = Request.Form["SettlementCurrency"];
                string sourceDistrict = Request.Form["sourceDistrict"];
                string isPayForGoods = Request.Form["isPayForGoods"];
                string Supplier = Request.Form["Supplier"];
                string SupplierName = Request.Form["SupplierName"];
                string Beneficiary = Request.Form["Beneficiary"];
                string totalPrice = Request.Form["totalPrice"];
                //产品信息
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
                string VoyageNumber = Request.Form["VoyageNumber"];
                //其它交货信息
                string package = Request.Form["package"];
                string TotalPackages = Request.Form["TotalPackages"];
                string TotalWeight = Request.Form["TotalWeight"];
                string TotalVolume = Request.Form["TotalVolume"];
                string Summary = Request.Form["Summary"];

                string isUnBox = Request.Form["isUnBox"];
                string isDetection = Request.Form["isDetection"];
                //文件信息
                var invoices = Request.Form["invoices"].Replace("&quot;", "'").Replace("amp;", "");
                var deliverys = Request.Form["deliverys"].Replace("&quot;", "'").Replace("amp;", "");
                var invoiceList = invoices.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();
                var deliveryList = deliverys.JsonTo<List<Yahv.Services.Models.CenterFileDescription>>();

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
                order.Type = OrderType.Recieved;
                order.InvoiceID = invoiceID;
                order.PayeeID = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A"; //默认芯达通
                order.Summary = Summary;
                order.CreatorID = order.OperatorID = Erp.Current.ID;
                order.SupplierID = Supplier;
                order.EnterCode = enterCode;
                order.SettlementCurrency = (Currency)Enum.Parse(typeof(Currency), SettlementCurrency);

                #region 订单的收货扩展
                order.OrderInput = new OrderInput();
                order.OrderInput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                order.OrderInput.BeneficiaryID = Beneficiary;
                order.OrderInput.IsPayCharge = isPayForGoods == "true" ? true : false;
                //入库条件
                var orderCondition = new OrderCondition();
                orderCondition.IsDetection = isDetection == "true" ? true : false;
                order.OrderInput.Conditions = orderCondition.Json();

                #region 添加运单数据
                if (order.OrderInput.Waybill == null)
                {
                    order.OrderInput.Waybill = new Waybill();
                    order.OrderInput.Waybill.IsClearance = false;
                    order.OrderInput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderInput.Waybill.Code = WaybillCode;
                order.OrderInput.Waybill.Type = (WaybillType)Enum.Parse(typeof(WaybillType), hk_DeliveryType);
                order.OrderInput.Waybill.CarrierID = Carrier;
                order.OrderInput.Waybill.FreightPayer = isPayForFreight == "true" ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                order.OrderInput.Waybill.TotalParts = int.Parse(string.IsNullOrWhiteSpace(TotalPackages) ? "1" : TotalPackages);
                order.OrderInput.Waybill.TotalWeight = decimal.Parse(string.IsNullOrWhiteSpace(TotalWeight) ? "0" : TotalWeight);
                order.OrderInput.Waybill.TotalVolume = decimal.Parse(string.IsNullOrWhiteSpace(TotalVolume) ? "0" : TotalVolume);
                order.OrderInput.Waybill.EnterCode = enterCode;
                order.OrderInput.Waybill.VoyageNumber = VoyageNumber;
                order.OrderInput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderInput.Waybill.Supplier = SupplierName;
                order.OrderInput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderInput.Waybill.Source = CgNoticeSource.AgentEnter;
                order.OrderInput.Waybill.NoticeType = CgNoticeType.Enter;

                #region 交货人、收货人（默认供应商和香港库房）
                var supplier = new WsSupplierAlls().Where(item => item.ID == Supplier).FirstOrDefault();
                if (supplier != null)
                {
                    //交货人(默认供应商)
                    var Consignor = new Yahv.Services.Models.WayParter();
                    Consignor.Company = supplier.Name;
                    Consignor.Address = supplier.RegAddress;
                    Consignor.Contact = supplier.Contact?.Name;
                    Consignor.Phone = supplier.Contact?.Mobile;
                    Consignor.Email = supplier.Contact?.Email;
                    Consignor.Place = ((Origin)int.Parse(sourceDistrict)).GetOrigin().Code;
                    order.OrderInput.Waybill.Consignor = Consignor;
                }
                //收货人(内单：WLT，外单：CY)
                var Company = Erp.Current.WsOrder.Companys
                    .Where(item => item.Name.Contains(Helper.GetWarehouseName(enterCode))).FirstOrDefault();
                var Contact = Company?.Contacts.FirstOrDefault();
                var Consignee = new Yahv.Services.Models.WayParter();
                Consignee.Company = Company?.Name;
                Consignee.Address = Company?.RegAddress;
                Consignee.Contact = Contact?.Name;
                Consignee.Phone = Contact?.Tel ?? Contact?.Mobile;
                Consignee.Email = Contact?.Email;
                Consignee.Place = Origin.HKG.GetOrigin().Code;
                order.OrderInput.Waybill.Consignee = Consignee;
                #endregion

                #region 提货信息

                if ((WaybillType)int.Parse(hk_DeliveryType) == WaybillType.PickUp)
                {
                    var wayload = order.OrderInput.Waybill.WayLoading;
                    if (string.IsNullOrEmpty(wayload?.ID))
                    {
                        wayload = new Yahv.Services.Models.WayLoading();
                        wayload.CreatorID = Erp.Current.ID;
                    }
                    wayload.TakingDate = Convert.ToDateTime(pickUpTime);
                    wayload.TakingAddress = pickUpAddress;
                    wayload.TakingContact = pickUpName;
                    wayload.TakingPhone = pickUpTel;
                    wayload.ModifierID = Erp.Current.ID;

                    order.OrderInput.Waybill.WayLoading = wayload;
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
                    var WayCharge = new Yahv.Services.Models.WayCharge();
                    WayCharge.Payer = WayChargeType.PayCharge;
                    WayCharge.Currency = (Currency)int.Parse(currency);
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
                    orderItem.StorageID = item.StorageID;
                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                order.Enter();
                order.OperateLog("收货订单保存成功");
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                string message = "空间名：" + ex.Source + "；" + '\n' + "方法名：" + ex.TargetSite + '\n' + "故障点：" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf("\\") + 1, ex.StackTrace.Length - ex.StackTrace.LastIndexOf("\\") - 1) + '\n' + "错误提示：" + ex.Message;
                Response.Write((new { success = false, message = "保存失败：" + message }).Json());
            }
        }
    }
}
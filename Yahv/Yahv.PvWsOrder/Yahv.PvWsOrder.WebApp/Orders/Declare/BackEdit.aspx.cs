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
    public partial class BackEdit : ErpParticlePage
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
            this.Model.idType = ExtendsEnum.ToArray<IDType>()
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

                PickUpTime = query.OrderOutput.Waybill.WayLoading.TakingDate,
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
                .Where(item => item.OrderID == ID && item.Type == Services.Enums.OrderItemType.Modified).AsEnumerable();
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
                Status = t.Status,
            });

            return linq.OrderByDescending(t => t.Status);
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
            var data = beneficiary.Where(item => item.Methord != Methord.Cash).Select(item => new
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
                //产品信息
                string totalPrice = Request.Form["totalPrice"];
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();

                #endregion
                string orderID = Request.Form["orderID"];
                Order order = Erp.Current.WsOrder.Orders.Single(item => item.ID == orderID);
                order.TotalPrice = decimal.Parse(totalPrice);
                order.OperatorID = Erp.Current.ID;
                order.CreatorID = Erp.Current.ID;

                #region 订单项数据
                OrderItem oitem = Erp.Current.WsOrder.OrderItems
                    .FirstOrDefault(item => item.OrderID == orderID && item.Type == Services.Enums.OrderItemType.Modified);
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
                    orderItem.Currency = oitem.Currency;
                    orderItem.Unit = item.Unit == null ? LegalUnit.个 : item.Unit;
                    orderItem.TotalPrice = item.TotalPrice;
                    orderItem.UnitPrice = orderItem.TotalPrice / orderItem.Quantity;
                    string gwStr = item.GrossWeight;
                    orderItem.GrossWeight = string.IsNullOrEmpty(gwStr) ? 0.02M : decimal.Parse(gwStr);
                    string volStr = item.Volume;
                    orderItem.Volume = string.IsNullOrEmpty(volStr) ? 0.02M : decimal.Parse(volStr);
                    orderItem.Conditions = oitem.Conditions;
                    orderItem.StorageID = item.StorageID;
                    orderItem.ProductUniqueCode = item.ProductUniqueCode;
                    orderItem.Type = Services.Enums.OrderItemType.Modified;
                    orderItem.Status = Services.Enums.OrderItemStatus.Normal;
                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                order.BackToSubmit();
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
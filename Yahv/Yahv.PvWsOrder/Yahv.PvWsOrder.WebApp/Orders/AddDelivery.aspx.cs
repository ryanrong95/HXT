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
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvWsOrder.WebApp.Orders
{
    public partial class AddDelivery : ErpParticlePage
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
                .Select(item => new { Value = item, Text = item.GetDescription() }).Where(item => new int[] { 5, 24, 25, 26 }.Contains((int)item.Value));
            //证件类型
            this.Model.idType = ExtendsEnum.ToArray<IDType>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        private void LoadData()
        {
            string clientID = Request.QueryString["ClientID"];

            var consigee = Erp.Current.WsOrder.Consignees.Where(item => item.EnterpriseID == clientID);
            this.Model.consigeeData = consigee.Select(item => new
            {
                Value = item.ID,
                Text = string.IsNullOrEmpty(item.Title) ? item.Enterprise.Name : item.Title,
            });
        }

        protected object data()
        {
            string[] Ids = Request.QueryString["Ids"].Split(',');
            var query = Erp.Current.WsOrder.Storages.Where(item => Ids.Contains(item.ID)).ToArray();
            var linq = query.Select(t => new
            {
                StorageID = t.ID,
                InputID = t.InputID,
                DateCode = t.DateCode,
                PartNumber = t.PartNumber,
                Manufacturer = t.Manufacturer,
                Origin = t.Origin,
                TotalQuantity = t.Quantity,//库存数量
                Quantity = t.Quantity,//申报数量
                Unit = LegalUnit.个,
                Currency = t.Currency,
                TotalPrice = t.TotalPriceDec,
                GrossWeight = 0.00M,
                Volume = 0.00M,
                CreateDate = t.EnterDate.ToString("yyyy-MM-dd"),

                OriginDec = t.OriginDec,
                UnitDec = LegalUnit.个.GetDescription(),
                CurrencyDec = t.CurrencyDec,
            });
            return linq;
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
                IList<HttpPostedFile> files = HttpContext.Current.Request.Files.GetMultiple("uploadFile");
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
                string targetDistrict = Request.Form["targetDistrict"];
                string isReciveCharge = Request.Form["isReciveCharge"];
                string totalPrice = Request.Form["totalPrice"];
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                //香港发货信息
                string hk_DeliveryType = Request.Form["hk_DeliveryType"];
                string pickUpTime = Request.Form["pickUpTime"];
                string pickUpName = Request.Form["pickUpName"];
                string pickUpTel = Request.Form["pickUpTel"];
                string idCardType = Request.Form["idCardType"];
                string idCardNumber = Request.Form["idCardNumber"];
                string companyName = Request.Form["companyName"];
                string address = Request.Form["address"];
                string contacts = Request.Form["contacts"];
                string phone = Request.Form["phone"];
                string zipCode = Request.Form["zipCode"];
                string isPayForFreight = Request.Form["isPayForFreight"];
                //其它交货信息
                string package = Request.Form["package"];
                string summary = Request.Form["summary"];
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
                order.Type = OrderType.Delivery;
                order.InvoiceID = invoiceID;
                order.PayeeID = Erp.Current.Leagues.Current?.EnterpriseID ?? "DBAEAB43B47EB4299DD1D62F764E6B6A";
                order.Summary = summary;
                order.CreatorID = order.OperatorID = Erp.Current.ID;
                order.EnterCode = enterCode;
                order.SettlementCurrency = (Currency)Enum.Parse(typeof(Currency), SettlementCurrency);

                #region 订单的发货扩展
                order.OrderOutput = new OrderOutput();
                order.OrderOutput.BeneficiaryID = clientID;
                order.OrderOutput.IsReciveCharge = isReciveCharge == "true" ? true : false;
                order.OrderOutput.Currency = (Currency)Enum.Parse(typeof(Currency), currency);
                OrderCondition orderCondition = new OrderCondition();//出库条件
                orderCondition.IsDetection = isDetection == "true" ? true : false;
                orderCondition.IsCustomLabel = isCustomLabel == "true" ? true : false;
                orderCondition.IsRepackaging = isRepackaging == "true" ? true : false;
                orderCondition.IsVacuumPackaging = isVacuumPackaging == "true" ? true : false;
                orderCondition.IsWaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
                order.OrderOutput.Conditions = orderCondition.Json();

                #region 添加运单数据
                if (order.OrderOutput.Waybill == null)
                {
                    order.OrderOutput.Waybill = new Waybill();
                    order.OrderOutput.Waybill.IsClearance = false;
                    order.OrderOutput.Waybill.CreatorID = Erp.Current.ID;
                }
                order.OrderOutput.Waybill.Code = "";
                order.OrderOutput.Waybill.Type = (WaybillType)int.Parse(hk_DeliveryType);
                order.OrderOutput.Waybill.Subcodes = "";
                order.OrderOutput.Waybill.CarrierID = "";
                order.OrderOutput.Waybill.FreightPayer = isPayForFreight == "true" ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                order.OrderOutput.Waybill.CarrierAccount = "";
                order.OrderOutput.Waybill.EnterCode = enterCode;
                order.OrderOutput.Waybill.ModifierID = Erp.Current.ID;
                order.OrderOutput.Waybill.TransferID = "";
                order.OrderOutput.Waybill.Packaging = string.IsNullOrWhiteSpace(package) ? "22" : package;
                order.OrderOutput.Waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                order.OrderOutput.Waybill.Source = CgNoticeSource.AgentSend;
                order.OrderOutput.Waybill.NoticeType = CgNoticeType.Out;

                #region 交货人（默认香港库房）、收货人
                var Company = Erp.Current.WsOrder.Companys.Where(item => item.Name.Contains(Helper.GetWarehouseName(enterCode))).FirstOrDefault();
                var Contact = Company?.Contacts.FirstOrDefault();
                Yahv.Services.Models.WayParter Consignor = new Yahv.Services.Models.WayParter();
                Consignor.Company = Company?.Name;
                Consignor.Address = Company?.RegAddress;
                Consignor.Contact = Contact?.Name;
                Consignor.Phone = Contact?.Tel ?? Contact?.Mobile;
                Consignor.Email = Contact?.Email;
                Consignor.Place = Origin.HKG.GetOrigin().Code;
                order.OrderOutput.Waybill.Consignor = Consignor;
                //收货人
                Yahv.Services.Models.WayParter Consignee = new Yahv.Services.Models.WayParter();
                if (order.OrderOutput.Waybill.Type == WaybillType.PickUp)
                {
                    //客户自提
                    Consignee.Company = pickUpName;
                    Consignee.Address = Company?.RegAddress;
                    Consignee.Contact = pickUpName;
                    Consignee.Phone = pickUpTel;
                    Consignee.IDType = string.IsNullOrEmpty(idCardType) ? IDType.IDCard : (IDType)int.Parse(idCardType);
                    Consignee.IDNumber = idCardNumber;
                    Consignee.Place = ((Origin)int.Parse(targetDistrict)).GetOrigin().Code;
                }
                else
                {
                    Consignee.Company = companyName;
                    Consignee.Address = address;
                    Consignee.Contact = contacts;
                    Consignee.Phone = phone;
                    Consignee.Zipcode = zipCode;
                    Consignee.Place = ((Origin)int.Parse(targetDistrict)).GetOrigin().Code;
                }
                order.OrderOutput.Waybill.Consignee = Consignee;
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
                    wayload.TakingDate = Convert.ToDateTime(pickUpTime);
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
                Yahv.Services.Models.WayCondition condition = new Yahv.Services.Models.WayCondition();
                condition.PayForFreight = isPayForFreight == "true" ? true : false;
                condition.UnBoxed = isUnBox == "true" ? true : false;
                condition.LableServices = isCustomLabel == "true" ? true : false;
                condition.Repackaging = isRepackaging == "true" ? true : false;
                condition.VacuumPackaging = isVacuumPackaging == "true" ? true : false;
                condition.WaterproofPackaging = isWaterproofPackaging == "true" ? true : false;
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
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var item in productList)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.ID = item.ID;
                    orderItem.OrderID = item.OrderID;
                    orderItem.Product = new Yahv.Services.Models.CenterProduct
                    {
                        ID = item.ProductID,
                        Manufacturer = item.Manufacturer,
                        PartNumber = item.PartNumber
                    };
                    string originStr = item.Origin;
                    orderItem.Origin = string.IsNullOrEmpty(originStr) ? Origin.Unknown : (Origin)Enum.Parse(typeof(Origin), originStr);
                    orderItem.DateCode = item.DateCode;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Unit = item.Unit == null ? LegalUnit.个 : item.Unit;
                    string gwStr = item.GrossWeight;
                    orderItem.GrossWeight = string.IsNullOrEmpty(gwStr) ? 0.02M : decimal.Parse(gwStr);
                    string volStr = item.Volume;
                    orderItem.Volume = string.IsNullOrEmpty(volStr) ? 0.02M : decimal.Parse(volStr);

                    orderItem.Conditions = itemCondition.Json();
                    orderItem.Currency = (Currency)int.Parse(currency);
                    orderItem.TotalPrice = item.TotalPrice;
                    orderItem.UnitPrice = orderItem.TotalPrice / orderItem.Quantity;
                    orderItem.StorageID = item.StorageID; //库存编号
                    orderItem.InputID = item.InputID;//库存进项编号

                    orderItems.Add(orderItem);
                }
                order.Orderitems = orderItems;
                #endregion

                order.Enter();
                //生成出库通知并且锁定库存
                JMessage Message = order.CgOutNotice().JsonTo<JMessage>();
                if (!Message.success)
                {
                    order.Abandon();
                    throw new Exception(Message.data + Message.code);
                }
                else
                {
                    order.MainStatus = CgOrderStatus.待收货;
                    order.StatusLogUpdate();
                }
                order.OperateLog("发货订单保存成功");
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}
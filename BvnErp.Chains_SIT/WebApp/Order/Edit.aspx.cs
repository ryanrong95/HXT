using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApp.Order
{
    /// <summary>
    /// 新增订单-编辑界面
    /// 用于新增订单或编辑订单
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { CurrValue = item.Code, CurrText = item.Code + " " + item.Name }).Json();
            this.Model.OriginData = Needs.Wl.Admin.Plat.AdminPlat.Countries.OrderBy(item => item.Code).Select(item => new { OriginValue = item.Code, OriginText = item.Code + " " + item.Name }).Json();
            this.Model.UnitData = Needs.Wl.Admin.Plat.AdminPlat.Units.OrderBy(item => item.Code).Select(item => new { UnitValue = item.Code, UnitText = item.Code + " " + item.Name }).Json();
            this.Model.IdType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.IDType>().Select(item => new { TypeValue = item.Key, TypeText = item.Value }).Json();
            this.Model.WarpType = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.OrderBy(item => item.Code).Select(item => new { TypeValue = item.Code, TypeText = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 初始化订单基本信息
        /// </summary>
        protected void LoadData()
        {
            string orderID = Request.QueryString["OrderID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
            this.Model.IDs = new
            {
                OrderID = Request.QueryString["OrderID"],
                ClientID = order?.Client.ID ?? Request.QueryString["ClientID"]
            }.Json();

            //订单编辑
            if (order != null)
            {
                var orderConsignee = order.OrderConsignee;
                var orderConsignor = order.OrderConsignor;
                var deliveryFile = order.MainOrderFiles.Where(f => f.FileType == FileType.DeliveryFiles).FirstOrDefault();
                if (deliveryFile != null)
                {
                    deliveryFile.Url = deliveryFile.Url.Replace(@"\", @"/");
                }

                var orderPayExchangeSupplier = order.PayExchangeSuppliers;

                this.Model.IsReturned = order.OrderStatus == OrderStatus.Returned;
                this.Model.IsPrePaid = order.Client.Agreement.ProductFeeClause.PeriodType == PeriodType.PrePaid;
                this.Model.ClientCode = new { order.Client.ClientCode }.Json();
                this.Model.Suppliers = order.Client.Suppliers.Select(item => new { item.ID, Name = item.ChineseName }).Json();
                this.Model.Consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientConsignees.Where(item => item.ClientID == order.Client.ID &&
                                                                                        item.Status == Status.Normal)
                                                                                        .Select(item => new
                                                                                        {
                                                                                            item.ID,
                                                                                            item.Name,
                                                                                            Contact = item.Contact.Name,
                                                                                            item.Contact.Mobile,
                                                                                            item.Address,
                                                                                            item.IsDefault
                                                                                        }).Json();
                this.Model.AllData = new
                {
                    //订单基本信息
                    Currency = order.Currency,
                    IsFullVehicle = order.IsFullVehicle,
                    IsLoan = order.IsLoan,
                    PackNo = order.PackNo,
                    WarpType = order.WarpType,
                    Summary = order.Summary,

                    //香港交货信息
                    Supplier = orderConsignee.ClientSupplier.ID,
                    HKDeliveryType = orderConsignee.Type,
                    SupplierContact = orderConsignee.Contact,
                    SupplierContactMobile = orderConsignee.Mobile,
                    SupplierAddress = orderConsignee.Address,
                    PickupTime = orderConsignee.PickUpTime,
                    WayBillNo = orderConsignee.WayBillNo,
                    DeliveryFile = deliveryFile,
                    DeliveryFileUrl = FileDirectory.Current.FileServerUrl + "/" + deliveryFile?.Url.ToUrl(),

                    //国内交货信息
                    SZDeliveryType = orderConsignor.Type,
                    ClientConsignee = orderConsignor.Name,
                    ClientContact = orderConsignor.Contact,
                    ClientContactMobile = orderConsignor.Mobile,
                    ClientConsigneeAddress = orderConsignor.Address,
                    ClientPicker = orderConsignor.Contact,
                    ClientPickerMobile = orderConsignor.Mobile,
                    IDType = orderConsignor.IDType,
                    IDNumber = orderConsignor.IDNumber,

                    //付汇供应商
                    PayExchangeSupplier = order.PayExchangeSuppliers.Select(item => item.ClientSupplier.ID)
                }.Json();
            }
            //订单新增
            else
            {
                var clientID = Request.QueryString["ClientID"];
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients[clientID];

                this.Model.IsReturned = false;
                this.Model.IsPrePaid = client.Agreement.ProductFeeClause.PeriodType == PeriodType.PrePaid;
                this.Model.ClientCode = new { client.ClientCode }.Json();
                this.Model.Suppliers = client.Suppliers.Select(item => new { item.ID, Name = item.ChineseName }).Json();
                this.Model.Consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientConsignees.Where(item => item.ClientID == clientID &&
                                                                                        item.Status == Status.Normal)
                                                                                        .Select(item => new
                                                                                        {
                                                                                            item.ID,
                                                                                            item.Name,
                                                                                            Contact = item.Contact.Name,
                                                                                            item.Contact.Mobile,
                                                                                            item.Address,
                                                                                            item.IsDefault
                                                                                        }).Json();
                this.Model.AllData = null;
            }
        }

        /// <summary>
        /// 初始化产品数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
                Func<OrderItem, object> convert = orderItem => new
                {
                    orderItem.ID,
                    Batch = orderItem.Batch,
                    Name = orderItem.Name,
                    Manufacturer = orderItem.Manufacturer,
                    Model = orderItem.Model,
                    Origin = orderItem.Origin,
                    Qty = orderItem.Quantity,
                    Unit = orderItem.Unit,
                    UnitPrice = orderItem.UnitPrice,
                    TotalPrice = orderItem.TotalPrice,
                    GrossWeight = orderItem.GrossWeight
                };

                Response.Write(new
                {
                    rows = order.Items.Select(convert).ToList(),
                    total = order.Items.Count()
                }.Json());
            }
            else
            {
                Response.Write(new { rows = new List<OrderItem>(), total = 0 }.Json());
            }
        }

        /// <summary>
        /// 初始化原始PI数据
        /// </summary>
        protected void dataPI()
        {
            string orderID = Request.QueryString["OrderID"];
            if (!string.IsNullOrEmpty(orderID))
            {
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
                Func<MainOrderFile, object> convert = orderFile => new
                {
                    ID= orderFile.ID,
                    orderFile.Name,
                    FileName = orderFile.Name,
                    FileType = orderFile.FileType.GetDescription(),
                    FileFormat = orderFile.FileFormat,
                    VirtualPath = orderFile.Url,
                    // 判断从哪读取文件
                    Url = (DateTime.Compare(order.CreateDate, Convert.ToDateTime(FileDirectory.Current.IsChainsDate)) > 0)
                    ? FileDirectory.Current.PvDataFileUrl + "/" + orderFile.Url.ToUrl() : FileDirectory.Current.FileServerUrl + "/" + orderFile.Url.ToUrl()
                };

                var pis = order.MainOrderFiles.Where(item => item.FileType == FileType.OriginalInvoice);
                Response.Write(new
                {
                    rows = pis.Select(convert).ToList()
                }.Json());
            }
            else
            {
                Response.Write(new { rows = new List<MainOrderFile>(), total = 0 }.Json());
            }
        }

        /// <summary>
        /// 保存订单
        /// </summary>
        protected void SaveOrder()
        {
            try
            {
                #region 前台数据
                //1)录入的产品信息
                var replaceQuoteStr = Request.Form["ReplaceQuoteStr"];
                var products = Request.Form["Products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                //2)香港交货方式
                var hkDeliveryType = Request.Form["HKDeliveryType"];
                var supplierID = Request.Form["Supplier"];
                var waybillNO = Request.Form["WayBillNo"];
                var deliveryFile = Request.Files["DeliveryFile"];
                var deliveryFilePlusFormValue = "[" + Request.Form["DeliveryFilePlus"].Replace("&quot;", "'") + "]";
                var deliveryFilePlus = deliveryFilePlusFormValue.JsonTo<List<dynamic>>();
                var pickupTime = Request.Form["PickupTime"];
                var supplierAddress = Request.Form["SupplierAddressText"].Replace("&quot;", "'").Replace("amp;", ""); ;
                var supplierContact = Request.Form["SupplierContact"];
                var supplierContactMobile = Request.Form["SupplierContactMobile"];
                //3)国内交货方式
                var szDeliveryType = Request.Form["SZDeliveryType"];
                var clientPicker = Request.Form["ClientPicker"];
                var clientPickerMobile = Request.Form["ClientPickerMobile"];
                var idType = Request.Form["IDType"];
                var idNumber = Request.Form["IDNumber"];
                var clientConsignee = Request.Form["ClientConsigneeText"];
                var clientConsigneeAddress = Request.Form["ClientConsigneeAddress"].Replace("&quot;", "'").Replace("amp;", ""); ;
                var clientContact = Request.Form["ClientContact"];
                var clientContactMobile = Request.Form["ClientContactMobile"];
                //4)付汇供应商
                var payExchangeSupplierIDs = Request.Form["PayExchangeSupplier"].Split(',');
                var invoices = Request.Form["Invoices"].Replace("&quot;", "'");
                var invoiceList = invoices.JsonTo<List<dynamic>>();
                //5)其他信息
                var currency = Request.Form["Currency"];
                var isFullVehicle = bool.Parse(Request.Form["IsFullVehicle"]);
                var isLoan = string.IsNullOrWhiteSpace(Request.Form["IsLoan"]) ? false : bool.Parse(Request.Form["IsLoan"]);
                var wrapType = Request.Form["WarpType"];
                var packNO = string.IsNullOrWhiteSpace(Request.Form["PackNo"]) ? null : (int?)int.Parse(Request.Form["PackNo"]);
                var summary = Request.Form["Summary"];
                var isConfirm = bool.Parse(Request.Form["IsConfirm"]);
                #endregion

                //跟单员
                var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                var admin = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Admins[adminID];

                //客户
                var clientID = Request.Form["ClientID"];
                var client = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients[clientID];
                //客户供应商
                var supplier = client.Suppliers[supplierID];

                //订单
                string orderID = Request.Form["OrderID"];
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[orderID];
                if (order == null)
                {
                    order = new Needs.Ccs.Services.Models.Order();
                    order.Client = client;
                    order.OrderConsignee = new OrderConsignee();
                    order.OrderConsignee.OrderID = order.ID;
                    order.OrderConsignor = new OrderConsignor();
                    order.OrderConsignor.OrderID = order.ID;
                }
                else
                {
                    order.Items.RemoveAll();
                    order.PayExchangeSuppliers.RemoveAll();
                    order.Files.RemoveAll();
                    order.MainOrderFiles.RemoveAll();
                }
                order.AdminID = adminID;
                order.ClientAgreement = client.Agreement;

                //香港交货信息
                order.OrderConsignee.ClientSupplier = supplier;
                order.OrderConsignee.Type = (HKDeliveryType)Enum.Parse(typeof(HKDeliveryType), hkDeliveryType);
                if (order.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
                {
                    order.OrderConsignee.Contact = null;
                    order.OrderConsignee.Mobile = null;
                    order.OrderConsignee.Address = null;
                    order.OrderConsignee.PickUpTime = null;
                    order.OrderConsignee.WayBillNo = waybillNO;
                }
                else
                {
                    order.OrderConsignee.Contact = supplierContact;
                    order.OrderConsignee.Mobile = supplierContactMobile;
                    order.OrderConsignee.Address = supplierAddress;
                    order.OrderConsignee.PickUpTime = DateTime.Parse(pickupTime);
                    order.OrderConsignee.WayBillNo = null;

                    //提货单
                    if (order.OrderConsignee.Type == HKDeliveryType.PickUp)
                    {
                        string docType = "msword";
                        string docxType = "vnd.openxmlformats-officedocument.wordprocessingml.document";
                        var thisDeliveryFile = new MainOrderFile
                        {
                            MainOrderID = order.MainOrderID,
                            Admin = admin,
                            Name = deliveryFilePlus[0].Name,
                            FileType = FileType.DeliveryFiles,
                            FileFormat = Convert.ToString(deliveryFilePlus[0].FileFormat).Replace(docType, "doc").Replace(docxType, "docx"),
                            Url = Convert.ToString(deliveryFilePlus[0].VirtualPath).Replace(@"/", @"\"),
                            Type = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Delivery,
                            ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID
                        };
                        order.MainOrderFiles.Add(thisDeliveryFile);

                        //#region 订单文件保存到中心文件库

                        //var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                        //var dic = new { CustomName = deliveryFilePlus[0].Name, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };
                        //var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Delivery;
                        ////本地文件上传到服务器
                        //var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + Url, centerType, dic);
                        //#endregion
                    }
                }

                //国内交货信息
                order.OrderConsignor.Type = (SZDeliveryType)Enum.Parse(typeof(SZDeliveryType), szDeliveryType);
                if (order.OrderConsignor.Type == SZDeliveryType.PickUpInStore)
                {
                    order.OrderConsignor.Contact = clientPicker;
                    order.OrderConsignor.Mobile = clientPickerMobile;
                    order.OrderConsignor.IDType = idType;
                    order.OrderConsignor.IDNumber = idNumber;
                    order.OrderConsignor.Address = null;
                }
                else
                {
                    order.OrderConsignor.Name = clientConsignee;
                    order.OrderConsignor.Contact = clientContact;
                    order.OrderConsignor.Mobile = clientContactMobile;
                    order.OrderConsignor.Address = clientConsigneeAddress;
                    order.OrderConsignor.IDType = null;
                    order.OrderConsignor.IDNumber = null;
                }

                //付汇供应商
                foreach (var payExchangeSupplierID in payExchangeSupplierIDs)
                {
                    if (string.IsNullOrWhiteSpace(payExchangeSupplierID))
                        continue;

                    var payExchangeSupplier = client.Suppliers[payExchangeSupplierID];
                    order.PayExchangeSuppliers.Add(new OrderPayExchangeSupplier
                    {
                        OrderID = order.ID,
                        ClientSupplier = payExchangeSupplier
                    });
                }
                //判断 中心库和 老数据是否已存在，
                var files = new Needs.Ccs.Services.Views.CenterLinkXDTFilesTopView().Where(x => x.MainOrderID == order.MainOrderID && x.FileType == FileType.OriginalInvoice && x.Status != Status.Delete)
                    .Select(x => new CenterFileDescription { ID = x.ID, WsOrderID = x.MainOrderID, Url = x.Url, AdminID = x.AdminID, CreateDate = x.CreateDate, Status = FileDescriptionStatus.Delete, Type = (int)x.FileType })
                    .ToArray();
                string[] ids = files.Select(a => a.ID.ToString()).ToArray();
                //////  如果页面删除文件 ，则modify 中心
                //var mainfiles = new Needs.Ccs.Services.Views.MainOrderFilesView().Where(x => ids.Contains(x.ID)).ToArray();

                ////先删除 再插入
                // new CenterFilesTopView().Modify(files, ids);
                //合同发票
                foreach (var invoice in invoiceList)
                {
                    string ID = invoice.ID;
                    order.MainOrderFiles.Add(new MainOrderFile
                    {
                        MainOrderID = order.MainOrderID,
                        Admin = admin,
                        Name = invoice.Name,
                        FileType = FileType.OriginalInvoice,
                        FileFormat = invoice.FileFormat,
                        Url = invoice.VirtualPath,
                        Type = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice,
                        ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID,
                        ID= string.IsNullOrEmpty(ID)!=true?ID:null
                    });

                    //string ID = invoice.ID;
                    //if (ids.Contains(ID))
                    //    new CenterFilesTopView().Modify(new { WsOrderID = order.MainOrderID }, ID);
                    //else
                    //    new CenterFilesTopView().Modify(new { FileDescriptionStatus.Delete }, ID);
                    //string Name = invoice.Name;
                    //string VirtualPath = invoice.VirtualPath;
                    //DateTime createdate = invoice.CreateDate;
                    //#region 订单文件保存到中心文件库
                    ////如果中心不存在 上传到中心
                    //var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice;
                    //var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                    //var dic = new { CustomName = Name, WsOrderID = order.MainOrderID, AdminID = ErmAdminID };
                    ////本地文件上传到服务器
                    //var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + VirtualPath, centerType, dic);
                    //#endregion
                }

                order.ClassifyProducts = new List<ClassifyProduct>();

                var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];

                //订单项
                foreach (var pageProduct in productList)
                {
                    decimal qty = pageProduct.Qty;
                    decimal totalPrice = pageProduct.TotalPrice;
                    decimal? grossWeight = null;
                    if (pageProduct.GrossWeight != null && pageProduct.GrossWeight != "")
                    {
                        grossWeight = (decimal)pageProduct.GrossWeight;
                    }
                    order.ClassifyProducts.Add(new ClassifyProduct
                    {
                        ID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem),
                        OrderID = order.ID,
                        OrderType = OrderType.Outside,
                        Client = client,
                        Name = pageProduct.Name,
                        Model = ((string)pageProduct.Model).Trim().Replace(replaceQuoteStr, "\""),
                        Manufacturer = ((string)pageProduct.Manufacturer).Trim(),
                        Batch = pageProduct.Batch,
                        Origin = pageProduct.Origin,
                        Quantity = qty,
                        Unit = pageProduct.Unit,
                        UnitPrice = (totalPrice / qty).ToRound(4),
                        Currency = currency,
                        TotalPrice = totalPrice,
                        GrossWeight = grossWeight,
                        ProductUniqueCode = pageProduct.ProductUniqueCode,
                    });
                }

                //验证ProductUniqueCode 是否重复
                List<string> ProductUnionCodes = order.ClassifyProducts.Where(t => !string.IsNullOrEmpty(t.ProductUniqueCode) && t.ProductUniqueCode != null).Select(t => t.ProductUniqueCode).ToList();
                var OrderItemView = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin();

                var count = OrderItemView.Where(t => ProductUnionCodes.Contains(t.ProductUniqueCode)).Count();

                if (count > 0)
                {
                    Response.Write((new { success = false, message = "保存失败：物料号不能重复!" }).Json());
                    return;
                }

                //订单基本信息
                order.Type = OrderType.Outside;
                order.Currency = currency;
                order.CustomsExchangeRate = null;
                order.RealExchangeRate = null;
                order.IsFullVehicle = isFullVehicle;
                order.IsLoan = isLoan;
                order.WarpType = wrapType;
                order.PackNo = packNO;
                order.Summary = summary;
                order.DeclarePrice = order.ClassifyProducts.Select(item => item.TotalPrice).Sum();

                order.SetAdmin(admin);
                order.EnterSuccess += Order_EnterSuccess;
                //正常下单
                if (order.OrderStatus == OrderStatus.Draft)
                {
                    if (isConfirm)
                    {
                        order.OrderStatus = OrderStatus.Confirmed;
                    }

                    order.Enter();
                }
                //已退回订单重新下单
                else if (order.OrderStatus == OrderStatus.Returned)
                {
                    if (isConfirm)
                    {
                        order.OrderStatus = OrderStatus.Confirmed;
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Draft;
                    }

                    order.ReEnter();
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 订单保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var order = (Needs.Ccs.Services.Models.Order)e.Object;
            Response.Write((new { success = true, message = "保存成功！", ID = order.ID }).Json());
        }

        /// <summary>
        /// 选择供应商时，获取该供应商的提货地址
        /// </summary>
        /// <returns></returns>
        protected object GetSupplierAddress()
        {
            string SupplerID = Request.Form["SupplerID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliersAddresses.Where(item => item.ClientSupplierID == SupplerID &&
                                                                                            item.Status == Status.Normal)
                                                                                            .Select(item => new
                                                                                            {
                                                                                                item.ID,
                                                                                                item.Address,
                                                                                                Contact = item.Contact.Name,
                                                                                                item.Contact.Mobile,
                                                                                                item.IsDefault
                                                                                            });

            return data;
        }

        /// <summary>
        /// 新增客户供应商
        /// </summary>
        /// <returns></returns>
        protected object AddSupplier()
        {
            string ClientID = Request.Form["ClientID"];
            var suppliers = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliers.Where(item => item.ClientID == ClientID && item.Status == Status.Normal)
                                                                        .OrderBy(item => item.CreateDate).ToList();
            var data = new
            {
                Suppliers = suppliers.Select(item => new { item.ID, Name = item.ChineseName }),
                SupplierID = suppliers.LastOrDefault().ID
            };

            return data;
        }

        /// <summary>
        /// 新增客户收件信息
        /// </summary>
        /// <returns></returns>
        protected object AddConsignee()
        {
            string ClientID = Request.Form["ClientID"];
            var consignees = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientConsignees.Where(item => item.ClientID == ClientID && item.Status == Status.Normal)
                                                                            .OrderBy(item => item.CreateDate).ToList();
            var consignee = consignees.LastOrDefault();
            var data = new
            {
                Consignees = consignees.Select(item => new
                {
                    item.ID,
                    item.Name,
                    Contact = item.Contact.Name,
                    item.Contact.Mobile,
                    item.Address,
                    item.IsDefault
                }),
                consignee?.ID,
                consignee?.Name,
                Contact = consignee?.Contact.Name,
                consignee?.Contact.Mobile,
                consignee?.Address,
                consignee?.IsDefault
            };

            return data;
        }

        /// <summary>
        /// 新增供应商提货地址
        /// </summary>
        /// <returns></returns>
        protected object AddSupplierAddress()
        {
            string supplierID = Request.Form["SupplierID"];
            var supplierAddresses = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientSuppliersAddresses.Where(item => item.ClientSupplierID == supplierID &&
                                                                                                item.Status == Status.Normal)
                                                                                                .OrderBy(item => item.CreateDate).ToList();
            var supplierAddress = supplierAddresses.LastOrDefault();


            var data = new
            {
                SupplierAddresses = supplierAddresses.Select(item => new
                {
                    item.ID,
                    item.Address,
                    Contact = item.Contact.Name,
                    item.Contact.Mobile,
                    item.IsDefault
                }),
                supplierAddress?.ID,
                supplierAddress?.Address,
                Contact = supplierAddress?.Contact.Name,
                supplierAddress?.Contact.Mobile,
                supplierAddress?.IsDefault
            };

            return data;
        }

        /// <summary>
        /// 导入产品信息
        /// </summary>
        /// <returns></returns>
        protected void ImportProductInfo()
        {
            try
            {
                HttpPostedFile file = Request.Files["uploadExcel"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }

                //文件保存
                string fileName = file.FileName.ReName();

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                //生成DataTable
                var workbook = ExcelFactory.ReadFile(fileDic.FilePath);
                var npoi = new NPOIHelper(workbook);
                DataTable dt = npoi.ExcelToDataTable(false);

                var origins = Needs.Wl.Admin.Plat.AdminPlat.Countries.ToList();
                var products = new List<dynamic>();
                int rowNum = int.Parse(Request.Form["RowNum"]); //页面已有产品数
                int count = 0;  //计数器
                for (int i = 6; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i][5].ToString()) || string.IsNullOrEmpty(dt.Rows[i][6].ToString()) ||
                        string.IsNullOrEmpty(dt.Rows[i][7].ToString()) || string.IsNullOrEmpty(dt.Rows[i][10].ToString()) ||
                        string.IsNullOrEmpty(dt.Rows[i][11].ToString()) || string.IsNullOrEmpty(dt.Rows[i][12].ToString()))
                    {
                        continue;
                    }
                    //一个订单的产品型号不能超过20个
                    if (count >= 20 - rowNum)
                    {
                        break;
                    }
                    if (dt.Rows[i][0].ToString() == "合计")
                    {
                        break;
                    }

                    dynamic product = new ExpandoObject();
                    product.ProductUniqueCode = dt.Rows[i][1].ToString();
                    product.Name = dt.Rows[i][5].ToString();
                    product.Manufacturer = dt.Rows[i][6].ToString();
                    product.Model = dt.Rows[i][7].ToString();
                    product.Batch = dt.Rows[i][3].ToString();
                    var origin = dt.Rows[i][8].ToString();
                    product.Origin = origins.Where(o => o.Code == origin || o.Name == origin).FirstOrDefault()?.Code;
                    product.Qty = decimal.Parse(dt.Rows[i][10].ToString());
                    product.Unit = "007";
                    product.UnitPrice = decimal.Parse(dt.Rows[i][11].ToString());
                    product.TotalPrice = product.Qty * product.UnitPrice;
                    products.Add(product);

                    count++;
                }

                Response.Write((new { success = true, data = products }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传原始PI
        /// </summary>
        /// <returns></returns>
        protected void UploadPI()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadPI");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", ".pdf" };
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)或pdf格式！" }).Json());
                            return;
                        }

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.OriginalInvoice.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl
                            });

                            #region 订单文件保存到中心文件库

                            //var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                            //var dic = new { CustomName = fileName, AdminID = ErmAdminID };

                            //var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Invoice;

                            ////本地文件上传到服务器
                            //var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, centerType, dic);
                            
                            #endregion
                        }
                    }
                }
                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, data = new { } }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传提货文件
        /// </summary>
        protected void UploadDeliveryFile()
        {
            try
            {
                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadDeliveryFilePlus");
                if (files.Count > 0)
                {
                    var validTypes = new List<string>() { ".jpg", ".bmp", ".jpeg", ".gif", ".png", ".pdf", ".doc", ".docx" };
                    for (int i = 0; i < files.Count; i++)
                    {
                        string ext = Path.GetExtension(files[i].FileName);
                        if (!validTypes.Contains(ext.ToLower()))
                        {
                            Response.Write((new { success = false, message = "上传的原始PI只能是图片(jpg、bmp、jpeg、gif、png)或pdf或doc、docx格式！" }).Json());
                            return;
                        }

                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            //文件保存
                            string fileName = files[i].FileName.ReName();

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            fileList.Add(new
                            {
                                Name = file.FileName,
                                FileType = FileType.DeliveryFiles.GetDescription(),
                                FileFormat = file.ContentType,
                                VirtualPath = fileDic.VirtualPath,
                                Url = fileDic.FileUrl
                            });
                            //#region 订单文件保存到中心文件库

                            //var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                            //var dic = new { CustomName = fileName, AdminID = ErmAdminID };
                            //var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.Delivery;
                            ////本地文件上传到服务器
                            //var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, centerType, dic);
                            //#endregion
                        }
                    }
                }
                if (fileList.Count() == 0)
                {
                    Response.Write((new { success = false, data = new { } }).Json());
                }
                else
                {
                    Response.Write((new { success = true, data = fileList }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }
    }
}
using Needs.Underly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Enums;
using WebApi.Models;
using Needs.Utils.Serializers;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderController : MyApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult GetReceptorOrder(JPost obj)
        {
            try
            {
                var receptor = obj.ToObject<Models.ReceptorOrder.ReceptorOrder>();

                //记录原始报文
                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    OrderID = receptor.ID,
                    TinyOrderID = receptor.ID + "-01",
                    RequestContent = receptor.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "代仓储下单"
                };
                apiLog.Enter();

                System.Threading.Tasks.Task.Run(() =>
                {
                    PushMsg pushMsg = new PushMsg((int)SpotName.DOrdered, receptor.ID);
                    pushMsg.push();
                });
              
                //整理下单人信息
                var IsAdmin = false;
                var admin = new Needs.Ccs.Services.Models.Admin();
                var user = new Needs.Ccs.Services.Models.User();
                if (!string.IsNullOrEmpty(receptor.AdminID))
                {
                    IsAdmin = true;
                    var OriginAdmin = new AdminsTopView2().FirstOrDefault(t => t.ID == receptor.AdminID);
                    admin = new Needs.Ccs.Services.Models.Admin
                    {
                        ID = OriginAdmin.OriginID,
                        UserName = OriginAdmin.UserName,
                        RealName = OriginAdmin.RealName
                    };
                }
                else
                {
                    IsAdmin = false;
                    user = new UsersView().FirstOrDefault(t => t.ID == receptor.UserID);
                    //user = new Needs.Ccs.Services.Models.User
                    //{
                    //    ID = receptor.UserID,
                    //    Name = receptor.ClientName
                    //};
                }


                foreach (var tiny in receptor.Items.Select(t => t.TinyOrderID).Distinct())
                {
                    #region 一个小订单

                    var order = new Needs.Ccs.Services.Models.Order();
                    order.ID = tiny;//订单ID
                    order.MainOrderID = receptor.ID;//主订单ID

                    //客户
                    order.Client = new ClientsView().FirstOrDefault(t => t.Company.Name == receptor.ClientName && t.ClientCode == receptor.ClientCode);

                    //协议
                    order.ClientAgreement = new ClientAgreementsView().FirstOrDefault(t => t.Status == Status.Normal && t.ClientID == order.Client.ID);

                    //香港交货、供应商
                    order.OrderConsignee = new Needs.Ccs.Services.Models.OrderConsignee();
                    var supplier = new ClientSuppliers(order.Client).FirstOrDefault(t => t.Name == receptor.OrderConsignee.ClientSupplierName);

                    order.OrderConsignee.OrderID = order.ID;
                    order.OrderConsignee.ClientSupplier = supplier;
                    order.OrderConsignee.Type = receptor.OrderConsignee.Type;
                    if (order.OrderConsignee.Type != HKDeliveryType.PickUp)
                    {
                        order.OrderConsignee.Contact = null;
                        order.OrderConsignee.Mobile = null;
                        order.OrderConsignee.Address = null;
                        order.OrderConsignee.PickUpTime = null;
                        order.OrderConsignee.WayBillNo = receptor.OrderConsignee.WayBillNo;
                    }
                    else
                    {
                        order.OrderConsignee.Contact = receptor.OrderConsignee.Contact;
                        order.OrderConsignee.Mobile = receptor.OrderConsignee.Mobile;
                        order.OrderConsignee.Address = receptor.OrderConsignee.Address;
                        order.OrderConsignee.PickUpTime = receptor.OrderConsignee.PickUpTime;
                        order.OrderConsignee.WayBillNo = null;

                        //提货单
                        //if (order.OrderConsignee.Type == HKDeliveryType.PickUp)
                        //{
                        //    string docType = "msword";
                        //    string docxType = "vnd.openxmlformats-officedocument.wordprocessingml.document";
                        //    var thisDeliveryFile = new Needs.Ccs.Services.Models.MainOrderFile
                        //    {
                        //        MainOrderID = order.MainOrderID,
                        //        Admin = admin,
                        //        Name = deliveryFilePlus[0].Name,
                        //        FileType = FileType.DeliveryFiles,
                        //        FileFormat = Convert.ToString(deliveryFilePlus[0].FileFormat).Replace(docType, "doc").Replace(docxType, "docx"),
                        //        Url = Convert.ToString(deliveryFilePlus[0].VirtualPath).Replace(@"/", @"\")
                        //    };
                        //    order.MainOrderFiles.Add(thisDeliveryFile);
                        //}
                    }


                    //深圳交货
                    order.OrderConsignor = new Needs.Ccs.Services.Models.OrderConsignor
                    {
                        OrderID = order.ID,
                        Type = receptor.OrderConsignor.Type,
                        Name = receptor.OrderConsignor.Name,
                        Contact = receptor.OrderConsignor.Contact,
                        Mobile = receptor.OrderConsignor.Mobile,
                        Tel = receptor.OrderConsignor.Tel,
                        Address = receptor.OrderConsignor.Address,
                        IDType = receptor.OrderConsignor.IDType,
                        IDNumber = receptor.OrderConsignor.IDNumber
                    };

                    //付汇供应商
                    order.PayExchangeSuppliers.RemoveAll();
                    foreach (var item in receptor.PayExchangeSuppliers)
                    {
                        var payExchangeSupplier = new ClientSuppliers(order.Client).FirstOrDefault(t => t.Name == item.ClientSupplierName);
                        order.PayExchangeSuppliers.Add(new Needs.Ccs.Services.Models.OrderPayExchangeSupplier
                        {
                            OrderID = order.ID,
                            ClientSupplier = payExchangeSupplier
                        });
                    }

                    //订单项
                    order.ClassifyProducts = new List<Needs.Ccs.Services.Models.ClassifyProduct>();
                    var preProductView = new ClientPreProductView();//客户预归类视图
                    foreach (var item in receptor.Items.Where(t => t.TinyOrderID == order.ID))
                    {
                        decimal qty = item.Quantity;
                        decimal totalPrice = item.TotalPrice;
                        decimal? grossWeight = null;
                        if (item.GrossWeight != null)
                        {
                            grossWeight = item.GrossWeight;
                        }

                        if (!string.IsNullOrEmpty(item.PreProductID) && preProductView.Any(t => t.ID == item.PreProductID))  //已归类产品
                        {
                            #region 快捷下单
                            //查询归类信息
                            var category = preProductView.Where(t => t.ID == item.PreProductID).FirstOrDefault();

                            //产品分类
                            Needs.Ccs.Services.Models.OrderItemCategory orderItemCategory = new Needs.Ccs.Services.Models.OrderItemCategory();
                            orderItemCategory.CIQCode = category.CIQCode;
                            orderItemCategory.Elements = category.Elements;
                            orderItemCategory.HSCode = category.HSCode;
                            orderItemCategory.Name = category.ProductName;
                            orderItemCategory.TaxCode = category.TaxCode;
                            orderItemCategory.TaxName = category.TaxName;
                            orderItemCategory.Type = category.Type.Value;
                            orderItemCategory.Unit1 = category.Unit1;
                            orderItemCategory.Unit2 = category.Unit2;
                            //orderItemCategory.Declarant = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(pageProduct.Declarant);
                            orderItemCategory.ClassifyFirstOperator = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(category.ClassifyFirstOperator);
                            orderItemCategory.ClassifySecondOperator = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(category.ClassifySecondOperator);
                            //关税
                            Needs.Ccs.Services.Models.OrderItemTax ImportTax = new Needs.Ccs.Services.Models.OrderItemTax();
                            ImportTax.Rate = category.TariffRate.Value;
                            ImportTax.ImportPreferentialTaxRate = category.TariffRate.Value;
                            ImportTax.OriginRate = 0m;
                            ImportTax.ReceiptRate = category.TariffRate.Value;
                            ImportTax.Type = CustomsRateType.ImportTax;

                            //增值税率
                            Needs.Ccs.Services.Models.OrderItemTax AddedValueTax = new Needs.Ccs.Services.Models.OrderItemTax();
                            AddedValueTax.Rate = category.AddedValueRate.Value;
                            AddedValueTax.ReceiptRate = category.AddedValueRate.Value;
                            AddedValueTax.Type = CustomsRateType.AddedValueTax;

                            //消费税
                            Needs.Ccs.Services.Models.OrderItemTax ExciseTax = new Needs.Ccs.Services.Models.OrderItemTax();
                            ExciseTax.Rate = category.ExciseTaxRate.GetValueOrDefault();
                            ExciseTax.ReceiptRate = category.ExciseTaxRate.GetValueOrDefault();
                            ExciseTax.Type = CustomsRateType.ConsumeTax;

                            var productControls = new ProductControlsView(category.Model).ToList();

                            var isSysCCC = false;
                            var isSysForbid = false;
                            foreach (var control in productControls)
                            {
                                if (control.Type == ProductControlType.CCC)
                                {
                                    isSysCCC = true;
                                    orderItemCategory.Type |= ItemCategoryType.CCC;
                                }
                                if (control.Type == ProductControlType.Forbid)
                                {
                                    isSysForbid = true;
                                    orderItemCategory.Type |= ItemCategoryType.Forbid;
                                }
                            }

                            //如果来自疫区，则为归类类型添加“检疫”
                            if (new CustomsQuarantinesView().IsQuarantine(item.Origin))
                            {
                                orderItemCategory.Type |= ItemCategoryType.Quarantine;
                            }

                            order.ClassifyProducts.Add(new Needs.Ccs.Services.Models.ClassifyProduct
                            {
                                ID = item.ID,
                                OrderID = order.ID,
                                //Product = product,
                                Client = order.Client,
                                Name = item.Name,
                                Manufacturer = item.Manufacturer,
                                Model = item.Model,
                                Batch = item.Batch,
                                Origin = item.Origin,
                                Quantity = qty,
                                Unit = item.Unit,
                                UnitPrice = item.UnitPrice,
                                Currency = receptor.Currency,
                                TotalPrice = totalPrice,
                                GrossWeight = grossWeight,
                                ClassifyStatus = ClassifyStatus.Done,
                                Category = orderItemCategory,
                                IsSysForbid = isSysForbid,
                                IsSysCCC = isSysCCC,
                                IsCCC = (orderItemCategory.Type & ItemCategoryType.CCC) > 0,
                                IsOriginProof = (orderItemCategory.Type & ItemCategoryType.OriginProof) > 0,
                                IsInsp = (orderItemCategory.Type & ItemCategoryType.Inspection) > 0,
                                InspectionFee = category.InspectionFee,
                                ImportTax = ImportTax,
                                AddedValueTax = AddedValueTax,
                                ExciseTax = ExciseTax,
                                ProductUniqueCode = item.ProductUniqueCode,
                            });
                            #endregion
                        }
                        else
                        {
                            order.ClassifyProducts.Add(new Needs.Ccs.Services.Models.ClassifyProduct
                            {
                                ID = item.ID,
                                OrderID = order.ID,
                                OrderType = OrderType.Outside,
                                Client = order.Client,
                                Name = item.Name,
                                Model = ((string)item.Model).Trim(),
                                Manufacturer = ((string)item.Manufacturer).Trim(),
                                Batch = item.Batch,
                                Origin = item.Origin,
                                Quantity = qty,
                                Unit = item.Unit,
                                UnitPrice = item.UnitPrice,
                                Currency = receptor.Currency,
                                TotalPrice = totalPrice,
                                GrossWeight = grossWeight,
                                ProductUniqueCode = item.ProductUniqueCode
                            });
                        }
                    }

                    //附件
                    //foreach (var file in receptor.MainOrderFiles)
                    //{
                    //    var orderfile = new Needs.Ccs.Services.Models.MainOrderFile();
                    //    orderfile.MainOrderID = order.MainOrderID;
                    //    if (IsAdmin)
                    //    {
                    //        orderfile.Admin = admin;
                    //    }
                    //    else
                    //    {
                    //        orderfile.User = user;
                    //    }
                    //    orderfile.Name = file.Name;
                    //    orderfile.FileType = file.FileType;
                    //    orderfile.FileFormat = file.FileFormat;
                    //    orderfile.Url = file.Url;

                    //    order.MainOrderFiles.Add(orderfile);
                    //}


                    //基本信息
                    order.AdminID = order.Client.Merchandiser.ID;
                    order.Type = OrderType.Outside;
                    order.Currency = receptor.Currency;
                    order.CustomsExchangeRate = null;
                    order.RealExchangeRate = null;
                    order.IsFullVehicle = receptor.IsFullVehicle;
                    order.IsLoan = receptor.IsLoan;
                    order.WarpType = receptor.WarpType;
                    order.PackNo = receptor.PackNo;
                    order.Summary = receptor.Summary;
                    order.DeclarePrice = order.ClassifyProducts.Select(item => item.TotalPrice).Sum();

                    //判断是否快捷下单：
                    if (!string.IsNullOrEmpty(receptor.Items[0].PreProductID))
                    {
                        order.OrderStatus = OrderStatus.Classified;
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Confirmed;
                    }

                    if (IsAdmin)
                    {
                        order.SetAdmin(admin);
                    }
                    else
                    {
                        order.SetUser(user);
                        order.UserID = user.ID;
                    }

                    //判断是否退回重新下单
                    var tinyOrder = new Orders2View().Where(t => t.MainOrderID == receptor.ID && t.Status == Status.Normal).ToList();// && (t.OrderStatus == OrderStatus.Returned || t.OrderStatus == OrderStatus.Canceled));

                    //没有订单
                    if (tinyOrder.Count() == 0)
                    {
                        //正常下单
                        order.Enter();
                    }
                    //当前订单是退回订单
                    else if (tinyOrder.Any(t => t.ID == order.ID && (t.OrderStatus == OrderStatus.Returned || t.OrderStatus == OrderStatus.Canceled)))
                    {
                        //已退回订单重新下单
                        order.ReEnter();
                    }
                    else
                    {
                        //有多个小订单ID，并且当前订单不是退回订单,不处理
                    }

                    #endregion
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 订单确认
        /// 2022-01-19 香港库房重构、需要生成EntryNotice
        /// </summary>
        /// <param name="OrderConfirmed"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult OrderConfirmOld(JPost OrderConfirmed)
        {
            try
            {
                var confirm = OrderConfirmed.ToObject<Models.Order.OrderConfirmed>();


                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    // OrderID = applies.Items[0].VastOrderID,
                    TinyOrderID = confirm?.OrderID,
                    RequestContent = confirm.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "代仓储调用确认订单"
                };
                apiLog.Enter();

                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.QuotedOrder, bool>> expression = item => true;
                List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.QuotedOrder, bool>> lambda = item => item.MainOrderID == confirm.OrderID.Trim();
                lambdas.Add(lambda);

                var user = new UsersView()[confirm.UserID];

                var orders = new Orders1ViewBase<Needs.Ccs.Services.Models.QuotedOrder>().GetAlls(expression, lambdas.ToArray());

                //待报价,确认
                if (!confirm.IsCancel)
                {
                    #region 废弃
                    //if (!confirm.IsHangUpConfirm)
                    //{
                    //    var order = orders.Where(t => t.OrderStatus == OrderStatus.Quoted).ToList();
                    //    foreach (var item in order)
                    //    {
                    //        item.SetUser(user);
                    //        item.QuoteConfirm();
                    //    }
                    //}
                    //else
                    //{
                    //    //挂起订单重新确认
                    //    //因小订单可能单独被挂起
                    //    var order = orders.Where(t => t.IsHangUp).ToList();
                    //    foreach (var item in order)
                    //    {
                    //        item.SetUser(user);
                    //        item.ModelModifiedConfirm();
                    //    }
                    //}
                    #endregion

                    //确认订单
                    //根据订单状态判断，正常确认
                    var order = orders.Where(t => t.OrderStatus == OrderStatus.Quoted).ToList();
                    foreach (var item in order)
                    {
                        item.SetUser(user);
                        item.QuoteConfirm();
                    }

                    //挂起订单重新确认
                    //因小订单可能单独被挂起
                    var order1 = orders.Where(t => t.IsHangUp).ToList();
                    foreach (var item in order1)
                    {
                        item.SetUser(user);
                        item.ModelModifiedConfirm();
                    }
                }
                else
                {
                    //取消订单
                    var order = orders.Where(t => t.OrderStatus == OrderStatus.Quoted || t.IsHangUp).ToList();
                    foreach (var item in order)
                    {
                        //如果是因为修改（删除型号/修改数量）引发的确认，需要将管控置为已处理（通过），并将订单取消挂起
                        //if (confirm.IsHangUpConfirm)
                        //{
                        //    var clientUnConfirmedControl = new Needs.Ccs.Services.Models.ClientUnConfirmedControl(item.ID, item.OrderStatus.GetHashCode(), user);
                        //    clientUnConfirmedControl.CancelHangUp();
                        //}

                        //订单取消，直接改为取消挂起
                        var clientUnConfirmedControl = new Needs.Ccs.Services.Models.ClientUnConfirmedControl(item.ID, item.OrderStatus.GetHashCode(), user);
                        clientUnConfirmedControl.CancelHangUp();

                        var newQuotedOrder = new Needs.Ccs.Services.Models.QuotedOrder();
                        newQuotedOrder.ID = item.ID;
                        newQuotedOrder.SetUser(user);
                        newQuotedOrder.CanceledSummary = confirm.CancelReason;
                        newQuotedOrder.Cancel();
                    }
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("带仓储调用确认订单");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 委托书、对账单附件，代仓储前端调用
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public ActionResult OrderFile(JPost File)
        {
            try
            {
                var file = File.ToObject<Needs.Ccs.Services.Models.MainOrderFile>();

                file.User = new Needs.Ccs.Services.Models.User()
                {
                    ID = file.UserID,
                };

                if (file != null)
                {
                    file.Enter();

                    var json = new JMessage()
                    {
                        code = 200,
                        success = true,
                        data = "提交成功"
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var json = new JMessage()
                    {
                        code = 100,
                        success = true,
                        data = "提交数据不正确"
                    };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 用户确定订单收货
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult UserConfirmReceipt(JPost obj)
        {
            string batchID = Guid.NewGuid().ToString("N");

            var requestModel = obj.ToObject<Models.Order.UserConfirmReceiptModel>();

            string MainOrderID = requestModel.MainOrderID;
            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                OrderID = MainOrderID,
                RequestContent = requestModel.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = "用户确定订单收货"
            };
            apiLog.Enter();

            try
            {
                Needs.Ccs.Services.Models.UserConfirmReceipt userConfirmReceipt = new Needs.Ccs.Services.Models.UserConfirmReceipt(MainOrderID);
                userConfirmReceipt.Execute();

                var json = new JMessage() { code = 200, success = true, data = "提交成功" };

                apiLog.ResponseContent = json.Json();
                apiLog.Enter();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };

                apiLog.ResponseContent = json.Json();
                apiLog.Enter();

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 销售合同生成PDF文件路径，会员端调用
        /// </summary>
        /// <param name="ID"></param>
        /// <returns>PDF文件路径</returns>
        public ActionResult SalesContract(string ID)
        {
            try
            {
                Purchaser seller = PurchaserContext.Current;

                //取第一个订单
                var orders = new Needs.Ccs.Services.Views.SalesContractOrdersView().Where(x => x.MainOrderID == ID).ToArray();

                var orderIDs = orders.Select(t => t.ID).ToList();
                var order = orders.FirstOrDefault();
                //订单客户
                var client = order.Client;
                //客户补充协议
                var agreement = client.Agreement;

                //基础信息
                var model = new Needs.Ccs.Services.Models.SalesContract
                {
                    ID = ID,
                    SalesDate = order.CreateDate,
                    Buyer = new Needs.Ccs.Services.Models.InvoiceBaseInfo
                    {
                        Title = client.Invoice.Title,
                        Address = client.Invoice.Address,
                        BankName = client.Invoice.BankName,
                        BankAccount = client.Invoice.BankAccount,
                        Tel = client.Invoice.Tel
                    },
                    Seller = new Needs.Ccs.Services.Models.InvoiceBaseInfo
                    {
                        Title = seller.CompanyName,
                        Address = seller.Address,
                        BankName = seller.BankName,
                        BankAccount = seller.AccountId,
                        Tel = seller.Tel,
                        SealUrl = seller.SealUrl
                    },
                    InvoiceType = agreement.InvoiceType
                };

                //型号信息
                var salesItems = new List<Needs.Ccs.Services.Models.ContractItem>();

                var orderItem = new Needs.Ccs.Services.Views.InvoiceOrderItemView().Where(x => orderIDs.Contains(x.OrderID));
                Needs.Ccs.Services.Models.InvoiceItemAmountCalc calc = new Needs.Ccs.Services.Models.InvoiceItemAmountCalc(orderIDs);
                List<Needs.Ccs.Services.Models.InvoiceItemAmountHelp> helper = calc.AmountResult();
                var units = new Needs.Ccs.Services.Views.BaseUnitsView().ToList();

                foreach (var item in orderItem)
                {
                    var sale = new Needs.Ccs.Services.Models.ContractItem
                    {

                        OrderItemID = item.ID,
                        ProductName = item.Category.Name,
                        Model = item.Model,
                        Quantity = item.Quantity,
                        Unit = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit,
                        //UnitPrice = item.UnitPrice,
                        TotalPrice = item.GetSalesTotalPriceRatSpeed(orders, agreement, helper).ToRound(2)
                    };

                    salesItems.Add(sale);
                }

                model.ContractItems = salesItems;

                //保存文件
                string fileName = DateTime.Now.Ticks + ".pdf";
                Needs.Utils.FileDirectory fileDic = new Needs.Utils.FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var contractPdf = new Needs.Ccs.Services.Models.SalesContractToPdf(model);
                contractPdf.SaveAs(fileDic.FilePath);

                var returnUrl = System.Configuration.ConfigurationManager.AppSettings["APIFileServerUrl"] + "/" + fileDic.VirtualPath.Replace(@"\", "/");

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = returnUrl
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 香港代仓储 转报关，添加订单费用
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="orderFee"></param>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public ActionResult AddOrderPremiums(string orderID, decimal orderFee, string adminID)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    string originAdminID = "";
                    originAdminID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>().Where(t => t.ID == adminID).FirstOrDefault()?.OriginID;

                    string[] orderArray = orderID.Split('-');
                    if (orderArray.Length == 1)
                    {
                        orderID = orderID + "-01";
                    }

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new Layer.Data.Sqls.ScCustoms.OrderPremiums
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                        OrderID = orderID,

                        AdminID = originAdminID,
                        Type = (int)OrderPremiumType.OtherFee,
                        Count = 1,
                        UnitPrice = orderFee,
                        Currency = MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.Currency>(Needs.Ccs.Services.Enums.Currency.CNY),
                        Rate = 1,
                        Status = (int)Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }


                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = ""
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 订单确认
        /// </summary>
        /// <param name="OrderConfirmed"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult OrderConfirm(JPost OrderConfirmed)
        {
            try
            {
                var confirm = OrderConfirmed.ToObject<Models.Order.OrderConfirmed>();


                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    // OrderID = applies.Items[0].VastOrderID,
                    TinyOrderID = confirm?.OrderID,
                    RequestContent = confirm.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "代仓储调用确认订单"
                };
                apiLog.Enter();

                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.QuotedOrder, bool>> expression = item => true;
                List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.QuotedOrder, bool>> lambda = item => item.MainOrderID == confirm.OrderID.Trim();
                lambdas.Add(lambda);

                var user = new UsersView()[confirm.UserID];

                var orders = new Orders1ViewBase<Needs.Ccs.Services.Models.QuotedOrder>().GetAlls(expression, lambdas.ToArray());

                //待报价,确认
                if (!confirm.IsCancel)
                {
                    //确认订单
                    //根据订单状态判断，正常确认
                    var order = orders.Where(t => t.OrderStatus == OrderStatus.Quoted).ToList();
                    foreach (var item in order)
                    {
                        item.SetUser(user);
                        item.QuoteConfirm();
                    }

                    //挂起订单重新确认
                    //因小订单可能单独被挂起
                    var order1 = orders.Where(t => t.IsHangUp).ToList();
                    foreach (var item in order1)
                    {
                        item.SetUser(user);
                        item.ModelModifiedConfirm();
                    }

                    List<string> orderIDs = new List<string>();
                    //生成EntryNotice
                    foreach (var item in order)
                    {
                        orderIDs.Add(item.ID);
                        EntryNotice entryNotice = new EntryNotice();
                        entryNotice.OrderID = item.ID;
                        entryNotice.ClientCode = item.ID.Substring(0, 5);
                        entryNotice.SortingRequire = SortingRequire.Packed;
                        entryNotice.WarehouseType = WarehouseType.HongKong;
                        entryNotice.Enter();
                    }

                    #region 生成EntryNoticeItem
                    Task task1 = new Task(() =>
                    {
                        GenerateEntryNotice(orderIDs);
                    });
                    task1.Start();
                    #endregion
                }
                else
                {
                    //取消订单
                    var order = orders.Where(t => t.OrderStatus == OrderStatus.Quoted || t.IsHangUp).ToList();
                    foreach (var item in order)
                    {
                        //订单取消，直接改为取消挂起
                        var clientUnConfirmedControl = new Needs.Ccs.Services.Models.ClientUnConfirmedControl(item.ID, item.OrderStatus.GetHashCode(), user);
                        clientUnConfirmedControl.CancelHangUp();

                        var newQuotedOrder = new Needs.Ccs.Services.Models.QuotedOrder();
                        newQuotedOrder.ID = item.ID;
                        newQuotedOrder.SetUser(user);
                        newQuotedOrder.CanceledSummary = confirm.CancelReason;
                        newQuotedOrder.Cancel();
                    }
                }

                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ex.CcsLog("带仓储调用确认订单");
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        private void GenerateEntryNotice(List<string> orderIDs)
        {
            foreach (var item in orderIDs)
            {
                var EntryNotice = new EntryNoticeView().Where(t => t.OrderID == item).FirstOrDefault();
                if (EntryNotice != null)
                {
                    var items = EntryNotice.Order.Items;
                    foreach (var orderItem in items)
                    {
                        EntryNoticeItem entryNoticeItem = new EntryNoticeItem();
                        entryNoticeItem.EntryNoticeID = EntryNotice.ID;
                        entryNoticeItem.OrderItem = orderItem;
                        entryNoticeItem.IsSportCheck = false;
                        entryNoticeItem.Enter();
                    }
                }
            }
        }

    }
}
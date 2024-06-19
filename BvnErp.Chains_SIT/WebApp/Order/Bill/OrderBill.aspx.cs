using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Models;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Order.Bill
{
    /// <summary>
    /// 对账单
    /// </summary>
    public partial class OrderBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 加载对账单数据
        /// </summary>
        protected void LoadData()
        {
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;
            string ReplaceSingleQuotes = "这里是一个单引号";
            this.Model.ReplaceSingleQuotes = ReplaceSingleQuotes;

            var id = Request.QueryString["ID"];

            string[] mainOrder = id.Split('-');
            string MainOrderID = "";
            if (mainOrder.Length > 1)
            {
                MainOrderID = mainOrder[0];
            }
            else
            {
                MainOrderID = id;
            }


            var viewModel = getModel(MainOrderID);

            if (viewModel == null)
            {
                this.Model.IsShowBill = false;
            }
            else
            {
                this.Model.IsShowBill = true;
                foreach (var t in viewModel.Bills)
                {
                    foreach (var p in t.Products)
                    {
                        p.Model = p.Model.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes);
                    }
                }
            }

            this.Model.MainOrderID = MainOrderID;

            this.Model.Bill = viewModel.Json();
            //TODO:向后把IsHistory 逻辑完善
            this.Model.IsHistory = "0";

            var OrdersAdvanceMoney = new AdvanceMoneyOrderIdView().Where(item => item.OrderID == MainOrderID).ToList();
            if (OrdersAdvanceMoney.Count == 0)
            {
                this.Model.OrdersAdvanceMoney = 0;
            }
            else
            {
                this.Model.OrdersAdvanceMoney = OrdersAdvanceMoney.Sum(t => t.Amount);
            }

        }

        protected void GetVouchers()
        {
            string MainOrderID = Request.Form["MainOrderID"];

            //获取已使用的抵用券信息 Begin

            var vouchers = new Needs.Ccs.Services.Views.FinanceVoucherView().GetFinanceVoucherByMainOrderID(MainOrderID)
                .Select(item => new
                {
                    FinanceVoucherID = item.ID,
                    Amount = item.Amount,
                    OrderID = item.OrderID,
                });

            //获取已使用的抵用券信息 End

            Response.Write((new { success = true, vouchers = vouchers.Json(), }).Json());
        }

        /// <summary>
        /// 导出对账单
        /// </summary>
        protected void ExportBill()
        {
            try
            {
                var id = Request.Form["ID"];
                //var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderBills[id];
                string[] mainOrder = id.Split('-');
                string MainOrderID = "";
                if (mainOrder.Length > 1)
                {
                    MainOrderID = mainOrder[0];
                }
                else
                {
                    MainOrderID = id;
                }

                #region 不过超过10页

                var bill = getModel(MainOrderID);

                string fileName = "对账单" + DateTime.Now.ToString("MMddHHmmss") + ".pdf";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                if (bill.ClientType == ClientType.Internal)
                {
                    bill.ProductsForIcgoo = new List<MainOrderBillItemProduct>();
                    bill.PartProductsForIcgoo = new List<MainOrderBillItemProduct>();
                    foreach (var t in bill.Bills)
                    {
                        bill.ProductsForIcgoo.AddRange(t.Products);
                        bill.PartProductsForIcgoo.AddRange(t.PartProducts);
                    }
                    var orderbill = new OrderBillToPdf(bill);
                    orderbill.SaveAs(fileDic.FilePath);
                    //bill.SaveASIcgoo(fileDic.FilePath);
                }
                else
                {
                    //var orderbill = new OrderBillToPdf(bill);
                    //orderbill.SaveAs(fileDic.FilePath);
                    bill.SaveAs(fileDic.FilePath);
                }
                #endregion

                Response.Write((new
                {
                    success = true,
                    message = "导出成功",
                    url = fileDic.FileUrl,
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传对账单
        /// </summary>
        protected void UploadBill()
        {
            try
            {
                var id = Request.Form["ID"];
                var fileID = Request.Form["FileID"];
                var file = Request.Files["uploadFile"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //文件保存
                string ext = System.IO.Path.GetExtension(file.FileName);
                string fileName = DateTime.Now.Ticks + ext;

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Order);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                Needs.Ccs.Services.Models.MainOrderFile orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                if (!string.IsNullOrEmpty(fileID))
                {
                    orderBill.ID = fileID;
                    orderBill.Abandon();
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, fileID);
                }
                //orderBill = new Needs.Ccs.Services.Models.MainOrderFile();
                //orderBill.MainOrderID = id;
                //orderBill.Admin = admin;
                //orderBill.Name = file.FileName;
                //orderBill.FileType = FileType.OrderBill;
                //orderBill.FileFormat = file.ContentType;
                //orderBill.Url = fileDic.VirtualPath;
                // orderBill.FileStatus = OrderFileStatus.Audited;
                //orderBill.Enter();


                #region 本地文件同步中心文件库

                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, WsOrderID = id, AdminID = ErmAdminID };

                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.OrderBill;
                //本地文件上传到服务器
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(fileDic.FilePath, centerType, dic);
                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                #endregion

                //NoticeLog notice = new NoticeLog();
                //notice.NoticeType = SendNoticeType.OrderBillUploaded;
                //notice.MainID = id;
                //notice.SendNotice();
                ////因为是从管理端上传的对账单，上传的时候就设为已审批，所以发送两次通知，将消息置为已读
                //notice.SendNotice();

                Response.Write((new
                {
                    success = true,
                    message = "上传成功",
                    data = new
                    {
                        ID = id,
                        Name = result[0].FileName,
                        FileStatus = OrderFileStatus.Audited.GetDescription(),
                        Url = result[0].Url
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        protected void ApproveBill()
        {
            try
            {
                var id = Request.Form["ID"];
                //var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills[id];
                //bill.Approve();

                var OrderIDs = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.QuoteConfirmed)
                           .Select(item => item.ID).ToList();
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[OrderIDs[0]];
                var OrderBillFile = order.MainOrderFiles.Where(file => file.FileType == FileType.OrderBill && file.Status != Status.Delete).FirstOrDefault();
                if (OrderBillFile != null)
                {
                    MainOrderBillViewModel viewModel = new MainOrderBillViewModel();
                    viewModel.FileID = OrderBillFile.ID;
                    viewModel.Approve();

                    NoticeLog notice = new NoticeLog();
                    notice.NoticeType = SendNoticeType.OrderBillUploaded;
                    notice.Readed = true;
                    notice.MainID = order.MainOrderID;
                    notice.SendNotice();
                }


                Response.Write((new
                {
                    success = true,
                    message = "审核成功",
                    data = new
                    {
                        ID = order.MainOrderID,
                        Name = OrderBillFile.Name,
                        FileStatus = OrderFileStatus.Audited.GetDescription()
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单审核对账单通知代仓储失败");
                Response.Write((new { success = false, message = "审核失败：" + ex.Message }).Json());
            }
        }

        protected MainOrderBillViewModel getModel(string id)
        {
            var viewModel = new MainOrderBillViewModel();

            var model = getModelStander(id);
            if (model == null)
            {
                return null;
            }
            else
            {
                #region 两个Model 转换              
                viewModel.MainOrderID = id;
                var OrdersAdvanceMoney = new AdvanceMoneyOrderIdView().Where(item => item.OrderID == id).ToList();
                decimal AdvanceMoney = 0;
                if (OrdersAdvanceMoney.Count == 0)
                {
                    AdvanceMoney = 0;
                }
                else
                {
                    AdvanceMoney = OrdersAdvanceMoney.Sum(t => t.Amount);
                }
                viewModel.Bills = model.Bills;

                var purchaser = PurchaserContext.Current;
                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();

                viewModel.ClientName = model.OrderBill.Client.Company.Name;
                viewModel.ClientTel = model.OrderBill.Client.Company.Contact.Tel;
                viewModel.ClientAddress = model.OrderBill.Client.Company.Address;
                viewModel.ClientCode = model.OrderBill.Client.ClientCode;
                viewModel.Currency = model.OrderBill.Currency;
                viewModel.IsLoan = model.OrderBill.IsLoan;

                //2023-09-28 更新
                //特殊客户单抬头开票增值税不包含运保杂计算，当前有两个客户webconfig
                var NoTransPremiumInsurance = System.Configuration.ConfigurationManager.AppSettings["NoTransPremiumInsurance"];
                viewModel.HasYBZ = !NoTransPremiumInsurance.Split(',').Contains(model.OrderBill.Client.ClientCode);

                viewModel.Agreement = model.OrderBill.Agreement;
                //  viewModel.DueDate = model.OrderBill.GetDueDate().ToString("yyyy年MM月dd日");
                //1.存在部分垫资，其中已还款，但是日期最近，也显示已还款那笔垫资的订单
                //2.不考虑已退单存在垫资的订单
                if (OrdersAdvanceMoney.Count == 0)
                {
                    viewModel.DueDate = model.OrderBill.GetDueDate1().ToString("yyyy年MM月dd日");
                }
                else
                {
                    foreach (var item in OrdersAdvanceMoney)
                    {
                        viewModel.DueDate = (item.AdvanceTime.AddDays(item.LimitDays) > model.OrderBill.GetDueDate1() ? model.OrderBill.GetDueDate1() : item.AdvanceTime.AddDays(item.LimitDays)).ToString("yyyy年MM月dd日");
                    }
                }
                viewModel.CreateDate = model.OrderBill.CreateDate.ToString();
                viewModel.ClientType = model.OrderBill.Client.ClientType;

                var OrderBillFile = model.OrderBillFile;

                viewModel.FileID = OrderBillFile?.ID;
                viewModel.FileStatus = OrderBillFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                                        OrderBillFile.FileStatus.GetDescription();
                viewModel.FileName = OrderBillFile == null ? "" : OrderBillFile.Name;
                // viewModel.Url = OrderBillFile == null ? "" : OrderBillFile.Url;
                // viewModel.FileStatusValue = OrderBillFile == null ? Needs.Wl.Models.Enums.OrderFileStatus.NotUpload : OrderBillFile.FileStatus;
                viewModel.FileStatusValue = OrderBillFile == null ? OrderFileStatus.NotUpload : OrderBillFile.FileStatus;
                // viewModel.Url = FileDirectory.Current.FileServerUrl + "/" + OrderBillFile?.Url.ToUrl();
                //判断是否从中心库读取文件
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                if (OrderBillFile != null && DateTime.Compare(OrderBillFile.CreateDate, t1) > 0)
                    viewModel.Url = FileDirectory.Current.PvDataFileUrl + "/" + OrderBillFile?.Url.ToUrl();
                else
                    viewModel.Url = FileDirectory.Current.FileServerUrl + "/" + OrderBillFile?.Url.ToUrl();
                viewModel.summaryTotalPrice = model.BillTotalPrice;
                viewModel.summaryTotalCNYPrice = model.BillTotalCNYPrice;
                viewModel.summaryTotalCNYPrice1 = AdvanceMoney;//model.BillTotalCNYPrice;
                viewModel.summaryTotalTariff = model.BillTotalTariff;
                viewModel.summaryTotalExciseTax = model.BillTotalExciseTax;
                viewModel.summaryTotalAddedValueTax = model.BillTotalAddedValueTax;
                viewModel.summaryTotalAgencyFee = model.BillTotalAgencyFee;
                viewModel.summaryTotalIncidentalFee = model.BillTotalIncidentalFee;

                viewModel.summaryPay = model.BillTotalTaxAndFee;
                viewModel.summaryPayAmount = model.BillTotalDeclarePrice;
                viewModel.summaryTotalQty1 = model.BillTotalQty;

                viewModel.CreateDate = model.MainOrder.CreateDate.ToString("yyyy-MM-dd HH:mm");
                #endregion

                return viewModel;
            }
        }

        private MainOrderBillStander getModelStander(string id)
        {
            var Orders = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                  && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                         .ToList();

            var purchaser = PurchaserContext.Current;
            if (Orders.Count == 0)
            {
                return null;
            }
            else
            {
                MainOrderBillStander mainOrderBillStander = new MainOrderBillStander(purchaser, Orders);

                return mainOrderBillStander;
            }
        }

    }
}
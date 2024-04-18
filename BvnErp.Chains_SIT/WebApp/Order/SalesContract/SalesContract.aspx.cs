using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.SalesContract
{
    /// <summary>
    /// 国内销售合同--增值税开票
    /// </summary>
    public partial class SalesContract : Uc.PageBase
    {
        Purchaser sellerXDT = PurchaserContext.Current;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;
            string ReplaceSingleQuotes = "这里是一个单引号";
            this.Model.ReplaceSingleQuotes = ReplaceSingleQuotes;
            var id = Request.QueryString["ID"];
            //取第一个订单
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SalesContractOrderView.Where(x => x.MainOrderID == id).ToArray();

            var orderIDs = orders.Select(t => t.ID).ToList();

            if (orderIDs.Count == 0)
            {
                this.Model.IsShowSalesContract = false;
            }
            else
            {
                //协议信息
                var order = orders.FirstOrDefault();

                //订单客户
                var client = order.Client;

                //客户补充协议
                var agreement = client.Agreement;

                if (agreement.InvoiceType == InvoiceType.Full)
                {
                    this.Model.IsShowSalesContract = true;

                    //基础信息
                    var model = new Needs.Ccs.Services.Models.SalesContract
                    {
                        ID = id,
                        SalesDate = order.CreateDate,
                        Buyer = new InvoiceBaseInfo
                        {
                            Title = client.Invoice.Title,
                            Address = client.Invoice.Address,
                            BankName = client.Invoice.BankName,
                            BankAccount = client.Invoice.BankAccount,
                            Tel = client.Invoice.Tel
                        },
                        Seller = new InvoiceBaseInfo
                        {
                            Title = sellerXDT.CompanyName,
                            Address = sellerXDT.Address,
                            BankName = sellerXDT.BankName,
                            BankAccount = sellerXDT.AccountId,
                            Tel = sellerXDT.Tel
                        },
                        InvoiceType = agreement.InvoiceType
                    };

                    //型号信息
                    var salesItems = new List<ContractItem>();

                    var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceOrderItem.Where(x => orderIDs.Contains(x.OrderID));
                    InvoiceItemAmountCalc calc = new InvoiceItemAmountCalc(orderIDs);
                    List<InvoiceItemAmountHelp> helper = calc.AmountResult();
                    var units = Needs.Wl.Admin.Plat.AdminPlat.Units.ToList();

                    foreach (var item in orderItem)
                    {
                        var sale = new ContractItem
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

                    this.Model.SalesContract = model.Json();

                    //销售合同文件
                    var files = new CenterLinkXDTFilesTopView().Where(t => t.MainOrderID == id && t.Status != Status.Delete && t.FileType == Needs.Ccs.Services.Enums.FileType.SalesContract).ToList();
                    var hasFile = files.Count < 1;
                    var file = files.FirstOrDefault();
                    this.Model.FileInfo = new
                    {
                        FileID = file?.ID,
                        FileStatus = hasFile ? OrderFileStatus.NotUpload.GetDescription() : file.FileStatus.GetDescription(),
                        FileName = hasFile ? " " : file.Name,
                        FileStatusValue = hasFile ? OrderFileStatus.NotUpload : file.FileStatus,
                        Url = hasFile ? null : FileDirectory.Current.PvDataFileUrl + "/" + file?.Url.ToUrl(),
                        SealUrl = "../../" + PurchaserContext.Current.SealUrl.ToUrl(),
                    }.Json();

                }
                else {
                    this.Model.IsShowSalesContract = false;
                }
            }





            

          

        }

        /// <summary>
        /// 导出销售合同
        /// </summary>
        protected void ExportSalesContract()
        {
            try
            {
                var id = Request.Form["ID"];

                //取第一个订单
                var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SalesContractOrderView.Where(x => x.MainOrderID == id).ToArray();

                var orderIDs = orders.Select(t => t.ID).ToList();
                var order = orders.FirstOrDefault();
                //订单客户
                var client = order.Client;
                //客户补充协议
                var agreement = client.Agreement;

                //基础信息
                var model = new Needs.Ccs.Services.Models.SalesContract
                {
                    ID = id,
                    SalesDate = order.CreateDate,
                    Buyer = new InvoiceBaseInfo
                    {
                        Title = client.Invoice.Title,
                        Address = client.Invoice.Address,
                        BankName = client.Invoice.BankName,
                        BankAccount = client.Invoice.BankAccount,
                        Tel = client.Invoice.Tel
                    },
                    Seller = new InvoiceBaseInfo
                    {
                        Title = sellerXDT.CompanyName,
                        Address = sellerXDT.Address,
                        BankName = sellerXDT.BankName,
                        BankAccount = sellerXDT.AccountId,
                        Tel = sellerXDT.Tel,
                        SealUrl = sellerXDT.SealUrl
                    },
                    InvoiceType = agreement.InvoiceType
                };

                //型号信息
                var salesItems = new List<ContractItem>();

                var orderItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceOrderItem.Where(x => orderIDs.Contains(x.OrderID));
                InvoiceItemAmountCalc calc = new InvoiceItemAmountCalc(orderIDs);
                List<InvoiceItemAmountHelp> helper = calc.AmountResult();
                var units = Needs.Wl.Admin.Plat.AdminPlat.Units.ToList();

                foreach (var item in orderItem)
                {
                    var sale = new ContractItem
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


                Response.Write((new { success = true, message = "导出成功", url = fileDic.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        protected void ApproveSalesContract()
        {
            try
            {
                var id = Request.Form["ID"];

                var file = new CenterLinkXDTFilesTopView().Where(t => t.MainOrderID == id && t.Status != Status.Delete && t.FileType == Needs.Ccs.Services.Enums.FileType.SalesContract).FirstOrDefault();

                var model = new Needs.Ccs.Services.Models.SalesContract();
                model.ID = id;
                model.Approve(file.ID);

                Response.Write((new
                {
                    success = true,
                    message = "审核成功",
                    data = new
                    {
                        ID = id,
                        Name = file.Name,
                        FileStatus = OrderFileStatus.Audited.GetDescription()
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单审核销售合同通知代仓储失败");
                Response.Write((new { success = false, message = "审核失败：" + ex.Message }).Json());
            }

        }

        /// <summary>
        /// 上传销售合同
        /// </summary>
        protected void UploadSalesContract()
        {
            try
            {
                var id = Request.Form["ID"];
                var fileID = Request.Form["FileID"];
                var file = Request.Files["uploadFile"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //文件保存
                string fileName = file.FileName.ReName();

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);

                Needs.Ccs.Services.Models.MainOrderFile salesContract = new Needs.Ccs.Services.Models.MainOrderFile();
                if (!string.IsNullOrEmpty(fileID))
                {
                    var datafiles = new Needs.Ccs.Services.Views.CenterLinkXDTFilesTopView().Where(x => x.ID == fileID).ToArray();
                    salesContract.ID = fileID;
                    salesContract.Abandon();
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, fileID);

                }

                #region 订单文件保存到中心文件库
                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = file.FileName, WsOrderID = id, AdminID = ErmAdminID, Status = FileDescriptionStatus.Approved };
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.SalesContract;
                //本地文件上传到服务器
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + fileDic.VirtualPath, centerType, dic);
                string[] ID = { result[0].FileID };
                new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                #endregion

                Response.Write((new
                {
                    success = true,
                    message = "上传成功",
                    data = new
                    {
                        ID = result[0].FileID,
                        Name = result[0].FileName,
                        FileStatus = OrderFileStatus.Audited.GetDescription(),
                        Url = result[0].Url.ToUrl()
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败：" + ex.Message }).Json());
            }

        }
    }
}
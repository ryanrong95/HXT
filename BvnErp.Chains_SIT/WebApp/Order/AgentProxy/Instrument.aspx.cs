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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Order.AgentProxy
{
    /// <summary>
    /// 代理报关委托书
    /// </summary>
    public partial class Instrument : Uc.PageBase
    {
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
            var Orders = new Orders2View().OrderBy(item => item.ID).Where(item => item.MainOrderID == id && item.OrderStatus != OrderStatus.Canceled && item.OrderStatus != OrderStatus.Returned).ToList();
            bool isShow = true;
            if (Orders.Count == 0)
            {
                isShow = false;
            }
            foreach (var order in Orders)
            {
                //如果订单尚未完成归类，产品项缺少关税率，不能生成代理报关委托书
                if (order.OrderStatus < OrderStatus.Classified)
                {
                    isShow = false;
                }
            }

            if (isShow)
            {
                this.Model.IsShowInstrument = true;
                var purchaser = PurchaserContext.Current;
                var vendor = new VendorContext(VendorContextInitParam.Instrument, Orders.FirstOrDefault().ID, "CaiWu").Current1;

                var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MainOrderAgentProxies[Orders.FirstOrDefault().ID];
                MainOrderAgentProxyViewModel model = new MainOrderAgentProxyViewModel();
                model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                model.Orders = Orders;
                var units = Needs.Wl.Admin.Plat.AdminPlat.Units.ToList();
                var types = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.ToList();
                var totalGwt = model.Items.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value.ToRound(2);
                var order = Orders.FirstOrDefault();
                var t1 = Convert.ToDateTime(FileDirectory.Current.IsChainsDate);
                 //var t2 = order.CreateDate;
                this.Model.Instrument = new
                {
                    ID = order.MainOrderID,
                    ContractNo = order.MainOrderID,
                    ClientName = agentProxy.Client.Company.Name,
                    ClientAddress = agentProxy.Client.Company.Address,
                    ClientContact = agentProxy.Client.Company.Contact.Name,
                    ClientTel = agentProxy.Client.Company.Contact.Mobile,
                    AgentName = purchaser.CompanyName,
                    Company = vendor.CompanyName,
                    Address = vendor.Address,
                    Contact = vendor.Contact,
                    Tel = vendor.Tel,
                    ShortName = purchaser.ShortName,
                    Currency = agentProxy.Currency,
                    CreateDate = agentProxy.CreateDate.ToString(),
                    WrapType = types.Where(t => t.Code == agentProxy.WarpType).FirstOrDefault()?.Name,
                    PackNo = model.Orders.Sum(t => t.PackNo),
                    TotalGwt = totalGwt == 0M ? null : totalGwt.ToString("0.##") + " KG",
                    //代理报关委托书文件
                    FileID = agentProxy.MainFile?.ID,
                    FileStatus = agentProxy.MainFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                              agentProxy.MainFile.FileStatus.GetDescription(),
                    FileName = agentProxy.MainFile == null ? " " : agentProxy.MainFile.Name,
                    FileStatusValue = agentProxy.MainFile == null ? OrderFileStatus.NotUpload : agentProxy.MainFile.FileStatus,
                    Url = agentProxy.MainFile == null?null: DateTime.Compare(agentProxy.MainFile.CreateDate, t1) > 0 ? FileDirectory.Current.PvDataFileUrl + "/" + agentProxy.MainFile?.Url.ToUrl() :
                      FileDirectory.Current.FileServerUrl + "/" + agentProxy.MainFile?.Url.ToUrl(),
                    //Url = FileDirectory.Current.FileServerUrl + "/" + agentProxy.MainFile?.Url.ToUrl(),
                    SealUrl = "../../" + PurchaserContext.Current.SealUrl.ToUrl(),

                    //产品信息
                    Products = model.Items.Select(item => new
                    {
                        Batch = item.Batch==null ? string.Empty: item.Batch.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes),
                        Name = item.Category?.Name ?? item.Name,
                        Manufacturer = item.Manufacturer.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes),
                        Model = item.Model.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes),
                        Origin = item.Origin ?? "",
                        Quantity = item.Quantity,
                        Unit = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit,
                        UnitPrice = item.UnitPrice.ToString("0.0000"),
                        TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                        TariffRate = item.ImportTax?.Rate.ToString("0.0000")
                    })
                }.Json();
            }
            else
            {
                this.Model.IsShowInstrument = false;
            }



        }

        /// <summary>
        /// 加载委托书数据
        /// </summary>
        protected void LoadDataOld()
        {
            #region
            var id = Request.QueryString["ID"];
            var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderAgentProxies[id];
            var purchaser = PurchaserContext.Current;
            var vendor = new VendorContext(VendorContextInitParam.OrderID, id).Current1;

            string ReplaceQuotes = "这里是一个双引号";
            this.Model.ReplaceQuotes = ReplaceQuotes;
            string ReplaceSingleQuotes = "这里是一个单引号";
            this.Model.ReplaceSingleQuotes = ReplaceSingleQuotes;

            //如果订单尚未完成归类，产品项缺少关税率，不能生成代理报关委托书
            if (agentProxy.OrderStatus < OrderStatus.Classified)
            {
                this.Model.IsShowInstrument = false;
            }
            else
            {
                this.Model.IsShowInstrument = true;

                var units = Needs.Wl.Admin.Plat.AdminPlat.Units.ToList();
                var types = Needs.Wl.Admin.Plat.AdminPlat.BasePackType.ToList();
                var totalGwt = agentProxy.Items.Where(item => item.GrossWeight != null).Select(item => item.GrossWeight).Sum().Value.ToRound(2);

                this.Model.Instrument = new
                {
                    ID = agentProxy.ID,
                    ContractNo = agentProxy.ID,
                    ClientName = agentProxy.Client.Company.Name,
                    ClientAddress = agentProxy.Client.Company.Address,
                    ClientContact = agentProxy.Client.Company.Contact.Name,
                    ClientTel = agentProxy.Client.Company.Contact.Mobile,
                    AgentName = purchaser.CompanyName,
                    Company = vendor.CompanyName,
                    Address = vendor.Address,
                    Contact = vendor.Contact,
                    Tel = vendor.Tel,
                    ShortName = purchaser.ShortName,
                    Currency = agentProxy.Currency,
                    CreateDate = agentProxy.CreateDate.ToString(),
                    WrapType = types.Where(t => t.Code == agentProxy.WarpType).FirstOrDefault()?.Name,
                    PackNo = agentProxy.PackNo,
                    TotalGwt = totalGwt == 0M ? null : totalGwt.ToString("0.##") + " KG",
                    //代理报关委托书文件
                    FileID = agentProxy.File?.ID,
                    FileStatus = agentProxy.File == null ? OrderFileStatus.NotUpload.GetDescription() :
                                  agentProxy.File.FileStatus.GetDescription(),
                    FileName = agentProxy.File == null ? " " : agentProxy.File.Name,
                    FileStatusValue = agentProxy.File == null ? OrderFileStatus.NotUpload : agentProxy.File.FileStatus,
                    Url = FileDirectory.Current.FileServerUrl + "/" + agentProxy.File?.Url.ToUrl(),
                    SealUrl = "../../" + PurchaserContext.Current.SealUrl.ToUrl(),

                    //产品信息
                    Products = agentProxy.Items.Select(item => new
                    {
                        Batch = item.Batch ?? string.Empty,
                        Name = item.Category?.Name ?? item.Name,
                        Manufacturer = item.Manufacturer.Replace("\'", ReplaceSingleQuotes),
                        Model = item.Model.Replace("\"", ReplaceQuotes).Replace("\'", ReplaceSingleQuotes),
                        Origin = item.Origin ?? "",
                        Quantity = item.Quantity,
                        Unit = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name ?? item.Unit,
                        UnitPrice = item.UnitPrice.ToString("0.0000"),
                        TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                        TariffRate = item.ImportTax?.Rate.ToString("0.0000")
                    })
                }.Json();
            }
            #endregion
        }

        /// <summary>
        /// 导出委托书
        /// </summary>
        protected void ExportInstrument()
        {
            try
            {
                var id = Request.Form["ID"];
                var Orders = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus != OrderStatus.Returned && item.OrderStatus != OrderStatus.Canceled).ToList();
                var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MainOrderAgentProxies[Orders.FirstOrDefault().ID];

                //保存文件
                string fileName = "委托书" + DateTime.Now.ToString("MMddHHmmss") + ".pdf";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                if (agentProxy.Client.ClientType == ClientType.External)
                {
                    MainOrderAgentProxyViewModel model = new MainOrderAgentProxyViewModel();
                    model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                    model.Orders = Orders;
                    var order = Orders.FirstOrDefault();
                    model.ID = order.MainOrderID;
                    model.Client = agentProxy.Client;
                    model.PackNo = Orders.Sum(t => t.PackNo);
                    model.WarpType = order.WarpType;
                    model.Currency = order.Currency;
                    model.SaveAs(fileDic.FilePath);
                }
                else
                {
                    //itextsharp生成，超过10页
                    AgentProxyToPdf model = new AgentProxyToPdf();
                    model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                    model.Orders = Orders;
                    var order = Orders.FirstOrDefault();
                    model.ID = order.MainOrderID;
                    model.Client = agentProxy.Client;
                    model.PackNo = Orders.Sum(t => t.PackNo);
                    model.WarpType = order.WarpType;
                    model.Currency = order.Currency;
                    model.SaveAs(fileDic.FilePath);
                }

                Response.Write((new { success = true, message = "导出成功", url = fileDic.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出委托书
        /// </summary>
        protected void ExportInstrumentOld()
        {
            try
            {
                var id = Request.Form["ID"];
                var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderAgentProxies[id];

                //保存文件
                string fileName = DateTime.Now.Ticks + ".pdf";
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                var vendor = new VendorContext(VendorContextInitParam.OrderID, agentProxy.Order.ID).Current1;

                agentProxy.SaveAs(fileDic.FilePath, vendor);

                Response.Write((new { success = true, message = "导出成功", url = fileDic.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传委托书
        /// </summary>
        protected void UploadInstrument()
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

                Needs.Ccs.Services.Models.MainOrderFile instrument = new Needs.Ccs.Services.Models.MainOrderFile();
                if (!string.IsNullOrEmpty(fileID))
                {
                    var datafiles = new Needs.Ccs.Services.Views.CenterLinkXDTFilesTopView().Where(x => x.ID == fileID).ToArray();
                    instrument.ID = fileID;
                    instrument.Abandon();
                    new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, fileID);

                }
                //instrument = new Needs.Ccs.Services.Models.MainOrderFile();
                //instrument.MainOrderID = id;
                //instrument.Admin = admin;
                //instrument.Name = file.FileName;
                //instrument.FileType = FileType.AgentTrustInstrument;
                //instrument.FileFormat = file.ContentType;
                //instrument.Url = fileDic.VirtualPath;
                //instrument.FileStatus = OrderFileStatus.Audited;
                //instrument.Enter();

                #region 订单文件保存到中心文件库
                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = file.FileName, WsOrderID = id, AdminID = ErmAdminID, Status= FileDescriptionStatus.Approved };
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.AgentTrustInstrument;
                //本地文件上传到服务器
                var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + fileDic.VirtualPath, centerType, dic);
                string[] ID = { result[0].FileID };
                 new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Approved }, ID);
                #endregion

                //NoticeLog notice = new NoticeLog();
                //notice.NoticeType = SendNoticeType.AgentProxyUploaded;
                //notice.MainID = id;
                //notice.SendNotice();
                ////因为是从管理端上传的代理报关委托书，上传的时候就设为已审批，所以发送两次通知，将消息置为已读
                //notice.SendNotice();

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

        /// <summary>
        /// 审核通过
        /// </summary>
        protected void ApproveInstrument()
        {
            try
            {
                var id = Request.Form["ID"];
                //var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderAgentProxies[id];
                //var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                //agentProxy.Order.SetAdmin(admin);
                //agentProxy.Approve();
                var Orders = new Orders2View().Where(item => item.MainOrderID == id).ToList();
                var agentProxy = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MainOrderAgentProxies[Orders.FirstOrDefault().ID];
                MainOrderAgentProxyViewModel model = new MainOrderAgentProxyViewModel();
                model.MainFile = agentProxy.MainFile;

                model.Approve();

                //NoticeLog notice = new NoticeLog();
                //notice.NoticeType = SendNoticeType.AgentProxyUploaded;
                //notice.MainID = Orders.FirstOrDefault().MainOrderID;
                //notice.Readed = true;
                //notice.SendNotice();

                Response.Write((new
                {
                    success = true,
                    message = "审核成功",
                    data = new
                    {
                        ID = Orders.FirstOrDefault().MainOrderID,
                        Name = agentProxy.MainFile.Name,
                        FileStatus = OrderFileStatus.Audited.GetDescription()
                    }
                }).Json());
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单审核报关委托书通知代仓储失败");
                Response.Write((new { success = false, message = "审核失败：" + ex.Message }).Json());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using System.Data;

namespace Yahv.PvOms.WebApp.LsOrders
{
    public partial class MyList : ErpParticlePage

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            //租赁订单状态
            this.Model.StatusData = ExtendsEnum.ToArray<LsOrderStatus>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
            //开票状态
            this.Model.InvoiceData = ExtendsEnum.ToArray<OrderInvoiceStatus>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        protected object data()
        {
            Expression<Func<LsOrder, bool>> expression = Predicate();
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var enterpriseid = Erp.Current.Leagues?.Current?.EnterpriseID;
            var query = Erp.Current.WsOrder.LsOrder(enterpriseid).GetPageList(page, rows, expression);
            return new
            {
                rows = query.Select(t => new
                {
                    ID = t.ID,
                    CompanyName = t.wsClient.Name,
                    EnterCode = t.wsClient.EnterCode,
                    Status = t.Status.GetDescription(),
                    InvoiceStatus = t.InvoiceStatus.GetDescription(),
                    Creator = t.Admin?.RealName,
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    InheritStatus = t.InheritStatus,
                    IsInvoiced = t.IsInvoiced,
                }).ToArray(),
                total = query.Total,
            }.Json();
        }

        private Expression<Func<LsOrder, bool>> Predicate()
        {
            //查询参数
            var ClientID = Request.QueryString["ID"];
            var OrderID = Request.QueryString["OrderID"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];
            var Status = Request.QueryString["Status"];
            var InvoiceStatus = Request.QueryString["InvoiceStatus"];

            Expression<Func<LsOrder, bool>> predicate = item => true;
            if (!string.IsNullOrWhiteSpace(ClientID))
            {
                ClientID = ClientID.Trim();
                predicate = predicate.And(item => item.ClientID.Contains(ClientID));
            }
            if (!string.IsNullOrWhiteSpace(OrderID))
            {
                OrderID = OrderID.Trim();
                predicate = predicate.And(item => item.ID.Contains(OrderID));
            }
            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate.Trim());
                predicate = predicate.And(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate.Trim()).AddDays(1);
                predicate = predicate.And(item => item.CreateDate < end);
            }
            if (!string.IsNullOrWhiteSpace(Status))
            {
                LsOrderStatus status = (LsOrderStatus)int.Parse(Status);
                predicate = predicate.And(item => item.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(InvoiceStatus))
            {
                OrderInvoiceStatus status = (OrderInvoiceStatus)int.Parse(InvoiceStatus);
                predicate = predicate.And(item => item.InvoiceStatus == status);
            }
            return predicate;
        }

        /// <summary>
        /// 删除租赁订单
        /// </summary>
        protected void Delete()
        {
            try
            {
                string ID = Request.Form["id"];
                var query = Erp.Current.WsOrder.LsOrderAll.Where(item => item.ID == ID).FirstOrDefault();
                if (query == null)
                {
                    throw new Exception("租赁订单不存在");
                }
                //续租订单删除
                query.OperatorID = Erp.Current.ID;
                query.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        #region 合同
        /// <summary>
        /// 导出合同
        /// </summary>
        protected void ExportContract()
        {
            try
            {
                string id = Request.Form["LsOrderID"];

                var fileName = id + ".docx";
                var order = Erp.Current.WsOrder.LsOrderAll.FirstOrDefault(s => s.ID == id);
                order.Export(fileName);
                var fileurl = @"../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "导出成功", fileurl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 上传合同
        /// </summary>
        protected void UploadContract()
        {
            try
            {
                string LsOrderID = Request.Form["LsOrderID"];

                List<dynamic> fileList = new List<dynamic>();
                IList<HttpPostedFile> files = System.Web.HttpContext.Current.Request.Files.GetMultiple("uploadContract");
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            FileDirectory dic = new FileDirectory(file.FileName, FileType.Contract);

                            var dics = new CenterFileDescription
                            {
                                CustomName = file.FileName,
                                AdminID = Erp.Current.ID,
                                LsOrderID = LsOrderID,
                                Type = (int)FileType.Contract,
                            };
                            dic.Save(file, dics);
                        }
                    }
                }
                Response.Write((new { success = true, message = "导入成功，可在租赁订单详情中查看。" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 支付订单能否上传合同
        /// </summary>
        /// <returns> true：未上传，false：已上传</returns>
        protected bool IsContracted()
        {
            var id = Request.Form["ID"];
            var file = new Yahv.Services.Views.CenterFilesTopView()
                .FirstOrDefault(f => f.LsOrderID == id && f.Type == (int)FileType.Contract && f.Status == FileDescriptionStatus.Normal);
            return file == null ? true : false;
        }
        #endregion

        #region 发票

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string id = Request.Form["LsOrderID"];
                var order = Erp.Current.WsOrder.LsOrderAll.FirstOrDefault(t => t.ID == id);
                //开票税率
                var InvoiceTaxRate = order.Contract == null ? 0.06M : order.Contract.InvoiceTaxRate;
                DataTable dt = CreateTable();
                //开票类型为服务费发票  服务费发票无差额
                var items = order.OrderItems.ToArray();
                var invoiceData = order.InvoiceData.FirstOrDefault();
                decimal totalQty, totalTaxOffAmount, totalTaxAmount;
                totalQty = items.Sum(t => t.Quantity * t.Lease.Month);
                totalTaxOffAmount = Math.Round((items.Sum(t => (t.UnitPrice / (1 + InvoiceTaxRate)) * t.Quantity * t.Lease.Month)), 4);
                totalTaxAmount = items.Sum(t => t.Quantity * t.Lease.Month * t.UnitPrice);
                for (int i = 0; i < items.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["NO"] = i + 1;
                    dr["Name"] = "*物流辅助服务*服务费";
                    dr["Unit"] = "个数*月数";
                    dr["TaxoffUnitPrice"] = Math.Round((items[i].UnitPrice / (1 + InvoiceTaxRate)), 4);

                    dr["Qty"] = items[i].Quantity * items[i].Lease.Month;
                    dr["TaxOffAmount"] = Math.Round(((items[i].UnitPrice / (1 + InvoiceTaxRate)) * items[i].Quantity * items[i].Lease.Month), 4);
                    dr["TaxPoint"] = InvoiceTaxRate;
                    dr["TaxNumber"] = "3040407040000000000";
                    dr["UnitPriceTax"] = Math.Round(items[i].UnitPrice, 4);
                    dr["TaxAmount"] = items[i].UnitPrice * items[i].Quantity * items[i].Lease.Month;
                    dr["DiffAmount"] = 0.00;
                    dr["OrderNo"] = id;
                    dr["ModelClassifiyInfo"] = "*物流辅助服务*服务费";

                    dr["CompanyName"] = order.wsClient.Name;
                    dr["Uscc"] = order.wsClient.Uscc;
                    dr["AddressPhone"] = order.wsClient.RegAddress;
                    dr["BankInfo"] = invoiceData.Bank + "/" + invoiceData.Account;

                    dr["RecipientCompanyName"] = invoiceData.CompanyName;
                    dr["RecipientAddress"] = invoiceData.Address;
                    dr["RecipientName"] = invoiceData.Name;
                    dr["RecipientPhone"] = invoiceData.Mobile ?? invoiceData.Tel;

                    dr["TotalQty"] = totalQty;
                    dr["TotalTaxOffAmount"] = totalTaxOffAmount;
                    dr["TotalTaxAmount"] = totalTaxAmount;
                    dr["TotalDiffAmount"] = 0.00M;

                    dr["Remark"] = "";
                    dt.Rows.Add(dr);
                }

                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.Invoice);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                Utils.Npoi.NPOIHelper.InvoiceInfoExcel(dt, filePath);

                var fileUrl = @"../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "申请成功", fileUrl }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "导出失败:" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出xml
        /// </summary>
        protected void ExportXml()
        {
            try
            {
                string id = Request.Form["LsOrderID"];
                var data = Erp.Current.WsOrder.LsOrderAll.Where(t => t.ID == id).AsEnumerable();
                Kp kp = new Kp(data);
                string xmlResult = kp.Xml(System.Text.Encoding.GetEncoding("GBK"));

                //保存xml文件地址
                var fileName = id + ".xml";
                FileDirectory fileDir = new FileDirectory(fileName, FileType.Invoice);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                Utils.Npoi.NPOIHelper.InvoiceInfoXml(xmlResult, filePath);
                var fileUrl = @"../Files/Download/" + fileName;
                Response.Write((new { success = true, message = "申请成功", fileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message, }).Json());
            }
        }

        /// <summary>
        /// 上传发票
        /// </summary>
        protected void UploadInvoice()
        {
            try
            {
                string LsOrderID = Request.Form["LsOrderID"];

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
                            Yahv.PvWsOrder.Services.Common.FileDirectory dic = new Yahv.PvWsOrder.Services.Common.FileDirectory(file.FileName, FileType.Invoice);

                            var dics = new CenterFileDescription
                            {
                                CustomName = file.FileName,
                                AdminID = Erp.Current.ID,
                                LsOrderID = LsOrderID,
                                Type = (int)FileType.Contract,
                            };
                            dic.Save(file, dics);
                        }
                    }
                }
                Response.Write((new { success = true, message = "导入成功，可在租赁订单详情中查看。" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败：" + ex.Message }).Json());
            }
        }

        private DataTable CreateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO"); //序号
            dt.Columns.Add("Name"); //品名
            dt.Columns.Add("Model"); //规格型号
            dt.Columns.Add("Unit"); //单位
            dt.Columns.Add("TaxoffUnitPrice"); //不含税单价
            dt.Columns.Add("Qty"); //数量
            dt.Columns.Add("TaxOffAmount"); //不含税金额
            dt.Columns.Add("TaxPoint"); //税率
            dt.Columns.Add("TaxNumber"); //税收分类编码
            dt.Columns.Add("UnitPriceTax"); //含税单价
            dt.Columns.Add("TaxAmount"); //含税金额
            dt.Columns.Add("DiffAmount"); //差额
            dt.Columns.Add("OrderNo");  //订单号合同号
            dt.Columns.Add("ModelClassifiyInfo");  //型号信息分类
            //开票资料
            dt.Columns.Add("CompanyName"); //购货单位名称
            dt.Columns.Add("Uscc");  //纳税人识别号
            dt.Columns.Add("AddressPhone");  //地址、电话
            dt.Columns.Add("BankInfo"); //开户银行及账号
            //寄票信息
            dt.Columns.Add("RecipientCompanyName");//收票公司名称
            dt.Columns.Add("RecipientAddress"); //收票公司地址
            dt.Columns.Add("RecipientName"); //收件人姓名
            dt.Columns.Add("RecipientPhone"); //收件人电话 

            //合计信息
            dt.Columns.Add("TotalQty");//合计数量
            dt.Columns.Add("TotalTaxOffAmount");//合计不含税金额
            dt.Columns.Add("TotalTaxAmount");//合计含税金额
            dt.Columns.Add("TotalDiffAmount");//合计差额

            //备注
            dt.Columns.Add("Remark");
            return dt;
        }
        #endregion
    }
}
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Temp
{
    public partial class ExportTest : Uc.PageBase
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
            var id = Request.QueryString["ID"];
            var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills[id];
            var units = Needs.Wl.Admin.Plat.AdminPlat.Units.ToList();

            this.Model.Bill = new
            {
                //基本信息
                ID = bill.ID,
                ContractNo = bill.ID,
                ClientName = bill.Client.Company.Name,
                ClientTel = bill.Client.Company.Contact.Tel,
                AgentName = HYComany.CompanyName,
                AgentTel = HYComany.Tel,
                Currency = bill.Currency,
                CreateDate = bill.CreateDate.ToShortDateString(),

                //对账单文件
                FileID = bill.File?.ID,
                FileStatus = bill.File == null ? OrderFileStatus.NotUpload.GetDescription() :
                             bill.File.Name + " " + bill.File.FileStatus.GetDescription(),
                Url = FileDirectory.Current.FileServerUrl + "/" + bill.File?.Url.ToUrl(),

                //报关商品明细
                Products = bill.Items.Select(item => new
                {
                    Name = item.Category.Name,
                    Model = item.Model,
                    Quantity = item.Quantity,
                    Unit = units.Where(u => u.Code == item.Unit).FirstOrDefault()?.Name,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    TariffRate = item.ImportTax.Rate,
                    DeclareValue = (item.TotalPrice * bill.ProductFeeExchangeRate),
                }),

                //费用明细
                Fees = bill.GetFeeDetails()
            }.Json();
        }

        ///// <summary>
        ///// 导出对账单
        ///// </summary>
        //protected void ExportBill()
        //{
        //    try
        //    {
        //        var id = Request.Form["ID"];
        //        var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills[id];

        //        //保存文件
        //        string fileName = DateTime.Now.Ticks + ".pdf";
        //        FileDirectory fileDic = new FileDirectory(fileName);
        //        fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
        //        fileDic.CreateDataDirectory();
        //        bill.SaveAs(fileDic.FilePath);

        //        Response.Write((new { success = true, message = "导出成功", url = fileDic.FileUrl }).Json());
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
        //    }
        //}

        /// <summary>
        /// 导出对账单
        /// </summary>
        protected void ExportBill()
        {
            try
            {
                var id = Request.Form["ID"];
                var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills[id];

                var mStream = bill.SaveAs();
                byte[] bytes = mStream.GetBuffer();

                //HttpResponse httpResponse = HttpContext.Current.Response;
                //httpResponse.Clear();
                //httpResponse.Buffer = true;
                //httpResponse.Charset = Encoding.UTF8.BodyName;
                //httpResponse.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(DateTime.Now.Ticks + ".pdf", Encoding.UTF8));
                //httpResponse.ContentEncoding = Encoding.UTF8;
                //httpResponse.ContentType = "application/vnd.ms-excel; charset=UTF-8";
                //httpResponse.OutputStream.Write(bytes, 0, bytes.Length);
                //httpResponse.End();

                Response.ContentType = "application/pdf";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Charset = Encoding.UTF8.BodyName;
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(DateTime.Now.Ticks + ".pdf", Encoding.UTF8));
                //Response.BinaryWrite(mStream.GetBuffer());
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                //Response.Write((new { success = true, message = "导出成功", Stream = bytes }).Json());
                Response.End();
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

                Needs.Ccs.Services.Models.OrderFile orderBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderFiles[fileID];
                orderBill?.Abandon();
                orderBill = new Needs.Ccs.Services.Models.OrderFile();
                orderBill.ID = fileID;
                orderBill.OrderID = id;
                orderBill.Admin = admin;
                orderBill.Name = file.FileName;
                orderBill.FileType = FileType.OrderBill;
                orderBill.FileFormat = file.ContentType;
                orderBill.Url = fileDic.VirtualPath;
                orderBill.FileStatus = OrderFileStatus.Audited;
                orderBill.Enter();

                Response.Write((new { success = true, message = "上传成功" }).Json());
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
                var bill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderBills[id];
                bill.Approve();

                Response.Write((new { success = true, message = "审核成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "审核失败：" + ex.Message }).Json());
            }
        }
    }
}
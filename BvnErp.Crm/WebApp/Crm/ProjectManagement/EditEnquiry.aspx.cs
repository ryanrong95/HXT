using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.ProjectManagement
{
    public partial class EditEnquiry : Uc.PageBase
    {
        /// <summary>
        /// 询价ID
        /// </summary>
        protected string EnquiryID
        {
            get
            {
                return Request["id"];
            }
        }

        protected string ProductItemID
        {
            get
            {
                return Request["productItemID"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(this.ProductItemID))
                {
                    Response.Write("询价页面处理异常!");
                    Response.End();
                }
                Enquiry enquiry = string.IsNullOrEmpty(this.EnquiryID) ? new Enquiry(this.ProductItemID) : new ProductItemEnquiriesView(this.ProductItemID).FirstOrDefault(item => item.ID == this.EnquiryID);

                this.Model.Enquiry = enquiry;

                this.Model.CurrencyData = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            }
        }


        /// <summary>
        /// 保存（报备信息、送样信息）
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.ProductItemID))
            {
                Response.Write("询价页面处理异常!");
                Response.End();
            }

            string adminID = Needs.Erp.ErpPlot.Current.ID;

            Enquiry enquiry = string.IsNullOrEmpty(this.EnquiryID) ? new Enquiry(this.ProductItemID) : new ProductItemEnquiriesView(this.ProductItemID).FirstOrDefault(item => item.ID == this.EnquiryID);
            if (!string.IsNullOrEmpty(this.EnquiryID) && enquiry == null)
            {
                Response.Write("询价页面处理异常");
                Response.End();
            }
            // 文件保存
            if (OriginUpload.HasFile)
            {
                var filename = SaveFile(OriginUpload.PostedFile);//文件保存
                if (enquiry.Voucher == null)
                {
                    enquiry.Voucher = new ProductItemFile
                    {
                        Name = OriginUpload.PostedFile.FileName,
                        Type = FileType.OriginReply,
                        AdminID = adminID,
                        ProductItemID = this.ProductItemID,
                        SubID = this.EnquiryID,
                        Url = Request.ApplicationPath + "/UploadFiles/" + filename
                    };
                }
                else
                {
                    enquiry.Voucher.SubID = EnquiryID;
                    enquiry.Voucher.Name = OriginUpload.PostedFile.FileName;
                    enquiry.Voucher.Type = FileType.OriginReply;
                    enquiry.Voucher.AdminID = adminID;
                    enquiry.Voucher.Url = Request.ApplicationPath + "/UploadFiles/" + filename;
                }
            }

            enquiry.ReportDate = DateTime.Parse(Request["reportDate"]);
            enquiry.ReplyPrice = decimal.Parse(Request["replyPrice"]);
            enquiry.ReplyDate = DateTime.Parse(Request["replyDate"]);
            enquiry.RFQ = Request["rfq"];
            enquiry.OriginModel = Request["originModel"] ?? "";
            enquiry.MOQ = int.Parse(Request["moq"]);
            enquiry.TaxRate = (decimal)0.13;
            if (!string.IsNullOrEmpty(Request["mpq"]))
            {
                enquiry.MPQ = int.Parse(Request["mpq"]);
            }
            enquiry.Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), Request["currency"]);
            if (!string.IsNullOrEmpty(Request["exchangeRate"]))
            {
                enquiry.ExchangeRate = decimal.Parse(Request["exchangeRate"]);
            }
            if (!string.IsNullOrEmpty(Request["taxRate"]))
            {
                enquiry.TaxRate = decimal.Parse(Request["taxRate"]);
            }
            if (!string.IsNullOrEmpty(Request["tariff"]))
            {
                enquiry.Tariff = decimal.Parse(Request["tariff"]);
            }
            if (!string.IsNullOrEmpty(Request["otherRate"]))
            {
                enquiry.OtherRate = decimal.Parse(Request["otherRate"]);
            }
            if (!string.IsNullOrEmpty(Request["cost"]))
            {
                enquiry.Cost = decimal.Parse(Request["cost"]);
            }
            else {
                enquiry.Cost = enquiry.ReplyPrice * (enquiry.ExchangeRate.HasValue ? enquiry.ExchangeRate : 1) * (1 + enquiry.Tariff) * (1 + enquiry.OtherRate);
            }
            

            enquiry.Validity = DateTime.Parse(Request["validity"]);
            if (!string.IsNullOrEmpty(Request["validityCount"]))
            {
                enquiry.ValidityCount = int.Parse(Request["validityCount"]);
            }
            if (!string.IsNullOrEmpty(Request["salePrice"]))
            {
                enquiry.SalePrice = decimal.Parse(Request["salePrice"]);
            }
            enquiry.Summary = Request["summary"];
            enquiry.Enter();

            Alert("保存成功", Request.Url, true);
        }
        /// <summary>
        /// 保存上传的文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string SaveFile(HttpPostedFile file)
        {
            string filepath = Server.MapPath("~/UploadFiles/");
            string fileName = Guid.NewGuid().ToString("N") + System.IO.Path.GetExtension(file.FileName).ToLower();
            file.SaveAs(filepath + fileName);

            return fileName;
        }
    }
}
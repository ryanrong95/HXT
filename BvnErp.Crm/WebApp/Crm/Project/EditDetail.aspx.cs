using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;

namespace WebApp.Crm.Project
{
    public partial class EditDetail : Uc.PageBase
    {
        private ApplyType ApplyType;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboData();
                LoadData();
            }
        }


        #region 数据初始化加载
        /// <summary>
        /// 加载销售机会数据
        /// </summary>
        protected void LoadData()
        {
            var Itemid = Request.QueryString["ItemID"];
            var projectId = Request.QueryString["ProjectID"];
            var project = new NtErp.Crm.Services.Views.ProjectAlls().SearchByID(projectId);
            var productitem = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByID(Itemid);
            this.Model.Project = new
            {
                project.ClientName,
                project.CompanyName,
                project.Name,
                project.ProductName,
                IndustryName = project.Industry?.Name,
                CurrencyName = project.Currency.GetDescription(),
            }.Json();
            if (productitem != null)
            {
                this.Model.ProductItem = new
                {
                    ItemID = productitem.ID,
                    ItemName = productitem.standardProduct.Name,
                    ItemOrigin = productitem.standardProduct.Origin,
                    ManufactureID = productitem.standardProduct.ManufacturerID,
                    productitem.RefUnitQuantity,
                    productitem.RefQuantity,
                    productitem.RefUnitPrice,
                    productitem.Status,
                    productitem.ExpectRate,
                    productitem.ExpectDate,
                    productitem.ExpectQuantity,
                    productitem.ExpectTotal,
                    productitem.Summary,
                    CompeteManu = productitem.CompeteProduct?.ManufacturerID,
                    CompeteModel = productitem.CompeteProduct?.Name,
                    CompetePrice = productitem.CompeteProduct?.UnitPrice,
                    productitem.SaleAdminID,
                    productitem.AssistantAdiminID,
                    productitem.PurchaseAdminID,
                    productitem.PMAdminID,
                    productitem.FAEAdminID,
                    productitem.ReportDate,
                    IsReport = productitem.IsReport.GetValueOrDefault() ? 1 : 0,
                    SampleType = productitem.Sample?.Type,
                    SampleDate = productitem.Sample?.Date,
                    SampleQuantity = productitem.Sample?.Quantity,
                    SamplePrice = productitem.Sample?.UnitPrice,
                    SampleContactor = productitem.Sample?.Contactor,
                    SamplePhone = productitem.Sample?.Phone,
                    SampleAddress = productitem.Sample?.Address,
                }.Json();
                this.Model.Files = productitem.Files.Json();
                this.Model.IsSample = productitem.IsSample;
            }
            else
            {
                this.Model.ProductItem = productitem.Json();
                this.Model.Files = productitem.Json();
                this.Model.IsSample = false;
            }
        }

        /// <summary>
        /// 下拉框数据加载
        /// </summary>
        protected void LoadComboData()
        {
            var clientID = Request.QueryString["ClientID"];
            this.Model.SaleAdmins = new NtErp.Crm.Services.Views.MapsClientView().GetClientOwner(clientID).Select(item => new { item.ID, item.RealName }).Json();
            this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
            this.Model.IsReport = EnumUtils.ToDictionary<IsProtected>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Status = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.SampleType = EnumUtils.ToDictionary<SampleType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Currency = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Vender = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name })
                .OrderBy(item => item.Name).Json();
        }
        #endregion

        #region 数据保存
        /// <summary>
        /// 数据保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var itemid = Request.QueryString["ItemID"];
            var adminid = Needs.Erp.ErpPlot.Current.ID;
            var product = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByID(itemid) ?? new ProductItem();
            product.RefUnitQuantity = int.Parse(Request.Form["RefUnitQuantity"]);
            product.RefQuantity = int.Parse(Request.Form["RefQuantity"]);
            product.RefUnitPrice = decimal.Parse(Request.Form["RefUnitPrice"]);
            product.Summary = Request.Form["Summary"] ?? string.Empty;
            var Status = (ProductStatus)int.Parse(Request.Form["Status"]);
            if ((Status > ProductStatus.DO) && product.Status != Status)
            {
                ApplyType = (ApplyType)Status;
            }
            else
            {
                product.Status = Status;
                product.ExpectRate = (int)Status;
            }
            if (!string.IsNullOrWhiteSpace(Request.Form["ExpectQuantity"]))
            {
                product.ExpectQuantity = int.Parse(Request.Form["ExpectQuantity"]);
            }
            else
            {
                product.ExpectQuantity = null;
            }
            
            if (!string.IsNullOrWhiteSpace(Request.Form["ExpectDate"]))
            {
                product.ExpectDate = DateTime.Parse(Request.Form["ExpectDate"]);
            }
            else
            {
                product.ExpectDate = null;
            }

            #region 文件保存
            List<ProductItemFile> postfiles = new List<ProductItemFile>();
            if (FileUpload.HasFile)
            {
                var file = product.Files?.SingleOrDefault(item => item.Type == FileType.Item) ?? new ProductItemFile();
                var filename = SaveFile(FileUpload.PostedFile);//文件保存
                file.Name = FileUpload.PostedFile.FileName;
                file.Type = FileType.Item;
                file.AdminID = adminid;
                file.Url = Request.ApplicationPath + "/UploadFiles/" + filename;

                postfiles.Add(file);
            }            

            product.Files = postfiles.ToArray();
            #endregion

            #region 产品数据
            //标准产品,竞争产品初始化
            product.standardProduct = product.standardProduct ?? new StandardProduct();
            product.CompeteProduct = product.CompeteProduct ?? new CompeteProduct();
            product.standardProduct.Name = Request.Form["ItemName"].Trim();
            product.standardProduct.Origin = Request.Form["ItemOrigin"].Trim();
            product.standardProduct.ManufacturerID = Request.Form["ManufactureID"];
            product.standardProduct.Manufacturer = Needs.Underly.FkoFactory<Company>.Create(Request.Form["ManufactureID"]);
            product.CompeteProduct.Name = Request.Form["CompeteModel"];
            product.CompeteProduct.ManufacturerID = Request.Form["CompeteManu"];
            if (!string.IsNullOrWhiteSpace(Request.Form["CompetePrice"]))
            {
                product.CompeteProduct.UnitPrice = decimal.Parse(Request.Form["CompetePrice"]);
            }
            else if(!string.IsNullOrWhiteSpace(product.CompeteProduct.ID))
            {
                product.CompeteProduct.UnitPrice = null;
            }
            #endregion

            //人员信息
            product.SaleAdminID = Request.Form["SaleAdminID"];
            product.AssistantAdiminID = Request.Form["AssistantAdiminID"];
            product.PurchaseAdminID = Request.Form["PurchaseAdminID"];
            product.PMAdminID = Request.Form["PMAdminID"];
            product.FAEAdminID = Request.Form["FAEAdminID"];

            #region 产品拓展数据
            bool isSample = bool.Parse(Request.Form["IsSample"]);
            string sampleType = Request.Form["送样类型"];
            string sampleQuantity = Request.Form["送样数量"];
            string sampleUnitPrice = Request.Form["送样单价"];
            string sampleDate = Request.Form["送样时间"];
            string sampleContactor = Request.Form["送样联系人"];
            string samplePhone = Request.Form["送样联系电话"];
            string sampleAddress = Request.Form["送样联系地址"];

            string sampleString = string.Concat(sampleType + sampleQuantity + sampleUnitPrice + sampleDate + sampleContactor + sampleAddress);
            if (isSample && !string.IsNullOrEmpty(sampleString))
            {
                Sample sample = new Sample(product.ID);
                sample.Type = (SampleType)int.Parse(sampleType);
                sample.Quantity = int.Parse(sampleQuantity);
                sample.UnitPrice = decimal.Parse(sampleUnitPrice);
                sample.Date = DateTime.Parse(sampleDate);
                sample.TotalPrice = sample.Quantity * sample.UnitPrice;
                sample.Contactor = sampleContactor;
                sample.Phone = samplePhone;
                sample.Address = sampleAddress;
                if (product.Sample == null)
                {
                    product.Sample = sample;
                }
                else
                {
                    sample.ID = product.Sample.ID;
                    product.Sample = sample;
                }

            }
            else
            {
                product.Sample = null;
            }            
            #endregion
                        
            product.EnterSuccess += Product_EnterSuccess;             
            product.Enter();
        }

        private void Product_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var product = sender as ProductItem;
            var projectId = Request.QueryString["ProjectID"];
            //绑定项目
            Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.BindingItem(projectId, product);

            new NtErp.Crm.Services.Views.ProjectAlls().SearchByID(projectId).Enter();
            //申请表插入
            if (this.ApplyType == ApplyType.DIApply || this.ApplyType == ApplyType.DWApply || this.ApplyType == ApplyType.MPApply)
            {
                var apply = new Apply();
                apply.MainID = product.ID;
                apply.Type = this.ApplyType;
                apply.Admin = Needs.Underly.FkoFactory<AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
                apply.Summary = "销售机会产品状态申请";
                apply.Status = ApplyStatus.Audting;
                apply.Enter();
            }
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
        #endregion
    }
}
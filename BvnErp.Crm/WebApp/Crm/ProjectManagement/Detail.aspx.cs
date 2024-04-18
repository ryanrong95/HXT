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
    /// <summary>
    /// 销售机会管理产品详情页面
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        /// <summary>
        /// 产品型号ID
        /// </summary>
        protected string ProductItemID
        {
            get
            {
                return Request["id"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var product = new ProductItemsView().GetWith(Needs.Erp.ErpPlot.Current.ID).FirstOrDefault(item => item.ProductItem.ID == ProductItemID);
                //var product = new ProductItemsView().GetAll().FirstOrDefault(item => item.ProductItem.ID == ProductItemID);

                if (product == null)
                {
                    Response.Write("产品详情查看页面处理异常");
                    Response.End();
                }
                else
                {
                    this.Model.Product = product;
                }

                this.Model.SampleTypeData = EnumUtils.ToDictionary<SampleType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            }
        }

        /// <summary>
        /// 询价列表数据集
        /// </summary>
        protected void data()
        {
            this.Paging(new ProductItemEnquiriesView(this.ProductItemID).OrderByDescending(t=>t.UpdateDate), item => new
            {
                ID = item.ID,
                Voucher = item.Voucher == null ? "" : item.Voucher.Name, // 原厂批复凭证名称
                VoucherUrl = item.Voucher == null ? "" : item.Voucher.Url, // 原厂批复凭证地址
                ReplyDate = item.ReplyDate.ToString("yyyy-MM-dd"), // 批复时间
                RFQ = item.RFQ, // 原厂RFQ
                OriginModel = item.OriginModel, // 原厂型号
                MOQ = item.MOQ, // 最小起订量
                MPQ = item.MPQ, // 最小包装量
                ReplyPrice = item.ReplyPrice, // 批复单价
                CurrencyStr = item.Currency.GetDescription(), // 币种
                ExchangeRate = item.ExchangeRate, // 汇率
                TaxRate = item.TaxRate, // 税率
                Tariff = item.Tariff, // 关税点
                OtherRate = item.OtherRate, // 其他附加点
                Cost = item.Cost, // 含税人民币成本价
                Validity = item.Validity.ToString("yyyy-MM-dd"), // 有效时间
                ValidityCount = item.ValidityCount, // 有效数量
                ReportDate=item.ReportDate, // 报备时间
                SalePrice = item.SalePrice, // 参考售价
                Summary = item.Summary // 特殊备注
            });
        }

        /// <summary>
        /// 保存（报备信息、送样信息）
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            var adminID = Needs.Erp.ErpPlot.Current.ID;
            var model = new ProductItemsView().GetWith(adminID).FirstOrDefault(item => item.ProductItem.ID == ProductItemID);

            if (model == null)
            {
                Response.Write("保存处理异常");
                Response.End();
            }

            var product = model.ProductItem;
            // 送样信息
            if (product.Sample == null)
            {
                var sample = new Sample
                {
                    Type = (SampleType)Enum.Parse(typeof(SampleType), Request["sampleType"]),
                    Quantity = int.Parse(Request["sampleQuantity"]),
                    UnitPrice = decimal.Parse(Request["samplePrice"]),
                    Date = DateTime.Parse(Request["sampleDate"]),
                    Contactor = Request["sampleContactor"],
                    Phone = Request["samplePhone"],
                    Address = Request["sampleAddress"]
                };
                product.Sample = sample;
            }
            else
            {
                product.Sample.Type = (SampleType)Enum.Parse(typeof(SampleType), Request["sampleType"]);
                product.Sample.Quantity = int.Parse(Request["sampleQuantity"]);
                product.Sample.UnitPrice = decimal.Parse(Request["samplePrice"]);
                product.Sample.Date = DateTime.Parse(Request["sampleDate"]);
                product.Sample.Contactor = Request["sampleContactor"];
                product.Sample.Phone = Request["samplePhone"];
                product.Sample.Address = Request["sampleAddress"];
            }

            product.Save();

            //Alert("保存成功", Request.Url, true);

            var url = Request.UrlReferrer ?? Request.Url;
            var path = url.OriginalString;
            if (string.IsNullOrWhiteSpace(ProductItemID))
            {
                path = path + "&id=" + ProductItemID;
            }
            Alert("保存成功", path, false);
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
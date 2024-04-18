using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Models.Projects;
using NtErp.Crm.Services.Views.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.ProjectManagement
{
    /// <summary>
    /// 销售机会管理列表页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var clientid = Request["id"];            
            this.Model.ClientName = Request["name"].Json();
            if (!IsPostBack)
            {
                // 销售状态
                Dictionary<string, string> statusData = new Dictionary<string, string>() { { "-1", "全部" } };
                
                this.Model.StatusData = statusData.Concat(EnumUtils.ToDictionary<NtErp.Crm.Services.Enums.ProductStatus>()).Select(item => new
                {
                    text = item.Value,
                    value = item.Key
                }).Json();
                // 报备状态
                this.Model.ReportStatusData = new Dictionary<string, string>() { { "-1", "全部" }, { "true", "已报备" }, { "false", "未报备" } }.Select(item => new
                {
                    text = item.Value,
                    value = item.Key
                }).Json();
                // 送样状态
                this.Model.SampleStatusData = new Dictionary<string, string>() { { "-1", "全部" }, { "true", "已送样" }, { "false", "未送样" } }.Select(item => new
                {
                    text = item.Value,
                    value = item.Key
                }).Json();
                // 送样类型
                var sampleTypeData = new Dictionary<string, string>() { { "-1", "全部" } };
                this.Model.SampleTypeData = sampleTypeData.Concat(EnumUtils.ToDictionary<NtErp.Crm.Services.Models.SampleType>()).Select(item => new
                {
                    text = item.Value,
                    value = item.Key
                }).Json();
            }
        }

        /// <summary>
        /// 列表数据集
        /// </summary>
        protected void data()
        {
            // 搜索条件
            Expression<Func<ProjectProductItem, bool>> predicate = query();

            var name = Request["name"];
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(item => item.Project.Client.Name.Contains(name));
            }

            this.Paging(new ProductItemsView().GetWith(Needs.Erp.ErpPlot.Current.ID).Where(predicate).OrderByDescending(t => t.Project.UpdateDate), item => new
            //this.Paging(new ProductItemsView().GetAll().Where(predicate), item => new
            {
                ProductItemID = item.ProductItem.ID,
                ClientID = item.Project.ClientID,
                ClientName = item.Project.Client.Name, // 客户名称
                ProjectName = item.Project.Name, // 项目名称
                ProjectDate = string.Concat(item.Project.StartDate?.ToString("yyyy-MM-dd"), " - ", item.Project.EndDate?.ToString("yyyy-MM-dd")), // 项目起止日期
                Contactor = item.Project.Contactor, // 联系人
                Phone = item.Project.Phone, // 联系电话
                CreateDate = item.Project.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                UpdateDate = item.Project.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ProductName = item.Project.ProductName, //产品全称
                Currency = item.Project.Currency.GetDescription(), // 币种
                CompanyName = item.Project.Company.Name, // 我方公司
                IndustryName = item.Project.Industry.Name, // 行业
                ProjectType = item.Project.Type.GetDescription(), // 机会类型

                Name = item.ProductItem.StandardProduct.Name, // 型号
                FullName = item.ProductItem.StandardProduct.Origin, // 型号全称
                Manufacturer = item.ProductItem.StandardProduct.Manufacturer.Name, // 品牌
                StatusStr = item.ProductItem.Status.GetDescription(), // 销售状态
                RefUnitQuantity = item.ProductItem.RefUnitQuantity, // 单机用量
                RefQuantity = item.ProductItem.RefQuantity, // 项目用量
                RefUnitPrice = item.ProductItem.RefUnitPrice, // 参考单价（CNY）
                ExpectRate = item.ProductItem.ExpectRate, // 成交概率
                ExpectQuantity = item.ProductItem.ExpectQuantity, // 预计成交量
                ExpectTotal = item.ProductItem.ExpectTotal, // 预计成交额(CNY)
                CompeteName = item.ProductItem.CompeteProduct?.Name, // 竞品型号
                CompeteManufacturer = item.ProductItem.CompeteProduct?.ManufacturerID, // 竞品厂商
                CompeteUnitPrice = item.ProductItem.CompeteProduct?.Currency, // 竞品单价
                Voucher = item.ProductItem.Voucher?.Name, // 凭证
                VoucherUrl = item.ProductItem.Voucher?.Url, // 凭证Url
                Summary = item.ProductItem.Summary, // 备注

                Sale = item.ProductItem.SaleAdmin?.RealName, // 销售
                Assistant = item.ProductItem.AssistantAdmin?.RealName, // 销售助理
                PM = item.ProductItem.PMAdmin?.RealName, // PM
                Purchaser = item.ProductItem.PurChaseAdmin?.RealName, // 采购助理
                FAE = item.ProductItem.FAEAdmin?.RealName, // FAE

                SampleStatus = item.ProductItem.Sample == null ? "否" : "是", // 是否送样
                SampleType = item.ProductItem.Sample?.Type.GetDescription(), // 送样类型
                SampleDate = item.ProductItem.Sample?.Date.ToString("yyyy-MM-dd"), // 送样时间
                SampleQuantity = item.ProductItem.Sample?.Quantity, // 送样数量
                SampleUnitPrice = item.ProductItem.Sample?.UnitPrice, // 送样单价
                SampleTotalPrice = item.ProductItem.Sample?.TotalPrice, // 送样总金额
                SampleContactor = item.ProductItem.Sample?.Contactor, // 联系人
                SamplePhone = item.ProductItem.Sample?.Phone, // 联系电话
                SampleAddress = item.ProductItem.Sample?.Address, // 送样地址

                EnquiryReportDate = item.ProductItem.Enquiry?.ReportDate.ToString("yyyy-MM-dd"), // 批复时间
                EnquiryVoucher = item.ProductItem.Enquiry?.Voucher?.Name, // 原厂批复凭证
                EnquiryVoucherUrl = item.ProductItem.Enquiry?.Voucher?.Url, // 原厂批复凭证
                EnquiryReplyDate = item.ProductItem.Enquiry?.ReplyDate.ToString("yyyy-MM-dd"), // 批复时间
                EnquiryRFQ = item.ProductItem.Enquiry?.RFQ, // 原厂RFQ
                EnquiryOriginModel = item.ProductItem.Enquiry?.OriginModel, // 原厂型号
                EnquiryMOQ = item.ProductItem.Enquiry?.MOQ, // 最小起订量
                EnquiryMPQ = item.ProductItem.Enquiry?.MPQ, // 最小包装量
                EnquiryReplyPrice = item.ProductItem.Enquiry?.ReplyPrice, // 批复单价
                EnquiryCurrency = item.ProductItem.Enquiry?.Currency.GetDescription(), // 币种
                EnquiryExchangeRate = item.ProductItem.Enquiry?.ExchangeRate, // 汇率
                EnquiryTaxRate = item.ProductItem.Enquiry?.TaxRate, // 税率
                EnquiryTariff = item.ProductItem.Enquiry?.Tariff, // 关税点
                EnquiryOtherRate = item.ProductItem.Enquiry?.OtherRate, // 其他附加点
                EnquiryCost = item.ProductItem.Enquiry?.Cost, // 含税人民币成本价
                EnquriyValidity = item.ProductItem.Enquiry?.Validity.ToString("yyyy-MM-dd"), // 有效时间
                EnquiryValidityCount = item.ProductItem.Enquiry?.ValidityCount, // 有效数量
                EnquirySalePrice = item.ProductItem.Enquiry?.SalePrice, // 参考售价
                EnquirySummary = item.ProductItem.Enquiry?.Summary // 特殊备注

            });
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<ProjectProductItem, bool>> query()
        {
            string mf = Request["s_manufacturer"];
            string product = Request["s_name"];
            //string reportStatus = Request["s_reportStatus"];
            string sampleStatus = Request["s_sampleStatus"];
            string statusStr = Request["s_status"];
            string adminName = Request["s_adminName"];
            string clientName = Request["s_clientName"];

            Expression<Func<ProjectProductItem, bool>> predicate = item => true; // 条件拼接

            if (!string.IsNullOrEmpty(mf))
            {
                predicate = predicate.And(item => item.ProductItem.StandardProduct.Manufacturer.Name.ToUpper().StartsWith(mf.ToUpper()));
            }

            if (!string.IsNullOrEmpty(product))
            {
                predicate = predicate.And(item => item.ProductItem.StandardProduct.Name.ToUpper().StartsWith(product.ToUpper()));
            }
            if (!string.IsNullOrEmpty(adminName))
            {
                predicate = predicate.And(item => item.ProductItem.AssistantAdmin.RealName.Contains(adminName) || item.ProductItem.FAEAdmin.RealName.Contains(adminName) || item.ProductItem.SaleAdmin.RealName.Contains(adminName) || item.ProductItem.FAEAdmin.RealName.Contains(adminName) || item.ProductItem.PMAdmin.RealName.Contains(adminName));
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                predicate = predicate.And(item => item.Project.Client.Name.Contains(clientName));
            }
            //bool isReport;
            //if (reportStatus != "-1" && bool.TryParse(reportStatus, out isReport))
            //{
            //    predicate = predicate.And(item => item.ProductItem.IsReport == isReport);
            //}

            bool isSample;
            if (sampleStatus != "-1" && bool.TryParse(sampleStatus, out isSample))
            {
                predicate = predicate.And(item => item.ProductItem.IsSample == isSample);
            }

            NtErp.Crm.Services.Enums.ProductStatus status;
            if (statusStr != "-1" && Enum.TryParse(statusStr, out status))
            {
                predicate = predicate.And(item => item.ProductItem.Status == status);
            }

            return predicate;
        }

    }
}
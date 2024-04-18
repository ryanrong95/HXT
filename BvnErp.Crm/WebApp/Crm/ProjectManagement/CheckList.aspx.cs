using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.ProjectManagement
{
    public partial class CheckList : Uc.PageBase
    {
        private static List<ProjectExcelData> Datas;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string fileName = Request["filename"];
                var path = Path.Combine(Server.MapPath("~/UploadFiles/"), fileName);
                var projectWithProducts = ExcelHelper.Current.ExcelToLinq(path).OrderBy(item => item.项目名称).ToList();
                Datas = CheckData(projectWithProducts);
            }
        }

        /// <summary>
        /// 加载经过校验的数据
        /// </summary>
        protected void data()
        {
            var linq = Datas?.Select(item => new
            {
                Message = item.ExcelProject.Message,
                ClientName = item.ExcelProject.客户名称,
                ProjectName = item.ExcelProject.项目名称,
                ProductName = item.ExcelProject.产品全称,
                Currency = item.ExcelProject.币种,
                CompanyName = item.ExcelProject.我方公司,
                IndustryName = item.ExcelProject.行业,
                ProjectType = item.ExcelProject.机会类型,
                Name = item.ExcelProject.型号,
                Manufacturer = item.ExcelProject.品牌,

                SampleType = item.ExcelProject.送样类型,
                SampleDate = item.ExcelProject.送样时间,
                SampleQuantity = item.ExcelProject.送样数量,
                SampleUnitPrice = item.ExcelProject.送样单价,
                SampleTotalPrice = item.ExcelProject.送样金额,
                SampleContactor = item.ExcelProject.送样联系人,
                SamplePhone = item.ExcelProject.送样联系电话,
                SampleAddress = item.ExcelProject.送样联系地址,

                EnquiryReportDate = item.ExcelProject.报备时间,
                EnquiryReplyDate = item.ExcelProject.批复时间,
                EnquiryOriginModel = item.ExcelProject.原厂型号,
                EnquiryRFQ = item.ExcelProject.原厂RFQ号,
                EnquiryMOQ = item.ExcelProject.MOQ,
                EnquiryMPQ = item.ExcelProject.MPQ,
                EnquiryReplyPrice = item.ExcelProject.批复单价,
                EnquiryCurrency = item.ExcelProject.询价币种,
                EnquiryExchangeRate = item.ExcelProject.汇率,
                EnquiryTaxRate = item.ExcelProject.税率,
                EnquiryTariff = item.ExcelProject.关税点,
                EnquiryOtherRate = item.ExcelProject.其他附加点,
                EnquiryCost = item.ExcelProject.含税人民币成本价,
                EnquriyValidity = item.ExcelProject.有效时间,
                EnquiryValidityCount = item.ExcelProject.有效数量,
                EnquirySalePrice = item.ExcelProject.参考售价,
                EnquirySummary = item.ExcelProject.特殊备注,
            });
            this.Paging(linq);
        }

        /// <summary>
        /// 确认数据导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            this.btnImport.Enabled = false;

            if (Datas.Count == 0)
            {
                this.Alert("没有数据可导入!", Request.UrlReferrer ?? Request.Url, true);
                return;
            }
            foreach (var data in Datas.Where(t => string.IsNullOrEmpty(t.ExcelProject.Message)))
            {
                data.ExcelProject.Message = new NtErp.Crm.Services.Views.ProjectAlls().ExcelReportEnter(data);
            }

            var filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), Request.QueryString["filename"]);
           
            ExcelHelper.Current.ExportProjectByWeb(filePath, Datas);
        }

        #region 导入数据校验
        /// <summary>
        /// 校验Excel数据
        /// </summary>
        /// <param name="projectWithProducts"></param>
        /// <returns></returns>
        private List<ProjectExcelData> CheckData(List<ExcelProject> projectWithProducts)
        {
            List<ProjectExcelData> datas = new List<ProjectExcelData>();
            var Currencys = EnumUtils.ToDictionary<CurrencyType>();
            var clients = new NtErp.Crm.Services.Views.ClientAlls();
            var companys = new NtErp.Crm.Services.Views.CompanyAlls();
            var projectTypes = EnumUtils.ToDictionary<ProjectType>();
            var industries = new NtErp.Crm.Services.Views.IndustryAlls();
            var sampleTypes = EnumUtils.ToDictionary<NtErp.Crm.Services.Models.SampleType>();

            foreach (var item in projectWithProducts)
            {
                var data = new ProjectExcelData();
                data.ExcelProject = item;

                try
                {
                    #region 项目,及型号品牌信息
                    var project = new NtErp.Crm.Services.Models.Project();
                    if (string.IsNullOrEmpty(item.项目名称))
                    {
                        data.ExcelProject.Message += "项目名称不能为空; ";
                    }
                    else
                    {
                        project.Name = item.项目名称;
                    }

                    if (string.IsNullOrEmpty(item.产品全称))
                    {
                        data.ExcelProject.Message += "产品全称不能为空; ";
                    }
                    else
                    {
                        project.ProductName = item.产品全称;
                    }

                    if (string.IsNullOrEmpty(item.币种))
                    {
                        data.ExcelProject.Message += "币种不能为空; ";
                    }
                    else
                    {
                        project.Currency = (CurrencyType)int.Parse(Currencys.FirstOrDefault(c => c.Value == item.币种).Key);
                    }

                    if (string.IsNullOrEmpty(item.客户名称))
                    {
                        data.ExcelProject.Message += "客户名称不能为空; ";
                    }
                    else
                    {
                        var client = clients.FirstOrDefault(c => c.Name == item.客户名称 && c.Status == ActionStatus.Complete);

                        project.ClientID = client?.ID;
                        if (string.IsNullOrEmpty(project.ClientID))
                        {
                            data.ExcelProject.Message += "客户名称不正确，或客户未审批通过! ";
                        }
                    }

                    if (string.IsNullOrEmpty(item.我方公司))
                    {
                        data.ExcelProject.Message += "我方公司不能为空; ";
                    }
                    else
                    {
                        project.CompanyID = companys.FirstOrDefault(c => c.Name == item.我方公司)?.ID;
                        if (string.IsNullOrEmpty(project.CompanyID))
                        {
                            data.ExcelProject.Message += "我方公司不正确，没有找到对应的我放公司; ";
                        }
                    }

                    if (string.IsNullOrEmpty(item.机会类型))
                    {
                        data.ExcelProject.Message += "机会类型不能为空; ";
                    }
                    else
                    {
                        project.Type = (ProjectType)int.Parse(projectTypes.FirstOrDefault(t => t.Value == item.机会类型).Key);
                    }

                    if (string.IsNullOrEmpty(item.行业))
                    {
                        data.ExcelProject.Message += "行业不能为空; ";
                    }
                    else
                    {
                        project.Industry = industries.FirstOrDefault(c => c.Name == item.行业);
                        if (project.Industry == null)
                        {
                            data.ExcelProject.Message += "行业名称不正确!";
                        }
                    }

                    var productItem = new NtErp.Crm.Services.Models.ProductItem();
                    productItem.standardProduct = new NtErp.Crm.Services.Models.StandardProduct();

                    if (string.IsNullOrEmpty(item.型号))
                    {
                        data.ExcelProject.Message += "型号不能为空; ";
                    }
                    else
                    {
                        productItem.standardProduct.Name = item.型号;
                    }

                    if (string.IsNullOrEmpty(item.品牌))
                    {
                        data.ExcelProject.Message += "品牌不能为空; ";
                    }
                    else
                    {
                        productItem.standardProduct.Manufacturer = companys.FirstOrDefault(c => c.Name == item.品牌 || c.Code == item.品牌);
                        if (productItem.standardProduct.Manufacturer == null)
                        {
                            data.ExcelProject.Message += "品牌名称不正确!没有找到对应的品牌; ";
                        }
                    }

                    if (!string.IsNullOrEmpty(data.ExcelProject.Message))
                    {
                        datas.Add(data);
                        continue;
                    }
                    data.Project = project;
                    if (project.ClientID == null)
                    {
                        data.ProjectMD5 = string.Concat(project.Name, null, project.ProductName, project.Currency, project.CompanyID, project.Industry, project.Type).MD5();
                    }
                    else
                    {
                        data.ProjectMD5 = string.Concat(project.Name, project.ClientName, project.ProductName, project.Currency, project.CompanyID, project.Industry, project.Type).MD5();
                    }

                    #endregion

                    #region 送样信息
                    var sampleString = item.送样类型 + item.送样单价 + item.送样数量 + item.送样时间
                        + item.送样联系人 + item.送样联系地址 + item.送样联系电话 + item.送样金额;

                    if (!string.IsNullOrEmpty(sampleString))
                    {
                        productItem.Sample = new NtErp.Crm.Services.Models.Sample();
                        if (!string.IsNullOrEmpty(item.送样类型))
                        {
                            productItem.Sample.Type = (SampleType)int.Parse(sampleTypes.FirstOrDefault(c => c.Value == item.送样类型).Key);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样类型不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样单价))
                        {
                            productItem.Sample.UnitPrice = Convert.ToDecimal(System.Decimal.Parse((item.送样单价).ToString(), System.Globalization.NumberStyles.Float));
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样单价不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样数量))
                        {
                            productItem.Sample.Quantity = int.Parse(item.送样数量);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样数量不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样时间))
                        {
                            productItem.Sample.Date = DateTime.Parse(item.送样时间);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样时间不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样金额))
                        {
                            productItem.Sample.TotalPrice = Convert.ToDecimal(System.Decimal.Parse((item.送样金额).ToString(), System.Globalization.NumberStyles.Float));
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样时间不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系人))
                        {
                            productItem.Sample.Contactor = item.送样联系人;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系人不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系电话))
                        {
                            productItem.Sample.Phone = item.送样联系电话;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系电话不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系地址))
                        {
                            productItem.Sample.Address = item.送样联系地址;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系地址不能为空; ";
                        }
                    }
                    #endregion                    

                    #region 询价信息
                    var enquiries = new List<NtErp.Crm.Services.Models.Enquiry>();
                    var enquiryString = item.报备时间 + item.批复时间 + item.原厂型号 + item.MOQ + item.MPQ + item.批复单价 + item.询价币种 + item.汇率 + item.税率 + item.关税点 + item.其他附加点 + item.含税人民币成本价 + item.有效时间 + item.有效数量 + item.参考售价 + item.特殊备注;
                    if (!string.IsNullOrEmpty(enquiryString))
                    {
                        var enquiry = new NtErp.Crm.Services.Models.Enquiry();
                        if (!string.IsNullOrEmpty(item.报备时间))
                        {
                            enquiry.ReportDate = DateTime.Parse(item.报备时间);
                        }
                        else
                        {
                            data.ExcelProject.Message += "报备时间不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.批复时间))
                        {
                            enquiry.ReplyDate = DateTime.Parse(item.批复时间);
                        }
                        else
                        {
                            data.ExcelProject.Message += "批复时间不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.原厂型号))
                        {
                            enquiry.OriginModel = item.原厂型号;
                        }

                        if (!string.IsNullOrEmpty(item.MOQ))
                        {
                            enquiry.MOQ = int.Parse(item.MOQ);
                        }
                        //else
                        //{
                        //    data.ExcelProject.Message += "MOQ不能为空; ";
                        //}
                        enquiry.RFQ = item.原厂RFQ号;
                        if (!string.IsNullOrEmpty(item.MPQ))
                        {
                            enquiry.MPQ = int.Parse(item.MPQ);
                        }

                        if (!string.IsNullOrEmpty(item.批复单价))
                        {
                            enquiry.ReplyPrice = Convert.ToDecimal(System.Decimal.Parse((item.批复单价).ToString(), System.Globalization.NumberStyles.Float));
                        }
                        else
                        {
                            data.ExcelProject.Message += "批复单价不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.询价币种))
                        {
                            enquiry.Currency = (CurrencyType)int.Parse(Currencys.FirstOrDefault(c => c.Value == item.询价币种).Key);
                        }
                        else
                        {
                            data.ExcelProject.Message += "询价币种不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.汇率))
                        {
                            enquiry.ExchangeRate = decimal.Parse(item.汇率);
                        }

                        if (!string.IsNullOrEmpty(item.税率))
                        {
                            enquiry.TaxRate = decimal.Parse(item.税率);
                        }

                        if (!string.IsNullOrEmpty(item.关税点))
                        {
                            enquiry.Tariff = decimal.Parse(item.关税点);
                        }

                        if (!string.IsNullOrEmpty(item.其他附加点))
                        {
                            enquiry.OtherRate = decimal.Parse(item.其他附加点);
                        }

                        if (!string.IsNullOrEmpty(item.含税人民币成本价))
                        {
                            enquiry.Cost = Convert.ToDecimal(System.Decimal.Parse((item.含税人民币成本价).ToString(), System.Globalization.NumberStyles.Float));
                        }

                        if (!string.IsNullOrEmpty(item.有效时间))
                        {
                            enquiry.Validity = DateTime.Parse(item.有效时间);
                        }
                        else
                        {
                            data.ExcelProject.Message += "有效时间不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.有效数量))
                        {
                            enquiry.ValidityCount = int.Parse(item.有效数量);
                        }

                        if (!string.IsNullOrEmpty(item.参考售价))
                        {
                            enquiry.SalePrice = Convert.ToDecimal(System.Decimal.Parse((item.参考售价).ToString(), System.Globalization.NumberStyles.Float));
                        }

                        if (!string.IsNullOrEmpty(item.特殊备注))
                        {
                            enquiry.Summary = item.特殊备注;
                        }

                        enquiries.Add(enquiry);
                    }
                    #endregion

                    productItem.Enquiries = enquiries;
                    data.ProductItem = productItem;

                    datas.Add(data);
                }
                catch (Exception ex)
                {
                    data.ExcelProject.Message += ex.Message;
                    datas.Add(data);
                    continue;
                }

            }

            return datas;
        }
        #endregion
    }

}
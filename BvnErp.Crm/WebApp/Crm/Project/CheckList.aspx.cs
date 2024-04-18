using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Models;

namespace WebApp.Crm.Project
{
    public partial class CheckList : Uc.PageBase
    {
        private static List<ProjectExcelData> Datas;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string filename = Request.QueryString["filename"];
                var path = Path.Combine(Server.MapPath("~/UploadFiles/"), filename);
                var projects = ExcelHelper.Current.ExcelToLinq(path).OrderBy(item => item.项目名称).ToList();
                Datas = this.CheckData(projects);
            }
        }

        /// <summary>
        /// 加载导入数据
        /// </summary>
        protected void data()
        {
            var linq = Datas.Select(item => new
            {
                Message = item.ExcelProject.Message,
                Name = item.ExcelProject.项目名称,
                ProductName = item.ExcelProject.产品全称,
                Type = item.ExcelProject.机会类型,
                ClientName = item.ExcelProject.客户名称,
                IndustryName = item.ExcelProject.行业,
                CompanyID = item.ExcelProject.我方公司,
                Currency = item.ExcelProject.币种,
                AdminID = Needs.Erp.ErpPlot.Current.ID,
                Contactor = item.ExcelProject.联系人,
                Phone = item.ExcelProject.联系电话,
                Address = item.ExcelProject.地址,

                ItemName = item.ExcelProject.型号,
                ItemOrigrin = item.ExcelProject.型号全称,
                VendorName = item.ExcelProject.品牌,
                RefUnitQuantity = item.ExcelProject.单机用量,
                RefQuantity = item.ExcelProject.项目用量,
                RefUnitPrice = item.ExcelProject.参考单价,
                ExpectDate = item.ExcelProject.预计成交日期,
                ExpectRate = item.ExcelProject.预计成交概率,
                ExpectQuantity = item.ExcelProject.预计成交量,
                Status = item.ExcelProject.销售状态,
                CompeteModel = item.ExcelProject.竞争对手型号,
                CompeteManu = item.ExcelProject.竞争对手品牌,
                CompetePrice = item.ExcelProject.竞争对手单价,
                PMAdminID = item.ExcelProject.PM,
                FAEAdminID = item.ExcelProject.FAE,
                SaleAdminID = item.ExcelProject.Sales,
                PurchaseAdminID = item.ExcelProject.MSO,
                AssistantAdiminID = item.ExcelProject.CS,

                SampleType = item.ExcelProject.送样类型,
                SamplePrice = item.ExcelProject.送样单价,
                SampleQuantity = item.ExcelProject.送样数量,
                SampleDate = item.ExcelProject.送样时间,
                SampleTotal = item.ExcelProject.送样金额,
                SampleContactor = item.ExcelProject.送样联系人,
                SamplePhone = item.ExcelProject.送样联系电话,
                SampleAddress = item.ExcelProject.送样联系地址,
            });
            this.Paging(linq);
        }

        /// <summary>
        /// 数据导入
        /// </summary>
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
                data.ExcelProject.Message = new NtErp.Crm.Services.Views.ProjectAlls().ExcelDataEnter2(data);
            }

            var filePath = Path.Combine(Server.MapPath("~/UploadFiles/"), Request.QueryString["filename"]);

            ExcelHelper.Current.ExportProjectByWeb(filePath, Datas);
        }


        #region 导入数据校验
        /// <summary>
        /// 校验Excel数据
        /// </summary>
        /// <param name="projects">excel数据</param>
        /// <returns></returns>
        private List<ProjectExcelData> CheckData(List<ExcelProject> projects)
        {
            List<ProjectExcelData> datas = new List<ProjectExcelData>();
            var proejectTypes = EnumUtils.ToDictionary<ProjectType>();
            var Currencys = EnumUtils.ToDictionary<CurrencyType>();
            var Status = EnumUtils.ToDictionary<ProductStatus>();
            var companys = new NtErp.Crm.Services.Views.CompanyAlls();
            var admins = new NtErp.Crm.Services.Views.AdminTopView();
            var sampletypes = EnumUtils.ToDictionary<NtErp.Crm.Services.Models.SampleType>();
            var industrys = new NtErp.Crm.Services.Views.IndustryAlls().Where(item => item.FatherID == null);
            var clients = new NtErp.Crm.Services.Views.ClientAlls();
            foreach (var item in projects)
            {
                var data = new ProjectExcelData();
                data.ExcelProject = item;
                try
                {
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
                        project.Type = (ProjectType)int.Parse(proejectTypes.FirstOrDefault(t => t.Value == item.机会类型).Key);
                    }

                    project.Contactor = item.联系人;
                    project.Phone = item.联系电话;
                    project.Address = item.地址;
                    project.AdminID = Needs.Erp.ErpPlot.Current.ID;

                    if (string.IsNullOrEmpty(item.行业))
                    {
                        data.ExcelProject.Message += "行业不能为空; ";
                    }
                    else
                    {
                        project.Industry = industrys.FirstOrDefault(c => c.Name == item.行业);
                        if (project.Industry == null)
                        {
                            data.ExcelProject.Message += "行业名称不正确!";
                        }
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

                    var productitem = new NtErp.Crm.Services.Models.ProductItem();
                    productitem.standardProduct = new NtErp.Crm.Services.Models.StandardProduct();
                    if (string.IsNullOrWhiteSpace(item.型号))
                    {
                        data.ExcelProject.Message = "型号不能为空!";
                    }
                    else
                    {
                        productitem.standardProduct.Name = item.型号;
                    }

                    productitem.standardProduct.Origin = item.型号全称;
                    if (item.品牌 == "C&K")
                    {
                        item.品牌 = "C&amp;K";
                    }

                    if (string.IsNullOrEmpty(item.品牌))
                    {
                        data.ExcelProject.Message += "品牌不能为空; ";
                    }
                    else
                    {
                        productitem.standardProduct.Manufacturer = companys.FirstOrDefault(a => a.Name == item.品牌 || a.Code == item.品牌);
                        if (productitem.standardProduct.Manufacturer == null)
                        {
                            data.ExcelProject.Message += "品牌名称不正确!没有找到对应的品牌; ";
                        }
                    }

                    if (string.IsNullOrEmpty(item.销售状态))
                    {
                        data.ExcelProject.Message += "销售状态不能为空; ";
                    }
                    else
                    {
                        productitem.Status = (ProductStatus)int.Parse(Status.FirstOrDefault(a => a.Value == item.销售状态).Key);
                    }

                    if (string.IsNullOrEmpty(item.单机用量))
                    {
                        data.ExcelProject.Message += "单机用量不能为空; ";
                    }
                    else
                    {
                        productitem.RefUnitQuantity = int.Parse(item.单机用量);
                    }

                    if (string.IsNullOrEmpty(item.项目用量))
                    {
                        data.ExcelProject.Message += "项目用量不能为空; ";
                    }
                    else
                    {
                        productitem.RefQuantity = int.Parse(item.项目用量);
                    }

                    if (string.IsNullOrEmpty(item.参考单价))
                    {
                        data.ExcelProject.Message += "参考单价不能为空; ";
                    }
                    else
                    {
                        productitem.RefUnitPrice = Convert.ToDecimal(System.Decimal.Parse((item.参考单价).ToString(), System.Globalization.NumberStyles.Float));
                    }

                    productitem.ExpectRate = (int)productitem.Status;
                    if (!string.IsNullOrWhiteSpace(item.预计成交日期))
                    {
                        productitem.ExpectDate = DateTime.Parse(item.预计成交日期);
                    }
                    if (!string.IsNullOrWhiteSpace(item.预计成交量))
                    {
                        productitem.ExpectQuantity = int.Parse(item.预计成交量);
                    }
                    if (!string.IsNullOrWhiteSpace(item.预计成交概率))
                    {
                        productitem.ExpectRate = int.Parse(item.预计成交概率);
                    }
                    if (!string.IsNullOrWhiteSpace(item.竞争对手型号 + item.竞争对手品牌 + item.竞争对手单价))
                    {
                        productitem.CompeteProduct = new NtErp.Crm.Services.Models.CompeteProduct();
                        productitem.CompeteProduct.Name = item.竞争对手型号;
                        productitem.CompeteProduct.ManufacturerID = item.竞争对手品牌;
                        if (!string.IsNullOrWhiteSpace(item.竞争对手单价))
                        {
                            productitem.CompeteProduct.UnitPrice = Convert.ToDecimal(System.Decimal.Parse((item.竞争对手单价).ToString(), System.Globalization.NumberStyles.Float)); ;
                        }
                    }

                    if (string.IsNullOrEmpty(item.FAE))
                    {
                        data.ExcelProject.Message += "FAE不能为空; ";
                    }
                    else
                    {
                        productitem.FAEAdminID = admins.FirstOrDefault(a => a.RealName == item.FAE)?.ID;
                    }

                    if (string.IsNullOrEmpty(item.PM))
                    {
                        data.ExcelProject.Message += "PM不能为空; ";
                    }
                    else
                    {
                        productitem.PMAdminID = admins.FirstOrDefault(a => a.RealName == item.PM)?.ID;
                    }

                    if (string.IsNullOrEmpty(item.Sales))
                    {
                        data.ExcelProject.Message += "Sales不能为空; ";
                    }
                    else
                    {
                        productitem.SaleAdminID = admins.FirstOrDefault(a => a.RealName == item.Sales)?.ID;
                    }

                    productitem.PurchaseAdminID = admins.FirstOrDefault(a => a.RealName == item.MSO)?.ID;
                    productitem.AssistantAdiminID = admins.FirstOrDefault(a => a.RealName == item.CS)?.ID;

                    #region 送样数据校验
                    var samplestring = item.送样类型 + item.送样单价 + item.送样数量 + item.送样时间
                        + item.送样联系人 + item.送样联系地址 + item.送样联系电话 + item.送样金额;
                    if (!string.IsNullOrWhiteSpace(samplestring))
                    {
                        productitem.Sample = new NtErp.Crm.Services.Models.Sample();
                        if (!string.IsNullOrWhiteSpace(item.送样类型))
                        {
                            productitem.Sample.Type = (NtErp.Crm.Services.Models.SampleType)int.Parse(sampletypes.FirstOrDefault(a => a.Value == item.送样类型).Key);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样类型不能为空; ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.送样单价))
                        {
                            productitem.Sample.UnitPrice = Convert.ToDecimal(System.Decimal.Parse((item.送样单价).ToString(), System.Globalization.NumberStyles.Float));
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样单价不能为空; ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.送样数量))
                        {
                            productitem.Sample.Quantity = int.Parse(item.送样数量);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样数量不能为空; ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.送样时间))
                        {
                            productitem.Sample.Date = DateTime.Parse(item.送样时间);
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样时间不能为空; ";
                        }

                        if (!string.IsNullOrWhiteSpace(item.送样金额))
                        {
                            productitem.Sample.TotalPrice = Convert.ToDecimal(System.Decimal.Parse((item.送样金额).ToString(), System.Globalization.NumberStyles.Float));
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样金额不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系人))
                        {
                            productitem.Sample.Contactor = item.送样联系人;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系人不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系电话))
                        {
                            productitem.Sample.Phone = item.送样联系电话;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系电话不能为空; ";
                        }

                        if (!string.IsNullOrEmpty(item.送样联系地址))
                        {
                            productitem.Sample.Address = item.送样联系地址;
                        }
                        else
                        {
                            data.ExcelProject.Message += "送样联系地址不能为空; ";
                        }
                    }

                    #endregion

                    data.ProductItem = productitem;

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
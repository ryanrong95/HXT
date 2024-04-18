using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Projects;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Api.Views
{
    /// <summary>
    /// 销售机会管理数据集
    /// </summary>
    public class ProjectsView : QueryView<Api.Models.Project, BvCrmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectsView()
        {

        }

        internal ProjectsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 基础数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Api.Models.Project> GetIQueryable()
        {

            var clientsView = new ClientAlls(this.Reponsitory);
            var companyall = new CompanyAlls(this.Reponsitory);//公司视图
            var standardsView = new StandardProductAlls(this.Reponsitory);

            var products = from company in companyall
                           join product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>() on company.ID equals product.ManufacturerID
                           select new
                           {
                               ID = product.ID,
                               Name = product.Name,
                               MF = company.Name,
                               MFShortName = company.Code,
                           };

            var projects = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                           join client in clientsView on project.ClientID equals client.ID
                           select new
                           {
                               ID = project.ID,
                               Name = project.Name,
                               ClientID = project.ClientID,
                               ClientName = client.Name
                           };

            var es = from product in products
                     join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on product.ID equals item.StandardID
                     join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on item.ID equals map.ProductItemID
                     join project in projects on map.ProjectID equals project.ID
                     join map1 in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProductItemEnquiry>() on item.ID equals map1.ProductItemID
                     join enquiry in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItemEnquiries>() on map1.ProductItemEnquiryID equals enquiry.ID
                     select new Api.Models.Project.EnquirySingle
                     {
                         ClientID = project.ClientID,
                         Mf = product.MF,
                         MfShortName = product.MFShortName,
                         ReportDate = enquiry.ReportDate
                     };


            var applies = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>()
                          where apply.Status == (int)ApplyStatus.Approval
                          group apply by apply.MainID into gs
                          select new
                          {
                              MainID = gs.Key,
                              Apply = gs.OrderByDescending(t => t.Type).Select(t => new Models.Project.ApplySingle
                              {
                                  Type = (ApplyType)t.Type,
                                  UpdateDate = t.UpdateDate
                              }).FirstOrDefault()
                          };

            var linqs = from product in products
                        join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on product.ID equals item.StandardID
                        join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on item.ID equals map.ProductItemID
                        join project in projects on map.ProjectID equals project.ID
                        join _apply in applies on item.ID equals _apply.MainID into aps
                        from apply in aps.DefaultIfEmpty()

                        join _enquiry in es on new { ClientID = project.ClientID, Mf = product.MF } equals new { ClientID = _enquiry.ClientID, Mf = _enquiry.Mf }
                        into enquires
                        select new Api.Models.Project
                        {
                            Name = project.Name,
                            ClientName = project.ClientName,
                            Partnumber = product.Name,
                            Manufaturer = product.MF,
                            ManufaturerShortName = product.MFShortName,
                            Status = (ProductStatus)item.Status,
                            StatusDate = item.UpdateDate,
                            Apply = apply.Apply,
                            Enquiries = enquires
                        };
            return linqs;
        }

    }
}

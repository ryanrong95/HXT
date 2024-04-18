//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Layer.Data.Sqls;
//using Needs.Erp.Generic;
//using Needs.Linq;
//using Needs.Utils.Converters;
//using NtErp.Crm.Services.Enums;
//using NtErp.Crm.Services.Models;
//using NtErp.Crm.Services.Models.Generic;

//namespace NtErp.Crm.Services.Views
//{
//    abstract public class ProjcetDossierAlls : QueryView<ProjectDossier, BvCrmReponsitory>
//    {
//        IGenericAdmin Admin;

//        protected ProjcetDossierAlls()
//        {

//        }

//        internal ProjcetDossierAlls(IGenericAdmin admin)
//        {
//            this.Admin = admin;
//        }

//        internal ProjcetDossierAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
//        {

//        }

//        protected override IQueryable<ProjectDossier> GetIQueryable()
//        {
//            return this.GetIQueryable(Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>());
//        }

//        internal IQueryable<ProjectDossier> GetIQueryable(IQueryable<Layer.Data.Sqls.BvCrm.Projects> query = null)
//        {
//            AdminTopView adminview = new AdminTopView(this.Reponsitory);
//            ClientAlls ClientView = new ClientAlls(this.Reponsitory);
//            CompanyAlls CompanyView = new CompanyAlls(this.Reponsitory);
//            ProductItemAlls productItems = new ProductItemAlls(this.Reponsitory);

//            if (query == null)
//            {
//                query = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>();
//            }
//            var products_linq = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
//                                join pis in productItems on map.ProductItemID equals pis.ID
//                                select new
//                                {
//                                    ProjectID = map.ProjectID,
//                                    Product = pis
//                                };

//            return from project in query
//                   join admin in adminview on project.AdminID equals admin.ID  //创建人
//                   join client in ClientView on project.ClientID equals client.ID // 项目所属客户
//                   join company in CompanyView on project.CompanyID equals company.ID //项目代表公司
//                   join product in products_linq on project.ID equals product.ProjectID into products
//                   select new ProjectDossier
//                   {
//                       Project = new Project
//                       {
//                           ID = project.ID,
//                           Name = project.Name,
//                           Type = (ProjectType)project.Type,
//                           Client = client,
//                           Company = company,
//                           Valuation = project.Valuation,
//                           Currency = (CurrencyType)project.Currency,
//                           Admin = admin,
//                           StartDate = project.StartDate,
//                           EndDate = project.EndDate,
//                           CreateDate = project.CreateDate,
//                           UpdateDate = project.UpdateDate,
//                           Status = (ActionStatus)project.Status,
//                           Summary = project.Summary,
//                       },
//                       Products = products.Select(item => item.Product).ToArray()
//                   };
//        }

//    }
//}

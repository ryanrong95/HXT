using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ProjectStatisticsAlls : UniqueView<Models.ProductItemExtend, BvCrmReponsitory>
    {
        //人员对象
        AdminTop Admin;

        protected ProjectStatisticsAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public ProjectStatisticsAlls(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        protected override IQueryable<ProductItemExtend> GetIQueryable()
        {
            ProjectAlls projects = new ProjectAlls(Reponsitory);
            var standardsView = new StandardProductAlls(this.Reponsitory);
            var competesView = new CompeteProductAlls(this.Reponsitory);
            //我的员工视图
            return from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                   join project in projects on maps.ProjectID equals project.ID
                   join productitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                   on maps.ProductItemID equals productitem.ID
                   join standard in standardsView on productitem.StandardID equals standard.ID
                   join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>() on project.ClientID equals client.ID
                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>() on project.CompanyID equals company.ID
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>() on project.AdminID equals admin.ID
                   join _compete in competesView on productitem.CompeteID equals _compete.ID into competes
                   from compete in competes.DefaultIfEmpty()
                   select new ProductItemExtend
                   {
                       ProjectID = project.ID,
                       ProjectClientID = project.ClientID,
                       ProjectClientName = client.Name,
                       ProjectAdminID = project.AdminID,
                       ProjectAdminName = admin.RealName,
                       ProjectCompanyID = project.CompanyID,
                       ProjectCompanyName = company.Name,
                       ProjectCurrency = project.Currency,
                       ProjectType = project.Type,
                       ProjectUpdateDate = project.UpdateDate,
                       ID = productitem.ID,
                       RefUnitQuantity = productitem.RefUnitQuantity,
                       RefQuantity = productitem.RefQuantity,
                       RefUnitPrice = productitem.RefUnitPrice,
                       ExpectRate = productitem.ExpectRate,
                       Status = (Enums.ProductStatus)productitem.Status,
                       CreateDate = productitem.CreateDate,
                       UpdateDate = productitem.UpdateDate,
                       standardProduct = standard,
                       CompeteProduct = compete
                   };
        }
    }
}

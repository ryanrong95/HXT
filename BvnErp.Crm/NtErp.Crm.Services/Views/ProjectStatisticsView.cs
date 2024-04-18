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
    public class ProjectStatisticsView : UniqueView<Models.ProductItemExtend, BvCrmReponsitory>
    {
        //人员对象
        AdminTop Admin;

        public ProjectStatisticsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public ProjectStatisticsView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        protected override IQueryable<ProductItemExtend> GetIQueryable()
        {
            ProjectAlls projects = new ProjectAlls(Reponsitory);
            ProductItemAlls items = new ProductItemAlls(Reponsitory);
            //我的员工视图
            //初始化区域树
            var districts = new DistrictTree(this.Admin.ID);

            //获取当前用户的所有子员工
            string[] mystaffids = districts.AdminDescendants;

            return from project in projects
                   join maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                   on project.ID equals maps.ProjectID
                   join item in items
                   on maps.ProductItemID equals item.ID
                   where mystaffids.Contains(project.Admin.ID)
                   select new ProductItemExtend
                   {
                       ProjectID = project.ID,
                       ProjectAdminName = project.Admin.RealName,
                       ProjectClientName = project.Client.Name,
                       ProjectCompanyName = project.Company.Name,
                       ProjectCurrency = project.Currency,
                       ProjectType = project.Type,
                       ProjectUpdateDate=project.UpdateDate,
                       ID = item.ID,
                       standardProduct = item.standardProduct,
                       CompeteProduct = item.CompeteProduct,
                       RefQuantity = item.RefQuantity,
                       RefUnitPrice = item.RefUnitPrice,
                       UnitPrice = item.UnitPrice,
                       Quantity = item.Quantity,
                       Status = item.Status,
                       Count = item.Count,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate,
                       ExpectDate = item.ExpectDate,
                       OriginNumber = item.OriginNumber,
                   };
        }
    }
}

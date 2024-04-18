using Layer.Data.Sqls;
using NtErp.Crm.Services.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace NtErp.Crm.Services.Views.Generic
{
    public class ProjectClassicsAlls : Needs.Linq.Generic.Query1Classics<ProjectDossier, BvCrmReponsitory>
    {
        protected ProjectClassicsAlls()
        {

        }

        protected ProjectClassicsAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ProjectDossier> GetIQueryable(Expression<Func<ProjectDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            ProjectAlls projects = new ProjectAlls(this.Reponsitory);

            var linq = projects.AsQueryable();

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Models.Project, bool>>);
            }

            var projectdossiers = from project in linq
                                  select new ProjectDossier
                                  {
                                      Project = project,
                                  };
            return projectdossiers.Where(expression);
        }

        protected override IEnumerable<ProjectDossier> OnReadShips(ProjectDossier[] results)
        {
            ProductItemAlls productItems = new ProductItemAlls(this.Reponsitory);
            IndustryAlls industries = new IndustryAlls(this.Reponsitory);
            CompanyAlls companies = new CompanyAlls(this.Reponsitory);
            AdminTopView admins = new AdminTopView(this.Reponsitory);

            var ids = results.Select(item => item.Project.ID).ToArray();
            var clientids = results.Select(item => item.Project.ClientID).ToArray();
            var product_linq = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>().AsEnumerable()
                               join product in productItems on map.ProductItemID equals product.ID
                               where ids.Contains(map.ProjectID)
                               select new
                               {
                                   map.ProjectID,
                                   product,
                               };

            var industry_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>().AsEnumerable()
                                join industry in industries on maps.IndustryID equals industry.ID
                                where ids.Contains(maps.ProjectID)
                                select new
                                {
                                    maps.ProjectID,
                                    industry,
                                };

            var Manufacture_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>().AsEnumerable()
                                   join manufacture in companies on maps.ManufacturerID equals manufacture.ID
                                   where ids.Contains(maps.ProjectID)
                                   select new
                                   {
                                       maps.ProjectID,
                                       manufacture,
                                   };

            var Clientadmins_linq = from maps in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                                    join admin in admins on maps.AdminID equals admin.ID
                                    where clientids.Contains(maps.ClientID)
                                    select new
                                    {
                                        maps.ClientID,
                                        admin,
                                    };

            var arry = product_linq.ToArray();
            bool twomethods = true;
            if (twomethods)
            {
                return from result in results
                       join product in product_linq on result.Project.ID equals product.ProjectID into _products
                       join industry in industry_linq on result.Project.ID equals industry.ProjectID into _industries
                       join manufacture in Manufacture_linq on result.Project.ID equals manufacture.ProjectID into _manufactures
                       join clientadmin in Clientadmins_linq on result.Project.ClientID equals clientadmin.ClientID into _clientadmins
                       select new ProjectDossier
                       {
                           Project = result.Project,
                           Products = _products.Select(item => item.product).ToArray(),
                           Industries = _industries.Select(item => item.industry).ToArray(),
                           Manufactures = _manufactures.Select(item => item.manufacture).ToArray(),
                           ClientAdmins = _clientadmins.Select(item=>item.admin).ToArray(),
                       };
            }
            else
            {
                return results.Select(result =>
                {
                    result.Products = arry.Where(item => item.ProjectID == result.Project.ID)
                        .Select(item => item.product).ToArray();
                    return result;
                });
            }
        }


        /// <summary>
        /// 关联查询条件
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual public string[] MapCondition(Expression<Func<Layer.Data.Sqls.BvCrm.MapsProject, bool>> expression)
        {
            var linq = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>().Where(expression);

            return linq.Select(item => item.ProjectID).ToArray();
        }
    }
}

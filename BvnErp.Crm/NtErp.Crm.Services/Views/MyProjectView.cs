using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Models.Generic;

namespace NtErp.Crm.Services.Views
{
    public class MyProjectView : Generic.ProjectClassicsAlls
    {
        //人员对象
        AdminTop Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public MyProjectView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public MyProjectView(IGenericAdmin admin)
        {
            this.Admin = Extends.AdminExtends.GetTop(admin.ID);
        }

        /// <summary>
        /// 获取项目数据集合
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <param name="expressions">lambda表达式</param>
        /// <returns></returns>
        protected override IQueryable<ProjectDossier> GetIQueryable(Expression<Func<ProjectDossier, bool>> expression, params LambdaExpression[] expressions)
        {
            var projectdossiers = from projectDossier in base.GetIQueryable(expression, expressions)
                                  where projectDossier.Project.Status != ActionStatus.Delete
                                  select projectDossier;

            if (Admin.JobType != JobType.TPM)
            {
                //获取所有员工
                var mystaffids = new MyStaffsView(this.Admin, Reponsitory).Select(item => item.ID).ToArray();

                var linq1 = from maps in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                            join projectDossier in projectdossiers on maps.ClientID equals projectDossier.Project.ClientID
                            where mystaffids.Contains(maps.AdminID)
                            select projectDossier;

                if (Admin.JobType != JobType.Sales)
                {
                    var linq2 = from mapsadmin in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>()
                                join mapsproject in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                on mapsadmin.ManufactureID equals mapsproject.ManufacturerID
                                join projectDossier in projectdossiers on mapsproject.ProjectID equals projectDossier.Project.ID
                                where mystaffids.Contains(mapsadmin.AdminID)
                                select projectDossier;

                    linq1 = linq1.Union(linq2);
                }

                projectdossiers = linq1;
            }

            return projectdossiers.Distinct().OrderByDescending(item => item.Project.UpdateDate);
        }

        /// <summary>
        /// 根据项目状态起止时间查询项目ID
        /// </summary>
        /// <param name="status"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public string[] GetProductIds(ProductStatus status, string startDate, string endDate)
        {
            var applys = new ApplyAlls(this.Reponsitory).Where(item => item.Status == ApplyStatus.Approval);
            var productitems = new ProductItemAlls(Reponsitory).AsQueryable();
            if (!string.IsNullOrWhiteSpace(startDate))
            {
                if (status == ProductStatus.DO || status == ProductStatus.DL)
                {
                    productitems = productitems.Where(item => item.UpdateDate >= DateTime.Parse(startDate) && item.Status == status);
                }
                else
                {
                    ApplyType Type = (ApplyType)(int)status;
                    productitems = from item in productitems
                                   join apply in applys.Where(item => item.Type == Type && item.UpdateDate >= DateTime.Parse(startDate))
                                   on item.ID equals apply.MainID
                                   select item;
                }
            }
            if (!string.IsNullOrWhiteSpace(endDate))
            {
                if (status == ProductStatus.DO || status == ProductStatus.DL)
                {
                    productitems = productitems.Where(item => item.UpdateDate < DateTime.Parse(endDate) && item.Status == status);
                }
                else
                {
                    ApplyType Type = (ApplyType)(int)status;
                    productitems = from item in productitems
                                   join apply in applys.Where(item => item.Type == Type && item.UpdateDate < DateTime.Parse(endDate))
                                   on item.ID equals apply.MainID
                                   select item;
                }
            }

            if (string.IsNullOrWhiteSpace(endDate) && string.IsNullOrWhiteSpace(startDate))
            {
                productitems = productitems.Where(item => item.Status == status);
            }

            var projectids = from map in Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                             join item in productitems on map.ProductItemID equals item.ID
                             select map.ProjectID;

            return projectids.Distinct().ToArray();
        }


        /// <summary>
        /// 加载销售机会中我管理产品
        /// </summary>
        /// <param name="result"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public ProductItem[] MyProducts(string Projectid)
        {
            var result = this.GetTop(1, item => item.Project.ID == Projectid).SingleOrDefault();

            var products = result.Products;

            //销售机会所属客户的客户所有人
            var clientAdmin = Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().Where(item => item.ClientID == result.Project.ClientID).
                Select(item => item.AdminID).ToArray();

            var mystaffids = new MyStaffsView(Admin, Reponsitory).Select(item => item.ID).ToArray();

            //该销售机会的维护人不在我的员工中
            if (!mystaffids.Contains(result.Project.AdminID) && mystaffids.Intersect(clientAdmin).Count() == 0)
            {
                var manufactures = Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsAdmin>().Where(item => mystaffids.Contains(item.AdminID)).
                    Select(item => item.ManufactureID).ToArray();

                products = products.Where(item => manufactures.Contains(item.standardProduct.Manufacturer.ID)).ToArray();
            }

            return products;
        }

        /// <summary>
        /// 产品绑定
        /// </summary>
        /// <param name="projectId">销售机会ID</param>
        /// <param name="item">产品</param>
        public void BindingItem(string projectId, ProductItem item)
        {
            if (item == null)
            {
                return;
            }

            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsProject>(c => c.ProjectID == projectId && c.ProductItemID == item.ID);

                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsProject
                {
                    ID = string.Concat(projectId, item.ID, item.standardProduct.Manufacturer.ID).MD5(),
                    ProjectID = projectId,
                    ProductItemID = item.ID,
                    ManufacturerID = item.standardProduct.Manufacturer.ID
                });
            }
        }


        /// <summary>
        /// 绑定行业
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="industry">行业</param>
        public void BindingIndustry(string projectId, Industry industry)
        {
            if (industry == null)
            {
                return;
            }
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsProject>(c => c.ProjectID == projectId && c.IndustryID != null);

                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsProject
                {
                    ID = string.Concat(projectId, industry.ID).MD5(),
                    ProjectID = projectId,
                    IndustryID = industry.ID,
                });
            }
        }

        /// <summary>
        /// 关联查询条件
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public override string[] MapCondition(Expression<Func<Layer.Data.Sqls.BvCrm.MapsProject, bool>> expression)
        {
            return base.MapCondition(expression);
        }
    }
}

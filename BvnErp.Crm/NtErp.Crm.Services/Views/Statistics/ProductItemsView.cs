using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views.Statistics
{
    /// <summary>
    /// 统计DI/DW个数
    /// </summary>
    public class ProductItemsView : QueryView<ProductItem, BvCrmReponsitory>
    {
        #region 构造函数
        public ProductItemsView()
        {

        }

        internal ProductItemsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        internal ProductItemsView(BvCrmReponsitory reponsitory, IQueryable<ProductItem> iQuery) : base(reponsitory, iQuery)
        {

        }
        #endregion

        protected override IQueryable<ProductItem> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                   select new ProductItem
                   {
                       ID = entity.ID,
                       StandardID = entity.StandardID,
                       Status = (ProductStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       PMAdmin = entity.PMAdmin,
                       FAEAdmin = entity.FAEAdmin,
                       SaleAdmin = entity.SaleAdmin,
                   };
        }

        /// <summary>
        /// 拼接完整数据
        /// </summary>
        /// <param name="name">人名</param>
        /// <param name="jobType">职位</param>
        /// <param name="applyType">类型</param>
        /// <returns></returns>
        public object ToMyObject(string name, JobType jobType, ApplyType applyType)
        {
            var iquery = this.IQueryable.Cast<ProductItem>();
            var ienum_iquery = iquery.ToArray();

            if (ienum_iquery.Count() == 0)
            {
                //没有数据，直接返回
                return new
                {
                    Count = 0,
                    Details = ienum_iquery
                };
            }
            else
            {
                #region 拼接完整对象
                //审批
                var mainIDs = ienum_iquery.Select(item => item.ID).Distinct().ToArray();
                var appliesView = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>()
                                  where apply.Type == (int)applyType && mainIDs.Contains(apply.MainID)
                                  select new
                                  {
                                      apply.ID,
                                      apply.Type,
                                      apply.MainID,
                                      apply.Status,
                                      apply.UpdateDate
                                  };
                var ienum_applies = appliesView.ToArray();
                var ienum_applyTargets = ienum_applies.OrderByDescending(item => item.UpdateDate)
                                                 .GroupBy(item => item.MainID)
                                                 .Select(item => item.First()).ToArray();

                //标准产品
                var standardIDs = ienum_iquery.Select(item => item.StandardID).Distinct().ToArray();
                var productsView = from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>()
                                   join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>() on product.ManufacturerID equals company.ID
                                   where standardIDs.Contains(product.ID)
                                   select new
                                   {
                                       product.ID,
                                       Partnumber = product.Name,
                                       Manufacturer = company.Name
                                   };
                var ienum_products = productsView.ToArray();

                //项目
                var productItemsID = ienum_iquery.Select(item => item.ID).ToArray();
                var mapsProjectView = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                      where productItemsID.Contains(map.ProductItemID)
                                      select new
                                      {
                                          map.ProjectID,
                                          map.ProductItemID
                                      };
                var ienum_mapsProject = mapsProjectView.ToArray();

                var projectIDs = ienum_mapsProject.Select(item => item.ProjectID).Distinct().ToArray();
                var projectsView = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                                   where projectIDs.Contains(project.ID)
                                   select new
                                   {
                                       project.ID,
                                       project.Name,
                                       project.ClientID
                                   };
                var ienum_projects = projectsView.ToArray();

                //客户
                var clientIDs = ienum_projects.Select(item => item.ClientID).Distinct().ToArray();
                var clientsView = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>()
                                  where clientIDs.Contains(client.ID)
                                  select new
                                  {
                                      client.ID,
                                      client.Name
                                  };
                var ienum_clients = clientsView.ToArray();

                var linq = from item in ienum_iquery
                           join product in ienum_products on item.StandardID equals product.ID
                           join map in ienum_mapsProject on item.ID equals map.ProductItemID
                           join project in ienum_projects on map.ProjectID equals project.ID
                           join client in ienum_clients on project.ClientID equals client.ID
                           join apply in ienum_applyTargets on item.ID equals apply.MainID into applies
                           from apply in applies.DefaultIfEmpty()
                           select new
                           {
                               item.ID,
                               item.StandardID,
                               item.PMAdmin,
                               item.FAEAdmin,
                               item.SaleAdmin,
                               map.ProjectID,
                               project.ClientID,

                               ClientName = client.Name,
                               product.Manufacturer,
                               ProjectName = project.Name,
                               product.Partnumber,
                               ApplyDate = apply?.UpdateDate ?? item.UpdateDate
                           };

                #endregion

                #region 同一个客户、同一个项目、相同型号在一个自然年中算一次[排重]

                //当前查询的数据先做一次排重
                linq = linq.GroupBy(item => new { item.ClientID, item.ProjectID, item.StandardID }).Select(item => item.First()).ToArray();

                DateTime minDate = linq.Min(item => item.ApplyDate);
                //当前查询的指定年月
                DateTime currentMonth = new DateTime(minDate.Year, minDate.Month, 1);
                //当前自然年的开始
                DateTime firstMonth = new DateTime(minDate.Year, minDate.Month, 1);

                //查询当前自然年中，项目在之前的月份是否有数据
                var previousView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                                   join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on item.ID equals map.ProductItemID
                                   join project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>() on map.ProjectID equals project.ID
                                   where projectIDs.Contains(project.ID) && item.UpdateDate > firstMonth && item.UpdateDate < currentMonth
                                   select new
                                   {
                                       item.ID,
                                       item.StandardID,
                                       ProjectID = project.ID
                                   };
                var ienum_previous = previousView.ToArray();

                //如果项目在之前的月份有数据，需要根据同项目、同型号做排重
                if (ienum_previous.Count() > 0)
                {
                    var duplicateIDs = from entity in linq
                                       join previous in ienum_previous on new { entity.StandardID, entity.ProjectID } equals new { previous.StandardID, previous.ProjectID }
                                       select entity.ID;

                    linq = from entity in linq
                           where !duplicateIDs.Contains(entity.ID)
                           select entity;
                }

                #endregion

                #region 根据不同职位类型返回需要的数据

                switch (jobType)
                {
                    case JobType.Sales:
                        //客户归属人（销售）
                        var mapsClientView = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>()
                                             join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>() on map.AdminID equals admin.ID
                                             where clientIDs.Contains(map.ClientID)
                                             select new
                                             {
                                                 map.ClientID,
                                                 admin.RealName
                                             };
                        var ienum_mapsClient = mapsClientView.ToArray();

                        var salesLinq = from entity in linq
                                        join map in ienum_mapsClient on entity.ClientID equals map.ClientID into maps
                                        from map in maps.DefaultIfEmpty()
                                        select new
                                        {
                                            Sales = map?.RealName ?? "", //客户归属人（销售）
                                            entity.ClientName, //客户名称
                                            entity.Manufacturer, //厂商
                                            entity.ProjectName, //项目名称
                                            entity.Partnumber, //型号
                                            ApplyDate = entity.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss") //审批时间
                                        };
                        return new
                        {
                            Count = salesLinq.Count(),
                            Details = salesLinq
                        };

                    case JobType.FAE:
                        var faeLinq = from entity in linq
                                      select new
                                      {
                                          FAE = name, //FAE
                                          entity.ClientName, //客户名称
                                          entity.Manufacturer, //厂商
                                          entity.ProjectName, //项目名称
                                          entity.Partnumber, //型号
                                          ApplyDate = entity.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss") //审批时间
                                      };
                        return new
                        {
                            Count = faeLinq.Count(),
                            Details = faeLinq
                        };

                    case JobType.PME:
                        var pmLinq = from entity in linq
                                     select new
                                     {
                                         PME = name, //PM
                                         entity.ClientName, //客户名称
                                         entity.Manufacturer, //厂商
                                         entity.ProjectName, //项目名称
                                         entity.Partnumber, //型号
                                         ApplyDate = entity.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss") //审批时间
                                     };
                        return new
                        {
                            Count = pmLinq.Count(),
                            Details = pmLinq
                        };
                    default:
                        throw new NotSupportedException($"不支持的职位类型【{jobType.GetDescription()}】");
                }

                #endregion
            }
        }

        /// <summary>
        /// 数量统计
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            var iquery = this.IQueryable.Cast<ProductItem>();
            var ienum_iquery = iquery.ToArray();

            if (ienum_iquery.Count() == 0)
                return 0;

            //项目
            var productItemsID = ienum_iquery.Select(item => item.ID).ToArray();
            var mapsProjectView = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                  where productItemsID.Contains(map.ProductItemID)
                                  select new
                                  {
                                      map.ProjectID,
                                      map.ProductItemID
                                  };
            var ienum_mapsProject = mapsProjectView.ToArray();

            var linq = from item in ienum_iquery
                       join map in ienum_mapsProject on item.ID equals map.ProductItemID
                       select new
                       {
                           item.ID,
                           item.StandardID,
                           map.ProjectID,
                           ApplyDate = item.UpdateDate
                       };

            #region 同一个项目、相同型号在一个自然年中算一次[排重]

            //当前查询的数据先做一次排重
            linq = linq.GroupBy(item => new { item.ProjectID, item.StandardID }).Select(item => item.First()).ToArray();

            DateTime minDate = linq.Min(item => item.ApplyDate);
            //当前查询的指定年月
            DateTime currentMonth = new DateTime(minDate.Year, minDate.Month, 1);
            //当前自然年的开始
            DateTime firstMonth = new DateTime(minDate.Year, minDate.Month, 1);

            //查询当前自然年中，项目在之前的月份是否有数据
            var projectIDs = linq.Select(item => item.ProjectID).Distinct().ToArray();
            var previousView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>()
                               join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on item.ID equals map.ProductItemID
                               where projectIDs.Contains(map.ProjectID) && item.UpdateDate > firstMonth && item.UpdateDate < currentMonth
                               select new
                               {
                                   item.ID,
                                   item.StandardID,
                                   ProjectID = map.ProjectID
                               };
            var ienum_previous = previousView.ToArray();

            //如果项目在之前的月份有数据，需要根据同项目、同型号做排重
            if (ienum_previous.Count() > 0)
            {
                var duplicateIDs = from entity in linq
                                   join previous in ienum_previous on new { entity.StandardID, entity.ProjectID } equals new { previous.StandardID, previous.ProjectID }
                                   select entity.ID;

                linq = from entity in linq
                       where !duplicateIDs.Contains(entity.ID)
                       select entity;
            }

            #endregion

            return linq.Count();
        }

        #region 搜索方法
        /// <summary>
        /// 根据厂商查询
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public ProductItemsView SearchByManufacturer(string manufacturer)
        {
            var linq = from entity in this.IQueryable.Cast<ProductItem>()
                       join product in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>() on entity.StandardID equals product.ID
                       join mfr in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>() on product.ManufacturerID equals mfr.ID
                       where mfr.Name == manufacturer
                       select entity;

            return new ProductItemsView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据Admin查询
        /// </summary>
        /// <param name="name">人名</param>
        /// <param name="jobType">职位</param>
        /// <returns></returns>
        public ProductItemsView SearchByAdmin(string name, JobType jobType)
        {
            var admin = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminTopView>()
                .SingleOrDefault(item => item.RealName == name && item.Status == (int)Needs.Erp.Generic.Status.Normal);
            if (admin == null)
            {
                throw new Exception($"未查询到指定用户【{name}】");
            }

            var linq = this.IQueryable.Cast<ProductItem>();
            switch (jobType)
            {
                case JobType.Sales:
                    linq = linq.Where(item => item.SaleAdmin == admin.ID);
                    break;
                case JobType.FAE:
                    linq = linq.Where(item => item.FAEAdmin == admin.ID);
                    break;
                case JobType.PME:
                    linq = linq.Where(item => item.PMAdmin == admin.ID);
                    break;
                default:
                    throw new NotSupportedException($"不支持的职位类型【{jobType.GetDescription()}】");
            }

            return new ProductItemsView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据申请查询
        /// </summary>
        /// <param name="applyType">申请类型</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        public ProductItemsView SearchByApply(ApplyType applyType, ProductStatus status, int year, int month)
        {
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            //查出指定年月的DI/DW申请
            var appliesView = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>()
                              where apply.Type == (int)applyType && apply.UpdateDate > startDate && apply.UpdateDate < endDate
                              select new
                              {
                                  apply.ID,
                                  apply.Type,
                                  apply.MainID,
                                  apply.Status,
                                  apply.UpdateDate
                              };
            var ienum_applies = appliesView.ToArray();

            //ProductItems的修改审批，有从DI改为DW，又从DW改回DI的情况，以最近一次的审批为准？
            var ienum_targets = ienum_applies.OrderByDescending(item => item.UpdateDate)
                                             .GroupBy(item => item.MainID)
                                             .Select(item => item.First()).ToArray();
            //DI/DW审批通过的ProductItems
            var approvedIDs = ienum_targets.Where(item => item.Status == (int)ApplyStatus.Approval)
                                           .Select(item => item.MainID).ToArray();

            //有审批的ProductItems
            var mainIDs = from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>().Where(item => item.Type == (int)applyType)
                          select apply.MainID;

            //1.有审批并审批通过的
            //2.没有走修改审批流程的
            var linq = from entity in this.IQueryable.Cast<ProductItem>()
                       where approvedIDs.Contains(entity.ID) ||
                       (entity.UpdateDate > startDate && entity.UpdateDate < endDate && entity.Status == status && !mainIDs.Contains(entity.ID))
                       select entity;

            return new ProductItemsView(this.Reponsitory, linq);
        }
        #endregion
    }
}

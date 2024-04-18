using Layer.Data.Sqls;
using Needs.Linq;
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
    /// 统计新增客户数
    /// </summary>
    public class NewClientsView : QueryView<NewClient, BvCrmReponsitory>
    {
        #region 构造函数
        public NewClientsView()
        {

        }

        internal NewClientsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        internal NewClientsView(BvCrmReponsitory reponsitory, IQueryable<NewClient> iQuery) : base(reponsitory, iQuery)
        {

        }
        #endregion

        protected override IQueryable<NewClient> GetIQueryable()
        {
            return from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>()
                   join apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>() on client.ID equals apply.MainID
                   where apply.Type == (int)ApplyType.CreatedClient && apply.Status == (int)ApplyStatus.Approval
                   select new NewClient
                   {
                       ID = client.ID,
                       Name = client.Name,
                       ApplyDate = apply.UpdateDate
                   };
        }

        public object ToMyObject()
        {
            var iquery = this.IQueryable.Cast<NewClient>();
            var ienum_iquery = iquery.ToArray();

            return new
            {
                Count = ienum_iquery.Count(),
                Details = ienum_iquery.Select(item => new
                {
                    ClientName = item.Name, //客户名称
                    ApplyDate = item.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss") //审批时间
                })
            };
        }

        /// <summary>
        /// 数量统计
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.IQueryable.Cast<NewClient>().Count();
        }

        /// <summary>
        /// 详情信息
        /// </summary>
        /// <returns></returns>
        public object Details()
        {
            var iquery = this.IQueryable.Cast<NewClient>();
            var ienum_iquery = iquery.ToArray();

            return ienum_iquery.Select(item => new
            {
                ClientName = item.Name, //客户名称
                ApplyDate = item.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss") //审批时间
            });
        }

        #region 搜索方法
        /// <summary>
        /// 根据厂商查询
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public NewClientsView SearchByManufacturer(string manufacturer)
        {
            //根据销售机会厂商过滤
            var mapsProjectView = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                  join mfr in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>() on map.ManufacturerID equals mfr.ID
                                  where mfr.Name == manufacturer
                                  select map;
            var projectIDs = mapsProjectView.Select(item => item.ProjectID).Distinct().ToArray();

            var projectsView = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                               where projectIDs.Contains(project.ID)
                               select project;
            var clientIDs = projectsView.Select(item => item.ClientID).Distinct().ToArray();

            var linq = this.IQueryable.Cast<NewClient>().Where(item => clientIDs.Contains(item.ID));
            return new NewClientsView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据审批时间查询
        /// </summary>
        /// <param name="manufacturer">厂商</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        public NewClientsView SearchByApplyDate(int year, int month)
        {
            //当月第一天
            DateTime startDate = new DateTime(year, month, 1);
            //下个月第一天
            DateTime endDate = startDate.AddMonths(1);

            //当月系统注册审批通过的客户
            var linq = from entity in this.IQueryable.Cast<NewClient>()
                       where entity.ApplyDate > startDate && entity.ApplyDate < endDate
                       select entity;

            return new NewClientsView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据厂商、(产品的审批或更新)时间查询
        /// </summary>
        /// <param name="manufacturer">指定厂商</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        public NewClientsView SearchByMfrAndDate(string manufacturer, int year, int month)
        {
            //查询厂商
            var mfrID = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>().FirstOrDefault(item => item.Name == manufacturer)?.ID;
            if (mfrID == null)
            {
                throw new NotSupportedException($"未查询到指定厂商{manufacturer}");
            }

            //指定厂商的销售机会、销售产品
            var view = from map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                       join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on map.ProductItemID equals item.ID
                       join project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>() on map.ProjectID equals project.ID
                       where map.ManufacturerID == mfrID
                       select new
                       {
                           map.ProjectID,
                           map.ProductItemID,
                           item.UpdateDate,
                           project.ClientID
                       };

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            //指定年月, 有指定厂商销售机会的客户
            var clientIDs = view.Where(item => item.UpdateDate > startDate && item.UpdateDate < endDate)
                .Select(item => item.ClientID).Distinct().ToArray();

            //指定年月之前，已经有指定厂商销售机会的客户
            var preClientIDs = view.Where(item => item.UpdateDate < startDate).Select(item => item.ClientID).Distinct().ToArray();

            //求差集，得到指定年月，新增指定厂商销售机会的客户
            var ids = clientIDs.Except(preClientIDs).ToArray();

            var linq = from entity in this.IQueryable
                       where ids.Contains(entity.ID)
                       select entity;

            return new NewClientsView(this.Reponsitory, linq);
        }
        #endregion
    }
}

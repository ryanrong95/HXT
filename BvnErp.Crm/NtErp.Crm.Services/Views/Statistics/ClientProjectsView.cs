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
    /// 统计客户新增销售机会数
    /// </summary>
    public class ClientProjectsView : UniqueView<ClientProject, BvCrmReponsitory>
    {
        #region 构造函数
        public ClientProjectsView()
        {

        }

        internal ClientProjectsView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        internal ClientProjectsView(BvCrmReponsitory reponsitory, IQueryable<ClientProject> iQuery) : base(reponsitory, iQuery)
        {

        }
        #endregion

        protected override IQueryable<ClientProject> GetIQueryable()
        {
            return from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                   select new ClientProject
                   {
                       ID = project.ID,
                       Name = project.Name,
                       ClientID = project.ClientID,
                       CreateDate = project.CreateDate,
                   };
        }

        /// <summary>
        /// 数量统计
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return this.IQueryable.Count();
        }

        /// <summary>
        /// 详情信息
        /// </summary>
        /// <returns></returns>
        public object Details()
        {
            var ienum_iquery = this.IQueryable.ToArray();

            //补全数据
            var clientIDs = ienum_iquery.Select(item => item.ClientID).Distinct().ToArray();
            var clientsView = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>()
                              where clientIDs.Contains(client.ID)
                              select new
                              {
                                  client.ID,
                                  client.Name
                              };
            var ienum_clients = clientsView.ToArray();

            var linq = from project in ienum_iquery
                       join client in ienum_clients on project.ClientID equals client.ID
                       select new
                       {
                           ClientName = client.Name, //客户名称
                           ProjectName = project.Name, //销售机会名称
                           CreateDate = project.CreateDate.ToString("yyyy-MM-dd HH:mm:ss") //销售机会的创建时间
                       };

            return linq;
        }

        #region 搜索方法
        /// <summary>
        /// 根据厂商、销售机会的创建时间查询
        /// </summary>
        /// <param name="manufacturer">指定厂商</param>
        /// <param name="year">指定年</param>
        /// <param name="month">指定月</param>
        /// <returns></returns>
        public ClientProjectsView SearchByMfrAndDate(string manufacturer, int year, int month)
        {
            //查询厂商
            var mfrID = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>().FirstOrDefault(item => item.Name == manufacturer)?.ID;
            if (mfrID == null)
            {
                throw new NotSupportedException($"未查询到指定厂商{manufacturer}");
            }

            //指定厂商的销售机会
            var view = from project in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Projects>()
                       join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>() on project.ID equals map.ProjectID
                       where map.ManufacturerID == mfrID
                       select new
                       {
                           project.ID,
                           project.ClientID,
                           project.CreateDate,
                       };

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            //指定年月, 有指定厂商的销售机会
            var projects = view.Where(item => item.CreateDate > startDate && item.CreateDate < endDate).ToArray();
            var projectIDs = projects.Select(item => item.ID).Distinct().ToArray();

            //指定年月, 有指定厂商销售机会的客户
            var curClientIDs = projects.Select(item => item.ClientID).Distinct().ToArray();

            //指定年月之前，已经有指定厂商销售机会的客户
            var preClientIDs = view.Where(item => item.CreateDate < startDate)
                .Select(item => item.ClientID).Distinct().ToArray();

            //求差集，得到指定年月，新增指定厂商销售机会的客户
            var clientIDs = curClientIDs.Except(preClientIDs).ToArray();

            var linq = from entity in this.IQueryable
                       where projectIDs.Contains(entity.ID) && clientIDs.Contains(entity.ClientID)
                       select entity;

            return new ClientProjectsView(this.Reponsitory, linq);
        }
        #endregion
    }
}

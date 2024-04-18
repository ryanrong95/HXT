using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 报关量统计列表
    /// </summary>
    public class ClientDecStatisticsListView : QueryView<ClientDecStatisticsListViewModel, ScCustomsReponsitory>
    {
        private LambdaExpression[] _expressions { get; set; }

        public ClientDecStatisticsListView(params LambdaExpression[] expressions)
        {
            this._expressions = expressions;
        }

        protected override IQueryable<ClientDecStatisticsListViewModel> GetIQueryable()
        {
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientAdmins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var decLists = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>();
            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();

            var linq_data = from decHead in decHeads
                            join order in orders on decHead.OrderID equals order.ID
                            join clientAdmin in clientAdmins on order.ClientID equals clientAdmin.ClientID
                            join client in clients on order.ClientID equals client.ID
                            join clientAgreement in clientAgreements on client.ID equals clientAgreement.ClientID
                            join company in companies on client.CompanyID equals company.ID
                            where decHead.IsSuccess == true
                               && order.Status == (int)Enums.Status.Normal
                               && clientAdmin.Status == (int)Enums.Status.Normal
                               && clientAdmin.Type == (int)Enums.ClientAdminType.ServiceManager
                               && clientAgreement.Status == (int)Enums.Status.Normal
                            select new ClientDecStatisticsListViewModel
                            {
                                DecHeadID = decHead.ID,
                                DDate = decHead.DDate,
                                ClientID = order.ClientID,
                                CompanyName = company.Name,
                                ClientCode = client.ClientCode,
                                Currency = order.Currency,
                                ServiceManagerID = clientAdmin.AdminID,
                                CreateDate = client.CreateDate,
                                InvoiceType = clientAgreement.InvoiceType,
                                AgentRate = clientAgreement.AgencyRate
                            };

            if (this._expressions != null && this._expressions.Length > 0)
            {
                foreach (var exp in this._expressions)
                {
                    linq_data = linq_data.Where(exp as Expression<Func<ClientDecStatisticsListViewModel, bool>>);
                }
            }

            var iQuery = from data in linq_data
                         join decList in decLists on data.DecHeadID equals decList.DeclarationID
                         group new { data, decList, }
                         by new
                         {
                             ClientID = data.ClientID,
                             CompanyName = data.CompanyName,
                             ClientCode = data.ClientCode,
                             Currency = data.Currency,
                             ServiceManagerID = data.ServiceManagerID,
                             CreateDate = data.CreateDate,
                             InvoiceType = data.InvoiceType,
                             AgentRate = data.AgentRate
                         } into g
                         select new ClientDecStatisticsListViewModel
                         {
                             ClientID = g.Key.ClientID,
                             CompanyName = g.Key.CompanyName,
                             ClientCode = g.Key.ClientCode,
                             Currency = g.Key.Currency,
                             ServiceManagerID = g.Key.ServiceManagerID,
                             CreateDate = g.Key.CreateDate,
                             InvoiceType = g.Key.InvoiceType,
                             AgentRate = g.Key.AgentRate,
                             DecPriceTotal = g.Sum(t => t.decList.DeclTotal),
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ClientDecStatisticsListViewModel> iquery = this.IQueryable.Cast<ClientDecStatisticsListViewModel>().OrderBy(item => item.CompanyName);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myStatistics = iquery.ToArray();

            var serviceManagerIDs = ienum_myStatistics.Select(t => t.ServiceManagerID).ToArray();

            #region 业务员

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var linqs_serviceManager = from admin in adminsTopView2
                                       where serviceManagerIDs.Contains(admin.OriginID)
                                       group admin by new { admin.OriginID } into g
                                       select new
                                       {
                                           AdminID = g.Key.OriginID,
                                           AdminName = g.FirstOrDefault() != null ? g.FirstOrDefault().RealName : "",
                                       };

            var ienums_serviceManager = linqs_serviceManager.ToArray();

            #endregion

            var ienums_linq = from statistic in ienum_myStatistics
                              join serviceManager in ienums_serviceManager on statistic.ServiceManagerID equals serviceManager.AdminID into ienums_serviceManager2
                              from serviceManager in ienums_serviceManager2.DefaultIfEmpty()
                              select new ClientDecStatisticsListViewModel
                              {
                                  ClientID = statistic.ClientID,
                                  CompanyName = statistic.CompanyName,
                                  ClientCode = statistic.ClientCode,
                                  Currency = statistic.Currency,
                                  ServiceManagerID = statistic.ServiceManagerID,
                                  ServiceManagerName = serviceManager != null ? serviceManager.AdminName : "",
                                  DecPriceTotal = statistic.DecPriceTotal,
                                  CreateDate = statistic.CreateDate,
                                  InvoiceType = statistic.InvoiceType,
                                  AgentRate = statistic.AgentRate
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ClientDecStatisticsListViewModel, object> convert = item => new
            {
                ClientID = item.ClientID,
                CompanyName = item.CompanyName,
                ClientCode = item.ClientCode,
                Currency = item.Currency,
                ServiceManagerID = item.ServiceManagerID,
                ServiceManagerName = item.ServiceManagerName,
                DecPriceTotalStr = item.DecPriceTotal.ToString("0.0000"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                InvoiceType = item.InvoiceType == (int)Enums.InvoiceType.Full?  "全额发票" : "服务费发票",
                AgentRate = item.AgentRate
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public List<ClientDecStatisticsListViewModel> ToGetAll()
        {
            IQueryable<ClientDecStatisticsListViewModel> iquery = this.IQueryable.Cast<ClientDecStatisticsListViewModel>().OrderBy(item => item.CompanyName);
            int total = iquery.Count();

            //获取数据
            var ienum_myStatistics = iquery.ToArray();

            var serviceManagerIDs = ienum_myStatistics.Select(t => t.ServiceManagerID).ToArray();

            #region 业务员

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var linqs_serviceManager = from admin in adminsTopView2
                                       where serviceManagerIDs.Contains(admin.OriginID)
                                       group admin by new { admin.OriginID } into g
                                       select new
                                       {
                                           AdminID = g.Key.OriginID,
                                           AdminName = g.FirstOrDefault() != null ? g.FirstOrDefault().RealName : "",
                                       };

            var ienums_serviceManager = linqs_serviceManager.ToArray();

            #endregion

            var ienums_linq = from statistic in ienum_myStatistics
                              join serviceManager in ienums_serviceManager on statistic.ServiceManagerID equals serviceManager.AdminID into ienums_serviceManager2
                              from serviceManager in ienums_serviceManager2.DefaultIfEmpty()
                              select new ClientDecStatisticsListViewModel
                              {
                                  ClientID = statistic.ClientID,
                                  CompanyName = statistic.CompanyName,
                                  ClientCode = statistic.ClientCode,
                                  Currency = statistic.Currency,
                                  ServiceManagerID = statistic.ServiceManagerID,
                                  ServiceManagerName = serviceManager != null ? serviceManager.AdminName : "",
                                  DecPriceTotal = statistic.DecPriceTotal,
                                  CreateDate = statistic.CreateDate,
                                  InvoiceType = statistic.InvoiceType,
                                  AgentRate = statistic.AgentRate
                              };

            var results = ienums_linq;

            return results.ToList();
        }

        /// <summary>
        /// 合计数
        /// </summary>
        /// <returns></returns>
        public List<ClientDecStatisticsTotalModel> ToTotalData()
        {
            IQueryable<ClientDecStatisticsListViewModel> iquery = this.IQueryable.Cast<ClientDecStatisticsListViewModel>().OrderBy(item => item.CompanyName);

            //获取数据
            var ienum_myStatistics = iquery.ToArray();

            var serviceManagerIDs = ienum_myStatistics.Select(t => t.ServiceManagerID).ToArray();

            #region 业务员

            var adminsTopView2 = new AdminsTopView2(this.Reponsitory);

            var linqs_serviceManager = from admin in adminsTopView2
                                       where serviceManagerIDs.Contains(admin.OriginID)
                                       group admin by new { admin.OriginID } into g
                                       select new
                                       {
                                           AdminID = g.Key.OriginID,
                                           AdminName = g.FirstOrDefault() != null ? g.FirstOrDefault().RealName : "",
                                       };

            var ienums_serviceManager = linqs_serviceManager.ToArray();

            #endregion

            var ienums_linq = from statistic in ienum_myStatistics
                              join serviceManager in ienums_serviceManager on statistic.ServiceManagerID equals serviceManager.AdminID into ienums_serviceManager2
                              from serviceManager in ienums_serviceManager2.DefaultIfEmpty()
                              group statistic by new { statistic.Currency } into g_statistic
                              select new ClientDecStatisticsTotalModel
                              {
                                  Currency = g_statistic.Key.Currency,
                                  DecPriceTotal = g_statistic.Sum(t=>t.DecPriceTotal),
                              };

            var results = ienums_linq;

            return results.ToList();
        }
    }

    public class ClientDecStatisticsListViewModel
    {
        public string DecHeadID { get; set; }

        public DateTime? DDate { get; set; }

        public string ClientID { get; set; }

        public string CompanyName { get; set; }

        public string ClientCode { get; set; }

        public string Currency { get; set; }

        public string ServiceManagerID { get; set; }

        public string ServiceManagerName { get; set; }

        public decimal DecPriceTotal { get; set; }

        public DateTime CreateDate { get; set; }

        public decimal AgentRate { get; set; }

        public int InvoiceType { get; set; }

    }

    public class ClientDecStatisticsTotalModel
    {
        public string Currency { get; set; }
        public decimal DecPriceTotal { get; set; }
    }
}

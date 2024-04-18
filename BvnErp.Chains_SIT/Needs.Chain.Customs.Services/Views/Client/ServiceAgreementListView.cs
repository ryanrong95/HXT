using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 服务协议列表视图
    /// </summary>
    public class ServiceAgreementListView : QueryView<ServiceAgreementListViewModel, ScCustomsReponsitory>
    {
        public ServiceAgreementListView()
        {
        }

        protected ServiceAgreementListView(ScCustomsReponsitory reponsitory, IQueryable<ServiceAgreementListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ServiceAgreementListViewModel> GetIQueryable()
        {
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var iQuery = from client in clients
                         join company in companies on client.CompanyID equals company.ID
                         where client.Status == (int)Enums.Status.Normal
                            && company.Status == (int)Enums.Status.Normal
                         orderby client.CreateDate descending
                         select new ServiceAgreementListViewModel
                         {
                             ClientID = client.ID,
                             ClientName = company.Name,
                             IsSAPaperUpload = (client.IsSAUpload == (int)Enums.SAUploadStatus.UnUpload) ? false : true,
                             CreateDate = client.CreateDate,
                             ClientCode = client.ClientCode,
                             ClientStatus = (Enums.ClientStatus)client.ClientStatus,
                             ServiceType = (Enums.ServiceType)client.ServiceType,
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ServiceAgreementListViewModel> iquery = this.IQueryable.Cast<ServiceAgreementListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_clients = iquery.ToArray();

            var clientsID = ienum_clients.Select(item => item.ClientID);

            #region 电子服务协议地址

            var filesDescriptionTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var linq_eleSACount = from file in filesDescriptionTopView
                                  where clientsID.Contains(file.ClientID)
                                     && file.Type == (int)Enums.FileType.ServiceAgreement
                                     && file.Status == (int)Enums.Status.Normal
                                  orderby file.CreateDate descending
                                  group file by new { file.ClientID, } into g
                                  select new
                                  {
                                      ClientID = g.Key.ClientID,
                                      EleSACount = g.Count(),
                                      SAUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : "",
                                  };

            var ienums_eleSACount = linq_eleSACount.ToArray();

            #endregion

            #region 仓储协议地址

            var linq_eleStoCount = from file in filesDescriptionTopView
                                  where clientsID.Contains(file.ClientID)
                                     && file.Type == (int)Enums.FileType.StorageAgreement
                                     && file.Status == (int)Enums.Status.Normal
                                  orderby file.CreateDate descending
                                  group file by new { file.ClientID, } into g
                                  select new
                                  {
                                      ClientID = g.Key.ClientID,
                                      EleStoCount = g.Count(),
                                      StoUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : "",
                                  };

            var ienums_eleStoCount = linq_eleStoCount.ToArray();

            #endregion

            #region 业务员

            var clientAdminsView = new Views.ClientAdminsView(this.Reponsitory);

            var linq_clientAdmin = from clientAdmin in clientAdminsView
                                   where clientsID.Contains(clientAdmin.ClientID)
                                      && clientAdmin.Status == Enums.Status.Normal
                                      && clientAdmin.Type == Enums.ClientAdminType.ServiceManager
                                   select new
                                   {
                                       ClientID = clientAdmin.ClientID,
                                       ServiceManagerName = clientAdmin.Admin.RealName,
                                   };

            var ienums_clientAdmin = linq_clientAdmin.ToArray();

            #endregion

            var ienums_linq = from client in ienum_clients
                              join eleSACount in ienums_eleSACount on client.ClientID equals eleSACount.ClientID into ienums_eleSACount2
                              from eleSACount in ienums_eleSACount2.DefaultIfEmpty()
                              join eleStoCount in ienums_eleStoCount on client.ClientID equals eleStoCount.ClientID into ienums_eleStoCount2
                              from eleStoCount in ienums_eleStoCount2.DefaultIfEmpty()
                              join clientAdmin in ienums_clientAdmin on client.ClientID equals clientAdmin.ClientID into ienums_clientAdmin2
                              from clientAdmin in ienums_clientAdmin2.DefaultIfEmpty()
                              orderby client.CreateDate descending
                              select new ServiceAgreementListViewModel
                              {
                                  ClientID = client.ClientID,
                                  ClientName = client.ClientName,
                                  IsSAEleUpload = eleSACount != null ? true : false,
                                  SAUrl = eleSACount != null ? eleSACount.SAUrl : "",
                                  IsStoEleUpload = eleStoCount != null ? true : false,
                                  StoUrl = eleStoCount != null ? eleStoCount.StoUrl : "",
                                  IsSAPaperUpload = client.IsSAPaperUpload,
                                  ClientCode = client.ClientCode,
                                  ClientStatus = client.ClientStatus,
                                  ServiceType = client.ServiceType,
                                  CreateDate = client.CreateDate,
                                  ServiceManagerName = clientAdmin != null ? clientAdmin.ServiceManagerName : "",
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

            string PvDataFileUrl = ConfigurationManager.AppSettings["PvDataFileUrl"];

            Func<ServiceAgreementListViewModel, object> convert = client => new
            {
                ClientID = client.ClientID,
                ClientName = client.ClientName,
                IsSAEleUpload = client.IsSAEleUpload,
                IsSAEleUploadString = client.IsSAEleUpload ? "已上传" : "未上传",
                IsSAPaperUpload = client.IsSAPaperUpload,
                IsSAPaperUploadString = client.IsSAPaperUpload ? "已上交" : "未上交",
                IsStoEleUpload = client.IsStoEleUpload,
                IsStoEleUploadString = client.IsStoEleUpload ? "已上传" : "未上传",
                StoUrl = !string.IsNullOrEmpty(client.StoUrl) ? PvDataFileUrl + @"/" + client.StoUrl : "",
                ClientCode = client.ClientCode,
                ClientStatusString = client.ClientStatus.GetDescription(),
                ServiceType = client.ServiceType,
                ServiceTypeString = client.ServiceType.GetDescription(),
                CreateDate = client.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                SAUrl = !string.IsNullOrEmpty(client.SAUrl) ? PvDataFileUrl + @"/" + client.SAUrl : "",
                ServiceManagerName = client.ServiceManagerName,
            };


            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        #region Search Helper

        /// <summary>
        /// 过滤已完善客户
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public ServiceAgreementListView SearchByClientStatus(Enums.ClientStatus clientStatus)
        {
            var linq = from query in this.IQueryable
                       where query.ClientStatus == clientStatus
                       select query;

            var view = new ServiceAgreementListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public ServiceAgreementListView SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new ServiceAgreementListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据纸质版是否上传查询
        /// </summary>
        /// <param name="isSAPaperUpload"></param>
        /// <returns></returns>
        public ServiceAgreementListView SearchByIsSAPaperUpload(bool isSAPaperUpload)
        {
            var linq = from query in this.IQueryable
                       where query.IsSAPaperUpload == isSAPaperUpload
                       select query;

            var view = new ServiceAgreementListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据电子版是否上传查询
        /// </summary>
        /// <param name="isSAEleUpload"></param>
        /// <returns></returns>
        public ServiceAgreementListView SearchByIsSAEleUpload(bool isSAEleUpload)
        {
            var filesDescriptionTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var allUploadedClientIDs = filesDescriptionTopView
                .Where(t => t.Status == (int)Enums.Status.Normal 
                   && t.Type == (int)Enums.FileType.ServiceAgreement
                   && !t.Url.Contains("doc"))
                .Select(t => t.ClientID);

            var linq = from query in this.IQueryable
                       select query;

            if (isSAEleUpload)
            {
                linq = linq.Where(t => allUploadedClientIDs.Contains(t.ClientID));
            }
            else
            {
                linq = linq.Where(t => !allUploadedClientIDs.Contains(t.ClientID));
            }

            var view = new ServiceAgreementListView(this.Reponsitory, linq);
            return view;
        }

        #endregion

    }

    public class ServiceAgreementListViewModel
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 服务协议电子档是否上传
        /// </summary>
        public bool IsSAEleUpload { get; set; }

        /// <summary>
        /// 服务协议纸质稿是否上交
        /// </summary>
        public bool IsSAPaperUpload { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 客户状态
        /// </summary>
        public Enums.ClientStatus ClientStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Enums.ServiceType ServiceType { get; set; }

        /// <summary>
        /// 服务协议电子版地址
        /// </summary>
        public string SAUrl { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string ServiceManagerName { get; set; }

        /// <summary>
        /// 仓储协议是否上传
        /// </summary>
        public bool IsStoEleUpload { get; set; }

        /// <summary>
        /// 仓储协议路径
        /// </summary>
        public string StoUrl { get; set; }
    }

}

using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AgreementChangeListView : QueryView<AgreementChangeListViewModel, ScCustomsReponsitory>
    {
        public AgreementChangeListView()
        {
        }

        protected AgreementChangeListView(ScCustomsReponsitory reponsitory, IQueryable<AgreementChangeListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<AgreementChangeListViewModel> GetIQueryable()
        {
            var agreementChangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var clientAdmins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>();

            var iQuery = from agreementChangeApply in agreementChangeApplies
                         join client in clients on agreementChangeApply.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         join clientAdmin in clientAdmins on client.ID equals clientAdmin.ClientID
                         where client.IsValid == true
                         && client.Status == (int)Enums.Status.Normal
                         && company.Status == (int)Enums.Status.Normal
                         && clientAdmin.Status == (int)Enums.Status.Normal && clientAdmin.Type == (int)Enums.ClientAdminType.Merchandiser

                         select new AgreementChangeListViewModel
                         {
                             AgreementChangeApplyID = agreementChangeApply.ID,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID,
                             ClientRank = (Enums.ClientRank)client.ClientRank,
                             AgreementChangeApplyStatus = (Enums.AgreementChangeApplyStatus)agreementChangeApply.Status,
                             IntAgreementChangeApplyStatus = agreementChangeApply.Status,
                             AdminID = agreementChangeApply.AdminID,
                             CreateDate = agreementChangeApply.CreateDate,
                             Summary = agreementChangeApply.Summary,
                             ClientNature = client.ClientNature == null ? (int)Enums.ClientNature.Trade : (int)client.ClientNature,//(ClientNature)client.ClientNature ,
                             MerchandiserName = clientAdmin.AdminID,
                             IsSAPaperUpload = agreementChangeApply.IsPaperUpload == (int)Enums.SAUploadStatus.Uploaded ? true : false,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<AgreementChangeListViewModel> iquery = this.IQueryable.Cast<AgreementChangeListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_agreementChangeApplies = iquery.ToArray();

            var agreementChangeApplyIDs = ienum_agreementChangeApplies.Select(item => item.AgreementChangeApplyID);
            // var clientsID = ienum_agreementChangeApplies.Select(item => item.ClientID);
            var merchandiserNames = ienum_agreementChangeApplies.Select(item => item.MerchandiserName);
            var serviceManagerIDs = ienum_agreementChangeApplies.Select(item => item.AdminID);

            #region 协议变更(电子)文件

            var filesDescriptionTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var linq_eleSACount = from file in filesDescriptionTopView
                                  where agreementChangeApplyIDs.Contains(file.ApplicationID)
                                     && file.Type == (int)Enums.FileType.ChangeServiceAgreement
                                     && file.Status != (int)Models.FileDescriptionStatus.Delete
                                  orderby file.CreateDate descending
                                  group file by new { file.ApplicationID, } into g
                                  select new
                                  {
                                      ApplicationID = g.Key.ApplicationID,
                                      EleSACount = g.Count(),
                                      SAUrl = g.FirstOrDefault() != null ? g.FirstOrDefault().Url : "",
                                  };

            var ienums_eleSACount = linq_eleSACount.ToArray();

            #endregion

            #region 业务员、跟单员

            var adminsView = new AdminsTopView2(this.Reponsitory);

            var linq_merchandiserName = from adminDB in adminsView
                                        where merchandiserNames.Contains(adminDB.OriginID)
                                        select new
                                        {
                                            OriginID = adminDB.OriginID,
                                            RealName = adminDB.RealName,
                                        };
            var ienums_merchandiserName = linq_merchandiserName.ToArray();

            var linq_serviceManagerID = from adminDB in adminsView
                                        where serviceManagerIDs.Contains(adminDB.OriginID)
                                        select new
                                        {
                                            OriginID = adminDB.OriginID,
                                            RealName = adminDB.RealName,
                                        };
            var ienums_serviceManager = linq_serviceManagerID.ToArray();

            #endregion

            var ienums_linq = from agreementChangeApply in ienum_agreementChangeApplies
                              join merchandiser in ienums_merchandiserName on agreementChangeApply.MerchandiserName equals merchandiser.OriginID
                              into ienums_merchandiserName2
                              from merchandiser in ienums_merchandiserName2.DefaultIfEmpty()

                              join serviceManager in ienums_serviceManager on agreementChangeApply.AdminID equals serviceManager.OriginID into ienums_serviceManager2
                              from serviceManager in ienums_serviceManager2.DefaultIfEmpty()

                              join eleSACount in ienums_eleSACount on agreementChangeApply.AgreementChangeApplyID equals eleSACount.ApplicationID into ienums_eleSACount2
                              from eleSACount in ienums_eleSACount2.DefaultIfEmpty()
                              select new AgreementChangeListViewModel
                              {
                                  AgreementChangeApplyID = agreementChangeApply.AgreementChangeApplyID,
                                  ClientCode = agreementChangeApply.ClientCode,
                                  ClientName = agreementChangeApply.ClientName,
                                  ClientID = agreementChangeApply.ClientID,
                                  AdminID = agreementChangeApply.AdminID,
                                  ClientRank = agreementChangeApply.ClientRank,
                                  AgreementChangeApplyStatus = agreementChangeApply.AgreementChangeApplyStatus,
                                  Summary = agreementChangeApply.Summary,
                                  ClientNature = agreementChangeApply.ClientNature,
                                  MerchandiserName = merchandiser != null ? merchandiser.RealName : "",
                                  IntAgreementChangeApplyStatus = agreementChangeApply.IntAgreementChangeApplyStatus,
                                  CreateDate = agreementChangeApply.CreateDate,
                                  ServiceManager = serviceManager != null ? serviceManager.RealName : "",

                                  IsSAEleUpload = eleSACount != null ? true : false,
                                  SAUrl = eleSACount != null ? eleSACount.SAUrl : "",
                                  IsSAPaperUpload = agreementChangeApply.IsSAPaperUpload,
                              };

            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];

            Func<AgreementChangeListViewModel, object> convert = item => new
            {
                AgreementChangeApplyID = item.AgreementChangeApplyID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                ClientID = item.ClientID,
                AgreementChangeApplyStatus = item.AgreementChangeApplyStatus.GetDescription(),
                Summary = item.Summary,
                MerchandiserName = item.MerchandiserName,
                IntStatus = item.IntAgreementChangeApplyStatus,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                ClientNature = ((Enums.ClientNature)item.ClientNature).GetDescription(),//item.ClientNature.GetDescription(),
                ClientRank = item.ClientRank.GetDescription(),
                ServiceManager = item.ServiceManager,

                IsSAEleUpload = item.IsSAEleUpload,
                IsSAEleUploadString = item.IsSAEleUpload ? "已上传" : "未上传",
                SAUrl = !string.IsNullOrEmpty(item.SAUrl) ? FileServerUrl + @"/" + item.SAUrl : "",
                IsSAPaperUpload = item.IsSAPaperUpload,
                IsSAPaperUploadString = item.IsSAPaperUpload ? "已上交" : "未上交",
            };

            var results = ienums_linq;

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        ///  根据客户编号查询
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请状态查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByApplyStatus(int agreementChangeApplyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.AgreementChangeApplyStatus == (Enums.AgreementChangeApplyStatus)agreementChangeApplyStatus
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByCreateDateFrom(DateTime start)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= start
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期截止时间查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByCreateDateTo(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据业务员查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByServiceManager(string ServiceManager)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == ServiceManager
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public AgreementChangeListView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new AgreementChangeListView(this.Reponsitory, linq);
            return view;
        }


        public void SetSAUploaded(string agreementChangeApplyID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>(new
                {
                    IsPaperUpload = (int)Enums.SAUploadStatus.Uploaded,
                }, item => item.ID == agreementChangeApplyID);
            }
        }

        public void SetSAUnUpload(string agreementChangeApplyID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>(new
                {
                    IsPaperUpload = (int)Enums.SAUploadStatus.UnUpload,
                }, item => item.ID == agreementChangeApplyID);
            }
        }
    }

    public class AgreementChangeListViewModel
    {
        public string AgreementChangeApplyID { get; set; }

        public string ClientCode { get; set; }

        public string ClientName { get; set; }

        public string ClientID { get; set; }

        public Enums.ClientRank ClientRank { get; set; }

        public Enums.AgreementChangeApplyStatus AgreementChangeApplyStatus { get; set; }

        public int IntAgreementChangeApplyStatus { get; set; }

        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public int? ClientNature { get; set; }

        public string MerchandiserName { get; set; }

        public string ServiceManager { get; set; }

        public bool IsSAEleUpload { get; set; }

        public string SAUrl { get; set; }

        public bool IsSAPaperUpload { get; set; } 
    }
}

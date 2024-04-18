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
    public class AdvanceMoneyApplyListView : QueryView<AdvanceMoneyApplyListViewModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyApplyListView()
        {
        }

        protected AdvanceMoneyApplyListView(ScCustomsReponsitory reponsitory, IQueryable<AdvanceMoneyApplyListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<AdvanceMoneyApplyListViewModel> GetIQueryable()
        {
            var advanceMoneyApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var clientAdmins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>();

            var iQuery = from advanceMoneyApply in advanceMoneyApplies
                         join client in clients on advanceMoneyApply.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         join clientAdmin in clientAdmins on client.ID equals clientAdmin.ClientID
                         where client.IsValid == true
                         && advanceMoneyApply.Status == (int)Enums.AdvanceMoneyStatus.Effective
                         && client.Status == (int)Enums.Status.Normal
                         && company.Status == (int)Enums.Status.Normal
                         && clientAdmin.Status == (int)Enums.Status.Normal && clientAdmin.Type == (int)Enums.ClientAdminType.Merchandiser

                         select new AdvanceMoneyApplyListViewModel
                         {
                             AdvanceMoneyApplyID = advanceMoneyApply.ID,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID,
                             ClientRank = (Enums.ClientRank)client.ClientRank,
                             AdvanceMoneyApplyStatus = (Enums.AdvanceMoneyStatus)advanceMoneyApply.Status,
                             IntAdvanceMoneyApplyStatus = advanceMoneyApply.Status,
                             AdminID = advanceMoneyApply.AdminID,
                             CreateDate = advanceMoneyApply.CreateDate,
                             Summary = advanceMoneyApply.Summary,
                             ClientNature = client.ClientNature == null ? (int)Enums.ClientNature.Trade : (int)client.ClientNature,//(ClientNature)client.ClientNature ,
                             MerchandiserName = clientAdmin.AdminID,
                             IsSAPaperUpload = advanceMoneyApply.IsPaperUpload == (int)Enums.SAUploadStatus.Uploaded ? true : false,
                             Amount = advanceMoneyApply.Amount,
                             LimitDays = advanceMoneyApply.LimitDays,
                             InterestRate = advanceMoneyApply.InterestRate,
                             OverdueInterestRate = advanceMoneyApply.OverdueInterestRate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<AdvanceMoneyApplyListViewModel> iquery = this.IQueryable.Cast<AdvanceMoneyApplyListViewModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_advanceMoneyApplies = iquery.ToArray();

            var advanceMoneyApplyIDs = ienum_advanceMoneyApplies.Select(item => item.AdvanceMoneyApplyID);
            // var clientsID = ienum_agreementChangeApplies.Select(item => item.ClientID);
            var merchandiserNames = ienum_advanceMoneyApplies.Select(item => item.MerchandiserName);
            var serviceManagerIDs = ienum_advanceMoneyApplies.Select(item => item.AdminID);

            #region 垫资申请(电子)文件

            var filesDescriptionTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>();

            var linq_eleSACount = from file in filesDescriptionTopView
                                  where advanceMoneyApplyIDs.Contains(file.ApplicationID)
                                     && file.Type == (int)Enums.FileType.AdvanceMoneyApplyAgreement
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

            var ienums_linq = from advanceMoneyApply in ienum_advanceMoneyApplies
                              join merchandiser in ienums_merchandiserName on advanceMoneyApply.MerchandiserName equals merchandiser.OriginID
                              into ienums_merchandiserName2
                              from merchandiser in ienums_merchandiserName2.DefaultIfEmpty()

                              join serviceManager in ienums_serviceManager on advanceMoneyApply.AdminID equals serviceManager.OriginID into ienums_serviceManager2
                              from serviceManager in ienums_serviceManager2.DefaultIfEmpty()

                              join eleSACount in ienums_eleSACount on advanceMoneyApply.AdvanceMoneyApplyID equals eleSACount.ApplicationID into ienums_eleSACount2
                              from eleSACount in ienums_eleSACount2.DefaultIfEmpty()
                              select new AdvanceMoneyApplyListViewModel
                              {
                                  AdvanceMoneyApplyID = advanceMoneyApply.AdvanceMoneyApplyID,
                                  ClientCode = advanceMoneyApply.ClientCode,
                                  ClientName = advanceMoneyApply.ClientName,
                                  ClientID = advanceMoneyApply.ClientID,
                                  AdminID = advanceMoneyApply.AdminID,
                                  ClientRank = advanceMoneyApply.ClientRank,
                                  AdvanceMoneyApplyStatus = advanceMoneyApply.AdvanceMoneyApplyStatus,
                                  Summary = advanceMoneyApply.Summary,
                                  ClientNature = advanceMoneyApply.ClientNature,
                                  MerchandiserName = merchandiser != null ? merchandiser.RealName : "",
                                  IntAdvanceMoneyApplyStatus = advanceMoneyApply.IntAdvanceMoneyApplyStatus,
                                  CreateDate = advanceMoneyApply.CreateDate,
                                  ServiceManager = serviceManager != null ? serviceManager.RealName : "",

                                  IsSAEleUpload = eleSACount != null ? true : false,
                                  SAUrl = eleSACount != null ? eleSACount.SAUrl : "",
                                  IsSAPaperUpload = advanceMoneyApply.IsSAPaperUpload,

                                  Amount = advanceMoneyApply.Amount,
                                  LimitDays = advanceMoneyApply.LimitDays,
                                  InterestRate = advanceMoneyApply.InterestRate,
                                  OverdueInterestRate = advanceMoneyApply.OverdueInterestRate,
                              };

            string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];

            Func<AdvanceMoneyApplyListViewModel, object> convert = item => new
            {
                AdvanceMoneyApplyID = item.AdvanceMoneyApplyID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                ClientID = item.ClientID,
                AdvanceMoneyApplyStatus = item.AdvanceMoneyApplyStatus.GetDescription(),
                Summary = item.Summary,
                MerchandiserName = item.MerchandiserName,
                IntStatus = item.IntAdvanceMoneyApplyStatus,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                ClientNature = ((Enums.ClientNature)item.ClientNature).GetDescription(),//item.ClientNature.GetDescription(),
                ClientRank = item.ClientRank.GetDescription(),
                ServiceManager = item.ServiceManager,

                IsSAEleUpload = item.IsSAEleUpload,
                IsSAEleUploadString = item.IsSAEleUpload ? "已上传" : "未上传",
                SAUrl = !string.IsNullOrEmpty(item.SAUrl) ? FileServerUrl + @"/" + item.SAUrl : "",
                IsSAPaperUpload = item.IsSAPaperUpload,
                IsSAPaperUploadString = item.IsSAPaperUpload ? "已上交" : "未上交",

                Amount = item.Amount,
                LimitDays = item.LimitDays,
                InterestRate = item.InterestRate,
                OverdueInterestRate = item.OverdueInterestRate,
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
        public AdvanceMoneyApplyListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请状态查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByApplyStatus(int advanceMoneyApplyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.AdvanceMoneyApplyStatus == (Enums.AdvanceMoneyStatus)advanceMoneyApplyStatus
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByCreateDateFrom(DateTime start)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= start
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期截止时间查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByCreateDateTo(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据业务员查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByServiceManager(string ServiceManager)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == ServiceManager
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyListView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new AdvanceMoneyApplyListView(this.Reponsitory, linq);
            return view;
        }


        public void SetSAUploaded(string advanceMoneyApplyID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                {
                    IsPaperUpload = (int)Enums.SAUploadStatus.Uploaded,
                }, item => item.ID == advanceMoneyApplyID);
            }
        }

        public void SetSAUnUpload(string advanceMoneyApplyID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>(new
                {
                    IsPaperUpload = (int)Enums.SAUploadStatus.UnUpload,
                }, item => item.ID == advanceMoneyApplyID);
            }
        }
    }

    public class AdvanceMoneyApplyListViewModel
    {
        public string AdvanceMoneyApplyID { get; set; }

        public string ClientCode { get; set; }

        public string ClientName { get; set; }

        public string ClientID { get; set; }

        public Enums.ClientRank ClientRank { get; set; }

        public decimal Amount { get; set; }

        public decimal LimitDays { get; set; }

        public decimal InterestRate { get; set; }

        public decimal OverdueInterestRate { get; set; }
        public Enums.AdvanceMoneyStatus AdvanceMoneyApplyStatus { get; set; }

        public int IntAdvanceMoneyApplyStatus { get; set; }

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

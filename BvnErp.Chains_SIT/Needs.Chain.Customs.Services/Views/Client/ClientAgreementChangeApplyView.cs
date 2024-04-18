using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientAgreementChangeApplyView : UniqueView<Models.ClientAgreementChangeApplyModel, ScCustomsReponsitory>
    {
        public ClientAgreementChangeApplyView()
        {
        }
        internal ClientAgreementChangeApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.ClientAgreementChangeApplyModel> GetIQueryable()
        {
            var result = from agreementChangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>()
                         where agreementChangeApply.Status != (int)AgreementChangeApplyStatus.Delete && agreementChangeApply.Status != (int)AgreementChangeApplyStatus.Effective
                         select new Models.ClientAgreementChangeApplyModel
                         {
                             ID = agreementChangeApply.ID,
                             ClientID = agreementChangeApply.ClientID,
                             Status = (AgreementChangeApplyStatus)agreementChangeApply.Status,
                             AdminID = agreementChangeApply.AdminID,
                             CreateDate = agreementChangeApply.CreateDate,
                             Summary = agreementChangeApply.Summary
                         };

            return result;
        }
    }
    public class AgreementChangeAppliesView : UniqueView<Models.ClientAgreementChangeApplyModel, ScCustomsReponsitory>
    {
        public AgreementChangeAppliesView()
        {
        }
        internal AgreementChangeAppliesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.ClientAgreementChangeApplyModel> GetIQueryable()
        {
            //var adminsView = new AdminsTopView(this.Reponsitory).ToArray();

            var result = from agreementChangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>()
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on agreementChangeApply.ClientID equals client.ID
                         join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                         join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on agreementChangeApply.AdminID equals admin.ID
                         join clientAgreement in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>() on client.ID equals clientAgreement.ClientID
                         where client.Status == (int)Status.Normal && company.Status == (int)Status.Normal && clientAgreement.Status == (int)Status.Normal
                         select new Models.ClientAgreementChangeApplyModel
                         {
                             ID = agreementChangeApply.ID,
                             ClientID = agreementChangeApply.ClientID,
                             AdminID = admin.RealName,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientRank = (ClientRank)client.ClientRank,
                             Address = company.Address,
                             Corporate = company.Corporate,
                             StartDate = clientAgreement.StartDate.ToString(),
                             Status = (AgreementChangeApplyStatus)agreementChangeApply.Status
                         };

            return result;
        }
    }
    public class AgreementChangeApplyListView : QueryView<ClientAgreementChangeApplyModel, ScCustomsReponsitory>
    {
        public AgreementChangeApplyListView()
        {
        }
        internal AgreementChangeApplyListView(ScCustomsReponsitory reponsitory, IQueryable<ClientAgreementChangeApplyModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<ClientAgreementChangeApplyModel> GetIQueryable()
        {
            var result = from agreementChangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>()
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on agreementChangeApply.ClientID equals client.ID
                         join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                         join clientAdmin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>() on client.ID equals clientAdmin.ClientID
                         where client.IsValid == true
                         && client.Status == (int)Status.Normal
                         && company.Status == (int)Status.Normal
                         && clientAdmin.Status == (int)Status.Normal && clientAdmin.Type == (int)ClientAdminType.Merchandiser

                         select new Models.ClientAgreementChangeApplyModel
                         {
                             ID = agreementChangeApply.ID,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID,
                             ClientRank = (ClientRank)client.ClientRank,
                             Status = (AgreementChangeApplyStatus)agreementChangeApply.Status,
                             IntStatus = agreementChangeApply.Status,
                             AdminID = agreementChangeApply.AdminID,
                             CreateDate = agreementChangeApply.CreateDate,
                             Summary = agreementChangeApply.Summary,
                             ClientNature = client.ClientNature == null ? (int)ClientNature.Trade : (int)client.ClientNature,//(ClientNature)client.ClientNature ,
                             MerchandiserName = clientAdmin.AdminID
                         };

            return result;
        }
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ClientAgreementChangeApplyModel> iquery = this.IQueryable.Cast<ClientAgreementChangeApplyModel>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum = iquery.ToArray();
            var adminsView = new AdminsTopView(this.Reponsitory).ToArray();

            var serviceFile = new CenterFilesTopView().Where(x => x.Type == (int)Needs.Ccs.Services.Enums.FileType.ChangeServiceAgreement && x.Status != FileDescriptionStatus.Delete).ToArray();

            var ienums_linq = from ienums in ienum
                              join admin in adminsView on ienums.MerchandiserName equals admin.ID
                              join file in serviceFile on ienums.ID equals file.ApplicationID into files
                              from file in files.DefaultIfEmpty()
                              select new ClientAgreementChangeApplyModel
                              {
                                  ID = ienums.ID,
                                  ClientCode = ienums.ClientCode,
                                  ClientName = ienums.ClientName,
                                  ClientID = ienums.ClientID,
                                  AdminID = ienums.AdminID,
                                  ClientRank = ienums.ClientRank,
                                  Status = ienums.Status,
                                  Summary = ienums.Summary,
                                  ClientNature = ienums.ClientNature,
                                  MerchandiserName = admin.RealName,
                                  IntStatus = ienums.IntStatus,
                                  CreateDate = ienums.CreateDate,
                                  CustomName = file == null ? "否" : "是",

                              };

            var ienums_linq1 = ienums_linq;

            var ienums_linq2 = from ienums in ienums_linq1
                               join admin in adminsView on ienums.AdminID equals admin.ID
                               select new ClientAgreementChangeApplyModel
                               {
                                   ID = ienums.ID,
                                   ClientCode = ienums.ClientCode,
                                   ClientName = ienums.ClientName,
                                   ClientID = ienums.ClientID,
                                   AdminID = ienums.AdminID,
                                   ClientRank = ienums.ClientRank,
                                   Status = ienums.Status,
                                   Summary = ienums.Summary,
                                   ClientNature = ienums.ClientNature,
                                   MerchandiserName = ienums.MerchandiserName,
                                   IntStatus = ienums.IntStatus,
                                   CreateDate = ienums.CreateDate,
                                   CustomName = ienums.CustomName,
                                   ServiceManager = admin.RealName
                               };

            //业务员部门
            var ienums_depart = from adminwl in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>()
                                join depart in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Departments>() on adminwl.DepartmentID equals depart.ID into temp_depart
                                from depart in temp_depart.DefaultIfEmpty()
                                select new
                                {
                                    AdminID = adminwl.AdminID,
                                    DepartmentCode = depart == null ? "" : depart.Name
                                };

            var arr_depart = ienums_depart.ToArray();

            var ienums_linq3 = from ienums in ienums_linq2
                               join depart in arr_depart on ienums.AdminID equals depart.AdminID
                               select new ClientAgreementChangeApplyModel
                               {
                                   ID = ienums.ID,
                                   ClientCode = ienums.ClientCode,
                                   ClientName = ienums.ClientName,
                                   ClientID = ienums.ClientID,
                                   AdminID = ienums.AdminID,
                                   ClientRank = ienums.ClientRank,
                                   Status = ienums.Status,
                                   Summary = ienums.Summary,
                                   ClientNature = ienums.ClientNature,
                                   MerchandiserName = ienums.MerchandiserName,
                                   IntStatus = ienums.IntStatus,
                                   CreateDate = ienums.CreateDate,
                                   CustomName = ienums.CustomName,
                                   ServiceManager = ienums.ServiceManager,
                                   DepartmentCode = depart.DepartmentCode
                               };

            var results = ienums_linq3;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ClientAgreementChangeApplyModel, object> convert = item => new
            {
                ID = item.ID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                ClientID = item.ClientID,
                Status = item.Status.GetDescription(),
                Summary = item.Summary,
                MerchandiserName = item.MerchandiserName,
                IntStatus = item.IntStatus,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                ClientNature = ((ClientNature)item.ClientNature).GetDescription(),//item.ClientNature.GetDescription(),
                ClientRank = item.ClientRank.GetDescription(),
                CustomName = item.Status.GetDescription() == "已生效" ? item.CustomName : "否",
                ServiceManager = item.ServiceManager,
                DepartmentCode = item.DepartmentCode
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }
        /// <summary>
        ///  根据客户编号查询
        /// </summary>
        /// <param name="clientCode"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请状态查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByApplyStatus(int agreementChangeApplyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.Status == (AgreementChangeApplyStatus)agreementChangeApplyStatus
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByCreateDateFrom(DateTime start)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate >= start
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请日期截止时间查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByCreateDateTo(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.CreateDate < end
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据业务员查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByServiceManager(string ServiceManager)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == ServiceManager
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public AgreementChangeApplyListView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new AgreementChangeApplyListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class AgreementChangeApplyView : UniqueView<Models.ClientAgreementChangeApplyModel, ScCustomsReponsitory>
    {
        public AgreementChangeApplyView()
        {
        }
        internal AgreementChangeApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.ClientAgreementChangeApplyModel> GetIQueryable()
        {
            var result = from agreementChangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>()
                         join agreementChangeApplyItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplyItems>() on agreementChangeApply.ID equals agreementChangeApplyItem.ApplyID
                         where agreementChangeApplyItem.Status == (int)Status.Normal
                         select new Models.ClientAgreementChangeApplyModel
                         {
                             ID = agreementChangeApply.ID,
                             ClientID = agreementChangeApply.ClientID,
                             ApplyID = agreementChangeApplyItem.ApplyID,
                             AgreementChangeType = (AgreementChangeType)agreementChangeApplyItem.ChangeType,
                             OldValue = agreementChangeApplyItem.OldValue,
                             NewValue = agreementChangeApplyItem.NewValue
                         };

            return result;
        }
    }

    public class CheckAgreementChangeApplyView : UniqueView<Models.ClientAgreementChangeApplyModel, ScCustomsReponsitory>
    {
        public CheckAgreementChangeApplyView()
        {
        }
        internal CheckAgreementChangeApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.ClientAgreementChangeApplyModel> GetIQueryable()
        {
            var result = from agreementChangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AgreementChangeApplies>()
                         where agreementChangeApply.Status != (int)AgreementChangeApplyStatus.Delete && agreementChangeApply.Status != (int)AgreementChangeApplyStatus.Effective
                         select new Models.ClientAgreementChangeApplyModel
                         {
                             ID = agreementChangeApply.ID,
                             ClientID = agreementChangeApply.ClientID,
                             Status = (AgreementChangeApplyStatus)agreementChangeApply.Status,
                             AdminID = agreementChangeApply.AdminID,
                             CreateDate = agreementChangeApply.CreateDate,
                             Summary = agreementChangeApply.Summary
                         };

            return result;
        }
    }

    public class clientView : UniqueView<Models.Client, ScCustomsReponsitory>
    {
        public clientView()
        {
        }
        internal clientView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Client> GetIQueryable()
        {
            var result = from client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>()
                         where client.Status == (int)Status.Normal
                         select new Models.Client
                         {
                             ID = client.ID,
                             ClientStatus = (ClientStatus)client.ClientStatus,
                         };

            return result;
        }
    }
}

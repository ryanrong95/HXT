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
    public class CheckClientCodeView : UniqueView<Models.AdvanceMoneyApplyModel, ScCustomsReponsitory>
    {
        public CheckClientCodeView()
        {
        }
        internal CheckClientCodeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceMoneyApplyModel> GetIQueryable()
        {
            var result = from company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>()
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on company.ID equals client.CompanyID
                         where client.IsValid == true // && client.ServiceType == (int)ServiceType.Customs
                         && client.Status == (int)Status.Normal && company.Status == (int)Status.Normal
                         select new Models.AdvanceMoneyApplyModel
                         {
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID
                         };

            return result;
        }
    }
    public class CheckClientIDView : UniqueView<Models.AdvanceMoneyApplyModel, ScCustomsReponsitory>
    {
        public CheckClientIDView()
        {
        }
        internal CheckClientIDView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceMoneyApplyModel> GetIQueryable()
        {
            var result = from advanceMoneyApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                         where advanceMoneyApply.Status != (int)AdvanceMoneyStatus.Delete
                         select new Models.AdvanceMoneyApplyModel
                         {
                             ClientID = advanceMoneyApply.ClientID
                         };

            return result;
        }
    }
    public class CenterFilesView : UniqueView<Models.CenterFileModel, ScCustomsReponsitory>
    {
        public CenterFilesView()
        {
        }
        internal CenterFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.CenterFileModel> GetIQueryable()
        {
            var result = from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FilesDescriptionTopView>()
                         where file.Status != (int)FileDescriptionStatus.Delete
                         select new CenterFileModel
                         {
                             WsOrderID = file.WsOrderID,
                             LsOrderID = file.LsOrderID,
                             ApplicationID = file.ApplicationID,
                             WaybillID = file.WaybillID,
                             NoticeID = file.NoticeID,
                             StorageID = file.StorageID,
                             CustomName = file.CustomName,
                             Url = file.Url,
                             ClientID = file.ClientID,
                             AdminID = file.AdminID,
                             InputID = file.InputID,
                             PayID = file.PayID,
                             FileFormat = "",
                             FileType = (Enums.FileType)file.Type,
                             Status = (Enums.Status)file.Status,
                             CreateDate = file.CreateDate,
                             ID = file.ID
                         };

            return result;
        }
    }
    public class AdvanceMoneyApplyView : QueryView<AdvanceMoneyApplyModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyApplyView()
        {
        }
        internal AdvanceMoneyApplyView(ScCustomsReponsitory reponsitory, IQueryable<AdvanceMoneyApplyModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<AdvanceMoneyApplyModel> GetIQueryable()
        {
            var result = from advanceMoneyApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on advanceMoneyApply.ClientID equals client.ID
                         join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                         where client.IsValid == true //&& client.ServiceType == (int)ServiceType.Customs
                         && client.Status == (int)Status.Normal && company.Status == (int)Status.Normal
                         select new Models.AdvanceMoneyApplyModel
                         {
                             ID = advanceMoneyApply.ID,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID,
                             Amount = advanceMoneyApply.Amount,
                             AmountUsed = advanceMoneyApply.AmountUsed,
                             Status = (AdvanceMoneyStatus)advanceMoneyApply.Status,
                             IntStatus = advanceMoneyApply.Status,
                             LimitDays = advanceMoneyApply.LimitDays,
                             InterestRate = advanceMoneyApply.InterestRate,
                             OverdueInterestRate = advanceMoneyApply.OverdueInterestRate,
                             Summary = advanceMoneyApply.Summary,
                             AdminID = advanceMoneyApply.AdminID,
                             CreateDate = advanceMoneyApply.CreateDate
                         };

            return result;
        }
        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<AdvanceMoneyApplyModel> iquery = this.IQueryable.Cast<AdvanceMoneyApplyModel>().OrderByDescending(item => item.ClientID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum = iquery.ToArray();
            var adminsView = new AdminsTopView(this.Reponsitory).ToArray();
            var adminwlView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>();
            var separtmentView = new DepartmentView(this.Reponsitory);

            var ienums_linq = from ienums in ienum
                              join admin in adminsView on ienums.AdminID equals admin.ID
                              join adminwl in adminwlView on admin.ID equals adminwl.AdminID

                              join depart in separtmentView on adminwl.DepartmentID equals depart.ID into depart_temp
                              from depart in depart_temp.DefaultIfEmpty()

                              select new AdvanceMoneyApplyModel
                              {
                                  ID = ienums.ID,
                                  ClientCode = ienums.ClientCode,
                                  ClientName = ienums.ClientName,
                                  ClientID = ienums.ClientID,
                                  Amount = ienums.Amount,
                                  AmountUsed = ienums.AmountUsed,
                                  Status = ienums.Status,
                                  LimitDays = ienums.LimitDays,
                                  InterestRate = ienums.InterestRate,
                                  OverdueInterestRate = ienums.OverdueInterestRate,
                                  Summary = ienums.Summary,
                                  Name = admin.RealName,
                                  IntStatus = ienums.IntStatus,
                                  CreateDate = ienums.CreateDate,
                                  DepartmentCode = depart?.Name
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

            Func<AdvanceMoneyApplyModel, object> convert = item => new
            {
                ID = item.ID,
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,
                ClientID = item.ClientID,
                Amount = item.Amount,
                AmountUsed = item.AmountUsed,
                Status = item.Status.GetDescription(),
                LimitDays = item.LimitDays,
                InterestRate = Convert.ToDouble(item.InterestRate).ToString() + "%",
                OverdueInterestRate = Convert.ToDouble(item.OverdueInterestRate).ToString() + "%",
                Summary = item.Summary,
                Name = item.Name,
                IntStatus = item.IntStatus,
                CreateDate = item.CreateDate,
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
        public AdvanceMoneyApplyView SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new AdvanceMoneyApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据客户名称查询
        /// </summary>
        /// <param name="ClientName"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyView SearchByClientName(string ClientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(ClientName)
                       select query;

            var view = new AdvanceMoneyApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据申请状态查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyView SearchByApplyStatus(int advanceMoneyStatus)
        {
            var linq = from query in this.IQueryable
                       where query.Status == (AdvanceMoneyStatus)advanceMoneyStatus
                       select query;

            var view = new AdvanceMoneyApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据业务员查询
        /// </summary>
        /// <param name="advanceMoneyStatus"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyView SearchByServiceManager(string ServiceManager)
        {
            var linq = from query in this.IQueryable
                       where query.AdminID == ServiceManager
                       select query;

            var view = new AdvanceMoneyApplyView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public AdvanceMoneyApplyView SearchByClientAdmin(string adminID)
        {
            var clientIds = new ClientAdminsView().Where(t => t.Admin.ID == adminID).Select(t => t.ClientID).ToList();
            var linq = from query in this.IQueryable
                       where clientIds.Contains(query.ClientID)
                       select query;

            var view = new AdvanceMoneyApplyView(this.Reponsitory, linq);
            return view;
        }
    }

    public class PayExchangeApplyView : UniqueView<Models.PayExchangeApply, ScCustomsReponsitory>
    {
        public PayExchangeApplyView()
        {
        }
        internal PayExchangeApplyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.PayExchangeApply> GetIQueryable()
        {
            var result = from payExchangeApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                         where payExchangeApply.Status == (int)Status.Normal
                         select new Models.PayExchangeApply
                         {
                             ID = payExchangeApply.ID,
                             ClientID = payExchangeApply.ClientID,
                             IsAdvanceMoney = payExchangeApply.IsAdvanceMoney
                         };

            return result;
        }
    }

    public class AdvanceMoneyApplyView1 : UniqueView<Models.AdvanceMoneyApplyModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyApplyView1()
        {
        }
        internal AdvanceMoneyApplyView1(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceMoneyApplyModel> GetIQueryable()
        {
            var result = from advanceMoneyApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                         join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on advanceMoneyApply.ClientID equals client.ID
                         join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                         where client.IsValid == true && client.Status == (int)Status.Normal && company.Status == (int)Status.Normal
                         select new Models.AdvanceMoneyApplyModel
                         {
                             ID = advanceMoneyApply.ID,
                             ClientCode = client.ClientCode,
                             ClientName = company.Name,
                             ClientID = client.ID,
                             Amount = advanceMoneyApply.Amount,
                             AmountUsed = advanceMoneyApply.AmountUsed,
                             Status = (AdvanceMoneyStatus)advanceMoneyApply.Status,
                             IntStatus = advanceMoneyApply.Status,
                             LimitDays = advanceMoneyApply.LimitDays,
                             InterestRate = advanceMoneyApply.InterestRate,
                             OverdueInterestRate = advanceMoneyApply.OverdueInterestRate,
                             Summary = advanceMoneyApply.Summary,
                             AdminID = advanceMoneyApply.AdminID,
                             CreateDate = advanceMoneyApply.CreateDate
                         };

            return result;
        }

    }
    public class AdvanceMoneyRecordView : UniqueView<Models.AdvanceRecordModel, ScCustomsReponsitory>
    {
        public AdvanceMoneyRecordView()
        {
        }
        internal AdvanceMoneyRecordView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.AdvanceRecordModel> GetIQueryable()
        {
            var result = from advanceRecord in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>()
                         select new Models.AdvanceRecordModel
                         {
                             ID = advanceRecord.ID,
                             OrderID = advanceRecord.OrderID,
                             ClientID = advanceRecord.ClientID,
                             Amount = advanceRecord.Amount,
                             PaidAmount = advanceRecord.PaidAmount,
                             AdvanceTime = advanceRecord.AdvanceTime,
                             LimitDays = advanceRecord.LimitDays,
                             PayExchangeID = advanceRecord.PayExchangeID
                         };

            return result;
        }
    }
}

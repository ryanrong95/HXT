using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AttachApprovalApprovedListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public AttachApprovalApprovedListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public AttachApprovalApprovedListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        #region 审批者 已审核列表

        private IQueryable<AttachApprovalApprovedListViewModel> GetAllProvedForApprover(LambdaExpression[] expressions)
        {
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            var adminTopViewApplicant = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var adminTopViewApprove = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            Enums.OrderControlType[] needOrderControlTypes =
            {
                Enums.OrderControlType.GenerateBillApproval,
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            int[] needControlType = needOrderControlTypes.Select(t => (int)t).ToArray();

            var results = from orderControl in orderControls
                          join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID

                          join adminApplicant in adminTopViewApplicant on orderControl.Applicant equals adminApplicant.ID
                          join approveAdmin in adminTopViewApprove on orderControlStep.AdminID equals approveAdmin.ID into adminTopViewApprove2
                          from approveAdmin in adminTopViewApprove2.DefaultIfEmpty()

                          where needControlType.Contains(orderControl.ControlType)
                             && orderControl.Status == (int)Enums.Status.Normal

                             && orderControlStep.Step == (int)Enums.OrderControlStep.Headquarters
                             && (orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Approved || orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Rejected)
                             && orderControlStep.Status == (int)Enums.Status.Normal

                             && approveAdmin.Status == (int)Enums.Status.Normal

                          orderby orderControl.UpdateDate descending
                          select new AttachApprovalApprovedListViewModel
                          {
                              OrderControlStepID = orderControlStep.ID,
                              OrderControlID = orderControl.ID,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                              CreateDate = orderControl.CreateDate,
                              UpdateDate = orderControl.UpdateDate,
                              EventInfo = orderControl.EventInfo,
                              ApplicantID = orderControl.Applicant,
                              ApplicantName = adminApplicant.RealName,
                              //ClientName = company.Name,
                              //Currency = order.Currency,
                              //DeclarePrice = order.DeclarePrice,
                              ApproveAdminName = approveAdmin != null ? approveAdmin.RealName : string.Empty,
                              OrderControlStatus = (Enums.OrderControlStatus)orderControlStep.ControlStatus,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<AttachApprovalApprovedListViewModel, bool>>);
            }

            return results;
        }

        public List<AttachApprovalApprovedListViewModel> GetResultForApprover(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allProved = GetAllProvedForApprover(expressions);

            totalCount = allProved.Count();

            var approvedLists = allProved.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            // 通过MainOrderID 获取信息 Begin

            Enums.OrderControlType[] byMainOrderIDTypes = { Enums.OrderControlType.GenerateBillApproval, };
            string[] mainOrderIDs = approvedLists.Where(t => byMainOrderIDTypes.Contains(t.ControlType)).Select(t => t.MainOrderID).ToArray();
            var infosByMainOrderID = GetOtherInfoByMainOrderID(mainOrderIDs);

            Enums.OrderControlType[] byTinyOrderIDTypes = 
            {
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            string[] tinyOrderIDs = approvedLists.Where(t => byTinyOrderIDTypes.Contains(t.ControlType)).Select(t => t.TinyOrderID).ToArray();
            var infosByTinyOrderID = GetOtherInfoByTinyOrderID(tinyOrderIDs);

            approvedLists = (from approvedList in approvedLists
                             join infoMain in infosByMainOrderID on approvedList.MainOrderID equals infoMain.MainOrderID into infosByMainOrderID2
                             from infoMain in infosByMainOrderID2.DefaultIfEmpty()
                             join infoTiny in infosByTinyOrderID on approvedList.TinyOrderID equals infoTiny.TinyOrderID into infosByTinyOrderID2
                             from infoTiny in infosByTinyOrderID2.DefaultIfEmpty()
                             select new AttachApprovalApprovedListViewModel
                             {
                                 OrderControlStepID = approvedList.OrderControlStepID,
                                 OrderControlID = approvedList.OrderControlID,
                                 MainOrderID = approvedList.MainOrderID,
                                 TinyOrderID = approvedList.TinyOrderID,
                                 ControlType = approvedList.ControlType,
                                 CreateDate = approvedList.CreateDate,
                                 EventInfo = approvedList.EventInfo,
                                 ApplicantID = approvedList.ApplicantID,
                                 ApplicantName = approvedList.ApplicantName,
                                 ApproveAdminName = approvedList.ApproveAdminName,
                                 OrderControlStatus = approvedList.OrderControlStatus,

                                 ClientName = infoMain != null ? infoMain.ClientName : (infoTiny != null ? infoTiny.ClientName : string.Empty),
                                 Currency = infoMain != null ? infoMain.Currency : (infoTiny != null ? infoTiny.Currency : string.Empty),
                                 DeclarePrice = infoMain != null ? infoMain.DeclarePrice : (infoTiny != null ? infoTiny.DeclarePrice : 0),
                             }).ToList();

            // 通过MainOrderID 获取信息 End

            return approvedLists;
        }

        #endregion


        #region 申请人 已审核列表

        private IQueryable<AttachApprovalApprovedListViewModel> GetAllProvedForApplicant(LambdaExpression[] expressions)
        {
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            var adminsTopViewApplicant = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            Enums.OrderControlType[] needOrderControlTypes =
            {
                Enums.OrderControlType.GenerateBillApproval,
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            int[] needControlType = needOrderControlTypes.Select(t => (int)t).ToArray();

            var results = from orderControl in orderControls
                          join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                          join adminApplicant in adminsTopViewApplicant on orderControl.Applicant equals adminApplicant.ID

                          where needControlType.Contains(orderControl.ControlType)
                             && orderControl.Status == (int)Enums.Status.Normal

                             && orderControlStep.Step == (int)Enums.OrderControlStep.Headquarters
                             && (orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Approved
                              || orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Rejected
                              || orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Cancel)
                             && orderControlStep.Status == (int)Enums.Status.Normal

                          orderby orderControl.UpdateDate descending
                          select new AttachApprovalApprovedListViewModel
                          {
                              OrderControlStepID = orderControlStep.ID,
                              OrderControlID = orderControl.ID,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                              CreateDate = orderControl.CreateDate,
                              UpdateDate = orderControl.UpdateDate,
                              EventInfo = orderControl.EventInfo,
                              ApplicantID = orderControl.Applicant,
                              ApplicantName = adminApplicant.RealName,
                              OrderControlStatus = (Enums.OrderControlStatus)orderControlStep.ControlStatus,
                              ApproveAdminID = orderControlStep.AdminID,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<AttachApprovalApprovedListViewModel, bool>>);
            }

            return results;
        }

        public List<AttachApprovalApprovedListViewModel> GetResultForApplicant(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allProved = GetAllProvedForApplicant(expressions);

            totalCount = allProved.Count();

            var approvedLists = allProved.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            // 通过 MainOrderID 获取信息、获取 ApproveAdminName Begin

            Enums.OrderControlType[] byMainOrderIDTypes = { Enums.OrderControlType.GenerateBillApproval, };
            string[] mainOrderIDs = approvedLists.Where(t => byMainOrderIDTypes.Contains(t.ControlType)).Select(t => t.MainOrderID).ToArray();
            var infosByMainOrderID = GetOtherInfoByMainOrderID(mainOrderIDs);

            Enums.OrderControlType[] byTinyOrderIDTypes = 
            {
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            string[] tinyOrderIDs = approvedLists.Where(t => byTinyOrderIDTypes.Contains(t.ControlType)).Select(t => t.TinyOrderID).ToArray();
            var infosByTinyOrderID = GetOtherInfoByTinyOrderID(tinyOrderIDs);

            string[] approveAdminIDs = approvedLists.Select(t => t.ApproveAdminID).ToArray();
            var approveAdminNames = GetApproveAdminNameByAdminID(approveAdminIDs);

            approvedLists = (from approvedList in approvedLists
                             join infoMain in infosByMainOrderID on approvedList.MainOrderID equals infoMain.MainOrderID into infosByMainOrderID2
                             from infoMain in infosByMainOrderID2.DefaultIfEmpty()
                             join infoTiny in infosByTinyOrderID on approvedList.TinyOrderID equals infoTiny.TinyOrderID into infosByTinyOrderID2
                             from infoTiny in infosByTinyOrderID2.DefaultIfEmpty()
                             join approveAdminName in approveAdminNames on approvedList.ApproveAdminID equals approveAdminName.ApproveAdminID into approveAdminNames2
                             from approveAdminName in approveAdminNames2.DefaultIfEmpty()

                             select new AttachApprovalApprovedListViewModel
                             {
                                 OrderControlStepID = approvedList.OrderControlStepID,
                                 OrderControlID = approvedList.OrderControlID,
                                 MainOrderID = approvedList.MainOrderID,
                                 TinyOrderID = approvedList.TinyOrderID,
                                 ControlType = approvedList.ControlType,
                                 CreateDate = approvedList.CreateDate,
                                 EventInfo = approvedList.EventInfo,
                                 ApplicantID = approvedList.ApplicantID,
                                 ApplicantName = approvedList.ApplicantName,
                                 OrderControlStatus = approvedList.OrderControlStatus,
                                 ApproveAdminID = approvedList.ApproveAdminID,

                                 ClientName = infoMain != null ? infoMain.ClientName : (infoTiny != null ? infoTiny.ClientName : string.Empty),
                                 Currency = infoMain != null ? infoMain.Currency : (infoTiny != null ? infoTiny.Currency : string.Empty),
                                 DeclarePrice = infoMain != null ? infoMain.DeclarePrice : (infoTiny != null ? infoTiny.DeclarePrice : 0),

                                 ApproveAdminName = approveAdminName != null ? approveAdminName.ApproveAdminName : string.Empty,
                             }).ToList();

            // 通过 MainOrderID 获取信息、获取 ApproveAdminName End

            return approvedLists;
        }

        #endregion

        private List<AttachApprovalApprovedListViewModel> GetOtherInfoByMainOrderID(string[] orderMainIDs)
        {
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var results = from order in orders
                          join client in clients on order.ClientID equals client.ID
                          join company in companies on client.CompanyID equals company.ID

                          where orderMainIDs.Contains(order.MainOrderId)
                             && order.Status == (int)Enums.Status.Normal
                             && client.Status == (int)Enums.Status.Normal
                             && company.Status == (int)Enums.Status.Normal

                          select new AttachApprovalApprovedListViewModel
                          {
                              MainOrderID = order.MainOrderId,
                              ClientName = company.Name,
                              Currency = order.Currency,
                              DeclarePrice = order.DeclarePrice,
                          };

            results = from result in results
                      group result by new { result.MainOrderID } into g
                      select new AttachApprovalApprovedListViewModel
                      {
                          MainOrderID = g.Key.MainOrderID,
                          ClientName = g.FirstOrDefault().ClientName,
                          Currency = g.FirstOrDefault().Currency,
                          DeclarePrice = g.Sum(t => t.DeclarePrice),
                      };

            return results.ToList();
        }

        private List<AttachApprovalApprovedListViewModel> GetOtherInfoByTinyOrderID(string[] tinyOrderIDs)
        {
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var results = from order in orders
                          join client in clients on order.ClientID equals client.ID
                          join company in companies on client.CompanyID equals company.ID

                          where tinyOrderIDs.Contains(order.ID)
                             && order.Status == (int)Enums.Status.Normal
                             && client.Status == (int)Enums.Status.Normal
                             && company.Status == (int)Enums.Status.Normal

                          select new AttachApprovalApprovedListViewModel
                          {
                              TinyOrderID = order.ID,
                              ClientName = company.Name,
                              Currency = order.Currency,
                              DeclarePrice = order.DeclarePrice,
                          };

            return results.ToList();
        }

        private List<AttachApprovalApprovedListViewModel> GetApproveAdminNameByAdminID(string[] adminIDs)
        {
            var adminsTopView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            return (from admin in adminsTopView
                    where adminIDs.Contains(admin.ID)
                    select new AttachApprovalApprovedListViewModel
                    {
                        ApproveAdminID = admin.ID,
                        ApproveAdminName = admin.RealName,
                    }).ToList();
        }
    }

    public class AttachApprovalApprovedListViewModel
    {
        /// <summary>
        /// OrderControlStepID
        /// </summary>
        public string OrderControlStepID { get; set; } = string.Empty;

        /// <summary>
        /// OrderControlID
        /// </summary>
        public string OrderControlID { get; set; } = string.Empty;

        /// <summary>
        /// MainOrderID
        /// </summary>
        public string MainOrderID { get; set; } = string.Empty;

        /// <summary>
        /// OrderID
        /// </summary>
        public string TinyOrderID { get; set; } = string.Empty;

        /// <summary>
        /// 管控类型(审核类型)
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 产生时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// EventInfo (Json字符串)
        /// </summary>
        public string EventInfo { get; set; }

        /// <summary>
        /// 审批申请人ID
        /// </summary>
        public string ApplicantID { get; set; }

        /// <summary>
        /// 审批申请人姓名
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproveAdminID { get; set; }

        /// <summary>
        /// 审批人姓名
        /// </summary>
        public string ApproveAdminName { get; set; }

        /// <summary>
        /// 审批结果
        /// </summary>
        public Enums.OrderControlStatus OrderControlStatus { get; set; }
    }

}

using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 审批重新生成对账单信息视图
    /// </summary>
    public class ApproveGenerateBillInfoView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public ApproveGenerateBillInfoView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public ApproveGenerateBillInfoView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private ApproveGenerateBillInfoViewModel GetInfoByMainOrderID(string mainOrderID)
        {
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var results = (from order in orders
                           join client in clients on order.ClientID equals client.ID
                           join company in companies on client.CompanyID equals company.ID

                           where order.MainOrderId == mainOrderID
                              && order.Status == (int)Enums.Status.Normal
                              && client.Status == (int)Enums.Status.Normal
                              && company.Status == (int)Enums.Status.Normal
                           select new ApproveGenerateBillInfoViewModel
                           {
                               MainOrderID = order.MainOrderId,
                               ClientName = company.Name,
                               Currency = order.Currency,
                               DeclarePrice = order.DeclarePrice,
                           }).ToList();

            var info = (from result in results
                        group result by new { result.MainOrderID } into g
                        select new ApproveGenerateBillInfoViewModel
                        {
                            MainOrderID = g.Key.MainOrderID,
                            ClientName = g.FirstOrDefault().ClientName,
                            Currency = g.FirstOrDefault().Currency,
                            DeclarePrice = g.Sum(t =>t.DeclarePrice),
                        }).FirstOrDefault();

            return info;
        }

        private ApproveGenerateBillInfoViewModel GetInfoByTinyOrderID(string tinyOrderID)
        {
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var result = (from order in orders
                           join client in clients on order.ClientID equals client.ID
                           join company in companies on client.CompanyID equals company.ID

                           where order.ID == tinyOrderID
                              && order.Status == (int)Enums.Status.Normal
                              && client.Status == (int)Enums.Status.Normal
                              && company.Status == (int)Enums.Status.Normal
                           select new ApproveGenerateBillInfoViewModel
                           {
                               TinyOrderID = order.ID,
                               ClientName = company.Name,
                               Currency = order.Currency,
                               DeclarePrice = order.DeclarePrice,
                           }).FirstOrDefault();

            return result;
        }

        public ApproveGenerateBillInfoViewModel GetUnApproveResult(string orderControlStepID)
        {
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var adminsTopViewApplicants = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            var result = (from orderControlStep in orderControlSteps
                          join orderControl in orderControls on orderControlStep.OrderControlID equals orderControl.ID
                          join adminApplicant in adminsTopViewApplicants on orderControl.Applicant equals adminApplicant.ID

                          where orderControlStep.ID == orderControlStepID
                             && orderControlStep.Status == (int)Enums.Status.Normal
                             && orderControl.Status == (int)Enums.Status.Normal

                          select new ApproveGenerateBillInfoViewModel
                          {
                              OrderControlStepID = orderControlStep.ID,
                              OrderControlID = orderControl.ID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              ApplicantName = adminApplicant.RealName,
                              EventInfo = orderControl.EventInfo,
                              ReferenceInfo = orderControlStep.ReferenceInfo,
                              ApproveAdminID = orderControlStep.AdminID,
                              ApproveReason = orderControlStep.ApproveReason,
                              OrderControlStatus = (Enums.OrderControlStatus)orderControlStep.ControlStatus,
                          }).FirstOrDefault();

            switch (result.ControlType)
            {
                case Enums.OrderControlType.GenerateBillApproval:
                    string mainOrderID = result.MainOrderID;
                    var info = GetInfoByMainOrderID(mainOrderID);
                    result.ClientName = info.ClientName;
                    result.Currency = info.Currency;
                    result.DeclarePrice = info.DeclarePrice;
                    break;
                case Enums.OrderControlType.DeleteModelApproval:
                case Enums.OrderControlType.ChangeQuantityApproval:
                case Enums.OrderControlType.SplitOrderApproval:
                    string tinyOrderID = result.TinyOrderID;
                    var info2 = GetInfoByTinyOrderID(tinyOrderID);
                    result.ClientName = info2.ClientName;
                    result.Currency = info2.Currency;
                    result.DeclarePrice = info2.DeclarePrice;
                    break;
                default:
                    break;
            }

            return result;
        }

        public ApproveGenerateBillInfoViewModel GetApprovedResult(string orderControlStepID)
        {
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var adminsTopViewApplicants = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var adminsTopViewApprove = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            var result = (from orderControlStep in orderControlSteps
                          join orderControl in orderControls on orderControlStep.OrderControlID equals orderControl.ID

                          join adminApplicant in adminsTopViewApplicants on orderControl.Applicant equals adminApplicant.ID
                          join approveAdmin in adminsTopViewApprove on orderControlStep.AdminID equals approveAdmin.ID into adminsTopViewApprove2
                          from approveAdmin in adminsTopViewApprove2.DefaultIfEmpty()

                          where orderControlStep.ID == orderControlStepID
                             && orderControlStep.Status == (int)Enums.Status.Normal
                             && orderControl.Status == (int)Enums.Status.Normal

                          select new ApproveGenerateBillInfoViewModel
                          {
                              OrderControlStepID = orderControlStep.ID,
                              OrderControlID = orderControl.ID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              ApplicantName = adminApplicant.RealName,
                              EventInfo = orderControl.EventInfo,
                              ReferenceInfo = orderControlStep.ReferenceInfo,
                              ApproveAdminID = orderControlStep.AdminID,
                              ApproveAdminName = approveAdmin != null ? approveAdmin.RealName : string.Empty,
                              ApproveReason = orderControlStep.ApproveReason,
                              OrderControlStatus = (Enums.OrderControlStatus)orderControlStep.ControlStatus,
                          }).FirstOrDefault();

            switch (result.ControlType)
            {
                case Enums.OrderControlType.GenerateBillApproval:
                    string mainOrderID = result.MainOrderID;
                    var info = GetInfoByMainOrderID(mainOrderID);
                    result.ClientName = info.ClientName;
                    result.Currency = info.Currency;
                    result.DeclarePrice = info.DeclarePrice;
                    break;
                case Enums.OrderControlType.DeleteModelApproval:
                case Enums.OrderControlType.ChangeQuantityApproval:
                case Enums.OrderControlType.SplitOrderApproval:
                    string tinyOrderID = result.TinyOrderID;
                    var info2 = GetInfoByTinyOrderID(tinyOrderID);
                    result.ClientName = info2.ClientName;
                    result.Currency = info2.Currency;
                    result.DeclarePrice = info2.DeclarePrice;
                    break;
                default:
                    break;
            }

            return result;
        }

    }

    public class ApproveGenerateBillInfoViewModel
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
        /// 管控类型(审核类型)
        /// </summary>
        public Enums.OrderControlType ControlType { get; set; }

        /// <summary>
        /// 审批申请人姓名
        /// </summary>
        public string ApplicantName { get; set; }

        /// <summary>
        /// MainOrderID
        /// </summary>
        public string MainOrderID { get; set; } = string.Empty;

        /// <summary>
        /// TinyOrderID
        /// </summary>
        public string TinyOrderID { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; } = string.Empty;

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 申请时间信息
        /// </summary>
        public string EventInfo { get; set; } = string.Empty;

        /// <summary>
        /// 参考信息
        /// </summary>
        public string ReferenceInfo { get; set; } = string.Empty;

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproveAdminID { get; set; } = string.Empty;

        /// <summary>
        /// 审批人姓名
        /// </summary>
        public string ApproveAdminName { get; set; } = string.Empty;

        /// <summary>
        /// 审批原因
        /// </summary>
        public string ApproveReason { get; set; } = string.Empty;

        /// <summary>
        /// 审批状态
        /// </summary>
        public Enums.OrderControlStatus OrderControlStatus { get; set; }
    }
}

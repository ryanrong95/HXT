using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 增加审批-未审批列表
    /// </summary>
    public class AttachApprovalUnApprovedListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public AttachApprovalUnApprovedListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public AttachApprovalUnApprovedListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }


        private IQueryable<AttachApprovalUnApprovedListViewModel> GetAllUnProved(LambdaExpression[] expressions)
        {
            var orderControls = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            //var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            //var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            //var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

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
                          //join order in orders on orderControl.OrderID equals order.ID
                          //join client in clients on order.ClientID equals client.ID
                          //join company in companies on client.CompanyID equals company.ID
                          join admin in adminsTopView on orderControl.Applicant equals admin.ID

                          where needControlType.Contains(orderControl.ControlType)
                             && orderControl.Status == (int)Enums.Status.Normal

                             && orderControlStep.Step == (int)Enums.OrderControlStep.Headquarters
                             && orderControlStep.ControlStatus == (int)Enums.OrderControlStatus.Auditing
                             && orderControlStep.Status == (int)Enums.Status.Normal

                          //&& order.Status == (int)Enums.Status.Normal
                          //&& client.Status == (int)Enums.Status.Normal
                          //&& company.Status == (int)Enums.Status.Normal
                          orderby orderControl.CreateDate
                          select new AttachApprovalUnApprovedListViewModel
                          {
                              OrderControlStepID = orderControlStep.ID,
                              OrderControlID = orderControl.ID,
                              MainOrderID = orderControl.MainOrderID,
                              TinyOrderID = orderControl.OrderID,
                              ControlType = (Enums.OrderControlType)orderControl.ControlType,
                              CreateDate = orderControl.CreateDate,
                              EventInfo = orderControl.EventInfo,
                              ApplicantID = orderControl.Applicant,
                              ApplicantName = admin.RealName,
                              //ClientName = company.Name,
                              //Currency = order.Currency,
                              //DeclarePrice = order.DeclarePrice,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<AttachApprovalUnApprovedListViewModel, bool>>);
            }

            return results;
        }

        private List<AttachApprovalUnApprovedListViewModel> GetOtherInfoByMainOrderID(string[] orderMainIDs)
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

                          select new AttachApprovalUnApprovedListViewModel
                          {
                              MainOrderID = order.MainOrderId,
                              ClientName = company.Name,
                              Currency = order.Currency,
                              DeclarePrice = order.DeclarePrice,
                          };

            results = from result in results
                      group result by new { result.MainOrderID } into g
                      select new AttachApprovalUnApprovedListViewModel
                      {
                          MainOrderID = g.Key.MainOrderID,
                          ClientName = g.FirstOrDefault().ClientName,
                          Currency = g.FirstOrDefault().Currency,
                          DeclarePrice = g.Sum(t => t.DeclarePrice),
                      };

            return results.ToList();
        }

        private List<AttachApprovalUnApprovedListViewModel> GetOtherInfoByTinyOrderID(string[] tinyOrderID)
        {
            var orders = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var results = from order in orders
                          join client in clients on order.ClientID equals client.ID
                          join company in companies on client.CompanyID equals company.ID

                          where tinyOrderID.Contains(order.ID)
                             && order.Status == (int)Enums.Status.Normal
                             && client.Status == (int)Enums.Status.Normal
                             && company.Status == (int)Enums.Status.Normal

                          select new AttachApprovalUnApprovedListViewModel
                          {
                              TinyOrderID = order.ID,
                              ClientName = company.Name,
                              Currency = order.Currency,
                              DeclarePrice = order.DeclarePrice,
                          };

            return results.ToList();
        }

        public List<AttachApprovalUnApprovedListViewModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var allUnProved = GetAllUnProved(expressions);

            totalCount = allUnProved.Count();

            var unApprovedLists = allUnProved.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            // 通过 MainOrderID、TinyOrderID 获取信息 Begin

            Enums.OrderControlType[] byMainOrderIDTypes = { Enums.OrderControlType.GenerateBillApproval, };
            string[] mainOrderIDs = unApprovedLists.Where(t => byMainOrderIDTypes.Contains(t.ControlType)).Select(t => t.MainOrderID).ToArray();
            var infosByMainOrderID = GetOtherInfoByMainOrderID(mainOrderIDs);

            Enums.OrderControlType[] byTinyOrderIDTypes = 
            {
                Enums.OrderControlType.DeleteModelApproval,
                Enums.OrderControlType.ChangeQuantityApproval,
                Enums.OrderControlType.SplitOrderApproval,
            };
            string[] tinyOrderIDs = unApprovedLists.Where(t => byTinyOrderIDTypes.Contains(t.ControlType)).Select(t => t.TinyOrderID).ToArray();
            var infosByTinyOrderID = GetOtherInfoByTinyOrderID(tinyOrderIDs);

            unApprovedLists = (from unApprovedList in unApprovedLists
                               join infoMain in infosByMainOrderID on unApprovedList.MainOrderID equals infoMain.MainOrderID into infosByMainOrderID2
                               from infoMain in infosByMainOrderID2.DefaultIfEmpty()
                               join infoTiny in infosByTinyOrderID on unApprovedList.TinyOrderID equals infoTiny.TinyOrderID into infosByTinyOrderID2
                               from infoTiny in infosByTinyOrderID2.DefaultIfEmpty()
                               select new AttachApprovalUnApprovedListViewModel
                               {
                                   OrderControlStepID = unApprovedList.OrderControlStepID,
                                   OrderControlID = unApprovedList.OrderControlID,
                                   MainOrderID = unApprovedList.MainOrderID,
                                   TinyOrderID = unApprovedList.TinyOrderID,
                                   ControlType = unApprovedList.ControlType,
                                   CreateDate = unApprovedList.CreateDate,
                                   EventInfo = unApprovedList.EventInfo,
                                   ApplicantID = unApprovedList.ApplicantID,
                                   ApplicantName = unApprovedList.ApplicantName,

                                   ClientName = infoMain != null ? infoMain.ClientName : (infoTiny != null ? infoTiny.ClientName : string.Empty),
                                   Currency = infoMain != null ? infoMain.Currency : (infoTiny != null ? infoTiny.Currency : string.Empty),
                                   DeclarePrice = infoMain != null ? infoMain.DeclarePrice : (infoTiny != null ? infoTiny.DeclarePrice : 0),
                               }).ToList();

            // 通过 MainOrderID、TinyOrderID 获取信息 End

            return unApprovedLists;
        }

    }

    public class AttachApprovalUnApprovedListViewModel
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
    }
}

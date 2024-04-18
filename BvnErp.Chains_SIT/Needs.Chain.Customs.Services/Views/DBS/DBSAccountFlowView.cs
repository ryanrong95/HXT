using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.DBS
{
    public class DBSAccountFlowView : UniqueView<Models.DurAccountFlow, ForicDBSReponsitory>
    {
        public DBSAccountFlowView()
        {
        }

        protected DBSAccountFlowView(ForicDBSReponsitory reponsitory, IQueryable<DurAccountFlow> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        internal DBSAccountFlowView(ForicDBSReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<DurAccountFlow> GetIQueryable()
        {

            var result = from accountFlow in this.Reponsitory.ReadTable<Layer.Data.Sqls.foricDBS.AccountFlow>()                       
                         select new Models.DurAccountFlow
                         {
                             ID = accountFlow.ID,
                             accountNo = accountFlow.accountNo,
                             accountCcy = accountFlow.accountCcy,
                             availableBal = accountFlow.availableBal,
                             initiatingPartyName = accountFlow.initiatingPartyName,
                             drCrInd = accountFlow.drCrInd,
                             txnCode = accountFlow.txnCode,
                             txnDesc = accountFlow.txnDesc,
                             txnDate = accountFlow.txnDate,
                             valueDate = accountFlow.valueDate,
                             txnCcy = accountFlow.txnCcy,
                             txnAmount = accountFlow.txnAmount,
                             Status = (Enums.Status)accountFlow.Status,
                             CreateDate = accountFlow.CreateDate,
                             UpdateDate = accountFlow.UpdateDate,
                             Summary = accountFlow.Summary
                         };

            return result;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.DurAccountFlow> iquery = this.IQueryable.Cast<Models.DurAccountFlow>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_my = iquery.ToArray();

            var ienums_linq = from accountFlow in ienum_my
                              select new Models.DurAccountFlow
                              {
                                  ID = accountFlow.ID,
                                  accountNo = accountFlow.accountNo,
                                  accountCcy = accountFlow.accountCcy,
                                  availableBal = accountFlow.availableBal,
                                  initiatingPartyName = accountFlow.initiatingPartyName,
                                  drCrInd = accountFlow.drCrInd,
                                  txnCode = accountFlow.txnCode,
                                  txnDesc = accountFlow.txnDesc,
                                  txnDate = accountFlow.txnDate,
                                  valueDate = accountFlow.valueDate,
                                  txnCcy = accountFlow.txnCcy,
                                  txnAmount = accountFlow.txnAmount,
                                  Status = (Enums.Status)accountFlow.Status,
                                  CreateDate = accountFlow.CreateDate,
                                  UpdateDate = accountFlow.UpdateDate,
                                  Summary = accountFlow.Summary
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

            Func<DurAccountFlow, object> convert = accountFlow => new
            {
                AccountNo = accountFlow.accountNo,
                initiatingPartyName = accountFlow.initiatingPartyName,
                drCrInd = accountFlow.drCrInd.ToUpper().Equals("C")?"收款":"付款",
                txnAmount = accountFlow.txnAmount,
                txnCcy = accountFlow.txnCcy,
                Balance = accountFlow.availableBal.Replace("as of",""),
                txnDate = accountFlow.txnDate.ToShortDateString(),
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }

        public DBSAccountFlowView SearchByAccountNo(string accountNo)
        {
            var linq = from query in this.IQueryable
                       where query.accountNo == accountNo
                       select query;

            var view = new DBSAccountFlowView(this.Reponsitory, linq);
            return view;
        }

        public DBSAccountFlowView SearchByType(string type)
        {
            var linq = from query in this.IQueryable
                       where query.drCrInd == type
                       select query;

            var view = new DBSAccountFlowView(this.Reponsitory, linq);
            return view;
        }

        public DBSAccountFlowView SearchByStartDate(DateTime startTime)
        {
            var linq = from query in this.IQueryable
                       where query.txnDate >= startTime
                       select query;

            var view = new DBSAccountFlowView(this.Reponsitory, linq);
            return view;
        }

        public DBSAccountFlowView SearchByEndDate(DateTime endTime)
        {
            var linq = from query in this.IQueryable
                       where query.txnDate < endTime
                       select query;

            var view = new DBSAccountFlowView(this.Reponsitory, linq);
            return view;
        }

    }
}

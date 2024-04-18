using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class StorageVoucherListView : QueryView<StorageVoucherListViewModel, PsOrderRepository>
    {
        private string _clientID { get; set; }

        public StorageVoucherListView()
        {
        }

        public StorageVoucherListView(string clientID)
        {
            this._clientID = clientID;
        }

        protected StorageVoucherListView(PsOrderRepository reponsitory, IQueryable<StorageVoucherListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<StorageVoucherListViewModel> GetIQueryable()
        {
            var payeeLeftsTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayeeLefts>();

            var iQuery = from payeeLeft in payeeLeftsTopView
                         where payeeLeft.PayerID == this._clientID
                         group payeeLeft by new { payeeLeft.CutDateIndex } into g
                         select new StorageVoucherListViewModel
                         {
                             CutDateIndex = g.Key.CutDateIndex,
                             CutDateIndexStr = Convert.ToString(g.Key.CutDateIndex),
                             TotalAmount = g.Sum(t => t.Total),
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<StorageVoucherListViewModel, T> convert, int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<StorageVoucherListViewModel> iquery = this.IQueryable.Cast<StorageVoucherListViewModel>().OrderByDescending(item => item.CutDateIndex);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_vouchers = iquery.ToArray();

            var ienums_linq = from voucher in ienum_vouchers
                              select new StorageVoucherListViewModel
                              {
                                  CutDateIndex = voucher.CutDateIndex,
                                  CutDateIndexStr = voucher.CutDateIndexStr,
                                  TotalAmount = voucher.TotalAmount,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据账单期号查询
        /// </summary>
        /// <param name="cutDateIndex"></param>
        /// <returns></returns>
        public StorageVoucherListView SearchByCutDateIndex(string cutDateIndex)
        {
            var linq = from query in this.IQueryable
                       where query.CutDateIndexStr.Contains(cutDateIndex)
                       select query;

            var view = new StorageVoucherListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class StorageVoucherListViewModel
    {
        /// <summary>
        /// 期号
        /// </summary>
        public int? CutDateIndex { get; set; }

        /// <summary>
        /// 期号 Str
        /// </summary>
        public string CutDateIndexStr { get; set; }

        /// <summary>
        /// 账单总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}

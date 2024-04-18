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
    /// 用于代仓储应收款弹框的视图
    /// </summary>
    public class VouchersStatisticsView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public VouchersStatisticsView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public VouchersStatisticsView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<VouchersStatisticsViewModel> GetList(LambdaExpression[] expressions)
        {
            var vouchersStatisticsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.VouchersStatisticsView>();

            var linq = from voucher in vouchersStatisticsView
                        select new VouchersStatisticsViewModel
                        {
                            ReceivableID = voucher.ReceivableID,
                            MainOrderID = voucher.MainOrderID,
                            Catalog = voucher.Catalog,
                            Subject = voucher.Subject,
                            LeftPrice = voucher.LeftPrice,
                            RightPrice = voucher.RightPrice ?? 0,
                            ReducePrice = voucher.ReducePrice ?? 0,
                            ApplicationID = voucher.ApplicationID,
                        };

            foreach (var expression in expressions)
            {
                linq = linq.Where(expression as Expression<Func<VouchersStatisticsViewModel, bool>>);
            }

            return linq.ToList();
        }
    }

    public class VouchersStatisticsViewModel
    {
        /// <summary>
        /// ReceivableID
        /// </summary>
        public string ReceivableID { get; set; } = string.Empty;

        /// <summary>
        /// MainOrderID
        /// </summary>
        public string MainOrderID { get; set; } = string.Empty;

        /// <summary>
        /// Catalog
        /// </summary>
        public string Catalog { get; set; } = string.Empty;

        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// LeftPrice
        /// </summary>
        public decimal LeftPrice { get; set; }

        /// <summary>
        /// RightPrice
        /// </summary>
        public decimal RightPrice { get; set; }

        /// <summary>
        /// ReducePrice
        /// </summary>
        public decimal ReducePrice { get; set; }

        /// <summary>
        /// ApplicationID
        /// </summary>
        public string ApplicationID { get; set; }
    }

}

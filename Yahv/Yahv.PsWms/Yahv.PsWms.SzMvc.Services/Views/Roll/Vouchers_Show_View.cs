using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Roll
{
    public class Vouchers_Show_View : QueryRoll<VoucherShow, VoucherShow, PsOrderRepository>
    {
        #region 构造函数

        public Vouchers_Show_View()
        {
        }

        #endregion

        protected override IQueryable<VoucherShow> GetIQueryable(Expression<Func<VoucherShow, bool>> expression, params LambdaExpression[] expressions)
        {
            var vouchers = new Origins.VouchersOrigin(this.Reponsitory);
            var clients = new Origins.ClientsOrigin(this.Reponsitory);
            var payeeLefts = new Origins.PayeeLeftsOrigin(this.Reponsitory);
            var payeeRights = new Origins.PayeeRightsOrigin(this.Reponsitory);

            var temp_query = from entity in payeeLefts
                             join right in payeeRights on entity.ID equals right.LeftID into rights
                             select new VoucherShow
                             {
                                 ID = entity.PayerID,
                                 PayerID = entity.PayerID,
                                 Total = entity.Total,
                                 CutDateIndex = (int)entity.CutDateIndex,
                                 ReceiptTotal = rights.Sum(t => t.Price),
                             };

            var linq = from entity in vouchers
                       join client in clients on entity.PayerID equals client.ID
                       join payee in temp_query on new { entity.CutDateIndex, entity.PayerID } equals new { payee.CutDateIndex, payee.PayerID } into payees
                       select new VoucherShow
                       {
                           ID = entity.ID,
                           PayerID = entity.PayerID,
                           PayerName = client.Name,
                           CutDateIndex = entity.CutDateIndex,
                           Total = payees.Sum(t => t.Total),
                           ReceiptTotal = payees.Sum(t => t.ReceiptTotal),
                           Isinvoiced=entity.IsInvoiced,
                       };


            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<VoucherShow, bool>>);
            }
            linq = linq.Where(expression);
            return linq;
        }

        protected override IEnumerable<VoucherShow> OnReadShips(VoucherShow[] results)
        {
            var linq = from entity in results
                       select new VoucherShow
                       {
                           ID = entity.ID,
                           PayerName = entity.PayerName,
                           PayerID = entity.PayerID,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                           ReceiptTotal = entity.ReceiptTotal,
                           Isinvoiced=entity.Isinvoiced,
                       };
            return linq;
        }
    }

    public class VoucherShow : IUnique
    {
        public string ID { get; set; }
        public string PayerID { get; set; }
        public string PayerName { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public int CutDateIndex { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal? ReceiptTotal { get; set; }
        /// <summary>
        /// 开票状态
        /// </summary>
        public bool Isinvoiced { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal TaxTotal { get; set; }
        /// <summary>
        /// 开票差额
        /// </summary>
        public decimal? Difference { get; set; }
    }
}

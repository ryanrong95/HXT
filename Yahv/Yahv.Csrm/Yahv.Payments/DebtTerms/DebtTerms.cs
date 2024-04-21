using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;

namespace Yahv.Payments
{
    public class DebtTerms
    {
        Dictionary<string, DebtTerm> dic;
        private DateTime date;
        private OverdueType type;
        private PayInfo payInfo;
        private Currency currency;

        internal DebtTerms(PayInfo payInfo)
        {
            this.payInfo = payInfo;

            //数据库访问

            using (var credits = new PvbCrmReponsitory())
            {
                var linq = from item in credits.ReadTable<Layers.Data.Sqls.PvbCrm.DebtTerms>()
                           where item.Payee == payInfo.Payee
                            && item.Payer == payInfo.Payer
                            && item.Business == payInfo.Conduct
                           select new DebtTerm
                           {
                               Payer = item.Payer,
                               Payee = item.Payee,
                               Business = item.Business,
                               Catalog = item.Catalog,
                               SettlementType = (SettlementType)item.SettlementType,
                               Months = item.Months,
                               Days = item.Days,
                               AdminID = item.AdminID,
                               ExchangeType = (ExchangeType)item.ERateType,
                           };

                var arry = linq.ToArray();
                this.dic = arry.ToDictionary(item => item.Catalog, item => item);
            }
        }

        DebtTerms(PayInfo payInfo, DateTime date, OverdueType type, Currency currency)
        {
            this.payInfo = payInfo;
            this.date = date;
            this.type = type;
            this.currency = currency;
        }

        /// <summary>
        /// 分类索引
        /// </summary>
        /// <param name="catalog">名称</param>
        /// <returns>分类条款</returns>
        public DebtTerm this[string catalog]
        {
            get
            {
                DebtTerm outor;
                if (!dic.TryGetValue(catalog, out outor))
                {
                    dic[catalog] = new DebtTerm
                    {
                        Business = payInfo.Conduct,
                        AdminID = payInfo.Inputer.ID,
                        Catalog = catalog,
                        Payer = payInfo.Payer,
                        Payee = payInfo.Payee,
                    };
                }

                return dic[catalog];
            }
        }

        public DebtTerms this[DateTime date, Currency currency = Currency.CNY]
        {
            get
            {
                return new DebtTerms(this.payInfo, date, OverdueType.All, currency);
            }
        }

        public DebtTerms this[DateTime date, string catalog, Currency currency = Currency.CNY]
        {
            get
            {
                return new DebtTerms(this.payInfo, date, OverdueType.Catalog, currency);
            }
        }

        /// <summary>
        /// 是否逾期
        /// </summary>
        public bool IsOverdue
        {
            get
            {
                using (var reponsitory = new PvbCrmReponsitory())
                {
                    var query = DProduce.Overdue(payInfo.Payer, payInfo.Payee, payInfo.Conduct, date, reponsitory, type, currency);
                    if (type == OverdueType.All)
                    {
                        return query.SingleOrDefault().Total > 0;
                    }
                    else if (type == OverdueType.Catalog)
                    {
                        return query.SingleOrDefault(item => item.Catalog == payInfo.Catalog).Total > 0;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// 逾期金额
        /// </summary>
        public decimal Overdue
        {
            get
            {
                using (var reponsitory = new PvbCrmReponsitory())
                {
                    var query = DProduce.Overdue(payInfo.Payer, payInfo.Payee, payInfo.Conduct, date, reponsitory, type, currency);
                    if (type == OverdueType.All)
                    {
                        return query.SingleOrDefault().Total ?? 0m;
                    }
                    else if (type == OverdueType.Catalog)
                    {
                        return query.SingleOrDefault(item => item.Catalog == payInfo.Catalog).Total ?? 0m;
                    }

                    return 0;
                }
            }
        }
    }
}

using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Yahv.Payments.Models.Origins;
using Yahv.Payments.Models.Rolls;
using Yahv.Underly;
using Yahv.Utils.Http;

namespace Yahv.Payments
{
    /// <summary>
    /// 分类
    /// </summary>
    public class CreditCatalog
    {
        ConcurrentDictionary<Currency, CreditCatalogSubtatol> subTotals;
        PayInfo payInfo;

        public string Name { get; private set; }
        internal CreditCatalog(string name, PayInfo payInfo)
        {
            this.Name = name;
            this.payInfo = payInfo;
            this.subTotals = new ConcurrentDictionary<Currency, CreditCatalogSubtatol>();
        }

        /// <summary>
        /// 获取指定币种的 统计信息
        /// </summary>
        /// <param name="index">币种</param>
        /// <returns></returns>
        public CreditCatalogSubtatol this[Currency index]
        {
            get
            {
                return this.subTotals.GetOrAdd(index, new CreditCatalogSubtatol
                {
                    Cost = 0,
                    Total = 0,
                });
            }
        }

        /// <summary>
        /// 授信（信用只可以批复到分类）
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="price"></param>
        public string Credit(Currency currency, decimal price)
        {
            try
            {
                //减去信用（考虑可用值 是否可减）
                if (this[currency].Available + price < 0)
                {
                    return "可用信用不能小于扣除信用!";
                }

                //获取汇率
                var rate = ExchangeRates.Universal[currency, Currency.CNY];

                using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.FlowAccounts()
                    {
                        ID = PKeySigner.Pick(PKeyType.FlowAccount),
                        Type = (int)AccountType.CreditRecharge,
                        Currency = (int)currency,
                        AdminID = payInfo.Inputer.ID,
                        Business = payInfo.Conduct,
                        Catalog = this.Name,
                        CreateDate = DateTime.Now,
                        Currency1 = (int)Currency.CNY,
                        ERate1 = rate,
                        Price1 = price * rate,
                        Payee = payInfo.Payee,
                        Payer = payInfo.Payer,
                        Price = price,
                    });
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"添加失败!{ex.ToString()}";
            }
        }



    }

    /// <summary>
    /// 分类
    /// </summary>
    public class CreditCatalogSubtatol
    {
        /// <summary>
        /// 总批复
        /// </summary>
        public decimal Total { get; internal set; }

        /// <summary>
        /// 总花费
        /// </summary>
        public decimal Cost { get; internal set; }

        /// <summary>
        /// 可用
        /// </summary>
        public decimal Available { get { return this.Total - this.Cost; } }
    }
}

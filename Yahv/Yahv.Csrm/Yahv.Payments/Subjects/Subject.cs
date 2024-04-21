using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 科目
    /// </summary>
    public class Subject : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 科目类型（应收、应付）
        /// </summary>
        public SubjectType Type { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 是否需要录入个数
        /// </summary>
        public bool IsCount { get; set; }

        /// <summary>
        /// 是否需要流转给客户
        /// </summary>
        public bool IsToCustomer { get; set; }

        /// <summary>
        /// json格式存储 后续选择步骤。如果本次段不为null 就表示有后续步骤
        /// </summary>
        public string Steps { get; set; }
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //判断是否存在
                Expression<Func<Layers.Data.Sqls.PvbCrm.Subjects, bool>> predication = item =>
                item.Type == (int)this.Type
                && item.Conduct == this.Conduct
                && item.Catalog == this.Catalog
                && item.Name == this.Name;

                int? currency = null;
                if (this.Currency != null)
                {
                    currency = (int)this.Currency;
                }

                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Subjects>().Any(predication))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Subjects);
                    

                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Subjects()
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        Catalog = this.Catalog,
                        Conduct = this.Conduct,
                        Name = this.Name,

                        Currency = currency,
                        IsCount = this.IsCount,
                        IsToCustomer = this.IsToCustomer,
                        Price = this.Price,
                        Steps = this.Steps,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.Subjects>(new
                    {
                        Currency = currency,
                        IsCount = this.IsCount,
                        IsToCustomer = this.IsToCustomer,
                        Price = this.Price,
                        Steps = this.Steps,
                    }, predication);
                }
            }
        }
        #endregion
    }
}

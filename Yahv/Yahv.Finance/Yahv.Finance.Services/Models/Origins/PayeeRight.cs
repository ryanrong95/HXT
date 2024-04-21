using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 核销表
    /// </summary>
    public class PayeeRight : IUnique
    {
        #region 数据库属性
        public string ID { get; set; }

        /// <summary>
        /// 实收ID
        /// </summary>
        public string PayeeLeftID { get; set; }

        /// <summary>
        /// 账款分类
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 源系统ID
        /// </summary>
        public string SenderID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 本位币
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 本位币金额
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ERate1 { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                repository.Insert(new Layers.Data.Sqls.PvFinance.PayeeRights()
                {
                    ID = PKeySigner.Pick(PKeyType.PayeeRight),
                    PayeeLeftID = this.PayeeLeftID,
                    Currency = (int)this.Currency,
                    CreateDate = DateTime.Now,
                    CreatorID = this.CreatorID,
                    Currency1 = (int)this.Currency1,
                    Department = this.Department,
                    ERate1 = this.ERate1,
                    Price = this.Price,
                    Price1 = this.Price1,
                    SenderID = this.SenderID,
                    AccountCatalogID = this.AccountCatalogID,
                });
            }

        }
        #endregion
    }
}
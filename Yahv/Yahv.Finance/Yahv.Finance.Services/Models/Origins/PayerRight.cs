using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 实付（货款）
    /// </summary>
    public class PayerRight : IUnique
    {
        #region 事件
        /// <summary>
        /// 成功
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// 失败
        /// </summary>
        public event ErrorHanlder EnterError;
        #endregion

        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 应付ID
        /// </summary>
        public string PayerLeftID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 本位币 币种
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 本位币 汇率
        /// </summary>
        public decimal ERate1 { get; set; }

        /// <summary>
        /// 本位币 金额
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

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
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerRights>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.PayerRight);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerRights()
                    {
                        ID = PKeySigner.Pick(PKeyType.PayerRight),
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Price1 = this.Price1,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        FlowID = this.FlowID,
                        PayerLeftID = this.PayerLeftID,
                    });
                }

                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.PayerRights>(item => item.ID == this.ID);
            }
        }
        #endregion
    }
}
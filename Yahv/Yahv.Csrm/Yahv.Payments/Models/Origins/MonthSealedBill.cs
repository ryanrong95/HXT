using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;

namespace Yahv.Payments.Models.Origins
{
    /// <summary>
    /// 封账
    /// </summary>
    public class MonthSealedBill : IUnique
    {
        #region  属性
        public string ID { get; set; }

        /// <summary>
        /// 账期
        /// </summary>
        public int DateIndex { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 付款人ID（客户）
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 封账状态
        /// </summary>
        public MonthSealed Sealed { get; set; }

        public MonthSealedBill()
        {
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;


        }
        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //新增优惠券
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MonthSealedBills>().Any(item => item.ID == this.ID))
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.MonthSealedBills()
                    {
                        ID = Guid.NewGuid().ToString(),
                        Conduct = this.Conduct,
                        DateIndex = this.DateIndex,
                        Sealed = (int)this.Sealed,
                        Payer = this.Payer,
                        CreateDate = this.CreateDate,
                        OccurDate = this.OccurDate,
                        ModifyDate = this.ModifyDate,
                    });
                }
                else
                {

                    reponsitory.Update(new Layers.Data.Sqls.PvbCrm.MonthSealedBills()
                    {
                        ID = this.ID,
                        Conduct = this.Conduct,
                        DateIndex = this.DateIndex,
                        Sealed = (int)this.Sealed,
                        Payer = this.Payer,
                        CreateDate = this.CreateDate,
                        OccurDate = this.OccurDate,
                        ModifyDate = this.ModifyDate,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        #endregion
    }



}

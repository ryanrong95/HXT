using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Linq.Extends;
using Yahv.Underly;

namespace Yahv.Payments.Models.Origins
{
    /// <summary>
    /// 财务通知
    /// </summary>
    public class Voucher : IUnique
    {
        public Voucher()
        {
            this.Status = VoucherStatus.Confirmed;
        }

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 财务通知类型
        /// </summary>
        public VoucherType Type { get; set; }

        /// <summary>
        /// 付款公司
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// 收款公司
        /// </summary>
        public string Payee { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public VoucherStatus Status { get; set; }

        /// <summary>
        /// 付汇使用
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 期号，如果有期号就代表是action 是还款
        /// </summary>
        public int? DateIndex { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 是否结算其他费用（暂时为仓储费）
        /// </summary>
        public bool IsSettlement { get; set; } = false;
        #endregion

        #region 持久化

        public void Enter(PvbCrmReponsitory reponsitory = null)
        {
            if (reponsitory != null)
            {
                Insert(reponsitory);
            }
            else
            {
                using (reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    Insert(reponsitory);
                }
            }
        }

        private void Insert(PvbCrmReponsitory reponsitory)
        {
            //判断账单是否存在
            Expression<Func<Layers.Data.Sqls.PvbCrm.Vouchers, bool>> predication = item => item.Type == (int)this.Type;

            //普通记账
            if (!string.IsNullOrWhiteSpace(this.OrderID))
            {
                predication = predication.And(item => item.OrderID == this.OrderID);
            }
            if (!string.IsNullOrWhiteSpace(this.ApplicationID))
            {
                predication = predication.And(item => item.ApplicationID == this.ApplicationID);
            }


            //信用还款
            if (DateIndex != null && this.DateIndex > 0)
            {
                predication = predication.And(item => item.DateIndex == this.DateIndex);
            }


            if (!string.IsNullOrWhiteSpace(this.ID))
            {
                predication = predication.And(item => item.ID == this.ID);
            }


            //新增账单
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>().Any(predication))
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                    this.ID = PKeySigner.Pick(PKeyType.Vouchers);

                reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Vouchers()
                {
                    ID = this.ID,
                    Type = (int)this.Type,
                    OrderID = this.OrderID,
                    Payer = this.Payer,
                    Payee = this.Payee,
                    CreateDate = this.CreateDate ?? DateTime.Now,
                    ApplicationID = this.ApplicationID,
                    DateIndex = this.DateIndex,
                    Status = (int)this.Status, //默认为已确认
                    CreatorID = this.CreatorID,
                    Currency = (int)this.Currency,
                    IsSettlement = this.IsSettlement,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvbCrm.Vouchers>(new
                {
                    IsSettlement = this.IsSettlement,
                    Status = (int)this.Status,
                }, predication);
            }
        }
        #endregion
    }
}

using Needs.Wl.Models.Hanlders;
using Needs.Wl.Models;
using System;
using System.Linq;

namespace Needs.Wl.Client.Services
{
    /// <summary>
    /// 会员提交付汇申请
    /// </summary>
    public class UserSubmitPayExchange
    {
        private Models.UserPayExchangeApply UserPayExchangeApply;

        /// <summary>
        /// 当User提交付汇申请成功后发生
        /// </summary>
        public event PayExchangeSubmitedHanlder Submited;

        /// <summary>
        /// 当User删除付汇申请成功后发生
        /// </summary>
        public event PayExchangeDeletedHanlder Deleted;

        public UserSubmitPayExchange(Models.UserPayExchangeApply userPayExchangeApply)
        {
            this.UserPayExchangeApply = userPayExchangeApply;
        }

        /// <summary>
        /// 提交付汇申请
        /// </summary>
        public void Submit()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {
                this.UserPayExchangeApply.Reponsitory = reponsitory;
                this.UserPayExchangeApply.Enter();

                //插入Items
                var items = this.UserPayExchangeApply.Items;
                foreach (var item in items)
                {
                    item.Reponsitory = reponsitory;
                    item.PayExchangeApplyID = this.UserPayExchangeApply.ID;
                    item.ApplyStatus = Wl.Models.Enums.ApplyItemStatus.Appling;
                    item.Enter();

                    var curOrder = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                           .Where(s => s.ID == item.OrderID).FirstOrDefault();

                    //更新订单的已付汇金额
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                        new { PaidExchangeAmount = curOrder.PaidExchangeAmount + item.Amount }, s => s.ID == item.OrderID);

                }

                var files = this.UserPayExchangeApply.Files;
                if (files != null)
                {
                    foreach (var item in files)
                    {
                        item.Reponsitory = reponsitory;
                        item.PayExchangeApplyID = this.UserPayExchangeApply.ID;
                        item.UserID = this.UserPayExchangeApply.User.ID;
                        item.Enter();
                    }
                }

                reponsitory.Submit();

                this.OnSubmited();
            }
        }

        virtual protected void OnSubmited()
        {
            this.Submited?.Invoke(this, new PayExchangeSubmitedEventArgs(this.UserPayExchangeApply));
        }

        /// <summary>
        /// 删除付汇申请
        /// </summary>
        public void Delete()
        {
            if (this.UserPayExchangeApply.PayExchangeApplyStatus == Wl.Models.Enums.PayExchangeApplyStatus.Auditing || this.UserPayExchangeApply.PayExchangeApplyStatus == Wl.Models.Enums.PayExchangeApplyStatus.Cancled)
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    this.UserPayExchangeApply.Reponsitory = reponsitory;
                    this.UserPayExchangeApply.Abandon();

                    //待审核状态的付汇申请，做取消或删除时需要更新订单已付汇金额
                    if (this.UserPayExchangeApply.PayExchangeApplyStatus == Wl.Models.Enums.PayExchangeApplyStatus.Auditing)
                    {
                        foreach (var item in this.UserPayExchangeApply.Items)
                        {
                            //更新已付汇金额
                            var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                .Where(t => t.ID == item.OrderID).FirstOrDefault();

                            decimal amount = order.PaidExchangeAmount - item.Amount;

                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                                new { PaidExchangeAmount = amount }, t => t.ID == item.OrderID);
                        }
                    }
                    reponsitory.Submit();

                    this.OnDeleted();
                }
            }
            else
            {
                throw new Exception("不可删除已经审核完成的付汇申请");
            }
        }


        virtual protected void OnDeleted()
        {
            this.Deleted?.Invoke(this, new PayExchangeDeletedEventArgs(this.UserPayExchangeApply));
        }
    }
}
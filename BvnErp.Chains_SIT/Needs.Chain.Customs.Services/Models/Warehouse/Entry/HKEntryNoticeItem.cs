using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 香港入库通知Item
    /// </summary>
    public class HKEntryNoticeItem : EntryNoticeItem
    {
        public override OrderItem OrderItem
        {
            get
            {
                return base.OrderItem;
            }

            set
            {
                base.OrderItem = value;
            }
        }

        /// <summary>
        /// 是否抽检
        /// </summary>
        public override bool IsSportCheck
        {
            get
            {
                return base.IsSportCheck;
            }

            set
            {
                base.IsSportCheck = value;
            }
        }

        /// <summary>
        /// 未装箱数量
        /// </summary>
        public decimal? RelQuantity { get; set; }

        /// <summary>
        /// 抽检异常事件
        /// </summary>
        public event SpotAbnormalHanlders SpotAbnormaled;

        public HKEntryNoticeItem()
        {
            this.SpotAbnormaled += EntryNoticeItem_SpotAbnormaled;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Abandon()
        {
            base.Abandon();
        }

        /// <summary>
        ///抽检异常
        /// </summary>
        public void SpotAbnormal()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //TODO:抽检异常，后续流程如何处理，现在还不知道，是否要加 Item的状态：抽检异常

            }

            this.OnSpotAbnormaled();
        }

        public virtual void OnSpotAbnormaled()
        {
            if (this.SpotAbnormaled != null)
            {
                this.SpotAbnormaled(this, new SpotAbnormalEventArgs(this.OrderItem));
            }
        }

        private void EntryNoticeItem_SpotAbnormaled(object sender, SpotAbnormalEventArgs e)
        {
            e.OrderItem.OrderHangUp(OrderControlType.CheckingAbnomaly);
        }
    }

    public class HKEntryNoticeItems : BaseItems<HKEntryNoticeItem>
    {
        internal HKEntryNoticeItems(IEnumerable<HKEntryNoticeItem> enums) : base(enums)
        {
        }

        internal HKEntryNoticeItems(IEnumerable<HKEntryNoticeItem> enums, Action<HKEntryNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(HKEntryNoticeItem item)
        {
            base.Add(item);
        }
    }
}
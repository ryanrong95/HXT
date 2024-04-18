using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class Logs_PvWsOrder : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单或订单项ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 状态类型
        /// </summary>
        public OrderStatusType Type { get; set; }

        /// <summary>
        /// 状态值
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否是当前状态
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 管理员（会员）ID
        /// </summary>
        public string CreatorID { get; set; }

        #endregion

        public Logs_PvWsOrder()
        {
        }

        public void Enter()
        {
            using (var res = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                res.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                {
                    IsCurrent = false
                }, item => item.MainID == this.MainID && item.Type == (int)this.Type);

                res.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = this.MainID,
                    CreatorID = this.CreatorID,
                    Type = (int)this.Type,
                    CreateDate = DateTime.Now,
                    Status = this.Status,
                    IsCurrent = true
                });
            }
        }
    }

    public class WsOrderCurrentStatus : IUnique
    {
        /// <summary>
        /// MainID
        /// </summary>
        public string ID { get; set; }

        public int MainStatus { get; set; }

        public int PaymentStatus { get; set; }

        public int RemittanceStatus { get; set; }

        public int InvoiceStatus { get; set; }

    }
}

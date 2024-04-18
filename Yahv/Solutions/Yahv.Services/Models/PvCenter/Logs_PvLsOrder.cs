using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Models
{
    public class Logs_PvLsOrder : IUnique
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
        public LsOrderStatusType Type { get; set; }

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

        public Logs_PvLsOrder()
        {
        }

        public void Enter()
        {
            using (var res = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                res.Update<Layers.Data.Sqls.PvCenter.Logs_PvLsOrder>(new
                {
                    IsCurrent = false
                }, item => item.MainID == this.MainID && item.Type==(int)this.Type);
                res.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder
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

    public class LsOrderCurrentStatus : IUnique
    {
        /// <summary>
        /// MainID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int MainStatus { get; set; }

        /// <summary>
        /// 订单开票状态
        /// </summary>
        public int InvoiceStatus { get; set; }

    }
}

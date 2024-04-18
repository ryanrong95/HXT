using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单项变更日志
    /// </summary>
    [Serializable]
    public class OrderItemChangeLog : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 后台管理员
        /// </summary>
        public string AdminID { get; set; }
        public Admin Admin { get; set; }

        public Enums.OrderItemChangeType Type { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public OrderItemChangeLog()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDocuCode>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderItemChangeLogs
                {
                    ID =ChainsGuid.NewGuidUp(),
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    AdminID = this.Admin.ID,
                    Type = (int)this.Type,
                    CreateDate = DateTime.Now,
                    Summary = this.Summary
               
                });
                }

            }
        }
    }
}
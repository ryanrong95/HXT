using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class OrderChangeNoticeLog : IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        public string OrderChangeNoticeID { get; set; } = string.Empty;

        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string OrderItemID { get; set; } = string.Empty;

        /// <summary>
        /// AdminID
        /// </summary>
        public string AdminID { get; set; } = string.Empty;

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        #endregion

        public OrderChangeNoticeLog()
        {

        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs
                {
                    ID = this.ID,
                    OrderChangeNoticeID = this.OrderChangeNoticeID,
                    OrderID = this.OrderID,
                    OrderItemID = this.OrderItemID,
                    AdminID = this.AdminID,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary,
                });
            }
        }
    }
}

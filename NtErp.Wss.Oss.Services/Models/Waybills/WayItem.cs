using Needs.Linq;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{

    public class WayItemKit : IUnique
    {
        public string ID { get; set; }
        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }
        /// <summary>
        /// 重量
        /// </summary>
        public decimal Weight { get; set; }

        public WayItemSource Source { get; set; }

        public int Count { get; set; }

    }

    /// <summary>
    /// 运单项
    /// </summary>
    public class WayItem : WayItemKit, IUnique, IPersist
    {
        public WayItem()
        {

        }

        #region 持久化


        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq_order = from item in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>()
                                 where item.OrderID == this.OrderID
                                 select item;

                var orderitemCount = linq_order.Sum(item => item.Quantity);


                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.WaybillItem);

                    if (string.IsNullOrWhiteSpace(this.OrderItemID))
                    {
                        this.Count = orderitemCount;
                    }

                    reponsitory.Insert(this.ToLinq());
                }

                var linq_way = from item in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.WayItems>()
                               where item.OrderID == this.OrderID
                               select item;
                var wayitemCount = linq_way.Sum(item => item.Count);

                reponsitory.Update<Layer.Data.Sqls.CvOss.Orders>(new
                {
                    SendRate = (decimal)wayitemCount / orderitemCount,
                }, item => item.ID == this.OrderID);
            }
        }

        #endregion
    }


    public class WayItemOrder : WayItemKit
    {
        //public WaybillKit Bill
        //{
        //    get; set;
        //}
    }
}

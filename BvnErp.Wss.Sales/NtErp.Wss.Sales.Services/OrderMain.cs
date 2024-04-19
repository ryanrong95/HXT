using NtErp.Wss.Sales.Services.Extends;
using NtErp.Wss.Sales.Services.Model;
using NtErp.Wss.Sales.Services.Model.Orders;
using NtErp.Wss.Sales.Services.Model.Orders.Hanlders;
using NtErp.Wss.Sales.Services.Models;
using NtErp.Wss.Sales.Services.Models.Orders;
using NtErp.Wss.Sales.Services.Overalls;
using NtErp.Wss.Sales.Services.Overalls.Rates;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using NtErp.Wss.Sales.Services.Underly.Serializers;
using NtErp.Wss.Sales.Services.Utils.Converters;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services
{
    /// <summary>
    /// 订单
    /// </summary>
    public class OrderMain
    {
        #region 事件
        public event CloseSuccessHanlder CloseSuccess;
        public event ChangeSuccessHanlder ChangeSuccess;
        public event PlacedSuccessHanlder PlacedSuccess;

        #endregion 

        #region 属性
        public string ID { get; set; }
        public string UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string SiteUserName { get; set; }
        /// <summary>
        /// 公司名
        /// </summary>
        public string CompanyName { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Summary { get; set; }
        public OrderStatus Status { get; set; }

        /// <summary>
        /// 承运方式
        /// </summary>
        public TransportTerm Transport { get; set; }

        District district;

        /// <summary>
        /// 交货地
        /// </summary>
        public District District
        {
            get
            {
                return this.district;
            }
            set
            {
                this.district = value;
            }
        }

        /// <summary>
        /// 交易币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        /// <example>
        /// 特有的管理
        /// </example>
        public Source Source { get; set; }
        /// <summary>
        /// 发货比例
        /// </summary>
        public float DeliveryRatio { get; set; }
        /// <summary>
        /// 支付比例
        /// </summary>
        public float PaidRatio { get; set; }
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                this.UpdateDate = DateTime.Now;

                repository.Update(this.ToLinq(), item => item.ID == this.ID);

                //repository.Delete<Layer.Data.Sqls.BvOrders.OrderShowers>(item => item.MainID == this.ID);
                //repository.Insert(new Layer.Data.Sqls.BvOrders.OrderShowers
                //{
                //    MainID = this.ID,
                //    Xml = this.XmlEle()
                //});

            }
        }


        /// <summary>
        /// 订单完成
        /// </summary>
        public void Completed()
        {

            this.Status = OrderStatus.Completed;
            this.UpdateDate = DateTime.Now;

            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                repository.Update<Layer.Data.Sqls.BvOrders.Orders>(new
                {
                    Status = this.Status,
                    UpdateDate = this.UpdateDate
                }, item => item.ID == this.ID);
            }

        }


        /// <summary>
        /// 订单关闭
        /// </summary>
        public void Close()
        {

            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                this.Status = OrderStatus.Closed;
                repository.Update<Layer.Data.Sqls.BvOrders.Orders>(new
                {
                    Status = this.Status,
                    UpdateDate = DateTime.Now
                }, item => item.ID == this.ID);


            }
            if (this != null && this.CloseSuccess != null)
            {
                this.CloseSuccess(this, new CloseEventArgs());
            }

        }

        #endregion
    }
}

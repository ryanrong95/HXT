using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PsWms.SzMvc.Services.Views.Roll
{
    /// <summary>
    /// 入库订单视图
    /// </summary>
    public class OrdersIn_Show_View : QueryView<OrderShow, PsOrderRepository>
    {
        public OrdersIn_Show_View()
        {
        }

        protected OrdersIn_Show_View(PsOrderRepository reponsitory, IQueryable<OrderShow> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<OrderShow> GetIQueryable()
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory).Where(t => t.Type == Enums.OrderType.Inbound).OrderByDescending(t => t.CreateDate);
            var clients = new Origins.ClientsOrigin(this.Reponsitory);
            var transports = new Origins.OrderTransportsOrigin(this.Reponsitory);

            var linq = from entity in orders
                       join client in clients on entity.ClientID equals client.ID
                       join transport in transports on entity.ConsignorID equals transport.ID
                       select new OrderShow
                       {
                           ID = entity.ID,
                           CreateDate = entity.CreateDate,
                           Status = entity.Status,
                           ClientName = client.Name,
                           TransportMode = transport.TransportMode,
                       };

            return linq;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<OrderShow[], int> ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<OrderShow> iquery = this.IQueryable.Cast<OrderShow>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var linq = from entity in iquery.ToArray()
                       select new OrderShow
                       {
                           ID = entity.ID,
                           CreateDate = entity.CreateDate,
                           Status = entity.Status,
                           ClientName = entity.ClientName,
                           TransportMode = entity.TransportMode,
                       };

            return new Tuple<OrderShow[], int>(linq.ToArray(), total);
        }

        #region 查询方法

        /// <summary>
        /// 根据订单ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrdersIn_Show_View SearchByID(string id)
        {
            var linq = from query in this.IQueryable
                       where query.ID == id
                       select query;

            var view = new OrdersIn_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersIn_Show_View SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new OrdersIn_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersIn_Show_View SearchByStatus(Enums.OrderStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.Status == status
                       select query;

            var view = new OrdersIn_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersIn_Show_View SearchByDateTime(DateTime? start, DateTime? end)
        {
            var linq = this.IQueryable;
            if (start != null)
            {
                linq = linq.Where(t => t.CreateDate >= start);
            }
            if (end != null)
            {
                linq = linq.Where(t => t.CreateDate < ((DateTime)end).AddDays(1));
            }
            var view = new OrdersIn_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersIn_Show_View SearchByPartNumber(string partNumber)
        {
            var Items = new Origins.OrderItemsOrigin(this.Reponsitory);
            var products = new Origins.ProductsOrigin(this.Reponsitory);

            var linq = from entity in Items
                       join product in products on entity.ProductID equals product.ID
                       join order in this.IQueryable on entity.OrderID equals order.ID
                       where product.Partnumber == partNumber
                       select order;

            var view = new OrdersIn_Show_View(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }

    /// <summary>
    /// 出库订单视图
    /// </summary>
    public class OrdersOut_Show_View : QueryView<OrderShow, PsOrderRepository>
    {
        public OrdersOut_Show_View()
        {
        }

        protected OrdersOut_Show_View(PsOrderRepository reponsitory, IQueryable<OrderShow> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<OrderShow> GetIQueryable()
        {
            var orders = new Origins.OrdersOrigin(this.Reponsitory).Where(t => t.Type == Enums.OrderType.Outbound).OrderByDescending(t => t.CreateDate);
            var clients = new Origins.ClientsOrigin(this.Reponsitory);
            var transports = new Origins.OrderTransportsOrigin(this.Reponsitory);

            var linq = from entity in orders
                       join client in clients on entity.ClientID equals client.ID
                       join transport in transports on entity.ConsigneeID equals transport.ID
                       select new OrderShow
                       {
                           ID = entity.ID,
                           CreateDate = entity.CreateDate,
                           Status = entity.Status,
                           ClientName = client.Name,
                           TransportMode = transport.TransportMode,
                       };

            return linq;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<OrderShow[], int> ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<OrderShow> iquery = this.IQueryable.Cast<OrderShow>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var linq = from entity in iquery.ToArray()
                       select new OrderShow
                       {
                           ID = entity.ID,
                           CreateDate = entity.CreateDate,
                           Status = entity.Status,
                           ClientName = entity.ClientName,
                           TransportMode = entity.TransportMode,
                       };

            return new Tuple<OrderShow[], int>(linq.ToArray(), total);
        }

        #region 查询方法

        /// <summary>
        /// 根据订单ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrdersOut_Show_View SearchByID(string id)
        {
            var linq = from query in this.IQueryable
                       where query.ID == id
                       select query;

            var view = new OrdersOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersOut_Show_View SearchByClientName(string clientName)
        {
            var linq = from query in this.IQueryable
                       where query.ClientName.Contains(clientName)
                       select query;

            var view = new OrdersOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersOut_Show_View SearchByStatus(Enums.OrderStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.Status == status
                       select query;

            var view = new OrdersOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersOut_Show_View SearchByDateTime(DateTime? start, DateTime? end)
        {
            var linq = this.IQueryable;
            if (start != null)
            {
                linq = linq.Where(t => t.CreateDate >= start);
            }
            if (end != null)
            {
                linq = linq.Where(t => t.CreateDate < ((DateTime)end).AddDays(1));
            }
            var view = new OrdersOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        public OrdersOut_Show_View SearchByPartNumber(string partNumber)
        {
            var Items = new Origins.OrderItemsOrigin(this.Reponsitory);
            var products = new Origins.ProductsOrigin(this.Reponsitory);

            var linq = from entity in Items
                       join product in products on entity.ProductID equals product.ID
                       join order in this.IQueryable on entity.OrderID equals order.ID
                       where product.Partnumber == partNumber
                       select order;

            var view = new OrdersOut_Show_View(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }

    public class OrderShow : IUnique
    {
        public string ID { get; set; }

        public string ClientName { get; set; }

        public Enums.TransportMode TransportMode { get; set; }

        public DateTime CreateDate { get; set; }

        public Enums.OrderStatus Status { get; set; }

        public string TransportModeDec
        {
            get
            {
                return this.TransportMode.GetDescription();
            }
        }

        public string OrderStatusDec
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        public IEnumerable<OrderItemShow> items { get; set; }

    }

    public class OrderItemShow
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public Product Product { get; set; }
    }
}

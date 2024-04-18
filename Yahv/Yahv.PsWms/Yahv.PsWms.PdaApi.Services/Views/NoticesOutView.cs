using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PdaApi.Services.Enums;
using Yahv.PsWms.PdaApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 出库通知视图
    /// </summary>
    public class NoticesOutView : NoticesView
    {
        public NoticesOutView()
        {
        }

        internal NoticesOutView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal NoticesOutView(PsWmsRepository reponsitory, IQueryable<Notice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Notice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.NoticeType == Enums.NoticeType.Outbound)
                                       .OrderByDescending(item => item.CreateDate);
        }

        public override object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<Notice>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_iquery = iquery.ToArray();

            //客户信息
            var clientIDs = ienum_iquery.Select(item => item.ClientID).Distinct();
            var clientsView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                              where clientIDs.Contains(client.ID)
                              select new
                              {
                                  client.ID,
                                  client.Name
                              };
            var ienum_clients = clientsView.ToArray();

            //货运信息
            var consigneeIDs = ienum_iquery.Select(item => item.ConsigneeID);
            var noticeTransposrtsView = from transaport in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeTransports>()
                                        where consigneeIDs.Contains(transaport.ID)
                                        select new
                                        {
                                            transaport.ID,
                                            TransportMode = (TransportMode)transaport.TransportMode,
                                            transaport.Contact,
                                            transaport.Phone,
                                            transaport.Address
                                        };
            var ienum_noticeTransposrts = noticeTransposrtsView.ToArray();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                #region 视图补全

                //客服
                var trackerIDs = ienum_iquery.Select(item => item.TrackerID).Distinct();
                var trackersView = from tracker in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.AdminsTopView>()
                                   where trackerIDs.Contains(tracker.ID)
                                   select new
                                   {
                                       tracker.ID,
                                       tracker.UserName,
                                       tracker.RealName
                                   };
                var ienum_trackers = trackersView.ToArray();

                var linq = from notice in ienum_iquery
                           join transposrt in ienum_noticeTransposrts on notice.ConsigneeID equals transposrt.ID
                           join client in ienum_clients on notice.ClientID equals client.ID
                           join tracker in ienum_trackers on notice.TrackerID equals tracker.ID
                           select new
                           {
                               notice.ID,
                               notice.FormID,
                               TransportMode = transposrt.TransportMode.GetDescription(),
                               ClientName = client.Name,
                               TrackerName = tracker.RealName,
                               NoticeDate = notice.CreateDate.ToString("yyyy-MM-dd HH:mm")
                           };

                #endregion

                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = linq.ToArray(),
                };
            }
            else
            {
                #region 补充完整对象

                var noticeIDs = ienum_iquery.Select(item => item.ID);

                //特殊要求
                var requiresView = from require in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Requires>()
                                   where noticeIDs.Contains(require.NoticeID)
                                   select new
                                   {
                                       require.ID,
                                       require.NoticeID,
                                       require.NoticeTransportID,
                                       require.Name,
                                       require.Contents
                                   };
                var ienum_requires = requiresView.ToArray();

                //产品信息
                var productsView = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                                   join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on item.ProductID equals product.ID
                                   join shelve in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>() on item.ShelveID equals shelve.ID into shelves
                                   from shelve in shelves.DefaultIfEmpty()
                                   where noticeIDs.Contains(item.NoticeID)
                                   select new
                                   {
                                       item.ID,
                                       item.NoticeID,
                                       item.Mpq,
                                       item.PackageNumber,
                                       item.Total,
                                       item.ShelveID,
                                       item.Exception,
                                       product.Partnumber,
                                       product.Brand,
                                       product.Package,
                                       product.DateCode,
                                       ShelveCode = shelve.Code
                                   };
                var ienum_products = productsView.ToArray();

                var linq = from notice in ienum_iquery
                           join client in ienum_clients on notice.ClientID equals client.ID
                           join transport in ienum_noticeTransposrts on notice.ConsigneeID equals transport.ID
                           join require in ienum_requires on notice.ID equals require.NoticeID into requires
                           join product in ienum_products on notice.ID equals product.NoticeID into products
                           let deliveryBillRequire = requires.FirstOrDefault(item => item.Name == "发货单格式")
                           select new
                           {
                               notice.ID,
                               ClientName = client.Name,
                               notice.FormID,

                               transport.Contact,
                               transport.Phone,
                               transport.Address,
                               TransportMode = transport.TransportMode.GetDescription(),
                               TotalCount = products.Count(),

                               SpecialRequires = requires.Select(item => item.Contents).ToArray(),
                               DeliveryBillRequire = deliveryBillRequire == null ? null : deliveryBillRequire.Contents ?? "请参考指定附件",

                               Products = products.Select(item => new
                               {
                                   item.Partnumber,
                                   item.Brand,
                                   item.Mpq,
                                   item.PackageNumber,
                                   item.Total,
                                   item.Package,
                                   item.DateCode,
                                   item.ShelveCode,
                                   item.Exception
                               }).ToArray()
                           };

                #endregion

                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NoticesOutView SearchByID(string id)
        {
            var noticesOutView = this.IQueryable.Cast<Notice>();
            var linq = from noticeOut in noticesOutView
                       where noticeOut.ID == id
                       select noticeOut;

            var view = new NoticesOutView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据通知状态查询
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        public NoticesOutView SearchByStatus(params NoticeStatus[] statuses)
        {
            var noticesOutView = this.IQueryable.Cast<Notice>();
            var linq = from noticeOut in noticesOutView
                       where statuses.Contains(noticeOut.Status)
                       select noticeOut;

            var view = new NoticesOutView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public NoticesOutView SearchByOrderID(string orderID)
        {
            var noticesOutView = this.IQueryable.Cast<Notice>();
            var linq = from noticeOut in noticesOutView
                       where noticeOut.FormID.StartsWith(orderID)
                       select noticeOut;

            var view = new NoticesOutView(this.Reponsitory, linq);
            return view;
        }
        #endregion
    }
}

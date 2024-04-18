using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 运单详情视图
    /// </summary>
    public class WaybillDetailsView : NoticeTransportsView
    {
        public WaybillDetailsView()
        {
        }

        protected WaybillDetailsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected WaybillDetailsView(PsWmsRepository reponsitory, IQueryable<NoticeTransport> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public override object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<NoticeTransport>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            #region 补充完整对象

            var ienum_iquery = iquery.ToArray();

            //运费信息
            var waybillCodes = ienum_iquery.Select(item => item.WaybillCode);
            var waybillsView = from waybill in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Waybills>()
                               where waybillCodes.Contains(waybill.Code)
                               select new
                               {
                                   waybill.ID,
                                   waybill.Code,
                                   waybill.Freight,
                                   waybill.Weight
                               };
            var ienum_waybills = waybillsView.ToArray();

            //通知
            var transportIDs = ienum_iquery.Select(item => item.ID);
            var noticesView = from notice in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Notices>()
                              where transportIDs.Contains(notice.ConsigneeID)
                              select new
                              {
                                  notice.ID,
                                  notice.ConsigneeID
                              };
            var ienum_notices = noticesView.ToArray();

            //产品信息
            var noticeIDs = ienum_notices.Select(item => item.ID);
            var productsView = from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                               join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on item.ProductID equals product.ID
                               where noticeIDs.Contains(item.NoticeID)
                               select new
                               {
                                   item.ID,
                                   item.NoticeID,
                                   item.Mpq,
                                   item.PackageNumber,
                                   item.Total,
                                   item.UnitPrice,
                                   product.Partnumber,
                                   product.Brand
                               };
            var ienum_products = productsView.ToArray();

            var linq = from transport in ienum_iquery
                       join notice in ienum_notices on transport.ID equals notice.ConsigneeID
                       join waybill in ienum_waybills on transport.WaybillCode equals waybill.Code into waybills
                       from waybill in waybills.DefaultIfEmpty()
                       join product in ienum_products on notice.ID equals product.NoticeID into products
                       select new
                       {
                           transport.ID,
                           transport.WaybillCode,
                           transport.Carrier,
                           transport.Contact,
                           transport.Phone,
                           transport.Address,

                           waybill?.Freight,
                           waybill?.Weight,

                           TotalPrice = products.Sum(item => item.UnitPrice * item.PackageNumber), //总金额
                           TotalCount = products.Count(), //总型号数

                           Products = products.Select(item => new
                           {
                               item.Partnumber,
                               item.Brand,
                               item.Mpq,
                               item.PackageNumber,
                               item.Total
                           }).ToArray(),
                       };

            #endregion

            if (pageIndex.HasValue && pageSize.HasValue)
            {
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
                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据运单号查询
        /// </summary>
        /// <param name="waybillCode"></param>
        /// <returns></returns>
        public WaybillDetailsView SearchByWaybillCode(string waybillCode)
        {
            var noticeTransportsView = this.IQueryable.Cast<NoticeTransport>();
            var linq = from transaport in noticeTransportsView
                       where transaport.WaybillCode == waybillCode
                       select transaport;

            var view = new WaybillDetailsView(this.Reponsitory, linq);
            return view;
        }
        #endregion
    }
}

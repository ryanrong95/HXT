using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class WaybillTrackingCompanyView : UniqueView<Models.WaybillTrackingModel, ScCustomsReponsitory>
    {
        public WaybillTrackingCompanyView()
        {
        }
        internal WaybillTrackingCompanyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.WaybillTrackingModel> GetIQueryable()
        {
            var result = from companyCode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Express100CompanyCode>()
                         select new Models.WaybillTrackingModel
                         {
                             ID = companyCode.ID,
                             ExpCompanyCode = companyCode.CompanyCode,
                             ExpCompanyName = companyCode.CompanyName
                         };

            return result;
        }
    }
    public class WaybillTrackingView : QueryView<WaybillTrackingModel, ScCustomsReponsitory>
    {
        public WaybillTrackingView()
        {
        }
        internal WaybillTrackingView(ScCustomsReponsitory reponsitory, IQueryable<WaybillTrackingModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<WaybillTrackingModel> GetIQueryable()
        {
            var result = from waybillTracking in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Express100WaybillTracking>()
                         join waybillTrackingDetail in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Express100WaybillTrackingDetails>() on waybillTracking.ID equals waybillTrackingDetail.WaybillTrackingID
                         where waybillTracking.Status == (int)Status.Normal
                         select new Models.WaybillTrackingModel
                         {
                             ID = waybillTracking.ID,
                             ExpNumber = waybillTracking.ExpNumber,
                             Status = (Enums.Status)waybillTracking.Status,
                             State = (Enums.State)waybillTracking.State,
                             ExpCompanyCode = waybillTracking.ExpCompanyCode,
                             WaybillTrackingTime = waybillTrackingDetail.WaybillTrackingTime,
                             WaybillTrackingContext = waybillTrackingDetail.WaybillTrackingContext
                         };

            return result;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<WaybillTrackingModel> iquery = this.IQueryable.Cast<WaybillTrackingModel>().OrderByDescending(item => item.ID);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum = iquery.ToArray();

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return ienum.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<WaybillTrackingModel, object> convert = item => new
            {
                ID = item.ID,
                ExpNumber = item.ExpNumber,
                Status = item.Status.GetDescription(),
                State = item.State.GetDescription(),
                ExpCompanyCode = item.ExpCompanyCode,
                WaybillTrackingTime = item.WaybillTrackingTime,
                WaybillTrackingContext = item.WaybillTrackingContext
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = ienum.OrderByDescending(item => item.WaybillTrackingTime).Select(convert).ToArray(),
            };
        }
        public WaybillTrackingView SearchByCom(string com)
        {
            var linq = from query in this.IQueryable
                       where query.ExpCompanyCode == com
                       select query;

            var view = new WaybillTrackingView(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据运单号查询
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public WaybillTrackingView SearchByNum(string num)
        {
            var linq = from query in this.IQueryable
                       where query.ExpNumber == num && query.State == State.SignFor
                       select query;

            var view = new WaybillTrackingView(this.Reponsitory, linq);
            return view;
        }
    }
}
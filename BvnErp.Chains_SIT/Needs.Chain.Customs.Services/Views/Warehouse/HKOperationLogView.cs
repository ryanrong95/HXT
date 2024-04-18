using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class HKOperationLogView : QueryView<Models.HKOperationLogModel, ScCustomsReponsitory>
    {
        private string OrderID;

        public HKOperationLogView()
        {

        }

        public HKOperationLogView(string orderID) : this()
        {
            this.OrderID = orderID;
        }

        protected HKOperationLogView(ScCustomsReponsitory reponsitory, IQueryable<Models.HKOperationLogModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.HKOperationLogModel> GetIQueryable()
        {
            var sortings = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>();        
            var orderItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var baseCountries = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>();
            var AdminTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var result = from sorting in sortings

                         join orderItem in orderItems
                              on new
                              {
                                  OrderItemID = sorting.OrderItemID,
                                  OrderID = sorting.OrderID,
                                  SortingsDataStatus = sorting.Status,
                                  OrderItemsDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                              }
                              equals new
                              {
                                  OrderItemID = orderItem.ID,
                                  OrderID = this.OrderID,
                                  SortingsDataStatus = (int)Needs.Ccs.Services.Enums.Status.Normal,
                                  OrderItemsDataStatus = orderItem.Status,
                              }

                         join origin in baseCountries on orderItem.Origin equals origin.Code into baseCountries2
                         from origin in baseCountries2.DefaultIfEmpty()

                         join admin in AdminTopView on sorting.AdminID equals admin.OriginID into admins
                         from adminname in admins.DefaultIfEmpty()

                         orderby sorting.CreateDate
                         select new Models.HKOperationLogModel
                         {
                             ID = sorting.ID,
                             AdminName = adminname==null?"": adminname.RealName,
                             Model = orderItem.Model,
                             Brand = orderItem.Manufacturer,
                             Qty = sorting.Quantity,
                             Origin = origin.Name,
                             BoxIndex = sorting.BoxIndex,
                             CreateDate = sorting.CreateDate
                         };

            return result;

        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.HKOperationLogModel> iquery = this.IQueryable.Cast<Models.HKOperationLogModel>().OrderBy(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

        

            var res = ienum_EntryNotices.Select(
                        item => new
                        {
                            ID = item.ID,
                            AdminName = item.AdminName,
                            Model = item.Model,
                            Brand = item.Brand,
                            Qty = item.Qty,
                            Origin = item.Origin,
                            BoxIndex = item.BoxIndex,
                            CreateDate = item.CreateDate,
                            OperationTime = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss")
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return res.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public HKOperationLogView SearchByKeys(string key)
        {
            var linq = from query in this.IQueryable
                       where query.Model.Contains(key) || query.Brand.Contains(key)||query.BoxIndex.Contains(key)||query.Origin.Contains(key)
                       select query;

            var view = new HKOperationLogView(this.Reponsitory, linq);
            return view;
        }
    }
}

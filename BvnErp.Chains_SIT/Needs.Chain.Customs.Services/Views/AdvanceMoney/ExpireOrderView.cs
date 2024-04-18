using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExpireOrderView : QueryView<ExpireOrder, ScCustomsReponsitory>
    {
        public ExpireOrderView()
        {
        }
        internal ExpireOrderView(ScCustomsReponsitory reponsitory, IQueryable<ExpireOrder> iQueryable) : base(reponsitory, iQueryable)
        {
        }
        protected override IQueryable<Models.ExpireOrder> GetIQueryable()
        {
            var result = from expireOrder in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpireOrders>()
                         join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on expireOrder.OrderID equals order.ID
                         where expireOrder.Status == (int)Status.Normal && expireOrder.Amount>5
                         select new Models.ExpireOrder
                         {
                             ID = expireOrder.ID,                           
                             OrderID = expireOrder.OrderID,                          
                             Amount = expireOrder.Amount,   
                             ExpireDate = expireOrder.ExpireDate,
                             Status = (Status)expireOrder.Status,
                             CreateDate = expireOrder.CreateDate,
                             UpdateDate = expireOrder.UpdateDate,
                             Summary = expireOrder.Summary,
                             Admin = new Admin { ID=order.AdminID}
                         };

            return result;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<ExpireOrder> iquery = this.IQueryable.Cast<ExpireOrder>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum = iquery.ToArray();
            var adminsView = new AdminsTopView(this.Reponsitory).ToArray();

            var ienums_linq = from ienums in ienum
                                  //join admin in adminsView on ienums.AdminID equals admin.ID
                              select new ExpireOrder
                              {
                                  ID = ienums.ID,                                 
                                  OrderID = ienums.OrderID,
                                  Amount = ienums.Amount,
                                  ExpireDate = ienums.ExpireDate,
                                  Status = ienums.Status,
                                  CreateDate = ienums.CreateDate,
                                  UpdateDate = ienums.UpdateDate,
                                  Summary = ienums.Summary,
                                  Admin = ienums.Admin                                
                              };

            string days = System.Configuration.ConfigurationManager.AppSettings["ExpireDays"];
            var results = ienums_linq.Where(t=>t.ExpiredDays>=Convert.ToInt16(days));

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<ExpireOrder, object> convert = item => new
            {
                ID = item.ID,              
                OrderID = item.OrderID,              
                Amount = item.Amount,
                ExpireDate = item.ExpireDate.ToString("yyyy-MM-dd"),
                ExpiredDays = item.ExpiredDays,
                Status = item.Status,
                CreateDate = item.CreateDate,
                UpdateDate = item.UpdateDate,
                Summary = item.Summary               
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据某个跟单员自己的客户查询
        /// </summary>
        /// <param name="adminID"></param>
        /// <returns></returns>
        public ExpireOrderView SearchByAdminID(string adminID)
        {
            var linq = from query in this.IQueryable
                       where query.Admin.ID == adminID
                       select query;

            var view = new ExpireOrderView(this.Reponsitory, linq);
            return view;
        }

       
    }
}

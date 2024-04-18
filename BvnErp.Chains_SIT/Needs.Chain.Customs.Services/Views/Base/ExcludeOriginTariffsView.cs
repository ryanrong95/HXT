using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExcludeOriginTariffsView : QueryView<Models.ExcludeOriginTariffs, ScCustomsReponsitory>
    {
        public ExcludeOriginTariffsView()
        {
        }

        internal ExcludeOriginTariffsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected ExcludeOriginTariffsView(ScCustomsReponsitory reponsitory, IQueryable<Models.ExcludeOriginTariffs> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.ExcludeOriginTariffs> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExcludeOriginTariffs>()
                   select new Models.ExcludeOriginTariffs
                   {
                       ID = entity.ID,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       ExclusionPeriod = entity.ExclusionPeriod,
                       Origin = entity.Origin,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.ExcludeOriginTariffs> iquery = this.IQueryable.Cast<Models.ExcludeOriginTariffs>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue) //如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_my = iquery.ToArray();

            var ienums_linq = from entity in ienum_my
                              select new Models.ExcludeOriginTariffs
                              {
                                  ID = entity.ID,
                                  HSCode = entity.HSCode,
                                  Name = entity.Name,
                                  ExclusionPeriod = entity.ExclusionPeriod,
                                  Origin = entity.Origin,
                                  Status = (Enums.Status)entity.Status,
                                  CreateDate = entity.CreateDate,
                                  UpdateDate = entity.UpdateDate,
                                  Summary = entity.Summary
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<Models.ExcludeOriginTariffs, object> convert = accountFlow => new
            {
                ID = accountFlow.ID,
                HSCode = accountFlow.HSCode,
                Name = accountFlow.Name,
                ExclusionPeriod = accountFlow.ExclusionPeriod,
                Origin = accountFlow.Origin,
                Status = (Enums.Status)accountFlow.Status,
                CreateDate = accountFlow.CreateDate,
                UpdateDate = accountFlow.UpdateDate,
                Summary = accountFlow.Summary
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.OrderByDescending(item => item.CreateDate).Select(convert).ToArray(),
            };
        }

        public ExcludeOriginTariffsView SearchByHSCode(string hscode)
        {
            var linq = from query in this.IQueryable
                       where query.HSCode == hscode
                       select query;

            var view = new ExcludeOriginTariffsView(this.Reponsitory, linq);
            return view;
        }

        public ExcludeOriginTariffsView SearchByExclusionPeriod(string period)
        {
            var linq = from query in this.IQueryable
                       where query.ExclusionPeriod == period
                       select query;

            var view = new ExcludeOriginTariffsView(this.Reponsitory, linq);
            return view;
        }
    }
}

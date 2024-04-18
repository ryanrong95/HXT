using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 海关检疫的视图
    /// </summary>
    public class CustomsQuarantinesView : UniqueView<Models.CustomsQuarantine, ScCustomsReponsitory>
    {
        public CustomsQuarantinesView()
        {
        }

        internal CustomsQuarantinesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CustomsQuarantine> GetIQueryable()
        {
            return from quarantine in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsQuarantines>()
                   where quarantine.Status == (int)Enums.Status.Normal
                   select new Models.CustomsQuarantine
                   {
                       ID = quarantine.ID,
                       Origin = quarantine.Origin,
                       StartDate = quarantine.StartDate,
                       EndDate = quarantine.EndDate,
                       Status = (Enums.Status)quarantine.Status,
                       CreateDate = quarantine.CreateDate,
                       UpdateDate = quarantine.UpdateDate,
                       Summary = quarantine.Summary,
                   };
        }

        /// <summary>
        /// 是否检疫地区
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public bool IsQuarantine(string origin)
        {
            if (string.IsNullOrEmpty(origin))
            {
                return false;
            }

            var query = from quarantine in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsQuarantines>()
                        where quarantine.Status == (int)Enums.Status.Normal && quarantine.Origin == origin
                        && quarantine.StartDate <= DateTime.Now && quarantine.EndDate >= DateTime.Now
                        select new Models.CustomsQuarantine
                        {
                            ID = quarantine.ID,
                            Origin = quarantine.Origin,
                            StartDate = quarantine.StartDate,
                            EndDate = quarantine.EndDate,
                            Status = (Enums.Status)quarantine.Status,
                            CreateDate = quarantine.CreateDate,
                            UpdateDate = quarantine.UpdateDate,
                            Summary = quarantine.Summary,
                        };

            return query.Count() > 0;
        }
    }
}

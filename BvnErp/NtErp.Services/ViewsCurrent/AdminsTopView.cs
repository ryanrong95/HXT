using Needs.Linq;
using System;
using System.Linq;

namespace NtErp.Services.Views
{
    public class AdminsTopView<T> : UniqueView<Models.AdminTop, T> where T : Layer.Linq.IReponsitory, IDisposable, new()
    {
        internal AdminsTopView()
        {

        }
        public AdminsTopView(T reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.AdminTop> GetIQueryable()
        {
            var linqs = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.AdminsTopView>()
                   select new Models.AdminTop
                   {
                       ID = entity.ID,
                       UserName = entity.UserName,
                       RealName = entity.RealName,
                   };
            var ar = linqs.ToArray();
            return linqs;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models.PveCrm;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 企业通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class EnterprisesTopView<TReponsitory> : UniqueView<Enterprise, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public EnterprisesTopView()
        {

        }
        public EnterprisesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.EnterprisesTopView>()
                   select new Enterprise
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       RegAddress = entity.RegAddress,
                       Uscc = entity.Uscc,
                       Corporation = entity.Corperation,
                       Place = entity.Place,
                       District = entity.District
                   };
        }
    }
}

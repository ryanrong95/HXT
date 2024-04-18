using Layers.Data.Sqls.PvbCrm;
using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 品牌制造商视图
    /// </summary>
    public class ManufacturersTopView<TReponsitory> : QueryView<Manufacturer, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ManufacturersTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public ManufacturersTopView(TReponsitory reponsitory):base(reponsitory)
        {
        }


        protected override IQueryable<Manufacturer> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<ManufacturersTopView>()
                   select new Manufacturer
                   {
                       Name = entity.Name,
                       Agent = entity.Agent
                   };
        }

    }
}

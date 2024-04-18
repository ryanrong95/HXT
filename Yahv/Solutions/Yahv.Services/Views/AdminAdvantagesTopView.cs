using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 管理员优势品牌型号
    /// </summary>
    public class AdminAdvantagesTopView<TReponsitory> : QueryView<AdminAdvantage, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminAdvantagesTopView()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminAdvantagesTopView(TReponsitory reponsitory) :base(reponsitory)
        {
        }

        protected override IQueryable<AdminAdvantage> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.AdminAdvantagesTopView>()
                   select new AdminAdvantage
                   {
                       AdminID = entity.AdminID,
                       Manufacturers = entity.Manufacturers,
                       PartNumbers = entity.PartNumbers
                   };
        }

    }
}

using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 录入项视图
    /// </summary>
    internal class PayItemsOrigin : UniqueView<PayItem, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PayItemsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PayItemsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<PayItem> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<PayItems>()
                   select new PayItem()
                   {
                       ID = entity.ID,
                       DateIndex = entity.DateIndex,
                       UpdateDate = entity.UpdateDate,
                       Name = entity.Name,
                       Value = entity.Value,
                       CreateDate = entity.CreateDate,
                       AdminID = entity.AdminID,
                       PayID = entity.PayID,
                   };
        }
    }
}
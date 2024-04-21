using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    internal class PostionsOrigin : UniqueView<Postion, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal PostionsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal PostionsOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Postion> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Postions>()
                   select new Postion()
                   {
                       ID = entity.ID,
                       Status = (Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       Name = entity.Name,
                       AdminID = entity.AdminID,
                   };
        }
    }
}
using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 子节点
    /// </summary>
    public class LeagueSubs : UniqueView<League, PvbErmReponsitory>
    {
        League father;

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="father"></param>
        public LeagueSubs(League father)
        {
            this.father = father;
        }

        protected override IQueryable<League> GetIQueryable()
        {
            var leaguesView = new LeaguesOrigin(this.Reponsitory);
            return from item in leaguesView
                   where item.FatherID == this.father.ID
                   select item;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="entity"></param>
        public void Add(League entity)
        {
            entity.Enter();
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            if (this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Leagues>().Any(item => item.ID == id))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvbErm.Leagues>(new
                {
                    Status = Status.Delete,
                }, item => item.ID == id);
            }
        }
    }
}

using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class AdvantagesOrigin : Yahv.Linq.UniqueView<Advantage, PvbCrmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal AdvantagesOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal AdvantagesOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Advantage> GetIQueryable()
        {
            var enterpriseView = new Origins.EnterprisesOrigin(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Advantages>()
                   join enterprise in enterpriseView on entity.EnterpriseID equals enterprise.ID
                   select new Advantage
                   {
                       Manufacturers = entity.Manufacturers,
                       PartNumbers = entity.PartNumbers,
                       Enterprise = enterprise
                   };
        }
    }
}

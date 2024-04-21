using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class BrandsRoll : UniqueView<StandardBrand, PvdCrmReponsitory>
    {
        public  BrandsRoll(){}

        protected override IQueryable<Models.Origins.StandardBrand> GetIQueryable()
        {
            return new BrandsOrigin(this.Reponsitory);
        }


        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected BrandsRoll(PvdCrmReponsitory reponsitory, IQueryable<StandardBrand> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        public BrandsRoll SearchByName(string txt)
        {
            var iQuery = this.IQueryable;

            var linq = from brand in iQuery
                       where brand.Name.Contains(txt) || brand.Code.Contains(txt) || brand.ChineseName.Contains(txt)
                       select brand;

            return new BrandsRoll(this.Reponsitory, linq);
        }

        public BrandsRoll Search(Expression<Func<StandardBrand, bool>> expression)
        {
            var iQuery = this.IQueryable;

            return new BrandsRoll(this.Reponsitory, iQuery.Where(expression));
        }

    }

   
}

using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class StandardBrandsRoll : Origins.StandardBrandsOrigin
    {
        public StandardBrandsRoll()
        {

        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected StandardBrandsRoll(PvbCrmReponsitory reponsitory, IQueryable<StandardBrand> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<StandardBrand> GetIQueryable()
        {
            return base.GetIQueryable();
        }

        /// <summary>
        /// 根据同义词名称或是同义词进行搜索
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns>相关的品牌</returns>
        public StandardBrandsRoll SearchByName(string txt)
        {
            var iQuery = this.IQueryable;
            var linq = from brand in iQuery
                       where brand.Name.Contains(txt) || brand.ShortName.Contains(txt)
                       select brand;

            return new StandardBrandsRoll(this.Reponsitory, linq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public StandardBrandsRoll Search(Expression<Func<StandardBrand, bool>> expression)
        {
            var iQuery = this.IQueryable;

            return new StandardBrandsRoll(this.Reponsitory, iQuery.Where(expression));
        }


    }
}

using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class EnumsDictionariesRoll : Linq.UniqueView<Models.Origins.EnumsDictionary, PvdCrmReponsitory>
    {
        //string enumname;
        public EnumsDictionariesRoll()
        {

        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected EnumsDictionariesRoll(PvdCrmReponsitory reponsitory, IQueryable<Models.Origins.EnumsDictionary> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        protected override IQueryable<Models.Origins.EnumsDictionary> GetIQueryable()
        {
            return new Origins.EnumsDictionariesOrigin(this.Reponsitory);
        }
        public EnumsDictionariesRoll Search(Expression<Func<Models.Origins.EnumsDictionary, bool>> expression)
        {
            var iQuery = this.IQueryable;

            return new EnumsDictionariesRoll(this.Reponsitory, iQuery.Where(expression));
        }

        public EnumsDictionariesRoll Search<T>()
        {
            var type = typeof(T);
            var iQuery = this.IQueryable.Where(item => item.Enum == type.Name);
            return new EnumsDictionariesRoll(this.Reponsitory, iQuery);
        }


        //public EnumsDictionariesRoll this[ index]
        //{
        //    get { /* return the specified index here */ }
        //}

    }
}

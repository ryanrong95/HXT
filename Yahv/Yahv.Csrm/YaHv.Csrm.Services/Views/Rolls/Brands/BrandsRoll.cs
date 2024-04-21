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
    public class BrandsRoll : Origins.BrandsOrigin
    {
        public BrandsRoll()
        {

        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        /// <param name="iQueryable">替换可查询集</param>
        protected BrandsRoll(PvbCrmReponsitory reponsitory, IQueryable<Brand> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<Brand> GetIQueryable()
        {
            return base.GetIQueryable();
        }

        /// <summary>
        /// 根据同义词名称或是同义词进行搜索
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns>相关的品牌</returns>
        public BrandsRoll SearchByName(string txt)
        {
            var iQuery = this.IQueryable;
            var linq = from brand in iQuery
                       where brand.Name.Contains(txt) || brand.ShortName.Contains(txt)
                       select brand;

            return new BrandsRoll(this.Reponsitory, linq);
        }

        /// <summary>
        /// 根据同义词名称或是同义词进行搜索
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns>相关的品牌</returns>
        public BrandsRoll SearchByNameOrDic(string txt)
        {
            var iQuery = this.IQueryable;

            var linq_dic = from dic in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.BrandDictionary>()
                           where dic.Name.Contains(txt) || dic.OtherName.Contains(txt)
                           select dic.Name;

            var linq = from name in linq_dic.Distinct()
                       join brand in iQuery on name equals brand.Name
                       select brand;

            return new BrandsRoll(this.Reponsitory, linq);
        }


        /// <summary>
        /// 根据同义词名称或是同义词进行搜索
        /// </summary>
        /// <param name="txt">内容</param>
        /// <returns>相关的品牌</returns>
        public BrandsRoll Search(Expression<Func<Brand, bool>> expression)
        {
            var iQuery = this.IQueryable;

            return new BrandsRoll(this.Reponsitory, iQuery.Where(expression));
        }


     

        public object ToPaging(int pageIndex, int pageSize)
        {
            int total = this.IQueryable.Count();
            var data = this.IQueryable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToArray();
            var names = data.Select(item => item.Name);
            var linq = from dic in new Origins.BrandDictionaryOrigin(this.Reponsitory)
                       where names.Contains(dic.Name) || names.Contains(dic.OtherName)
                       select dic;

            var dicArry = linq.ToArray();

            var linqs = from brand in data
                        join dic in dicArry on brand.Name equals dic.Name into dics
                        select new
                        {
                            brand.ID,
                            brand.Name,
                            brand.Status,
                            StatusName = brand.Status.GetDescription(),
                            brand.ShortName,
                            OtherNames = dics.Select(item => item.OtherName)
                        };

            return new
            {
                rows = linqs.ToArray(),
                total = total
            };

        }
    }
}

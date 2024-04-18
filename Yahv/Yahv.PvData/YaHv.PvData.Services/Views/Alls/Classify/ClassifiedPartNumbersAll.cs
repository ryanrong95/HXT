using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 海关税则归类信息
    /// </summary>
    public class ClassifiedPartNumbersAll : UniqueView<Models.ClassifiedPartNumber, PvDataReponsitory>
    {
        public ClassifiedPartNumbersAll()
        {
        }

        internal ClassifiedPartNumbersAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClassifiedPartNumber> GetIQueryable()
        {
            return new Origins.ClassifiedPartNumbersOrigin(this.Reponsitory).OrderByDescending(cpn => cpn.OrderDate);
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌/制造商</param>
        /// <returns></returns>
        public Models.ClassifiedPartNumber this[string partNumber, string manufacturer]
        {
            get
            {
                return this.FirstOrDefault(cpn => cpn.PartNumber == partNumber && cpn.Manufacturer == manufacturer);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacturer">品牌</param>
        /// <param name="months">月数</param>
        /// <returns></returns>
        public Models.ClassifiedPartNumber this[string partNumber, string manufacturer, int months]
        {
            get
            {
                return this.FirstOrDefault(cpn => cpn.PartNumber == partNumber && cpn.Manufacturer == manufacturer && cpn.OrderDate >= DateTime.Now.AddMonths(months));
            }
        }
    }
}

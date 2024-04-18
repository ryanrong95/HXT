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
    /// 申报要素品牌的视图
    /// </summary>
    public class ElementsManufacturersAll : QueryView<Models.ElementsManufacturer, PvDataReponsitory>
    {
        public ElementsManufacturersAll()
        {
        }

        internal ElementsManufacturersAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ElementsManufacturer> GetIQueryable()
        {
            var linqs = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ElementsManufacturers>()
                        select new Models.ElementsManufacturer
                        {
                            ID = entity.ID,
                            Manufacturer = entity.Manufacturer,
                            MfrMapping = entity.MfrMapping,
                            CreateDate = entity.CreateDate,
                            ModifyDate = entity.ModifyDate
                        };

            return linqs;
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="manufacturer">品牌</param>
        /// <returns></returns>
        public Models.ElementsManufacturer this[string manufacturer]
        {
            get
            {
                return this.SingleOrDefault(item => item.Manufacturer.Trim().ToLower() == manufacturer.Trim().ToLower());
            }
        }
    }
}

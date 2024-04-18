using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 产品管控信息
    /// </summary>
    public class ProductControlsAll : UniqueView<Models.ProductControl, PvDataReponsitory>
    {
        public ProductControlsAll()
        {
        }

        internal ProductControlsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductControl> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ProductControls>()
                   select new Models.ProductControl
                   {
                       ID = entity.ID,
                       Type = (ControlType)entity.Type,
                       Name = entity.Name,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       CreateDate = entity.CreateDate
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="type">管控类型</param>
        /// <returns></returns>
        public Models.ProductControl this[string partNumber, ControlType type]
        {
            get
            {
                return this.FirstOrDefault(pc => pc.PartNumber == partNumber && pc.Type == type);
            }
        }
    }

    public class ProductControlsView : ProductControlsAll
    {
        private string PartNumber = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partNumber">产品型号</param>
        public ProductControlsView(string partNumber)
        {
            this.PartNumber = partNumber;
        }

        protected override IQueryable<Models.ProductControl> GetIQueryable()
        {
            return base.GetIQueryable().Where(pc => pc.PartNumber == this.PartNumber);
        }
    }

}

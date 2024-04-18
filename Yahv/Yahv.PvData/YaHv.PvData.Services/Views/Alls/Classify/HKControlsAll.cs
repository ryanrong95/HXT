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
    public class HKControlsAll : UniqueView<Models.HKControl, PvDataReponsitory>
    {

        public HKControlsAll()
        {
        }

        internal HKControlsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<HKControl> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.HKControls>()
                   select new HKControl()
                   {
                       ID = entity.ID,
                       Brand = entity.Brand,
                       Model = entity.Model,
                       Description = entity.Description,
                       Type = entity.Type,
                       isControl = entity.isControl,
                       Status = (Yahv.Underly.GeneralStatus)entity.Status,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate
                   };
        }


        public IQueryable<HKControl> this[string brand = null, string model = null, string isControl = null]
        {
            get
            {

                return this.Where(item => item.Brand.StartsWith(brand));
            }
        }
    }
}

using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// Eccn信息查询视图
    /// </summary>
    public class EccnsAll : UniqueView<Yahv.Services.Models.Eccn, PvDataReponsitory>
    {
        public EccnsAll()
        {

        }
        public EccnsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Yahv.Services.Models.Eccn> GetIQueryable()
        {
            return new Yahv.Services.Views.EccnsTopView<PvDataReponsitory>(this.Reponsitory);
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <returns></returns>
        public new IQueryable<Yahv.Services.Models.Eccn> this[string partNumber]
        {
            get
            {
                return this.Where(item => item.PartNumber.Trim() == partNumber.Trim()).OrderByDescending(item => item.ModifyDate);
            }
        }

        public void Enter(Yahv.Services.Models.Eccn eccn)
        {
            
            if (!this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Eccn>().Any(item => item.ID == eccn.ID))
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvData.Eccn()
                {
                    ID = eccn.ID,
                    PartNumber = eccn.PartNumber,
                    Manufacturer = eccn.Manufacturer,
                    Code = eccn.Code,
                    LastOrigin = eccn.LastOrigin,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PvData.Eccn>(new
                {
                    Code = eccn.Code,
                    ModifyDate = DateTime.Now
                }, a => a.ID == eccn.ID);
            }
        }
    }
}

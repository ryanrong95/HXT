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
    public class CustomsControlsAll : UniqueView<Models.CustomsControl, PvDataReponsitory>
    {
        public CustomsControlsAll()
        {
        }

        internal CustomsControlsAll(PvDataReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsControl> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvData.CustomsControls>()
                   where entity.Status == (int)Yahv.Underly.GeneralStatus.Normal
                   select new Models.CustomsControl
                   {
                       ID = entity.ID,
                       Type = (CustomsControlType)entity.Type,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       PartNumber = entity.PartNumber,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="HSCode">海关编码</param>
        /// <param name="type">卡控类型</param>
        /// <returns></returns>
        public Models.CustomsControl this[string HSCode, CustomsControlType type]
        {
            get
            {
                return this.FirstOrDefault(cc => cc.HSCode == HSCode && cc.Type == type);
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="partNumber">型号</param>
        /// <param name="name">品名</param>
        /// <param name="type">卡控类型</param>
        /// <returns></returns>
        public Models.CustomsControl this[string partNumber, string name, CustomsControlType type]
        {
            get
            {
                return this.FirstOrDefault(cc => cc.PartNumber == partNumber && cc.Name == name && cc.Type == type);
            }
        }
    }
}

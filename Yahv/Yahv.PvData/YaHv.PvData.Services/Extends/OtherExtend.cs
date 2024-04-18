using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Extends
{
    public static class OtherExtend
    {
        /// <summary>
        /// 更新Ccc管控历史纪录
        /// </summary>
        /// <param name="other"></param>
        public static void UpdateCccControl(this Models.Other other)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                {
                    Ccc = other.Ccc
                }, a => a.PartNumber == other.PartNumber && a.Manufacturer == other.Manufacturer);
            }
        }

        /// <summary>
        /// 更新禁运管控历史纪录
        /// </summary>
        /// <param name="other"></param>
        public static void UpdateEmbargoControl(this Models.Other other)
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                {
                    Embargo = other.Embargo
                }, a => a.PartNumber == other.PartNumber && a.Manufacturer == other.Manufacturer);
            }
        }
    }
}

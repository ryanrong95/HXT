using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.VcCsrm.Service.Models;

namespace YaHv.VcCsrm.Service.Views.Origins
{
    public class WsConsignorsOrigin : Yahv.Linq.UniqueView<WsConsignor, PvcCrmReponsitory>
    {
        internal WsConsignorsOrigin()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal WsConsignorsOrigin(PvcCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<WsConsignor> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvcCrm.WsConsignors>()
                       select new WsConsignor
                       {
                           ID = entity.ID,
                           Title = entity.Title,
                           WsSupplierID = entity.WsSupplierID,
                           DyjCode = entity.DyjCode,
                           Address = entity.Address,
                           Postzip = entity.Postzip,
                           Name = entity.Name,
                           Tel = entity.Tel,
                           Mobile = entity.Mobile,
                           Email = entity.Email,
                           Status = (GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           CreatorID = entity.CreatorID,
                           Province = entity.Province,
                           City = entity.City,
                           Area = entity.Area,
                           IsDefault=entity.IsDefault
                       };
            return linq;
        }
    }
}

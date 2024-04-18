using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Origins
{
    /// <summary>
    /// 海关检验检疫编码视图
    /// </summary>
    internal class CustomsCiqCodesOrigin : UniqueView<Models.Origins.CustomsCiqCode, ScCustomsReponsitory>
    {
        internal CustomsCiqCodesOrigin()
        {
        }

        internal CustomsCiqCodesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Origins.CustomsCiqCode> GetIQueryable()
        {
            return from cert in this.Reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsCiqCodes>()
                   select new Models.Origins.CustomsCiqCode
                   {
                       ID = cert.ID,
                       Category = cert.Category,
                       CreateDate = cert.CreateDate,
                       InspectionCode = cert.InspectionCode,
                       Name = cert.Name,
                       Status = (Enums.Status)cert.Status,
                       Summary = cert.Summary,
                       UpdateDate = cert.UpdateDate,
                   };
        }
    }
}

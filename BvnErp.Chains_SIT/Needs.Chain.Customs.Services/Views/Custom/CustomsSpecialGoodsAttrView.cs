using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 两步申报途数税号的视图
    /// </summary>
    public class CustomsSpecialGoodsAttrView : UniqueView<Models.CustomsSpecialGoodsAttr, ScCustomsReponsitory>
    {
        public CustomsSpecialGoodsAttrView()
        {
        }

        internal CustomsSpecialGoodsAttrView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<CustomsSpecialGoodsAttr> GetIQueryable()
        {
            return from specialAttr in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsSpecialGoodsAttr>()
                   where specialAttr.Status == (int)Enums.Status.Normal
                   select new Models.CustomsSpecialGoodsAttr
                   {
                       ID = specialAttr.ID,
                       HSCode = specialAttr.HSCode,
                       GoodsAttr = specialAttr.GoodsAttr,
                       Status = (Enums.Status)specialAttr.Status,
                       CreateDate = specialAttr.CreateDate
                   };
        }
    }
}
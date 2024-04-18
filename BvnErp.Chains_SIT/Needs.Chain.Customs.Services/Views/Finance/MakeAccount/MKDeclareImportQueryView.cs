using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class MKDeclareImportQueryView : UniqueView<Models.MKDeclareImport, ScCustomsReponsitory>
    {

        public MKDeclareImportQueryView()
        {
        }

        internal MKDeclareImportQueryView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected MKDeclareImportQueryView(ScCustomsReponsitory reponsitory, IQueryable<Models.MKDeclareImport> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Models.MKDeclareImport> GetIQueryable()
        {
            var iquery = from mk in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MKDeclareImport>()
                         select new Models.MKDeclareImport {
                             ID = mk.ID,
                             RequestID = mk.RequestID,
                             TemplateCode = mk.TemplateCode,
                             SchemeCode = mk.SchemeCode,
                             Type = mk.Type,
                             InvoiceType = mk.InvoiceType,
                             DeclareDate = mk.DeclareDate,
                             Tian = mk.Tian,
                             Jinkou = mk.Jinkou,
                             Huokuan = mk.Huokuan,
                             Yunbaoza = mk.Yunbaoza,
                             Guanshui = mk.Guanshui,
                             GuanshuiShijiao = mk.GuanshuiShijiao,
                             Xiaofeishui = mk.Xiaofeishui,
                             XiaofeishuiShijiao = mk.XiaofeishuiShijiao,
                             Shui = mk.Shui,
                             Jinxiangshui = mk.Jinxiangshui,
                             HuiduiSanfang = mk.HuiduiSanfang,
                             Sanfang = mk.Sanfang,
                             HuiduiWofang = mk.HuiduiWofang,
                             Huilv = mk.Huilv,
                             YingfuSanfang = mk.YingfuSanfang,
                             Wuliufang = mk.Wuliufang,
                             YingfuWofang = mk.YingfuWofang,
                             Currency = mk.Currency,
                             PingzhengZi = mk.PingzhengZi,
                             PingzhengHao = mk.PingzhengHao,
                             Status = (Enums.Status)mk.Status,
                             CreateDate = mk.CreateDate,
                             UpdateDate = mk.UpdateDate,
                             Summary = mk.Summary
                         };
            return iquery;
        }
    }
}

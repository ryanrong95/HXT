using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.Views
{
    public class WsSupplierAlls : UniqueView<WsSupplier, PvWsOrderReponsitory>
    {
        public WsSupplierAlls()
        {

        }

        internal WsSupplierAlls(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            var supplierTopView = new Yahv.Services.Views.wsnSuppliersTopView<PvWsOrderReponsitory>(this.Reponsitory);
            var linq = from entity in supplierTopView
                       where entity.Status == GeneralStatus.Normal
                       select new WsSupplier
                       {
                           ID = entity.ID,
                           OwnID = entity.OwnID,//所属企业即客户ID 
                           OwnName = entity.OwnName,//客户名称
                           RealEnterpriseID = entity.RealEnterpriseID,//供应商ID
                           RealEnterpriseName = entity.RealEnterpriseName,//供应商名称
                           Name = entity.RealEnterpriseName,//供应商名称
                           nGrade = entity.nGrade,
                           EnterCode = entity.EnterCode,
                           ClientGrade = entity.ClientGrade,
                           Place = entity.Place,
                           Status = entity.Status,
                           Corporation = entity.Corporation,//供应商企业信息
                           Uscc = entity.Uscc,//供应商企业信息
                           RegAddress = entity.RegAddress,//供应商企业信息
                           ChineseName = entity.ChineseName,//中文名称
                           EnglishName = entity.EnglishName,//英文名称
                           CHNabbreviation = entity.CHNabbreviation//中文简称
                       };
            return linq;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 客户供应商的私有地址
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsnSupplierConsignorsTopView<TReponsitory> : UniqueView<wsnSupplierConsignor, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsnSupplierConsignorsTopView()
        {

        }

        public wsnSupplierConsignorsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsnSupplierConsignor> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSupplierConsignor>()
                   select new wsnSupplierConsignor
                   {
                       ID = entity.ID,
                       nSupplierID = entity.nSupplierID,//供应商ID
                       OwnID = entity.OwnID,//客户
                       OwnName = entity.OwnName,//客户
                       RealEnterpriseID = entity.RealEnterpriseID,//供应商企业ID
                       RealEnterpriseName = entity.RealEnterpriseName,//供应商企业名称
                       nGrade = (SupplierGrade)entity.nGrade,
                       Title = entity.Title,
                       PostZip = entity.Postzip,
                       Address = entity.Address,
                       Place = entity.Place,
                       Contact = entity.Contact,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       EnterCode = entity.EnterCode,
                       ClientGrade = (ClientGrade)entity.ClientGrade,
                       IsDefault = entity.IsDefault,
                       Status = (GeneralStatus)entity.Status
                   };
        }
    }
}

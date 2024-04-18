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
    /// 客户供应商的私有联系人信息
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsnSupplierContactsTopView<TReponsitory> : UniqueView<wsnContact, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsnSupplierContactsTopView()
        {

        }

        public wsnSupplierContactsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsnContact> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSupplierContactsTopView>()
                   select new wsnContact
                   {
                       ID = entity.ID,
                       nSupplierID = entity.nSupplierID,//供应商ID
                       OwnID = entity.OwnID,//客户企业ID
                       OwnName = entity.OwnName,//客户名称
                       RealID = entity.RealID,//供应商企业ID
                       RealEnterpriseName = entity.RealName,//供应商企业名称
                       Name = entity.Name,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Fax = entity.Fax,
                       QQ = entity.QQ,
                       Status = (GeneralStatus)entity.Status
                   };
        }
    }
}

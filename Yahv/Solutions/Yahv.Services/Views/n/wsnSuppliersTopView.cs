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
    /// 客户的私有供应商
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsnSuppliersTopView<TReponsitory> : UniqueView<wsnSuppliers, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsnSuppliersTopView()
        {

        }

        public wsnSuppliersTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsnSuppliers> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSuppliersTopView>()
                       where entity.Status != (int)GeneralStatus.Deleted
                       select new wsnSuppliers
                       {
                           ID = entity.ID,
                           Name=entity.RealEnterpriseName,//和RealEnterpriseName重复了
                           OwnID = entity.OwnID,//所属企业即客户ID 
                           OwnName = entity.OwnName,//客户名称
                           RealEnterpriseID = entity.RealEnterpriseID,//供应商ID
                           RealEnterpriseName = entity.RealEnterpriseName,//供应商名称
                           nGrade = (SupplierGrade)entity.nGrade,
                           EnterCode = entity.EnterCode,
                           ClientGrade = (ClientGrade)entity.ClientGrade,
                           Place = entity.Place,
                           Status = (GeneralStatus)entity.Status,
                           Corporation = entity.Corporation,//供应商企业信息
                           Uscc = entity.Uscc,//供应商企业信息
                           RegAddress = entity.RegAddress,//供应商企业信息
                           ChineseName=entity.ChineseName,//中文名称
                           EnglishName=entity.EnglishName,//英文名称
                           CHNabbreviation=entity.CHNabbreviation//中文简称
                       };
            return linq;
        }
    }
}

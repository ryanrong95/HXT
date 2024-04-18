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
    /// 客户供应商的私有收款人
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsnSupplierPayeesTopView<TReponsitory> : UniqueView<wsnSupplierPayee, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsnSupplierPayeesTopView()
        {

        }

        public wsnSupplierPayeesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsnSupplierPayee> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSupplierPayeesTopView>()
                   select new wsnSupplierPayee
                   {
                       ID = entity.ID,
                       nSupplierID = entity.nSupplierID,//供应商ID
                       OwnID = entity.OwnID,//客户ID
                       OwnName = entity.OwnName,//客户名称
                       RealEnterpriseID = entity.RealEnterpriseID,//付款企业ID，即供应商企业ID
                       RealEnterpriseName = entity.RealEnterpriseName,//付款企业名称供应商企业名称
                       nGrade = (SupplierGrade)entity.nGrade,
                       Methord = (Methord)entity.Methord,
                       Bank = entity.Bank,
                       BankAddress = entity.BankAddress,
                       Account = entity.Account,
                       SwiftCode = entity.SwiftCode,
                       Currency = (Currency)entity.Currency,
                       Contact = entity.Contact,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Place = entity.Place,
                       EnterCode = entity.EnterCode,
                       ClientGrade = (ClientGrade)entity.ClientGrade,
                       Status = (GeneralStatus)entity.Status,
                       IsDefault = entity.IsDefault
                   };
            return linq;
        }
    }
}

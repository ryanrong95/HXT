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
    /// 公有付款人：客户付款人
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class wsPayersTopView<TReponsitory> : UniqueView<wsPayer, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public wsPayersTopView()
        {

        }

        public wsPayersTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsPayer> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsPayersTopView>()
                       select new wsPayer
                       {
                           ID = entity.ID,
                           EnterpriseID = entity.EnterpriseID,//客户
                           EnterpriseName = entity.EnterpriseName,//客户名称
                           RealEnterpriseID = entity.RealEnterpriseID,//真正的付款企业ID，或者叫做代付企业ID
                           RealEnterpriseName = entity.RealEnterpriseName,//真正的付款企业名称
                           Bank = entity.Bank,
                           BankAddress = entity.BankAddress,
                           Account = entity.Account,
                           Currency = (Currency)entity.Currency,
                           Methord = (Methord)entity.Methord,
                           SwiftCode = entity.SwiftCode,
                           Contact = entity.Contact,
                           Email = entity.Email,
                           Mobile = entity.Mobile,
                           Tel = entity.Tel,
                           Place = entity.Place,
                           Status = (GeneralStatus)entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           CreatorID = entity.Creator
                       };
            return linq;
        }
    }
}

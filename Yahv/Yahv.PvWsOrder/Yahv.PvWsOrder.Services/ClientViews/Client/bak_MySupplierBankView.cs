using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    ///// <summary>
    ///// 供应商受益人视图
    ///// </summary>
    //public class MySupplierBankView : WsBeneficiariesTopView<PvWsOrderReponsitory>
    //{
    //    private string EnterpriseID;
    //    private string SupplierID;

    //    private MySupplierBankView()
    //    {

    //    }

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    /// <param name="enterpriseid">客户ID</param>
    //    /// <param name="supplierid">供应商ID</param>
    //    public MySupplierBankView(string enterpriseid, string supplierid)
    //    {
    //        this.EnterpriseID = enterpriseid;
    //        this.SupplierID = supplierid;
    //    }

    //    /// <summary>
    //    /// 查询结果集
    //    /// </summary>
    //    /// <returns></returns>
    //    protected override IQueryable<Beneficiary> GetIQueryable()
    //    {
    //        return base.GetIQueryable().Where(item => item.ClientID == this.EnterpriseID && item.EnterpriseID == this.SupplierID);
    //    }

    //    /// <summary>
    //    /// 数据持久化
    //    /// </summary>
    //    /// <param name="beneficiary"></param>
    //    public void Enter(Beneficiary beneficiary)
    //    {
    //        //根据规则生成ID
    //        var id = beneficiary.ID ?? string.Join("", beneficiary.EnterpriseID, beneficiary.RealName.MD5(), beneficiary.Bank, beneficiary.BankAddress,
    //                beneficiary.Account, beneficiary.SwiftCode, beneficiary.Methord, beneficiary.Currency).MD5();

    //        if (Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.Beneficiaries>().Any(item => item.ID == id))
    //        {
    //            Reponsitory.Update<Layers.Data.Sqls.PvbCrm.Beneficiaries>(new
    //            {
    //                InvoiceType = (int?)beneficiary.InvoiceType,
    //                RealID = beneficiary.RealName.MD5(),
    //                RealName = beneficiary.RealName,
    //                Bank = beneficiary.Bank,
    //                BankAddress = beneficiary.BankAddress,
    //                Account = beneficiary.Account,
    //                SwiftCode = beneficiary.SwiftCode,
    //                Methord = (int)beneficiary.Methord,
    //                Currency = (int)beneficiary.Currency,
    //                District = (int)beneficiary.District,
    //                Name = beneficiary.Name,
    //                Tel = beneficiary.Tel,
    //                Mobile = beneficiary.Mobile,
    //                Email = beneficiary.Email,
    //                UpdateDate = DateTime.Now,
    //            }, item => item.ID == id);
    //        }
    //        else
    //        {
    //            Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Beneficiaries
    //            {
    //                ID = id,
    //                EnterpriseID = beneficiary.EnterpriseID,
    //                InvoiceType = (int)beneficiary.InvoiceType,
    //                RealID = beneficiary.RealName.MD5(),
    //                RealName = beneficiary.RealName,
    //                Bank = beneficiary.Bank,
    //                BankAddress = beneficiary.BankAddress,
    //                Account = beneficiary.Account,
    //                SwiftCode = beneficiary.SwiftCode,
    //                Methord = (int)beneficiary.Methord,
    //                Currency = (int)beneficiary.Currency,
    //                District = (int)beneficiary.District,
    //                Name = beneficiary.Name,
    //                Tel = beneficiary.Tel,
    //                Mobile = beneficiary.Mobile,
    //                Email = beneficiary.Email,
    //                Status = (int)ApprovalStatus.Normal,
    //                CreateDate = DateTime.Now,
    //                UpdateDate = DateTime.Now,
    //                AdminID = beneficiary.AdminID,
    //            });
    //        }

    //        //关系主键生成，关系插入
    //        var mapsid = "WsBeneficiary_" + string.Join("", this.EnterpriseID, id).MD5();
    //        if (!Reponsitory.GetTable<Layers.Data.Sqls.PvbCrm.MapsBEnter>().Any(item => item.ID == mapsid))
    //        {
    //            Reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.MapsBEnter
    //            {
    //                ID = mapsid,
    //                Bussiness = (int)Business.WarehouseServicing,
    //                Type = (int)MapsType.Beneficiary,
    //                EnterpriseID = this.EnterpriseID,
    //                SubID = id,
    //                CreateDate = DateTime.Now,
    //                CtreatorID = beneficiary.AdminID,
    //                IsDefault = false,
    //            });
    //        }
    //    }

    //    /// <summary>
    //    /// 删除
    //    /// </summary>
    //    /// <param name="ID"></param>
    //    public bool Delete(string ID)
    //    {
    //        var beneficiary = this[ID];
    //        if (beneficiary == null)
    //        {
    //            return false;
    //        }
    //        var mapsid = "WsBeneficiary_" + string.Join("", this.EnterpriseID, ID).MD5();
    //        //删除收益人与供应商的关系
    //        this.Reponsitory.Delete<Layers.Data.Sqls.PvbCrm.MapsBEnter>(item => item.ID == mapsid);
    //        return true;
    //    }
    //}
}

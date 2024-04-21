using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 当前登录人的供应商
    /// </summary>
    public class MyTradingSuppliersRoll : Origins.TradingSuppliersOrigin
    {

        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingSuppliersRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingSupplier> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else
            {
                var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
                return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
            }

        }
    }
    /// <summary>
    /// 某Admin的供应商
    /// </summary>
    public class AdminSuppliersRoll : Origins.TradingSuppliersOrigin
    {

        Admin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminSuppliersRoll(Admin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingSupplier> GetIQueryable()
        {
            var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
            return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));

        }
    }
    #region 合作公司
    ///// <summary>
    ///// 当前登录人的供应商
    ///// </summary>
    //public class MyTradingSuppliersRoll : Origins.TradingSuppliersOrigin
    //{

    //    IErpAdmin admin;

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public MyTradingSuppliersRoll(IErpAdmin admin)
    //    {
    //        this.admin = admin;
    //    }
    //    protected override IQueryable<TradingSupplier> GetIQueryable()
    //    {
    //        if (this.admin.IsSuper)
    //        {
    //            return base.GetIQueryable();
    //        }
    //        else
    //        {
    //            var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier, Business.Trading_Purchase).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
    //            return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
    //        }

    //    }
    //}
    ///// <summary>
    ///// 某Admin的供应商
    ///// </summary>
    //public class AdminSuppliersRoll : Origins.TradingSuppliersOrigin
    //{

    //    Admin admin;

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public AdminSuppliersRoll(Admin admin)
    //    {
    //        this.admin = admin;
    //    }
    //    protected override IQueryable<TradingSupplier> GetIQueryable()
    //    {
    //        var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Supplier, Business.Trading_Purchase).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
    //        return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));

    //    }
    //}
    #endregion

}

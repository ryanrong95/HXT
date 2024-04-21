using Layers.Data.Sqls;
using Layers.Linq;
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
    /// 当前登录人的客户
    /// </summary>
    public class MyTradingClientsRoll : Origins.TradingClientsOrigin
    {
        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingClientsRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingClient> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else
            {
                var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
                return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
            }
        }

    }


    /// <summary>
    /// 某Admin的客户
    /// </summary>
    public class AdminClientsRoll : Origins.TradingClientsOrigin
    {
        Admin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminClientsRoll(Admin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingClient> GetIQueryable()
        {
            var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
            return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
        }

    }
    #region 合作公司
    ///// <summary>
    ///// 当前登录人的客户
    ///// </summary>
    //public class MyTradingClientsRoll : Origins.TradingClientsOrigin
    //{
    //    IErpAdmin admin;

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public MyTradingClientsRoll(IErpAdmin admin)
    //    {
    //        this.admin = admin;
    //    }
    //    protected override IQueryable<TradingClient> GetIQueryable()
    //    {
    //        if (this.admin.IsSuper)
    //        {
    //            return base.GetIQueryable();
    //        }
    //        else
    //        {
    //            var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client, Business.Trading_Sale).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
    //            return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
    //        }
    //    }

    //}


    ///// <summary>
    ///// 某Admin的客户
    ///// </summary>
    //public class AdminClientsRoll : Origins.TradingClientsOrigin
    //{
    //    Admin admin;

    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public AdminClientsRoll(Admin admin)
    //    {
    //        this.admin = admin;
    //    }
    //    protected override IQueryable<TradingClient> GetIQueryable()
    //    {
    //        var entepriseids = new Rolls.TradingAdminsRoll(this.Reponsitory, MapsType.Client, Business.Trading_Sale).Where(a => a.ID == this.admin.ID).Select(item => item.EnterpriseID);
    //        return base.GetIQueryable().Where(item => entepriseids.Contains(item.ID));
    //    }

    //}
    #endregion

}

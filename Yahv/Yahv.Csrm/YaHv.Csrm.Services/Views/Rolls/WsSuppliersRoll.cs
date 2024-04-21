using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 当前登录人的的代仓储供应商
    /// </summary>
    public class MyWsSuppliersRoll : Origins.WsSuppliersOrigins
    {
        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyWsSuppliersRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                return base.GetIQueryable();
            }
            else
            {
                return from maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsAdmin>()
                       join wssupplier in base.GetIQueryable() on maps.RealID equals wssupplier.ID
                       where maps.AdminID == this.admin.ID
                       select wssupplier;
            }
        }

    }
    public class WsSuppliersRoll : Origins.WsSuppliersOrigins
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WsSuppliersRoll()
        {

        }
        protected override IQueryable<WsSupplier> GetIQueryable()
        {
            return base.GetIQueryable();

        }
    }
    //客户的供应商
    public class XdtWsSuppliersView : Origins.XdtWsSuppliersOrigins
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public XdtWsSuppliersView(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<XdtWsSupplier> GetIQueryable()
        {
            return from entity in base.GetIQueryable() where entity.WsClient.ID == this.enterprise.ID select entity;
        }
    }
}

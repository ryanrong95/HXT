using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 运输工具
    /// </summary>
    public class TransportsRoll : Origins.TansportOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 构造函数
        /// </summary>
        public TransportsRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Transport> GetIQueryable()
        {
            if (enterprise == null)
            {
                return base.GetIQueryable();
            }
            else
            {
                return base.GetIQueryable().Where(item => item.Enterprise.ID == this.enterprise.ID);
            }
        }
    }
    //public class OwnedTransportsRoll : Origins.TansportOrigin
    //{
    //    Enterprise enterprise;
    //    /// <summary>
    //    /// 构造函数
    //    /// </summary>
    //    public OwnedTransportsRoll(Enterprise Enterprise)
    //    {
    //        this.enterprise = Enterprise;
    //    }
    //    protected override IQueryable<Transport> GetIQueryable()
    //    {
    //        return base.GetIQueryable().Where(item => item.Enterprise.ID == this.enterprise.ID);
    //    }
    //}
}

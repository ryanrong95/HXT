using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 个人发票
    /// </summary>
    public class vInvoicesRoll : Origins.vInvoicesOrigin
    {
        string enterpriseid;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public vInvoicesRoll(string EnterpriseID)
        {
            this.enterpriseid = EnterpriseID;
        }
        public vInvoicesRoll()
        {
        }
        protected override IQueryable<vInvoice> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.enterpriseid))
            {
                return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterpriseid);
            }
            return base.GetIQueryable();

        }
    }

}

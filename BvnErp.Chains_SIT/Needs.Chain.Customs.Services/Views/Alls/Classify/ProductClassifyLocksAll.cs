using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Alls
{
    /// <summary>
    /// 产品归类锁定视图
    /// </summary>
    public class ProductClassifyLocksAll : UniqueView<Models.ProductClassifyLock, ScCustomsReponsitory>
    {
        public ProductClassifyLocksAll()
        {
        }

        internal ProductClassifyLocksAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ProductClassifyLock> GetIQueryable()
        {
            var productLocksView = new Origins.ProductClassifyLockOrigin(this.Reponsitory);
            var adminsView = new AdminsTopView(this.Reponsitory);
            return from entity in productLocksView
                   join admin in adminsView on entity.AdminID equals admin.ID
                   select new Models.ProductClassifyLock
                   {
                       ID = entity.ID,
                       IsLocked = entity.IsLocked,
                       LockDate = entity.LockDate,
                       AdminID = entity.AdminID,
                       Admin = admin,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}

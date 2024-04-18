using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;
using Needs.Linq;

namespace Needs.Ccs.Services.Views.Origins
{
    /// <summary>
    /// 产品归类锁定Origin视图
    /// </summary>
    internal class ProductClassifyLockOrigin : UniqueView<Models.ProductClassifyLock, ScCustomsReponsitory>
    {
        internal ProductClassifyLockOrigin()
        {
        }

        internal ProductClassifyLockOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ProductClassifyLock> GetIQueryable()
        {
            return from productClassifyLock in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>()
                   select new Models.ProductClassifyLock
                   {
                       ID = productClassifyLock.ID,
                       IsLocked = productClassifyLock.IsLocked,
                       LockDate = productClassifyLock.LockDate,
                       AdminID = productClassifyLock.AdminID,
                       Status = (Enums.Status)productClassifyLock.Status,
                       CreateDate = productClassifyLock.CreateDate,
                       UpdateDate = productClassifyLock.UpdateDate,
                       Summary = productClassifyLock.Summary
                   };
        }
    }
}

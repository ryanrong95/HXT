using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PendingClassifyView : UniqueView<Models.ClassifyResult, ScCustomsReponsitory>
    {
        public PendingClassifyView()
        {
        }

        internal PendingClassifyView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClassifyResult> GetIQueryable()
        {

            var icgooPreProductView = new IcgooPreProductView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);
            var taxView = new TaxCategoriesAllsView(this.Reponsitory);
            var productLocksView = from productLock in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>()
                                   join admin in adminView on productLock.AdminID equals admin.ID
                                   select new
                                   {
                                       productLock.ID,
                                       productLock.IsLocked,
                                       productLock.LockDate,
                                       Locker = admin
                                   };

            return from para in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>()
                   join icgoo in icgooPreProductView on para.PreProductID equals icgoo.ID
                   join productLock in productLocksView on para.PreProductID equals productLock.ID into productLocks
                   from productLock in productLocks.DefaultIfEmpty()
                   select new Models.ClassifyResult
                   {
                       PreProduct = icgoo,                       
                       ID = para.ID,
                       Model = para.Model,
                       Manufacturer = para.Manufacture,
                       ProductName = para.ProductName,
                       HSCode = para.HSCode,
                       TariffRate = para.TariffRate,
                       AddedValueRate = para.AddedValueRate,
                       TaxCode = para.TaxCode,
                       TaxName = para.TaxName,
                       ClassifyType = (IcgooClassifyTypeEnums)para.ClassifyType,
                       Type = (ItemCategoryType)para.Type,
                       InspectionFee = para.InspectionFee,
                       Unit1 = para.Unit1,
                       Unit2 = para.Unit2,
                       CIQCode = para.CIQCode,
                       Elements = para.Elements,
                       ClassifyStatus = (ClassifyStatus)para.ClassifyStatus,
                       CreateDate = para.CreateDate,
                       UpdateDate = para.UpdateDate,
                       Status = (Status)para.Status,
                       IsLocked = productLock == null ? false : productLock.IsLocked,
                       Locker = productLock == null ? null : productLock.Locker,
                       LockDate = productLock == null ? null : (DateTime?)productLock.LockDate,
                       Summary = para.Summary,
                   };
        }
    }
}

//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Underly;

//namespace YaHv.Csrm.Services.Views.Origins
//{
//    /// <summary>
//    /// 原厂品牌
//    /// </summary>
//    public class ManufacturerBrandOrigin : Yahv.Linq.UniqueView<Models.Origins.ManufacturerBrand, PvbCrmReponsitory>
//    {
//        internal ManufacturerBrandOrigin()
//        {
//        }
//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="reponsitory">数据库连接</param>
//        internal ManufacturerBrandOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
//        {
//        }


//        protected override IQueryable<Models.Origins.ManufacturerBrand> GetIQueryable()
//        {
//            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nBrand>()
//                   join brand in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Brands>() on entity.BrandID equals brand.ID
//                   join enteprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enteprise.ID
//                   select new Models.Origins.ManufacturerBrand
//                   {
//                       ID = entity.ID,
//                       SupplierID = enteprise.ID,
//                       SupplierName = enteprise.Name,
//                       BrandID = brand.ID,
//                       BrandName = brand.Name,
//                       CreateDate = brand.CreateDate,
//                       Status = (GeneralStatus)brand.Status
//                   };
//        }
//    }
//}

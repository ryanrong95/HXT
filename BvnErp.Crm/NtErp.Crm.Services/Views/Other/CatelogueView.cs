using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class CatelogueView : UniqueView<Catelogue, BvCrmReponsitory>, Needs.Underly.IFkoView<Catelogue>
    {
        IGenericAdmin Admin;

        private CatelogueView()
        {

        }

        public CatelogueView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        internal CatelogueView(BvCrmReponsitory reponsitory) : base(reponsitory)
        {
        }

        internal CatelogueView(IGenericAdmin admin, BvCrmReponsitory reponsitory) : base(reponsitory)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Catelogue> GetIQueryable()
        {
            var declareproducts = this.GetDeclareProducts();

            return from catelogue in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Catalogues>()
                   join declare in declareproducts on catelogue.ID equals declare.CatelogueID into declares
                   select new Catelogue()
                   {
                       ID = catelogue.ID,
                       CreateDate = catelogue.CreateDate,
                       UpdateDate = catelogue.UpdateDate,
                       Summary = catelogue.Summary,
                       DeclareProducts = declares.ToArray(),
                   };
        }

        /// <summary>
        /// 根据品牌过滤产品信息
        /// </summary>
        /// <returns></returns>
        private IQueryable<DeclareProduct> GetDeclareProducts()
        {
            AdminTop admintop = Extends.AdminExtends.GetTop(this.Admin.ID);

            DeclareProductAlls declareAlls = new DeclareProductAlls(this.Reponsitory);

            switch (admintop.JobType)
            {
                case JobType.FAE:
                case JobType.PME:
                    //{
                    //    var manufactures = admintop.Manufactures.Select(item => item.ID);
                    //    return declareAlls.Where(item => manufactures.Contains(item.StandardProduct.Manufacturer.ID));
                    //}
                case JobType.Sales:
                case JobType.TPM:
                    return declareAlls;
                default:
                    throw new NotImplementedException($"{admintop.JobType}'s Implement is not exsit!");
            }
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="CataloguesID"></param>
        /// <param name="vendor"></param>
        //public void Binding(string CataloguesID,Company vendor)
        //{
        //    var catalogue = this[CataloguesID];
        //    using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
        //    {
        //        int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsDeclareCompany>().Count(item => item.CatalogueID == catalogue.ID && item.ManufactureID == vendor.ID);
        //        if (count == 0)
        //        {
        //            reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsDeclareCompany
        //            {
        //                CatalogueID = catalogue.ID,
        //                ManufactureID = vendor.ID,
        //            });
        //        }
        //    }
        //}

        ///// <summary>
        ///// 绑定
        ///// </summary>
        ///// <param name="CataloguesID"></param>
        ///// <param name="vendor"></param>
        //public void DeleteBinding(string CataloguesID, Company vendor)
        //{
        //    var catalogue = this[CataloguesID];
        //    using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
        //    {
        //        reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsDeclareCompany>(item => item.CatalogueID == 
        //        catalogue.ID && item.ManufactureID == vendor.ID);
        //    }
        //}
    }
}

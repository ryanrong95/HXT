using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public enum nBrandType
    {
        Supplier = 1,
        Manufacturer = 2,
        Company = 3,
        //All = 1000,
    }

    public class nBrandsView : Origins.nBrandsOrigin_Chenhan
    {
        virtual protected nBrandType Type { get { return nBrandType.Supplier; } }

        public nBrandsView()
        {

        }
        //string SupplierID;
        //public nBrandsView(string supplierid)
        //{
        //    this.SupplierID = supplierid;
        //}
        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            var view = base.GetIQueryable();
            //if (!string.IsNullOrWhiteSpace(this.SupplierID))
            //{
            //    return view.Where(item => item.EnterpriseID == this.SupplierID);
            //}
            return view;
        }
    }

    /// <summary>
    /// 供应商品牌
    /// </summary>
    /// <remarks>
    /// 用于选择类似指定供应商
    /// </remarks>
    public class SupplierBrandsView : nBrandsView
    {
        protected override nBrandType Type
        {
            get
            {
                return nBrandType.Supplier;
            }
        }

        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>() on entity.EnterpriseID equals supplier.ID
                       join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enterprise.ID
                       select new nBrand_Chenhan
                       {
                           ID = entity.ID,
                           BrandID = entity.BrandID,
                           BrandName = entity.BrandName,
                           ChineseName = entity.ChineseName,
                           ShortName = entity.ShortName,
                           EnterpriseID = entity.EnterpriseID,
                           Status = entity.Status,
                           EnterpriseName = enterprise.Name
                       };

            return linq;
        }

    }

    /// <summary>
    /// 供应商（原厂，生产厂）品牌
    /// </summary>
    /// <remarks>
    /// 用于选择原厂供应商
    /// </remarks>
    public class ManufacturerBrandsView : SupplierBrandsView
    {
        virtual protected bool IsFactory
        {
            get
            {
                return true;
            }
        }
        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join supplier in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Suppliers>() on entity.EnterpriseID equals supplier.ID
                       join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enterprise.ID
                       where supplier.IsFactory == true
                       select new nBrand_Chenhan
                       {
                           ID = entity.ID,
                           BrandID = entity.BrandID,
                           BrandName = entity.BrandName,
                           ChineseName = entity.ChineseName,
                           ShortName = entity.ShortName,
                           EnterpriseID = entity.EnterpriseID,
                           Status = entity.Status,
                           EnterpriseName = enterprise.Name
                       };

            return linq;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    ///  <remarks>
    /// 用于选择内部公司代理相同品牌 
    /// </remarks>
    public class CompanyBrandsView : nBrandsView
    {
        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join company in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Companies>() on entity.EnterpriseID equals company.ID
                       join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enterprise.ID
                       where entity.IsAgent == true
                       select new nBrand_Chenhan
                       {
                           ID = entity.ID,
                           BrandID = entity.BrandID,
                           BrandName = entity.BrandName,
                           ChineseName = entity.ChineseName,
                           ShortName = entity.ShortName,
                           EnterpriseID = entity.EnterpriseID,
                           Status = entity.Status,
                           EnterpriseName = enterprise.Name
                       };

            return linq;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    ///  <remarks>
    /// 用于选择内部公司代理相同品牌 
    /// </remarks>
    public class AgnetBrandsView : nBrandsView
    {
        protected override IQueryable<nBrand_Chenhan> GetIQueryable()
        {
            var linq = from entity in base.GetIQueryable()
                       join company in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Companies>() on entity.EnterpriseID equals company.ID
                       join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>() on entity.EnterpriseID equals enterprise.ID
                       select new nBrand_Chenhan
                       {
                           ID = entity.ID,
                           BrandID = entity.BrandID,
                           BrandName = entity.BrandName,
                           ChineseName = entity.ChineseName,
                           ShortName = entity.ShortName,
                           EnterpriseID = entity.EnterpriseID,
                           Status = entity.Status,
                           EnterpriseName = enterprise.Name
                       };

            return linq;
        }
    }

}




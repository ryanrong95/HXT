using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views
{


    public class StandardBrandModel : IEntity
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        /// <summary>
        /// 品牌名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 简称
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 是否是代理
        /// </summary>

        public bool IsAgent { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        public DateTime ModifyDate { get; set; }
      
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }

        public string Summary { get; set; }

        #endregion

        #region  拓展字段

        public string[] PMs { get; set; }

        public string[] PMAs { get; set; }
        public string[] FAEs { get; set; }
        /// <summary>
        ///合作公司
        /// </summary>
        public string[] Companys { get; set; }
        /// <summary>
        ///合作供应商
        /// </summary>
        public string[] Suppliers { get; set; }
        #endregion

      
    }
    public class StandardBrandView:vDepthView<StandardBrand,StandardBrandModel, PvdCrmReponsitory>
    {
        public StandardBrandView()
        {
        }
        internal StandardBrandView(PvdCrmReponsitory reponsitory, IQueryable<StandardBrand> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public StandardBrandView(IQueryable<StandardBrand> iQueryable) : base(iQueryable)
        {
        }
        protected StandardBrandView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        //public StandardBrandView()
        //{
        //}
        //internal StandardBrandView(PvdCrmReponsitory reponsitory, IQueryable<StandardBrand> iQueryable) : base(reponsitory, iQueryable)
        //{
        //}
        //public StandardBrandView(IQueryable<StandardBrand> iQueryable) : base(iQueryable)
        //{
        //}
        //protected StandardBrandView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        //{

        //}
        protected override IQueryable<Models.Origins.StandardBrand> GetIQueryable()
        {

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardBrands>()
                   select new Models.Origins.StandardBrand
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       ChineseName = entity.ChineseName,
                       IsAgent = entity.IsAgent,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       Status = (DataStatus)entity.Status,
                       CreatorID = entity.CreatorID,
                   };
        }

        protected override IEnumerable<StandardBrandModel> OnMyPage(IQueryable<StandardBrand> iquery)
        {
            var data = iquery.ToArray();
            var brandids = data.Select(item => item.ID).ToArray();

            var linq_vbrand = from vbrand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.vBrands>()
                              join admin in new AdminsAllRoll(this.Reponsitory) on vbrand.AdminID equals admin.ID
                              where brandids.Contains(vbrand.BrandID)
                              select new
                              {
                                  vbrand.ID,
                                  vbrand.AdminID,
                                  vbrand.BrandID,
                                  admin.RoleID,
                                  admin.RealName,
                              };
            var arrAdmins = linq_vbrand.ToArray();
            var companynBrandView = new CompanynBrandView();
           
            var linq_company = from company in companynBrandView
                               where brandids.Contains(company.BrandID)
                               select new
                               {
                                   company.ID,
                                   company.nBrandID,
                                   company.BrandID,
                                   company.CompanyName
                               };
            var  arrCompanys = linq_company.ToArray();
            var suppliernBrandView = new SuppliernBrandView();
            var linq_supplier = from supplier in suppliernBrandView
                                where brandids.Contains(supplier.BrandID)
                                select new
                                {
                                    supplier.ID,
                                    supplier.SupplierName,
                                    supplier.BrandID,
                                    supplier.nBrandID
                                };
            var arrSupplier = linq_supplier.ToArray();

            return from entity in data
                   select new StandardBrandModel
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Code = entity.Code,
                       ChineseName = entity.ChineseName,
                       IsAgent = entity.IsAgent,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Summary = entity.Summary,
                       Status = (DataStatus)entity.Status,
                       PMs = arrAdmins.Where(item => item.BrandID == entity.ID && (item.RoleID == FixedRole.PM.GetFixedID())).Select(item => item.RealName).ToArray(),
                       PMAs= arrAdmins.Where(item=>item.BrandID==entity.ID &&(item.RoleID==FixedRole.PMa.GetFixedID())).Select(item => item.RealName).ToArray(),
                       FAEs = arrAdmins.Where(item => item.BrandID == entity.ID && (item.RoleID == FixedRole.FAE.GetFixedID())).Select(item => item.RealName).ToArray(),
                       Companys=arrCompanys.Where(item => item.BrandID == entity.ID).Select(item=>item.CompanyName).ToArray(),
                       Suppliers = arrSupplier.Where(item => item.BrandID == entity.ID).Select(item => item.SupplierName).ToArray()

                   };

        }



        public StandardBrandView SearchByName(string txt)
        {
            var iQuery = this.IQueryable;

            var linq = from brand in iQuery
                       where brand.Name.Contains(txt) || brand.Code.Contains(txt) || brand.ChineseName.Contains(txt)
                       select brand;

            return new StandardBrandView(this.Reponsitory, linq);
        }


        public StandardBrandView SerchByCompany(string name)
        {

            var enterprises = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Companies>()
                      join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on entity.ID equals enterprise.ID
                       where enterprise.Name.Contains(name) &&  enterprise.IsDraft==false
                       select entity;

            var enterpriseids = enterprises.Select(item => item.ID).ToArray();

            var brands = from nbrand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.nBrands>()
                           where enterpriseids.Contains(nbrand.EnterpriseID)
                           select nbrand;

            var brandids = brands.Select(item => item.BrandID).ToArray();
            var linq = this.IQueryable.Where(item => brandids.Contains(item.ID));
            return new  StandardBrandView(this.Reponsitory,linq);

        }

        public StandardBrandView SerchBySupplier(string name)
        {

            var enterprises = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Suppliers>()
                              join enterprise in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Enterprises>() on entity.ID equals enterprise.ID
                              where enterprise.Name.Contains(name) && enterprise.IsDraft == false
                              select entity;

            var enterpriseids = enterprises.Select(item => item.ID).ToArray();

            var brands = from nbrand in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.nBrands>()
                         where enterpriseids.Contains(nbrand.EnterpriseID)
                         select nbrand;

            var brandids = brands.Select(item => item.BrandID).ToArray();
            var linq = this.GetIQueryable().Where(item => brandids.Contains(item.ID));
            return new StandardBrandView(this.Reponsitory, linq);

        }

    
        public StandardBrandView Search(Expression<Func<StandardBrand, bool>> expression)
        {
            var iQuery = this.IQueryable;

            return new StandardBrandView(this.Reponsitory, iQuery.Where(expression));
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Web.Mvc;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApi.Controllers
{
    public class BrandsController : ClientController
    {
        // GET: Brands
        [HttpGet]
        public ActionResult Maps(string type, string id)
        {
            nBrandType nBrandType;
            if (!Enum.TryParse(type, out nBrandType))
            {
                throw new NotSupportedException();
            };

            IQueryable<nBrand_Chenhan> query;
            switch (nBrandType)
            {
                case nBrandType.Supplier:
                    query = new SupplierBrandsView();
                    break;
                case nBrandType.Manufacturer:
                    query = new ManufacturerBrandsView();
                    break;
                case nBrandType.Company:
                    query = new CompanyBrandsView();
                    break;
                default:
                    throw new NotSupportedException("不支持指定的类型:" + type);
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(item => item.BrandID == id);
            }

            //Response.StatusCode = 200;
            //return this.Json(query.Select(item => new
            //{
            //    BrandID = item.BrandID,
            //    BrandName = item.BrandName,
            //    EnterpriseID = item.EnterpriseID,
            //    EnterpriseName = item.EnterpriseName
            //}).ToArray(), JsonRequestBehavior.AllowGet);

            //Response.StatusCode = 200;
            //return this.Content("");

            return this.Json(new Result
            {
                Code = "200",
                Data = query.Select(item => new
                {
                    BrandID = item.BrandID,
                    BrandName = item.BrandName,
                    EnterpriseID = item.EnterpriseID,
                    EnterpriseName = item.EnterpriseName
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 代理品牌信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Agents(string id)
        {
            var items = new CompanyBrandsView().Select(item => new
            {
                ID = item.BrandID,
                Name = item.BrandName
            }).Distinct();

            return this.Json(new Result
            {
                Code = "200",
                Data = items.ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsPortal.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsPortal.Services.Views
{
    public class ClassifiedProductViews : UniqueView<ClassifiedProduct, PvDataReponsitory>
    {
        public ClassifiedProductViews()
        {

        }

        protected override IQueryable<ClassifiedProduct> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>()
                   orderby entity.OrderDate descending
                   select new ClassifiedProduct
                   {
                       ID = entity.ID,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       HSCode = entity.HSCode,
                       Name = entity.Name,
                       LegalUnit1 = entity.LegalUnit1,
                       LegalUnit2 = entity.LegalUnit2,
                       VATRate = entity.VATRate,
                       ImportPreferentialTaxRate = entity.ImportPreferentialTaxRate,
                       ExciseTaxRate = entity.ExciseTaxRate,
                       Elements = entity.Elements,
                       SupervisionRequirements = entity.SupervisionRequirements,
                       CIQC = entity.CIQC,
                       CIQCode = entity.CIQCode,
                       TaxCode = entity.TaxCode,
                       TaxName = entity.TaxName,
                       CreateDate = entity.CreateDate,
                       OrderDate = entity.OrderDate,
                   };
        }

        /// <summary>
        /// 根据ID获取税则详细信息
        /// </summary>
        /// <param name="ID"></param>
        public ClassifiedProduct GetDetailByID(string ID)
        {
            //调用接口获取数据
            var api = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[api.ApiName] + api.ClassifiedProductInfo;
            var result = Yahv.Utils.Http.ApiHelper.Current.Get<JSingle<dynamic>>(url, new
            {
                cpnId = ID,
            });
            ClassifiedProduct productinfo = new ClassifiedProduct
            {
                PartNumber = result.data.PartNumber,
                Manufacturer = result.data.Manufacturer,
                HSCode = result.data.HSCode,
                Name = result.data.TariffName,
                LegalUnit1 = result.data.LegalUnit1,
                LegalUnit2 = result.data.LegalUnit2,
                VATRate = result.data.VATRate,
                ImportPreferentialTaxRate = result.data.ImportPreferentialTaxRate,
                ImportGeneralTaxRate = result.data.ImportGeneralTaxRate,
                ExciseTaxRate = result.data.ExciseTaxRate,
                Elements = Convert.ToString(result.data.Elements),
                CIQCode = result.data.CIQCode,
                TaxCode = result.data.TaxCode,
                TaxName = result.data.TaxName,
                Ccc = result.data.Ccc,
            };
            productinfo.ElementsExtend = productinfo.Elements.JsonTo<Dictionary<string,string>>();

            return productinfo;
        }

        /// <summary>
        /// 根据型号获取税则详细信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ClassifiedProduct GetDetailByType(string type)
        {
            //调用接口获取数据
            var api = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[api.ApiName] + api.ClassifiedProduct;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JSingle<object>>(url, new
            {
                partNumber = type,
            });

            //TODO:code=200代表查询到数据，code=100代表未查询到数据，请根据页面需求自行修改
            if (result.code == 200)
                return result.data.ToString().JsonTo<ClassifiedProduct>();
            else
                return null;
        }
    }
}

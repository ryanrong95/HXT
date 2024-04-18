using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Extends
{
    /// <summary>
    /// 约束ID生成规则
    /// </summary>
    public static class ProductConstraintExtend
    {
        static public string GenID(this Interfaces.IProductConstraint product)
        {
            return string.Concat(product.PartNumber,
                                product.Manufacturer.ToLower(),
                                product.HSCode, 
                                product.TariffName,
                                product.LegalUnit1,
                                product.LegalUnit2,

                                product.VATRate,
                                product.ImportPreferentialTaxRate,
                                product.ExciseTaxRate,
                                product.Elements,

                                product.SupervisionRequirements,
                                product.CIQC,

                                product.CIQCode,
                                product.TaxCode,
                                product.TaxName).MD5();
        }
        static public string GenOtherID(this Interfaces.IProductConstraint product)
        {
            return string.Concat(product.PartNumber, product.Manufacturer).MD5();
        }
    }
}

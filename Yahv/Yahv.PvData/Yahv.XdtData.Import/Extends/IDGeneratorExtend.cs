using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace Yahv.XdtData.Import.Extends
{
    public static class IDGeneratorExtend
    {
        static public string GenID(this Layers.Data.Sqls.PvCenter.WayParters parter)
        {
            return string.Concat(parter.Company,
                                parter.Place,
                                parter.Address,
                                parter.Contact,
                                parter.Phone,
                                parter.Zipcode,
                                parter.Email,
                                parter.IDType,
                                parter.IDNumber).MD5();
        }

        static public string GenID(this Layers.Data.Sqls.PvData.ClassifiedPartNumbers cpn)
        {
            return string.Concat(cpn.PartNumber,
                                cpn.Manufacturer.ToLower(),
                                cpn.HSCode,
                                cpn.Name,
                                cpn.LegalUnit1,
                                cpn.LegalUnit2,

                                cpn.VATRate,
                                cpn.ImportPreferentialTaxRate,
                                cpn.ExciseTaxRate,
                                cpn.Elements,

                                cpn.SupervisionRequirements,
                                cpn.CIQC,

                                cpn.CIQCode,
                                cpn.TaxCode,
                                cpn.TaxName).MD5();
        }

        static public string GenID(this Layers.Data.Sqls.PvData.Products product)
        {
            return string.Concat(product.PartNumber, product.Manufacturer).MD5();
        }
    }
}

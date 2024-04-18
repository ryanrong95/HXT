using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 大赢家归类大数据
    /// </summary>
    public class DyjDataClassify : Classify
    {
        private const string url = "http://172.16.6.61:8100/users/type_details/";
        private const string code = "ec371813ff99a4a1028193df5a0d1263";

        public DyjDataClassify(IClassifyProduct product) : base(product)
        {
            this.Product.Classified += Product_Classified;
        }

        /** 自动归类完成之后需要做的操作
         * 1. 更新归类操作人
         */
        private void Product_Classified(object sender, ProductClassifiedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var classifyProduct = (PreClassifyProduct)e.Product;
                UpdateFirstOperator(classifyProduct, reponsitory);
            }
        }

        public override void DoClassify()
        {
            var classifyProduct = (PreClassifyProduct)this.Product;

            string jsonText = Needs.Utils.Http.ApiHelper.Current.JPost(DyjDataClassify.url, new
            {
                part_name = classifyProduct.Model,
                code = DyjDataClassify.code

            });
            if (jsonText == "")
            {
                return;
            }

            JObject jObject = (JObject)JToken.Parse(jsonText);
            if (jObject["status"].ToString() == "failure")
            {
                return;
            }

            //海关编码
            IEnumerable<HSCode> hsCodes = JsonConvert.DeserializeObject<IEnumerable<HSCode>>(jObject["HS编码"].ToString())
                                         .Where(c => c.hscode != null);
            if (hsCodes == null || hsCodes.Count() == 0)
            {
                return;
            }

            //申报要素
            IEnumerable<HSElements> hsElements = JsonConvert.DeserializeObject<IEnumerable<HSElements>>(jObject["申报要素"].ToString())
                                                .Where(e => e.declare_class != null);
            if (hsElements == null || hsElements.Count() == 0)
            {
                return;
            }

            DyjJsonObject jsonObject = new DyjJsonObject();
            jsonObject.HSCodes = hsCodes;
            jsonObject.HSElements = hsElements;

            //商检
            jsonObject.Inspections = JsonConvert.DeserializeObject<IEnumerable<Inspection>>(jObject["商检"].ToString());
            //禁运
            jsonObject.Embargos = JsonConvert.DeserializeObject<IEnumerable<Embargo>>(jObject["禁运"].ToString());
            //3C
            jsonObject.CCCs = JsonConvert.DeserializeObject<IEnumerable<CCC>>(jObject["3C"].ToString());
            //关税率
            jsonObject.TariffRates = JsonConvert.DeserializeObject<IEnumerable<TariffRate>>(jObject["关税率"].ToString());
            //税务编码
            jsonObject.TaxCodes = JsonConvert.DeserializeObject<IEnumerable<TaxCode>>(jObject["税收编码"].ToString());

            Supplement(classifyProduct, jsonObject);
            classifyProduct.ClassifyStatus = Enums.ClassifyStatus.First;
            classifyProduct.DoClassify();
        }

        private void Supplement(PreClassifyProduct classifyProduct, DyjJsonObject dyjClassified)
        {
            //海关编码
            var levelHSCode = dyjClassified.HSCodes.FirstOrDefault(item => item.level == 1) ?? dyjClassified.HSCodes.FirstOrDefault();
            classifyProduct.HSCode = levelHSCode.hscode;

            //海关申报要素
            var levelElements = dyjClassified.HSElements.FirstOrDefault(item => item.level == 1) ??
                                dyjClassified.HSElements.OrderByDescending(e => e.时间).FirstOrDefault();
            classifyProduct.Elements = levelElements.declare_class;

            //税务编码
            var levelTaxCode = dyjClassified.TaxCodes.FirstOrDefault(tc => tc.level == 1) ?? dyjClassified.TaxCodes.FirstOrDefault();
            classifyProduct.TaxCode = levelTaxCode?.tax_code;

            //特殊类型
            classifyProduct.Type = ItemCategoryType.Normal;
            //商检
            var levelInsp = dyjClassified.Inspections.FirstOrDefault(ccc => ccc.level == 1) ?? dyjClassified.Inspections.FirstOrDefault();
            if (levelInsp != null && levelInsp.inspection != "正常")
            {
                classifyProduct.Type |= ItemCategoryType.Inspection;
            }
            //3C
            var levelCcc = dyjClassified.CCCs.FirstOrDefault(ccc => ccc.level == 1) ?? dyjClassified.CCCs.FirstOrDefault();
            if (levelCcc != null && levelCcc.ccc != "正常")
            {
                classifyProduct.Type |= ItemCategoryType.CCC;
            }
            //禁运
            var levelEmbargo = dyjClassified.Embargos.FirstOrDefault(ccc => ccc.level == 1) ?? dyjClassified.Embargos.FirstOrDefault();
            if (levelEmbargo != null && levelEmbargo.embargo != "正常")
            {
                classifyProduct.Type |= ItemCategoryType.Forbid;
            }

            //调用中心数据接口获取海关税则信息
            var apisetting = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetTariff;
            var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
            {
                hsCode = classifyProduct.HSCode
            });
            if (result.code == 200)
            {
                //修正申报要素
                string elements = result.data.DeclareElements;
                var elementArr = elements.Split(';');
                for (int i = 0; i < elementArr.Length; i++)
                {
                    var arr = elementArr[i].Split(':');
                    if (arr[1] == "品牌")
                    {
                        var categoryElementArr = classifyProduct.Elements.Split('|');
                        categoryElementArr[i] = classifyProduct.Manufacturer + "牌";
                        classifyProduct.Elements = string.Join("|", categoryElementArr);
                        break;
                    }
                }

                //其他归类信息
                classifyProduct.ProductName = result.data.Name;
                classifyProduct.TariffRate = result.data.ImportPreferentialTaxRate;
                classifyProduct.AddedValueRate = result.data.VATRate;
                classifyProduct.ExciseTaxRate = result.data.ExciseTaxRate;
                classifyProduct.Unit1 = result.data.LegalUnit1;
                classifyProduct.Unit2 = result.data.LegalUnit2;
                classifyProduct.CIQCode = result.data.CIQCode;
            }
        }

        private void UpdateFirstOperator(PreClassifyProduct classifyProduct, Layer.Data.Sqls.ScCustomsReponsitory reponsitory)
        {
            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
            {
                ClassifyFirstOperator = Icgoo.DefaultCreator,
            }, item => item.ID == classifyProduct.ID);
        }
    }
}

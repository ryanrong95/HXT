using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.PvData.Services.Models;

namespace YaHv.PvData.Services.Extends
{
    public static class PreProductExtend
    {
        /// <summary>
        /// 归类操作日志持久化
        /// </summary>
        /// <param name="preProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void Log_ClassifyOperatingEnter(this PreProduct preProduct, PvDataReponsitory reponsitory)
        {
            if (preProduct.CreatorID != null)
            {
                StringBuilder type = new StringBuilder();
                if (preProduct.Ccc)
                    type.Append("3C" + " ");
                if (preProduct.Embargo)
                    type.Append("禁运" + " ");
                if(preProduct.HkControl)
                    type.Append("香港管制" + " ");
                if (preProduct.Coo)
                    type.Append("原产地证明" + " ");
                if (preProduct.CIQ)
                    type.Append("商检" + " ");
                if (preProduct.IsHighPrice)
                    type.Append("高价值" + " ");
                if (preProduct.IsDisinfected)
                    type.Append("检疫");
                if (type.Length == 0)
                    type.Append("普通");

                string summary = "报关员【" + preProduct.CreatorName + "】完成了" + preProduct.Step.GetDescription() + "。归类结果:" +
                        " 品牌【" + preProduct.Manufacturer +
                        "】; 型号【" + preProduct.PartNumber +
                        "】; 单价【" + preProduct.UnitPrice +
                        "】; 数量【" + preProduct.Quantity +
                        "】; 交易币种【" + preProduct.Currency +
                        "】; 海关编码【" + preProduct.HSCode +
                        "】; 报关品名【" + preProduct.TariffName +
                        "】; 类型【" + type +
                        "】; 商检费用【" + preProduct.CIQprice +
                        "】; 税务编码【" + preProduct.TaxCode +
                        "】; 税务名称【" + preProduct.TaxName +
                        "】; 优惠税率【" + preProduct.ImportPreferentialTaxRate +
                        "】; 加征税率【" + preProduct.OriginATRate +
                        "】; 增值税率【" + preProduct.VATRate +
                        "】; 消费税率【" + preProduct.ExciseTaxRate +
                        "】; 法一单位【" + preProduct.LegalUnit1 +
                        "】; 法二单位【" + preProduct.LegalUnit2 +
                        "】; 检验检疫编码【" + preProduct.CIQCode +
                        "】; 申报要素【" + preProduct.Elements +
                        "】; 摘要备注【" + preProduct.Summary + "】";
                preProduct.Log(LogType.Classify, summary, reponsitory);
            }
        }

        /// <summary>
        /// 归类历史记录持久化
        /// </summary>
        /// <param name="preProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void Log_ClassifiedPartNumberEnter(this PreProduct preProduct, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifiedPartNumberLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifiedPartNumber()
            {
                ID = id,
                PartNumber = preProduct.PartNumber,
                Manufacturer = preProduct.Manufacturer,
                HSCode = preProduct.HSCode,
                Name = preProduct.TariffName,
                VATRate = preProduct.VATRate,
                ImportPreferentialTaxRate = preProduct.ImportPreferentialTaxRate,
                OriginATRate = preProduct.OriginATRate,
                ExciseTaxRate = preProduct.ExciseTaxRate,
                Elements = preProduct.Elements,
                TaxCode = preProduct.TaxCode,
                TaxName = preProduct.TaxName,
                Currency = preProduct.Currency,
                UnitPrice = preProduct.UnitPrice,
                Quantity = preProduct.Quantity,
                CIQ = preProduct.CIQ,
                CIQprice = preProduct.CIQprice,
                CreatorID = preProduct.CreatorID,
                CreateDate = DateTime.Now
            });
        }

        /// <summary>
        /// 系统映射关系持久化
        /// </summary>
        /// <param name="preProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void MapsSystem_Enter(this PreProduct preProduct, ClassifyStep step, PvDataReponsitory reponsitory)
        {
            string id = string.Concat(preProduct.ID, step).MD5();
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.MapsSystem>().Any(t => t.ID == id))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.MapsSystem()
                {
                    ID = id,
                    MainID = preProduct.ID,
                    Step = (int)step,
                    CpnID = preProduct.CpnID,
                    ClientName = preProduct.ClientName,
                    ClientCode = preProduct.ClientCode,
                    CallBackUrl = preProduct.CallBackUrl,
                    Summary = preProduct.Summary,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.MapsSystem>(new
                {
                    CpnID = preProduct.CpnID,
                    ClientName = preProduct.ClientName,
                    ClientCode = preProduct.ClientCode,
                    CallBackUrl = preProduct.CallBackUrl,
                    Summary = preProduct.Summary,
                    ModifyDate = DateTime.Now
                }, a => a.ID == id);
            }
        }
    }
}

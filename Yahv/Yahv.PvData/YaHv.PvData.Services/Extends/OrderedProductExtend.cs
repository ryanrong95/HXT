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
    public static class OrderedProductExtend
    {
        /// <summary>
        /// 归类操作日志持久化
        /// </summary>
        /// <param name="orderedProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void Log_ClassifyOperatingEnter(this OrderedProduct orderedProduct, PvDataReponsitory reponsitory)
        {
            if (orderedProduct.CreatorID != null)
            {
                StringBuilder type = new StringBuilder();
                if (orderedProduct.Ccc)
                    type.Append("3C" + " ");
                if (orderedProduct.Embargo)
                    type.Append("禁运" + " ");
                if(orderedProduct.HkControl)
                    type.Append("香港管制" + " ");
                if (orderedProduct.Coo)
                    type.Append("原产地证明" + " ");
                if (orderedProduct.CIQ)
                    type.Append("商检" + " ");
                if (orderedProduct.IsHighPrice)
                    type.Append("高价值" + " ");
                if (orderedProduct.IsDisinfected)
                    type.Append("检疫");
                if (type.Length == 0)
                    type.Append("普通");

                string summary = "报关员【" + orderedProduct.CreatorName + "】完成了" + orderedProduct.Step.GetDescription() + "。归类结果:" +
                        " 品牌【" + orderedProduct.Manufacturer +
                        "】; 型号【" + orderedProduct.PartNumber +
                        "】; 单价【" + orderedProduct.UnitPrice +
                        "】; 交易币种【" + orderedProduct.Currency +
                        "】; 海关编码【" + orderedProduct.HSCode +
                        "】; 报关品名【" + orderedProduct.TariffName +
                        "】; 类型【" + type +
                        "】; 商检费用【" + orderedProduct.CIQprice +
                        "】; 税务编码【" + orderedProduct.TaxCode +
                        "】; 税务名称【" + orderedProduct.TaxName +
                        "】; 优惠税率【" + orderedProduct.ImportPreferentialTaxRate +
                        "】; 加征税率【" + orderedProduct.OriginATRate +
                        "】; 增值税率【" + orderedProduct.VATRate +
                        "】; 消费税率【" + orderedProduct.ExciseTaxRate +
                        "】; 法一单位【" + orderedProduct.LegalUnit1 +
                        "】; 法二单位【" + orderedProduct.LegalUnit2 +
                        "】; 检验检疫编码【" + orderedProduct.CIQCode +
                        "】; 申报要素【" + orderedProduct.Elements +
                        "】; 摘要备注【" + orderedProduct.Summary + "】";
                orderedProduct.Log(LogType.Classify, summary, reponsitory);
            }
        }

        /// <summary>
        /// 归类历史记录持久化
        /// </summary>
        /// <param name="orderedProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void Log_ClassifiedPartNumberEnter(this OrderedProduct orderedProduct, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifiedPartNumberLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifiedPartNumber()
            {
                ID = id,
                PartNumber = orderedProduct.PartNumber,
                Manufacturer = orderedProduct.Manufacturer,
                HSCode = orderedProduct.HSCode,
                Name = orderedProduct.TariffName,
                VATRate = orderedProduct.VATRate,
                ImportPreferentialTaxRate = orderedProduct.ImportPreferentialTaxRate,
                OriginATRate = orderedProduct.OriginATRate,
                ExciseTaxRate = orderedProduct.ExciseTaxRate,
                Elements = orderedProduct.Elements,
                TaxCode = orderedProduct.TaxCode,
                TaxName = orderedProduct.TaxName,
                Currency = orderedProduct.Currency,
                UnitPrice = orderedProduct.UnitPrice,
                Quantity = orderedProduct.Quantity,
                CIQ = orderedProduct.CIQ,
                CIQprice = orderedProduct.CIQprice,
                CreatorID = orderedProduct.CreatorID,
                CreateDate = DateTime.Now
            });
        }

        /// <summary>
        /// 系统映射关系持久化
        /// </summary>
        /// <param name="orderedProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void MapsSystem_Enter(this OrderedProduct orderedProduct, ClassifyStep step, PvDataReponsitory reponsitory)
        {
            string id = string.Concat(orderedProduct.MainID, orderedProduct.ID, step).MD5();
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.MapsSystem>().Any(t => t.ID == id))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.MapsSystem()
                {
                    ID = id,
                    MainID = orderedProduct.MainID,
                    ItemID = orderedProduct.ID,
                    Step = (int)step,
                    CpnID = orderedProduct.CpnID,
                    ClientName = orderedProduct.ClientName,
                    ClientCode = orderedProduct.ClientCode,
                    OrderedDate = orderedProduct.OrderedDate,
                    PIs = orderedProduct.PIs,
                    CallBackUrl = orderedProduct.CallBackUrl,
                    Unit = orderedProduct.Unit,
                    CustomName = orderedProduct.CustomName,
                    Summary = orderedProduct.Summary,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.MapsSystem>(new
                {
                    CpnID = orderedProduct.CpnID,
                    ClientName = orderedProduct.ClientName,
                    ClientCode = orderedProduct.ClientCode,
                    OrderedDate = orderedProduct.OrderedDate,
                    PIs = orderedProduct.PIs,
                    CallBackUrl = orderedProduct.CallBackUrl,
                    Unit = orderedProduct.Unit,
                    CustomName = orderedProduct.CustomName,
                    Summary = orderedProduct.Summary,
                    ModifyDate = DateTime.Now
                }, a => a.ID == id);
            }
        }

        /// <summary>
        /// 产品报价信息持久化
        /// </summary>
        /// <param name="orderedProduct">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        public static void ProductQuoteEnter(this OrderedProduct orderedProduct, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ProductQuote);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.ProductQuotes()
            {
                ID = id,
                PartNumber = orderedProduct.PartNumber,
                Manufacturer = orderedProduct.Manufacturer,
                Origin = orderedProduct.Origin,
                Currency = orderedProduct.Currency,
                UnitPrice = orderedProduct.UnitPrice,
                Quantity = orderedProduct.Quantity,
                CIQprice = orderedProduct.CIQprice,
                CreateDate = DateTime.Now
            });
        }
    }
}

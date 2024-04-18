using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DailyDeclare : IUnique
    {

        #region Declist 中的字段

        public string ID { get; set; }

        public OrderType OrderType { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public string DeclarationID { get; set; }
        public string OrderItemID { get; set; }
        //  public string DeclarationNoticeID { get; set; }

        public string DeclarationNoticeItemID { get; set; }
        public DeclarationNoticeItem DeclarationNoticeItem { get; set; }
        public string OrderID { get; set; }
        /// <summary>
        /// 原产地区
        /// </summary>
        public string OrigPlaceCode { get; set; }
        /// <summary>
        /// 货物属性 默认：19 正常
        /// </summary>
        public string GoodsAttr { get; set; }
        public string GoodsAttrName
        {
            get
            {
                string name = "";
                if (this.GoodsAttr != null && this.GoodsAttr != "")
                {
                    string[] attr = this.GoodsAttr.Split(',');
                    using (var view = new Needs.Ccs.Services.Views.BaseGoodsAttrsView())
                    {
                        for (int i = 0; i < attr.Length; i++)
                        {
                            name += view.Where(item => item.Code == attr[i]).Select(item => item.Name).FirstOrDefault() + ",";
                        }
                    }
                    name = name.Substring(0, name.Length - 1);
                }
                return name;
            }
        }
        /// <summary>
        /// 货物型号
        /// </summary>
        public string GoodsModel { get; set; }
        /// <summary>
        /// 货物品牌
        /// </summary>
        public string GoodsBrand { get; set; }
        /// <summary>
        /// 用途代码 默认：99 其它
        /// </summary>
        public string Purpose { get; set; }
        public string PurposeName
        {
            get
            {
                string name = "";
                if (this.Purpose != null && this.Purpose != "")
                {
                    using (var view = new Needs.Ccs.Services.Views.BasePurposesView())
                    {
                        name = view.Where(item => item.Code == this.Purpose).Select(item => item.Name).FirstOrDefault();
                    }
                }
                return name;
            }
        }

        public Order Order { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CusItemDecStatus CusDecStatus { get; set; }
        /// <summary>
        /// 商品序号/项号
        /// </summary>
        public int GNo { get; set; }

        /// <summary>
        /// 10位商编
        /// </summary>
        public string CodeTS { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CiqCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string GName { get; set; }

        /// <summary>
        /// 规格型号（申报要素）
        /// </summary>
        public string GModel { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal GQty { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>
        public string GUnit { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string FirstUnit { get; set; }

        public decimal? FirstQty { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string SecondUnit { get; set; }

        /// <summary>
        /// 法定第二数量
        /// </summary>        
        public decimal? SecondQty { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal DeclPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal DeclTotal { get; set; }

        /// <summary>
        /// 成交币制
        /// </summary>
        public string TradeCurr { get; set; }

        /// <summary>
        /// 原产地国别
        /// </summary>
        public string OriginCountry { get; set; }

        /// <summary>
        /// 原产地国别名
        /// </summary>
        public string OriginCountryName { get; set; }

        /// <summary>
        /// 最终目的国（地区）
        /// </summary>
        public string DestinationCountry
        {
            get
            {
                return "CHN";//目的国默认中国
            }
        }

        /// <summary>
        /// 目的地代码 国内行政区划代码 默认：440307 龙岗区
        /// </summary>
        public string DestCode
        {
            get
            {
                return "440307";//龙岗区
            }
        }

        /// <summary>
        /// 境内目的地/境内货源地 国内地区代码 默认：44031 深圳特区
        /// </summary>
        public string DistrictCode
        {
            get
            {
                return "44031";//深圳特区
            }
        }

        /// <summary>
        /// 征减免税方式 :默认照章征税1
        /// </summary>
        public int DutyMode
        {
            get
            {
                return 1;//照章征税
            }
        }
        /// <summary>
        /// 箱号
        /// </summary>
        public string CaseNo { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWt { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWt { get; set; }
        /// <summary>
        /// 检验检疫货物规格
        /// </summary>
        public string GoodsSpec { get; set; }

        #endregion


        /// <summary>
        /// 合同号
        /// </summary>
        public string ContrNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Qty { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 开票公司
        /// </summary>
        public string InvoiceCompany { get; set; }


        /// <summary>
        /// 金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsRate { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal TariffRate { get; set; }


        /// <summary>
        /// 关税（报关单）
        /// </summary>
        public decimal Tariff { get; set; }

        /// <summary>
        /// 总关税（报关单）
        /// </summary>
        public decimal TotalTariff { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 报关单号/海关编号
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// 运输批次号
        /// </summary>
        public string VoyNo { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime DDate { get; set; }

        public string IcgooOrderID { get; set; }

        /// <summary>
        /// 备案号
        /// </summary>
        public string RecordNo { get; set; }

        public bool IsInspection { get; set; }

        public bool? IsQuarantine { get; set; }

        public bool IsInspOrQuar
        {
            get
            {
                bool isQuarantine = false;
                if (this.IsQuarantine != null)
                {
                    isQuarantine = IsQuarantine.Value;
                }
                if (this.IsInspection || isQuarantine)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string GoodsAttr1 { get; set; }
        public string DomisticDestName { get; set; }
        public string DestName { get; set; }

        public int ContrNoSuffix
        {
            get
            {
                try
                {
                    string[] contrNos = this.ContrNo.Split('-');
                    int suffix = Convert.ToInt32(contrNos[2]);
                    return suffix;
                }
                catch(Exception ex)
                {
                    return 0;
                }                
            }
        }

        /// <summary>
        /// 0：反制措施排除代码
        /// </summary>
        public string CertCode0 { get; set; }
    }
}

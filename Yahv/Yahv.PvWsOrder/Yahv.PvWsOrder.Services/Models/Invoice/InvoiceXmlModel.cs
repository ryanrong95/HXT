using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.Models.InvoiceXmlA
{

    #region 转换成 Xml 的类 a

    [Serializable]
    public class Kp
    {
        //有此节点，则表示用带分类编码
        public string Version { get; set; }

        public Fpxx Fpxx { get; set; }
    }

    [Serializable]
    public class Fpxx
    {
        //单据数量
        public int Zsl { get; set; }

        public List<Fp> Fpsj { get; set; }
    }

    [Serializable]
    public class Fp
    {
        //单据号（20 字节）
        public string Djh { get; set; }
        //购方名称（100 字节）
        public string Gfmc { get; set; }
        //购方税号
        public string Gfsh { get; set; }
        //购方银行账号（100 字节）
        public string Gfyhzh { get; set; }
        //购方地址电话（100 字节）
        public string Gfdzdh { get; set; }
        //备注（240 字节）
        public string Bz { get; set; }
        //复核人（8 字节）
        public string Fhr { get; set; }
        //收款人（8 字节）
        public string Skr { get; set; }
        //商品编码版本号(20 字节)（必输项）（商品版本号是你开票机的版本号）
        public string Spbmbbh { get; set; }
        //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为 1，差额税为 2，其他为 0；
        public string Hsbz { get; set; }
        public List<Sph> Spxx { get; set; }
    }

    [Serializable]
    public class Sph
    {
        //序号
        public string Xh { get; set; }
        //商品名称
        public string Spmc { get; set; }
        //规格型号
        public string Ggxh { get; set; }
        //计量单位
        public string Jldw { get; set; }
        //商品编码(19 字节) （必输项）
        public string Spbm { get; set; }
        //企业商品编码（20 字节）
        public string Qyspbm { get; set; }
        //是否使用优惠政策标识 0： 不使用，1：使用（1 字节）
        public string Syyhzcbz { get; set; }
        //零税率标识 空：非零税率，0： 出口退税，1：免税，2：不征收，3 普通零税率（1 字节）
        public string Lslbz { get; set; }
        //优惠政策说明（50 字节）
        public string Yhzcsm { get; set; }
        //单价
        public decimal Dj { get; set; }
        //数量
        public decimal Sl { get; set; }
        //金额
        public decimal Je { get; set; }
        //税率
        public decimal Slv { get; set; }
        //税额
        public decimal Se { get; set; }
        //扣除额，用于差额税计算 
        public string Kce { get; set; }
    }

    public class InvoiceXmlConfig
    {
        //版本号
        public const string Version = "2.0";
        //复核人
        public const string Fhr = "鲁亚慧";
        //收款人
        public const string Skr = "郝红梅";
        //开票人
        public const string Kpr = "郝红梅";
        //商品编码版本号(20 字节)（商品版本号是你开票机的版本号）
        public const string Spbmbbh = "33.0";
        //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为 1，差额税为 2，其他为 0；
        public const string Hsbz = "1";

        ///// <summary>
        ///// 每张发票的限额
        ///// </summary>
        //public static decimal XianEPerFp = decimal.Parse(PurchaserContext.Current.MaxAmountPerInvoice);
    }

    #endregion
}

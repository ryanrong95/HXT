using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Common;
using Yahv.Underly;
using Yahv.Utils.Npoi;
using Yahv.Utils.Serializers;
//using Yahv.PvWsOrder.Services.Models.InvoiceXml;

namespace Yahv.PvWsOrder.Services.Models.InvoiceXmlA
{
    /// <summary>
    /// InvoiceNotice
    /// </summary>
    public class InvoiceNoticeForXml
    {
        public string InvoiceNoticeID { get; set; }

        public decimal KaiPiaoJinE { get; set; }

        public List<InvoiceNoticeItemsForXml> Items { get; set; }

        #region 填写 Xml 使用

        public string CompanyName { get; set; }

        public string TaxNumber { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 含税总金额
        /// </summary>
        public decimal 含税总金额 { get; set; }

        #endregion
    }

    /// <summary>
    /// InvoiceNoticeItem
    /// </summary>
    public class InvoiceNoticeItemsForXml
    {
        public string InvoiceNoticeItemID { get; set; }

        public string InvoiceNoticeID { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalKaiPiaoJinE { get; set; }

        /// <summary>
        /// 已使用数量(初始值同 Quantity, RemainingQuantity 一直在变动)
        /// </summary>
        public decimal UsedQuantity { get; set; }

        /// <summary>
        /// 已使用开票金额(初始值同 TotalKaiPiaoJinE, RemainingKaiPiaoJinE 一直在变动)
        /// </summary>
        public decimal UsedKaiPiaoJinE { get; set; }

        /// <summary>
        /// 剩余开票金额
        /// </summary>
        public decimal RemainingKaiPiaoJinE { get { return this.TotalKaiPiaoJinE - this.UsedKaiPiaoJinE; } }

        ///// <summary>
        ///// 是否被拆分过
        ///// </summary>
        //public bool IsSplitted { get; set; }
    }


    public class ServiceInvoiceHandler
    {
        public InvoiceNoticeForXml InvoiceNoticeForXml { get; set; }

        public List<ServiceInvoiceXml> ServiceInvoiceXmls { get; set; } = new List<ServiceInvoiceXml>();

        public List<string> XmlFilePaths = new List<string>();

        public ServiceInvoiceHandler(InvoiceNoticeForXml invoiceNoticeForXml)
        {
            this.InvoiceNoticeForXml = invoiceNoticeForXml;
        }

        /// <summary>
        /// 将一个 InvoiceNotice 转变成 XmlModel
        /// </summary>
        private void GenerateXmlModel()
        {
            ServiceInvoiceXml serviceInvoiceXml = new ServiceInvoiceXml();
            for (int i = 0; ;)
            {
                //如果这张发票已用满, 换一张发票
                if (serviceInvoiceXml.RemainingAmount <= 0)
                {
                    this.ServiceInvoiceXmls.Add(serviceInvoiceXml);
                    serviceInvoiceXml = new ServiceInvoiceXml();
                }

                if (i == this.InvoiceNoticeForXml.Items.Count)
                {
                    if (serviceInvoiceXml.UseAmount > 0)
                    {
                        this.ServiceInvoiceXmls.Add(serviceInvoiceXml);
                    }

                    break;
                }

                //如果一个 InvoiceNoticeItem 剩余的金额可以放入一张发票
                if (this.InvoiceNoticeForXml.Items[i].RemainingKaiPiaoJinE <= serviceInvoiceXml.RemainingAmount)
                {
                    decimal theAmount = this.InvoiceNoticeForXml.Items[i].RemainingKaiPiaoJinE;
                    decimal theQuantity = this.InvoiceNoticeForXml.Items[i].Quantity - this.InvoiceNoticeForXml.Items[i].UsedQuantity;

                    serviceInvoiceXml.XmlItems.Add(new ServiceInvoiceXmlItem
                    {
                        InvoiceNoticeItemID = this.InvoiceNoticeForXml.Items[i].InvoiceNoticeItemID,
                        Amount = theAmount,
                        Quantity = theQuantity,



                    });

                    this.InvoiceNoticeForXml.Items[i].UsedQuantity += theQuantity;
                    this.InvoiceNoticeForXml.Items[i].UsedKaiPiaoJinE += theAmount;

                    i++; //指向下一个
                }
                //如果一个 InvoiceNoticeItem 剩余的金额不能放入一张发票,把能放入的部分放入,并修改这个 InvoiceNoticeItem 的 IsSplitted 为 true
                else
                {
                    decimal theAmount = serviceInvoiceXml.RemainingAmount;
                    decimal theQuantity = (theAmount / this.InvoiceNoticeForXml.Items[i].TotalKaiPiaoJinE * this.InvoiceNoticeForXml.Items[i].Quantity).ToRound1(2);

                    serviceInvoiceXml.XmlItems.Add(new ServiceInvoiceXmlItem
                    {
                        InvoiceNoticeItemID = this.InvoiceNoticeForXml.Items[i].InvoiceNoticeItemID,
                        Amount = theAmount,
                        Quantity = theQuantity,



                    });

                    this.InvoiceNoticeForXml.Items[i].UsedQuantity += theQuantity;
                    this.InvoiceNoticeForXml.Items[i].UsedKaiPiaoJinE += theAmount;
                }
            }
        }

        /// <summary>
        /// 将 XmlModel 转换成 Xml 文件
        /// </summary>
        private void ConvertToXml()
        {
            int Djh = 1;

            foreach (var serviceInvoiceXml in this.ServiceInvoiceXmls)
            {
                Kp kp = new Kp();

                kp.Version = InvoiceXmlConfig.Version;
                kp.Fpxx = new Fpxx();
                kp.Fpxx.Fpsj = new List<Fp>();

                Fp fp = new Fp();
                fp.Djh = Djh.ToString();
                fp.Gfmc = this.InvoiceNoticeForXml.CompanyName;
                fp.Gfsh = this.InvoiceNoticeForXml.TaxNumber;
                fp.Gfyhzh = this.InvoiceNoticeForXml.BankName + this.InvoiceNoticeForXml.BankAccount;
                fp.Gfdzdh = this.InvoiceNoticeForXml.RegAddress + " " + this.InvoiceNoticeForXml.Tel;
                fp.Bz = "";
                fp.Fhr = InvoiceXmlConfig.Fhr;
                fp.Skr = InvoiceXmlConfig.Skr;
                fp.Spbmbbh = InvoiceXmlConfig.Spbmbbh;
                fp.Hsbz = InvoiceXmlConfig.Hsbz;

                fp.Spxx = new List<Sph>();
                int Xh = 1;

                foreach (var xmlItem in serviceInvoiceXml.XmlItems)
                {
                    Sph sph = new Sph();
                    sph.Xh = Xh.ToString();

                    sph.Spmc = "服务费";
                    sph.Ggxh = "";
                    sph.Jldw = "";
                    sph.Sl = xmlItem.Quantity;
                    sph.Spbm = "3040407040000000000";

                    sph.Qyspbm = "";
                    sph.Syyhzcbz = "";
                    sph.Lslbz = "";
                    sph.Yhzcsm = "";

                    sph.Je = xmlItem.Amount;
                    sph.Dj = (sph.Je / sph.Sl);
                    sph.Slv = ((decimal)0.06).ToRound1(4);
                    sph.Se = (sph.Je * (decimal)0.06).ToRound1(2);
                    sph.Kce = "";

                    fp.Spxx.Add(sph);
                    Xh += 1;
                }
                kp.Fpxx.Fpsj.Add(fp);
                Djh += 1;
                kp.Fpxx.Zsl = kp.Fpxx.Fpsj.Count;

                string xmlResult = kp.Xml(System.Text.Encoding.GetEncoding("GBK"));
                var ClientName = this.InvoiceNoticeForXml.CompanyName;
                var Amount = this.InvoiceNoticeForXml.含税总金额;
                var fileName = ClientName + Amount + "_" + Guid.NewGuid().ToString("N").Substring(6) + ".xml";

                FileDirectory fileDir = new FileDirectory(fileName, FileType.Test);
                fileDir.CreateDirectory();
                string filePath = fileDir.DownLoadRoot + fileName;
                NPOIHelper.InvoiceInfoXml(xmlResult, filePath);

                this.XmlFilePaths.Add(filePath);
            }

            
        }

        public void GenerateXml()
        {
            this.GenerateXmlModel();
            this.ConvertToXml();
        }
    }

    /// <summary>
    /// 服务费发票 一张发票的 Xml
    /// </summary>
    public class ServiceInvoiceXml
    {
        public ServiceInvoiceXml()
        {
            string ServiceInvoiceTotalAmount = ConfigurationManager.AppSettings["ServiceInvoiceTotalAmount"];
            this.TotalAmount = Convert.ToDecimal(ServiceInvoiceTotalAmount);
        }

        /// <summary>
        /// 总共金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        private decimal _useAmount { get; set; }

        /// <summary>
        /// 使用金额
        /// </summary>
        public decimal UseAmount
        {
            get { return this.XmlItems.Sum(t => t.Amount); }
            set { _useAmount = value; }
        }

        private decimal _remainingAmount { get; set; }

        /// <summary>
        /// 剩余金额
        /// </summary>
        public decimal RemainingAmount
        {
            get { return this.TotalAmount - this.UseAmount; }
            set { _remainingAmount = value; }
        }

        /// <summary>
        /// 服务费发票 Xml 发票项
        /// </summary>
        public List<ServiceInvoiceXmlItem> XmlItems { get; set; } = new List<ServiceInvoiceXmlItem>();
    }

    /// <summary>
    /// 服务费发票 Xml 发票项
    /// </summary>
    public class ServiceInvoiceXmlItem
    {
        /// <summary>
        /// InvoiceNoticeItemID
        /// </summary>
        public string InvoiceNoticeItemID { get; set; }

        /// <summary>
        /// Amount(计算好填进来, 开票时直接使用)
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 数量 0-1 中的小数(计算好填进来, 开票时直接使用)
        /// </summary>
        public decimal Quantity { get; set; }

        ///// <summary>
        ///// 是否是做减法得到的
        ///// </summary>
        //public bool IsSubtraction { get; set; }
    }
}

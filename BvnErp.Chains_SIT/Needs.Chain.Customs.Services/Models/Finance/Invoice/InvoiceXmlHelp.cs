using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Utils.Serializers;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceXmlHelp
    {
        private List<InvoiceNotice> notices { get; set; }

        //private List<XmlFileUnit> xmlFileUnits { get; set; }

        public InvoiceXmlHelp()
        { 
        }

        public InvoiceXmlHelp(InvoiceNotice notice)
        {
            this.notices = new List<InvoiceNotice>() { notice };
        }

        public InvoiceXmlHelp(List<InvoiceNotice> notices)
        {
            this.notices = notices;
        }

        /// <summary>
        /// 生成并导出XML文件
        /// </summary>
        /// <returns></returns>
        public string ExportXml()
        {
            List<XmlFileUnit> finalXmls = new List<XmlFileUnit>();
            //xmlAmount
           foreach (var notice in notices)
           {
             var xmls = GenerateXmlMulti(notice);
            
             foreach(var item in xmls)
             {
                finalXmls.Add(item);
             }
           }
           

            var filePaths = new List<string>();

            //生成XML文件
            foreach (var xml in finalXmls)
            {

                string xmlResult = xml.Kp.Xml(System.Text.Encoding.GetEncoding("GBK"));

                var ClientName = xml.InvoiceNotice.Client.Company.Name;
                //单个XML的含税总金额
                var Amount = xml.Kp.Fpxx.Fpsj.Sum(t=>t.Spxx.Sum(c=>c.Je + c.Se));

                var fileName = ClientName + xml.InvoiceNotice.ID + "_" + Amount + "_" + xml.InvoiceNotice.InvoiceType.GetDescription() + "_" + ChainsGuid.NewGuidUp().Substring(26) + ".xml";
                //创建文件夹
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //生成XML文件
                InvoiceInfoXml(xmlResult, file.FilePath);
                filePaths.Add(file.FilePath);
            }


            //压缩文件 Begin
            string zipFileName = "FPXML" + DateTime.Now.ToString("yyyyMMddHHmmss") + "N" + ChainsGuid.NewGuidUp().Substring(22) + ".zip";
            ZipFile zip = new ZipFile(zipFileName);
            zip.SetFilePath(Path.GetDirectoryName(filePaths.First()) + @"\");
            zip.Files = filePaths;
            zip.ZipFiles();
            //压缩文件 End

            string zipUrl = ConfigurationManager.AppSettings["FileServerUrl"]
                    + zip.ZipedPath.Substring(zip.ZipedPath.IndexOf("Files") + "Files".Length).Replace(@"\", @"/")
                    + zip.FileName;

            return zipUrl;
        }

        /// <summary>
        /// 生成并持久化Xml数据
        /// </summary>
        public void EntryXml(decimal amountLimit)
        {           
            var xmls = GenerateXml(amountLimit);
        }

        public List<XmlFileUnit> GenerateXmlFile(decimal amountLimit)
        {
            //xmlAmount           
            return GenerateXml(amountLimit);
        }

        /// <summary>
        /// 拆分通知
        /// 生成XML结构
        /// </summary>
        /// <returns></returns>
        private List<XmlFileUnit> GenerateXml(decimal amountLimit) 
        {
            List<XmlFileUnit> xmlFileUnits = new List<XmlFileUnit>();

            #region 拆分Xml

            //计算每个InvoiceNotice的开票金额 Begin
            for (int i = 0; i < notices.Count; i++)
            {
                notices[i].KaiPiaoJinE = ((notices[i].Amount + notices[i].Difference) / (1 + notices[i].InvoiceTaxRate)).ToRound(2);
            }
            //计算每个InvoiceNotice的开票金额 End

            //1、notice.total <= 10W
            var 小金额InvoiceNotices = notices.Where(t => t.KaiPiaoJinE <= amountLimit).ToList();
            foreach (var invoiceNotice in 小金额InvoiceNotices)
            {
                var file = KaiPiaoOperation(invoiceNotice);
                xmlFileUnits.Add(file);
            }


            //2、notice.total > 10W  Begin
            var 大金额InvoiceNotices = notices.Where(t => t.KaiPiaoJinE > amountLimit).ToList();
            foreach (var invoiceNotice in 大金额InvoiceNotices)
            {
                //item的未税价
                foreach (var item in invoiceNotice.InvoiceItems)
                {
                    item.KaiPiaoJinE = ((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2);
                }


                //3、Item <=10W 将会进行排序合并
                var LessThanLimitItems = invoiceNotice.InvoiceItems.Where(t => t.KaiPiaoJinE <= amountLimit).OrderBy(t => t.KaiPiaoJinE).ToList();
                xmlFileUnits.AddRange(LessThanLimit(invoiceNotice, LessThanLimitItems, amountLimit));


                //4、item > 10W Begin
                var 大于10W的InvoiceNoticeItems = invoiceNotice.InvoiceItems.Where(t => t.KaiPiaoJinE > amountLimit).ToArray();
                foreach (var 大于10W的InvoiceNoticeItem in 大于10W的InvoiceNoticeItems)
                {
                    Kp kp = new Kp(invoiceNotice, new string[] { 大于10W的InvoiceNoticeItem.ID });
                    if (kp.Fpxx.Fpsj[0].Spxx[0].Dj > amountLimit)
                    {
                        var files = KaiPiaoOperationForDaJinE_数量得分成小数(kp, 大于10W的InvoiceNoticeItem, invoiceNotice, amountLimit);
                        xmlFileUnits.AddRange(files);
                    }
                    else
                    {
                        var files = KaiPiaoOperationForDaJinE_数量分为整数(kp, 大于10W的InvoiceNoticeItem, invoiceNotice, amountLimit);
                        xmlFileUnits.AddRange(files);
                    }
                }
                //item > 10W End
            }
            //notice.total > 10W End

            #endregion

            #region 解决拆分误差

            //步骤
            //1、得到差额：InvoiceNotice.含税总金额 - Sum(Xml.金额+ Xml.税额)
            //2、找到默认第一个小于额度的XML
            //3、加上差额后的含税计算未税以及税额

            var grouptmp = xmlFileUnits.Select(t => new
            {
                InvoiceNoticeID = t.InvoiceNotice.ID,
                InvoiceNoticeAmount = (t.InvoiceNotice.Amount + t.InvoiceNotice.Difference).ToRound(2),
                InvoiceTaxRate = t.InvoiceNotice.InvoiceTaxRate,
                KpTotal = t.Kp.Fpxx.Fpsj.First().Spxx.Sum(c => c.Je + c.Se)
            });

            var group_notice = (from kp in grouptmp
                                group kp by new { kp.InvoiceNoticeID, kp.InvoiceNoticeAmount, kp.InvoiceTaxRate } into t_group
                                select new
                                {
                                    InvoiceNoticeID = t_group.Key.InvoiceNoticeID,
                                    InvoiceNoticeAmount = t_group.Key.InvoiceNoticeAmount,
                                    InvoiceTaxRate = t_group.Key.InvoiceTaxRate,
                                    KpTotal = t_group.Sum(t => t.KpTotal)
                                }).Distinct();


            foreach (var notice in group_notice.Where(t=>t.KpTotal != t.InvoiceNoticeAmount))
            {
                //差额 notice -  Kp
                var diff = notice.InvoiceNoticeAmount - notice.KpTotal;

                //限额
                var XianE = amountLimit * (1 + notice.InvoiceTaxRate);

                if (Math.Abs(diff) > 0.00M)
                {
                    //当前第一个小于限额的Kp的第一项
                    var oldJe = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je;
                    var oldSe = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Se;
                    var Sl = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Sl;

                    //计算该第一项的新的金额
                    var newJe = ((oldJe + oldSe + diff) / (1 + notice.InvoiceTaxRate)).ToRound(2);
                    var newDj = newJe / Sl;
                    var newSe = (newJe * notice.InvoiceTaxRate).ToRound(2);

                    //赋值
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je = newJe;
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Dj = newDj;
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Se = newSe;

                    //验证还差一分钱的情况
                    var newKptotal = xmlFileUnits.Where(t => t.InvoiceNotice.ID == notice.InvoiceNoticeID).Sum(kp => kp.Kp.Fpxx.Fpsj.First().Spxx.Sum(c => c.Je + c.Se));
                    if (notice.InvoiceNoticeAmount != newKptotal)
                    {
                        //此时，误差只能是 0.01 或者 -0.01, 税额不会影响
                        //一分太小了，税额算出来才0.0013元，开票是精确到0.01元，所以不影响
                        var newdiff = notice.InvoiceNoticeAmount - newKptotal;
                        newJe += newdiff;
                        newDj = newJe / Sl;

                        //重新赋值
                        xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                            .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je = newJe;
                        xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                            .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Dj = newDj;
                    }
                }
            }

            #endregion

            //得到所有的xmlFileUnits
            return xmlFileUnits;

        }

        private List<XmlFileUnit> GenerateXmlMulti(InvoiceNotice inotice)
        {
            List<XmlFileUnit> xmlFileUnits = new List<XmlFileUnit>();

            #region 拆分Xml


            inotice.KaiPiaoJinE = ((inotice.Amount + inotice.Difference) / (1 + inotice.InvoiceTaxRate)).ToRound(2);
            
            //计算每个InvoiceNotice的开票金额 End

            //1、notice.total <= 10W
            decimal amountLimit = inotice.AmountLimit == null? InvoiceXmlConfig.XianEPerFp : inotice.AmountLimit.Value;
            if(inotice.KaiPiaoJinE <= amountLimit)
            {
                var file = KaiPiaoOperation(inotice);
                xmlFileUnits.Add(file);
            }
            else
            {
                //2、notice.total > 10W  Begin
                foreach (var item in inotice.InvoiceItems)
                {
                    item.KaiPiaoJinE = ((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2);
                }
                //3、Item <=10W 将会进行排序合并
                var LessThanLimitItems = inotice.InvoiceItems.Where(t => t.KaiPiaoJinE <= amountLimit).OrderBy(t => t.KaiPiaoJinE).ToList();
                xmlFileUnits.AddRange(LessThanLimit(inotice, LessThanLimitItems, amountLimit));

                //4、item > 10W Begin
                var 大于10W的InvoiceNoticeItems = inotice.InvoiceItems.Where(t => t.KaiPiaoJinE > amountLimit).ToArray();
                foreach (var 大于10W的InvoiceNoticeItem in 大于10W的InvoiceNoticeItems)
                {
                    Kp kp = new Kp(inotice, new string[] { 大于10W的InvoiceNoticeItem.ID });
                    if (kp.Fpxx.Fpsj[0].Spxx[0].Dj > amountLimit)
                    {
                        var files = KaiPiaoOperationForDaJinE_数量得分成小数(kp, 大于10W的InvoiceNoticeItem, inotice, amountLimit);
                        xmlFileUnits.AddRange(files);
                    }
                    else
                    {
                        var files = KaiPiaoOperationForDaJinE_数量分为整数(kp, 大于10W的InvoiceNoticeItem, inotice, amountLimit);
                        xmlFileUnits.AddRange(files);
                    }
                }
            }
            #endregion

            #region 解决拆分误差

            //步骤
            //1、得到差额：InvoiceNotice.含税总金额 - Sum(Xml.金额+ Xml.税额)
            //2、找到默认第一个小于额度的XML
            //3、加上差额后的含税计算未税以及税额

            var grouptmp = xmlFileUnits.Select(t => new
            {
                InvoiceNoticeID = t.InvoiceNotice.ID,
                InvoiceNoticeAmount = (t.InvoiceNotice.Amount + t.InvoiceNotice.Difference).ToRound(2),
                InvoiceTaxRate = t.InvoiceNotice.InvoiceTaxRate,
                KpTotal = t.Kp.Fpxx.Fpsj.First().Spxx.Sum(c => c.Je + c.Se)
            });

            var group_notice = (from kp in grouptmp
                                group kp by new { kp.InvoiceNoticeID, kp.InvoiceNoticeAmount, kp.InvoiceTaxRate } into t_group
                                select new
                                {
                                    InvoiceNoticeID = t_group.Key.InvoiceNoticeID,
                                    InvoiceNoticeAmount = t_group.Key.InvoiceNoticeAmount,
                                    InvoiceTaxRate = t_group.Key.InvoiceTaxRate,
                                    KpTotal = t_group.Sum(t => t.KpTotal)
                                }).Distinct();


            foreach (var notice in group_notice.Where(t => t.KpTotal != t.InvoiceNoticeAmount))
            {
                //差额 notice -  Kp
                var diff = notice.InvoiceNoticeAmount - notice.KpTotal;

                //限额
                var XianE = amountLimit * (1 + notice.InvoiceTaxRate);

                if (Math.Abs(diff) > 0.00M)
                {
                    //当前第一个小于限额的Kp的第一项
                    var oldJe = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je;
                    var oldSe = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Se;
                    var Sl = xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Sl;

                    //计算该第一项的新的金额
                    var newJe = ((oldJe + oldSe + diff) / (1 + notice.InvoiceTaxRate)).ToRound(2);
                    var newDj = newJe / Sl;
                    var newSe = (newJe * notice.InvoiceTaxRate).ToRound(2);

                    //赋值
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je = newJe;
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Dj = newDj;
                    xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                        .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Se = newSe;

                    //验证还差一分钱的情况
                    var newKptotal = xmlFileUnits.Where(t => t.InvoiceNotice.ID == notice.InvoiceNoticeID).Sum(kp => kp.Kp.Fpxx.Fpsj.First().Spxx.Sum(c => c.Je + c.Se));
                    if (notice.InvoiceNoticeAmount != newKptotal)
                    {
                        //此时，误差只能是 0.01 或者 -0.01, 税额不会影响
                        //一分太小了，税额算出来才0.0013元，开票是精确到0.01元，所以不影响
                        var newdiff = notice.InvoiceNoticeAmount - newKptotal;
                        newJe += newdiff;
                        newDj = newJe / Sl;

                        //重新赋值
                        xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                            .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Je = newJe;
                        xmlFileUnits.FirstOrDefault(n => n.InvoiceNotice.ID == notice.InvoiceNoticeID && (n.Kp.Fpxx.Fpsj.Sum(p => p.Spxx.Sum(t => t.Je + t.Se)) + diff) < XianE)
                            .Kp.Fpxx.Fpsj.FirstOrDefault().Spxx.FirstOrDefault().Dj = newDj;
                    }
                }
            }

            #endregion

            //得到所有的xmlFileUnits
            return xmlFileUnits;
        }


        /// <summary>
        /// 开票操作
        /// 1、对于总金额小于10W的通知，可能会合并的
        /// 2、通知中，小于10W的Items
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private XmlFileUnit KaiPiaoOperation(InvoiceNotice notice, string[] invoiceNoticeItemIDs = null)
        {
            Kp kp = new Kp();

            if (invoiceNoticeItemIDs == null || invoiceNoticeItemIDs.Length <= 0)
            {
                kp = new Kp(notice);
            }
            else
            {
                kp = new Kp(notice, invoiceNoticeItemIDs);
            }

            return new XmlFileUnit(notice, kp);
        }


        /// <summary>
        /// 通知大于10W 其中的item小于10W
        /// </summary>
        /// <param name="invoiceNotice"></param>
        /// <param name="InvoiceNoticeItems"></param>
        /// <returns></returns>
        private List<XmlFileUnit> LessThanLimit(InvoiceNotice invoiceNotice, List<InvoiceNoticeItem> InvoiceNoticeItems,decimal amountLimit)
        {
            if (InvoiceNoticeItems == null || InvoiceNoticeItems.Count <= 0)
            {
                return new List<XmlFileUnit>();
            }

            List<XmlFileUnit> files = new List<XmlFileUnit>();

            decimal oneXmlTotalJinE = 0;
            List<string[]> groupedInvoiceNoticeItems = new List<string[]>();

            List<string> temp = new List<string>();

            for (int i = 0; i < InvoiceNoticeItems.Count;)
            {
                if (oneXmlTotalJinE + InvoiceNoticeItems[i].KaiPiaoJinE <= amountLimit)
                {
                    oneXmlTotalJinE = oneXmlTotalJinE + InvoiceNoticeItems[i].KaiPiaoJinE;
                    temp.Add(InvoiceNoticeItems[i].ID);
                    i++;
                }
                else
                {
                    groupedInvoiceNoticeItems.Add(temp.ToArray());
                    temp.Clear();
                    oneXmlTotalJinE = 0;
                }
            }

            groupedInvoiceNoticeItems.Add(temp.ToArray());
            temp.Clear();
            oneXmlTotalJinE = 0;

            //开始开票
            foreach (var item in groupedInvoiceNoticeItems)
            {
                var file = KaiPiaoOperation(invoiceNotice, item);
                files.Add(file);
            }

            return files;
        }

        /// <summary>
        /// 单价 小于 10W
        /// </summary>
        /// <param name="kp"></param>
        /// <param name="invoiceNoticeItem"></param>
        /// <param name="invoiceNotice"></param>
        /// <returns></returns>
        private List<XmlFileUnit> KaiPiaoOperationForDaJinE_数量分为整数(Kp kp, InvoiceNoticeItem invoiceNoticeItem, InvoiceNotice invoiceNotice,decimal amountLimit)
        {
            List<XmlFileUnit> listXml = new List<XmlFileUnit>();

            decimal 不含税单价 = kp.Fpxx.Fpsj[0].Spxx[0].Dj;
            decimal totalJe = kp.Fpxx.Fpsj[0].Spxx[0].Je;  //原总金额
            decimal totalSl = kp.Fpxx.Fpsj[0].Spxx[0].Sl;  //原总数量

            int 前N_1拆分成单个发票中的数量 = (int)(amountLimit / 不含税单价); 

            decimal 拆分数量 = Math.Floor(totalSl / 前N_1拆分成单个发票中的数量) + (totalSl % 前N_1拆分成单个发票中的数量 > 0 ? 1 : 0);

            //前 (拆分数量 - 1) 个的 金额、税额、数量
            decimal 前N_1金额 = (不含税单价 * 前N_1拆分成单个发票中的数量).ToRound(2);
            decimal 前N_1税额 = (前N_1金额 * invoiceNoticeItem.InvoiceTaxRate).ToRound(2);
            decimal 前N_1数量 = 前N_1拆分成单个发票中的数量;

            for (int i = 0; i < 拆分数量 - 1; i++)
            {
                Kp kpNew = (Kp)kp.Copy();

                kpNew.Fpxx.Fpsj[0].Spxx[0].Je = 前N_1金额;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Se = 前N_1税额;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Sl = 前N_1数量;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Dj = 前N_1金额 / 前N_1数量;

                listXml.Add(new XmlFileUnit(invoiceNotice, kpNew));
            }



            Kp kpNewLast = (Kp)kp.Copy();

            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je = totalJe - (拆分数量 - 1) * 前N_1金额;
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Se = (kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je * invoiceNoticeItem.InvoiceTaxRate).ToRound(2); 
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Sl = totalSl - (拆分数量 - 1) * 前N_1数量;
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Dj = kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je / kpNewLast.Fpxx.Fpsj[0].Spxx[0].Sl;


            listXml.Add(new XmlFileUnit(invoiceNotice, kpNewLast));

            return listXml;
        }

        /// <summary>
        /// 单价 大于 10W
        /// </summary>
        /// <param name="kp"></param>
        /// <param name="invoiceNoticeItem"></param>
        /// <param name="invoiceNotice"></param>
        /// <returns></returns>
        private List<XmlFileUnit> KaiPiaoOperationForDaJinE_数量得分成小数(Kp kp, InvoiceNoticeItem invoiceNoticeItem, InvoiceNotice invoiceNotice,decimal amountLimit)
        {
            List<XmlFileUnit> listXml = new List<XmlFileUnit>();

            decimal totalJe = kp.Fpxx.Fpsj[0].Spxx[0].Je;  //原总金额
            decimal totalSl = kp.Fpxx.Fpsj[0].Spxx[0].Sl;  //原总数量

            decimal 拆分数量 = Math.Floor(totalJe / amountLimit) + (totalJe % amountLimit > 0 ? 1 : 0); 

            //前 (拆分数量 - 1) 个的 金额、税额、数量
            decimal 前N_1金额 = (amountLimit).ToRound(2);
            decimal 前N_1税额 = (前N_1金额 * invoiceNotice.InvoiceTaxRate).ToRound(2);
            decimal 前N_1数量 = (amountLimit / totalJe * totalSl).ToRound(2);
            decimal 前N_1单价 = 前N_1金额 / 前N_1数量;

            for (int i = 0; i < 拆分数量 - 1; i++)
            {
                Kp kpNew = (Kp)kp.Copy();

                kpNew.Fpxx.Fpsj[0].Spxx[0].Je = 前N_1金额;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Se = 前N_1税额;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Sl = 前N_1数量;
                kpNew.Fpxx.Fpsj[0].Spxx[0].Dj = 前N_1单价;

                listXml.Add(new XmlFileUnit(invoiceNotice, kpNew));
            }

            Kp kpNewLast = (Kp)kp.Copy();

            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je = (totalJe - (拆分数量 - 1) * amountLimit).ToRound(2); 
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Se = (kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je * invoiceNotice.InvoiceTaxRate).ToRound(2);
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Sl = totalSl - (拆分数量 - 1) * 前N_1数量;
            kpNewLast.Fpxx.Fpsj[0].Spxx[0].Dj = kpNewLast.Fpxx.Fpsj[0].Spxx[0].Je / kpNewLast.Fpxx.Fpsj[0].Spxx[0].Sl;

            listXml.Add(new XmlFileUnit(invoiceNotice, kpNewLast));

            return listXml;
        }


        private void InvoiceInfoXml(string xml, string savePath)
        {
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.LoadXml(xml);
            xdoc.Save(savePath);
        }
    }

    /// <summary>
    /// XML容器
    /// </summary>
    public class XmlFileUnit
    {
        public XmlFileUnit(InvoiceNotice notice, Kp kp)
        {
            this.Kp = kp;
            this.InvoiceNotice = notice;
         }

        public Kp Kp { get; set; }

        public InvoiceNotice InvoiceNotice { get; set; }
    }

    #region Xml开票

    [Serializable]
    public class Kp
    {
        //有此节点，则表示用带分类编码
        public string Version { get; set; }

        public Fpxx Fpxx { get; set; }

        public Kp()
        {

        }

        public Kp(InvoiceNotice notice) : this(notice, null)
        {

        }

        public Kp(InvoiceNotice notice, string[] invoiceNoticeItemIDs)
        {
            var units = new BaseUnitsView();

            this.Version = InvoiceXmlConfig.Version;
            this.Fpxx = new Fpxx();
            Fpxx.Fpsj = new List<Fp>();
            int Djh = 1;
            //foreach (var notice in notices)
            //{
            Fp fp = new Fp();
            fp.Djh = Djh.ToString();
            fp.Gfmc = notice.Client.Company.Name;
            fp.Gfsh = notice.ClientInvoice.TaxCode;
            fp.Gfyhzh = notice.BankName + notice.BankAccount;
            fp.Gfdzdh = notice.Address + " " + notice.Tel;
            //外单加备注信息
            if (notice.Client.ClientType == Enums.ClientType.External)
            {
                List<string> list = new List<string>();

                var invocieNoticeItems = notice.InvoiceItems.AsEnumerable();
                if (invoiceNoticeItemIDs != null && invoiceNoticeItemIDs.Length > 0)
                {
                    invocieNoticeItems = invocieNoticeItems.Where(t => invoiceNoticeItemIDs.Contains(t.ID));
                }

                foreach (var item in invocieNoticeItems)
                {
                    string[] orderId = item.OrderID.Split(',');
                    for (int i = 0; i < orderId.Length; i++)
                    {
                        list.Add(orderId[i]);
                    }
                }
                list = list.Distinct().ToList();
                if (list.Count > 0)
                {
                    StringBuilder str = new StringBuilder();
                    foreach (var item in list)
                    {
                        str.Append(item + "/");
                    }
                    fp.Bz = str.ToString().Substring(0, str.Length - 1);
                }
            }
            else if (notice.Client.ClientType == Enums.ClientType.Internal)
            {
                fp.Bz = notice.Summary ?? "";
            }

            fp.Fhr = InvoiceXmlConfig.Fhr;
            fp.Skr = InvoiceXmlConfig.Skr;
            fp.Spbmbbh = InvoiceXmlConfig.Spbmbbh;
            fp.Hsbz = InvoiceXmlConfig.Hsbz;

            fp.Spxx = new List<Sph>();
            int Xh = 1;

            var invocieNoticeItems1 = notice.InvoiceItems.AsEnumerable();
            if (invoiceNoticeItemIDs != null && invoiceNoticeItemIDs.Length > 0)
            {
                invocieNoticeItems1 = invocieNoticeItems1.Where(t => invoiceNoticeItemIDs.Contains(t.ID));
            }

            foreach (var item in invocieNoticeItems1)
            {
                Sph sph = new Sph();
                sph.Xh = Xh.ToString();
                if (notice.InvoiceType == Enums.InvoiceType.Full)
                {
                    sph.Spmc = item.OrderItem?.Category.Name;
                    sph.Ggxh = item.OrderItem?.Model;
                    sph.InvoiceNoticeItemID = item.ID;
                    sph.Jldw = units.Where(u => u.Code == item.OrderItem.Unit).FirstOrDefault()?.Name;
                    sph.Sl = decimal.Parse(item.OrderItem.Quantity.ToString("####"));
                    sph.Spbm = item.TaxCode.Trim();//去除税务编码末尾空格，避免开票软件无法识别
                }
                else
                {
                    sph.Spmc = "服务费";
                    sph.Ggxh = "";
                    sph.InvoiceNoticeItemID = item.ID;
                    sph.Jldw = "";
                    sph.Sl = 1;
                    sph.Spbm = "3040407040000000000";
                }
                sph.Qyspbm = "";
                sph.Syyhzcbz = "";
                sph.Lslbz = "";
                sph.Yhzcsm = "";
                //开票金额 = 含税金额+差额
                sph.Je = ((item.Amount + item.Difference) / (1 + item.InvoiceTaxRate)).ToRound(2);
                sph.Dj = (sph.Je / sph.Sl);
                sph.Slv = item.InvoiceTaxRate.ToRound(4);
                sph.Se = (sph.Je * item.InvoiceTaxRate).ToRound(2);
                sph.Kce = "";

                fp.Spxx.Add(sph);
                Xh += 1;
            }
            Fpxx.Fpsj.Add(fp);
            Djh += 1;
            //}
            Fpxx.Zsl = Fpxx.Fpsj.Count;
        }
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

        /// <summary>
        /// 库存，自定义字段，用于财务出库，不影响税务开票
        /// </summary>
        public KCXX KCXX { get; set; }

        [XmlIgnore]
        public string InvoiceNoticeItemID { get; set; }
    }

    [Serializable]
    public class KCXX
    { 
        public List<KC> KC { get; set; }
    }

    [Serializable]
    public class KC 
    {
        public string KC_ID { get; set; }

        public decimal SL { get; set; }
    }

    #endregion
}

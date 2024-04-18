using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils;
using Needs.Wl.CustomsTool.WinForm.App_Utils;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    /// <summary>
    /// 报关单
    /// </summary>
    [Serializable]
    public sealed class DecHead : Needs.Ccs.Services.Models.DecHead
    {
        /// <summary>
        ///  当申报完成后发生
        /// </summary>
        public event DeclareApplyHanlder DeclareApplied;

        /// <summary>
        /// 报关成功时发生
        /// </summary>
        public new event DeclareSucceedHanlder DeclareSucceed;

        public DecHead()
        {
            //报关申报（报文准备就绪）
            this.DeclareApplied += DecHead_DeclareApply;
            this.DeclareSucceed += DecHead_DeclareSucceed;
        }

        /// <summary>
        /// 单据状态名字
        /// </summary>
        public string StatusName
        {
            get
            {
                return MultiEnumUtils.ToText<Needs.Ccs.Services.Enums.CusDecStatus>(this.CusDecStatus);
            }
        }

        /// <summary>
        /// 申报
        /// </summary>
        public new void Declare()
        {
            string fileName = this.ID + ".zip.temp";
            var fileServerUrl = Tool.Current.Company.FileServerUrl;  //服务器地址
            var decMainFolder = Tool.Current.Folder.DecMainFolder; //报关单主目录

            #region 下载文件到本地
            //创建文件夹
            FileDirectory file = new FileDirectory();
            System.Net.WebClient wbClient = new System.Net.WebClient();
            List<string> files = new List<string>();
            var clientPath = string.Empty;
            //下载pdf文件
            foreach (var doc in this.EdocRealations)
            {
                clientPath = decMainFolder + @"\" + ConstConfig.Edoc + @"\" + Path.GetFileName(doc.FileUrl);
                files.Add(clientPath);
                wbClient.DownloadFile(fileServerUrl + @"\" + doc.FileUrl, clientPath);
            }
            //下载xml文件
            clientPath = decMainFolder + @"\" + ConstConfig.Message + @"\" + this.ID + ".xml";
            files.Add(clientPath);
            wbClient.DownloadFile(fileServerUrl + @"\" + this.MarkingUrl, clientPath);
            #endregion

            #region 压缩并删除文件
            //压缩文件
            ZipFile zip = new ZipFile(fileName);
            zip.Files = files;
            zip.ZipedPath = decMainFolder + @"\" + ConstConfig.OutBox + @"\";
            zip.ZipFiles();
            //删除已被压缩的源文件
            files.ForEach(t =>
            {
                File.Delete(t);
            });
            //.temp文件重命名
            File.Move(zip.ZipedPath + fileName, zip.ZipedPath + this.ID + ".zip");
            #endregion

            #region 更新报关单状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.CusDecStatus = MultiEnumUtils.ToCode<Ccs.Services.Enums.CusDecStatus>(Ccs.Services.Enums.CusDecStatus.Declare);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
            this.OnApplied(new DeclareApplyEventArgs(this));
            #endregion
        }

        #region 报关成功

        /// <summary>
        /// 报关成功
        /// </summary>
        public new void DeclareSucceess()
        {
            var order = new QuoteConfirmedOrdersView().Where(item => item.ID == this.OrderID).FirstOrDefault();
            if (order != null)
            {
                order.SetAdmin(this.Inputer);
                order.DeclareSuccess();
            }
            //更新成功状态
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { IsSuccess = true }, item => item.ID == this.ID);
            }
            this.OnDeclareSucceed(new DeclareSucceedEventArgs(this, order));
        }

        /// <summary>
        /// 报关成功触发事件
        /// </summary>
        /// <param name="args"></param>
        public new void OnDeclareSucceed(DeclareSucceedEventArgs args)
        {
            this.DeclareSucceed?.Invoke(this, args);
        }

        /// <summary>
        /// 报关成功后执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecHead_DeclareSucceed(object sender, DeclareSucceedEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 废弃，不再使用报关库房

                ////生成香港出库通知
                //if (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>().Count(item => item.OrderID == e.DecHead.OrderID && item.WarehouseType == (int)WarehouseType.HongKong) == 0)
                //{
                //    var exitNotice = new Layer.Data.Sqls.ScCustoms.ExitNotices();
                //    exitNotice.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNotice);
                //    exitNotice.OrderID = e.DecHead.OrderID;
                //    exitNotice.AdminID = e.DecHead.Inputer.ID;
                //    exitNotice.DecHeadID = e.DecHead.ID;
                //    exitNotice.WarehouseType = (int)WarehouseType.HongKong;
                //    exitNotice.ExitNoticeStatus = (int)ExitNoticeStatus.UnExited;
                //    exitNotice.Status = (int)Status.Normal;
                //    exitNotice.CreateDate = DateTime.Now;
                //    exitNotice.UpdateDate = DateTime.Now;
                //    reponsitory.Insert(exitNotice);

                //    foreach (var list in e.DecHead.Lists)
                //    {
                //        var itemID = Needs.Overall.PKeySigner.Pick(PKeyType.ExitNoticeItem);
                //        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExitNoticeItems
                //        {
                //            ID = itemID,
                //            ExitNoticeID = exitNotice.ID,
                //            DecListID = list.ID,
                //            Quantity = list.GQty,
                //            ExitNoticeStatus = (int)ExitNoticeStatus.UnExited,
                //            Status = (int)Status.Normal,
                //            CreateDate = DateTime.Now,
                //            UpdateDate = DateTime.Now
                //        });
                //    }
                //}

                #endregion

                //写入海关汇率             
                //var currency = e.DecHead.Lists.FirstOrDefault().TradeCurr;
                //var Rate = new CustomExchangeRatesView(currency).ToRate();
                //if (Rate != null)
                //{
                //    e.DecHead.CustomsExchangeRate = Rate.Rate;
                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { e.DecHead.CustomsExchangeRate }, item => item.ID == e.DecHead.ID);
                //}

                //写入财务报关单数据
                var decTax = new Layer.Data.Sqls.ScCustoms.DecTaxs();
                decTax.ID = e.DecHead.ID;
                decTax.InvoiceType = (int)e.Order.ClientAgreement.InvoiceType;
                decTax.Status = (int)DecTaxStatus.Unpaid;
                decTax.CreateDate = DateTime.Now;
                decTax.UpdateDate = decTax.CreateDate;
                decTax.HandledType = 0;
                decTax.IsPutInSto = 0;
                reponsitory.Insert(decTax);


                //【融合】调用接口，生成香港出库通知--工具
                var exit = new Needs.Ccs.Services.Models.GenerateExitNotice(this);
                exit.Excute();

                ////重新生成对账单
                //if (this.isTwoStep)
                //{
                //    try {
                //        var order = new Needs.Ccs.Services.Views.OrdersView().FirstOrDefault(t => t.ID == this.OrderID);
                //        order.GenerateBill(order.OrderBillType, order.PointedAgencyFee);
                //    }
                //    catch (Exception ex) {
                //        ex.CcsLog("两步申报，重新生成对账单错误");
                //    }
                //}


                #region 对美加征 排除的，去掉加征部分关税率

                try
                {
                    if (e.DecHead.LicenseDocus.Any(t => t.DocuCode == "0"))
                    {
                        //当前日期
                        var dateNow = DateTime.Now.Date;

                        //原产地税则
                        var origin = new PvDataOriginTariffsView().ToArray();

                        //
                        var entityList = new List<OrderItemTax>();

                        foreach (var item in e.DecHead.Lists)
                        {
                            //判断当前型号是否原产地加征
                            var addedorigin = origin.FirstOrDefault(t => t.HSCode == item.CodeTS && t.Origin == item.OriginCountry && t.StartDate <= dateNow && (t.EndDate > dateNow || t.EndDate == null));
                            if (addedorigin != null)
                            {
                                //有加征，减去加征税率
                                var taxes = new OrderItemTaxesView().Where(t => t.OrderItemID == item.OrderItemID).ToList();

                                var tariff = taxes.FirstOrDefault(t => t.Type == CustomsRateType.ImportTax);
                                var addedtax = taxes.FirstOrDefault(t => t.Type == CustomsRateType.AddedValueTax);
                                var consume = taxes.FirstOrDefault(t => t.Type == CustomsRateType.ConsumeTax);


                                #region  重新计算应收关税、增值税 

                                //完税价格计算公式：Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0)
                                //var topPrice = (item.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                                var total = (item.DeclTotal * e.DecHead.CustomsExchangeRate.Value).ToRound(2);

                                if (tariff != null && (tariff.Rate - addedorigin.Rate / 100) > 0)
                                {
                                    //实际应交关税率
                                    var tariffRate = tariff.Rate - addedorigin.Rate / 100;

                                    //关税计算公式：Round(完税价格 * 关税率, 2)
                                    var importTaxValue = (total * tariffRate).ToRound(2);

                                    tariff.Value = importTaxValue;
                                    entityList.Add(new OrderItemTax
                                    {
                                        ID = tariff.ID,
                                        Rate = tariffRate,
                                        ReceiptRate = tariffRate,
                                        Value = importTaxValue,
                                        UpdateDate = DateTime.Now
                                    });

                                    if (consume != null)
                                    {
                                        //消费税计算公式：Round((完税价格＋关税)÷(1－消费税税率)×消费税税率, 2）
                                        var exciseTaxValue = ((total + importTaxValue) / (1 - consume.Rate) * consume.Rate).ToRound(2);

                                        consume.Value = exciseTaxValue;
                                        entityList.Add(new OrderItemTax
                                        {
                                            ID = consume.ID,
                                            Rate = consume.Rate,
                                            ReceiptRate = consume.ReceiptRate,
                                            Value = exciseTaxValue,
                                            UpdateDate = DateTime.Now
                                        });
                                    }

                                    if (addedtax != null)
                                    {
                                        var exciseTaxRate = consume?.Rate ?? 0m;
                                        //增值税计算公式：Round(((完税价 + 关税) + (完税价 + 关税) / (1-消费税税率) * 消费税税率) * 增值税率, 2)
                                        var addedValueTaxValue = (((total + importTaxValue) + (total + importTaxValue) / (1 - exciseTaxRate) * exciseTaxRate) * addedtax.Rate).ToRound(2);

                                        addedtax.Value = addedValueTaxValue;
                                        entityList.Add(new OrderItemTax
                                        {
                                            ID = addedtax.ID,
                                            Rate = addedtax.Rate,
                                            ReceiptRate = addedtax.ReceiptRate,
                                            Value = addedValueTaxValue,
                                            UpdateDate = DateTime.Now
                                        });
                                    }
                                }
                                #endregion
                            }
                        }

                        //更新进DB
                        foreach (var item in entityList)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new { Rate = item.Rate, ReceiptRate = item.ReceiptRate, Value = item.Value, UpdateDate = item.UpdateDate }, t => t.ID == item.ID);
                        }
                    }
                }
                catch (Exception ex)
                {

                    ex.CcsLog("报关完成修正加征关税率");

                }


                #endregion
            }
        }

        #endregion

        public void OnApplied(DeclareApplyEventArgs args)
        {
            this.DeclareApplied?.Invoke(this, args);
        }

        private void DecHead_DeclareApply(object sender, DeclareApplyEventArgs e)
        {
            //写入日志与轨迹
            e.DecHead.Trace("导出报文.zip至文件夹，等待发送或自动发送至海关");
        }
    }
}
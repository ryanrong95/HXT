using Needs.Utils.Serializers;
using Needs.Wl.CustomsTool.WinForm.App_Utils;
using Needs.Wl.CustomsTool.WinForm.Models;
using Needs.Wl.CustomsTool.WinForm.Models.Hanlders;
using Needs.Wl.CustomsTool.WinForm.Models.Messages;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class DecReceipt
    {
        /// <summary>
        /// 海关业务回执
        /// </summary>
        private DecHead DecHead { get; set; }

        private DEC_RESULT DecResult { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string FilePath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        private string FileName { get; set; }

        /// <summary>
        /// 备份路径
        /// </summary>
        private string BackupUrl { get; set; }

        public DecReceipt(string path)
        {
            this.DecResult = XmlSerializerExtend.XmlTo<DEC_RESULT>(XmlService.LoadXmlFile(path));
            this.DecHead = Tool.Current.Customs.DecHeads.Where(item => item.SeqNo == DecResult.CUS_CIQ_NO).FirstOrDefault();
            this.FilePath = path;
        }

        //日志记录
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 处理回执
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="FullPath"></param>
        public void HandleDecHeadResponse()
        {
            try
            {
                //业务回执
                Logger.Trace("-------------------");
                Logger.Trace("获取报关单导入业务回执:" + this.DecResult?.CUS_CIQ_NO);
                //content
                Logger.Trace("报关单业务回执内容:" + this.DecResult?.CUS_CIQ_NO + " CHANNEL: " + this.DecResult.CHANNEL + " NOTE: " + this.DecResult.NOTE);

                this.FileName = Path.GetFileName(this.FilePath);
                this.BackupUrl = Tool.Current.Folder.DecMainFolder + @"\" + ConstConfig.InBox_BK + @"\" + this.FileName;
                this.SaveAs();
                Logger.Trace("-------------------");
            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单导入回执失败：" + ex.Message);
                Logger.Error("-------------------");
                Logger.Error("StackTrace:" + ex.StackTrace);
                Logger.Error("InnerException:" + ex.InnerException);
                Logger.Error("========================================================");
            }
        }

        private void SaveAs()
        {
            switch (this.DecResult.CHANNEL)
            {
                case "0":
                case "1":
                    ReceiptNormal();
                    break;
                case "7":
                    ReceiptNormal();
                    //生成香港仓库的出库通知
                    HKOut();
                    //IcgooInsideAutoOut();
                    break;
                case "a":
                case "b":
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "F":
                case "G":
                case "H":
                case "I":
                    ReceiptNormal();
                    break;
                case "K":
                    ReceiptNormal();
                    break;
                case "L":
                    ReceiptNormal();
                    //ReceiptL();
                    break;
                case "M":
                case "N":
                    ReceiptNormal();
                    break;
                case "P":
                    ReceiptNormal();
                    break;
                case "R":
                case "S":
                case "T":
                case "W":
                case "X":
                case "Y":
                case "Z":
                case "f0":
                case "F1":
                case "S0":
                    ReceiptNormal();
                    break;
                case "J":
                    //改状态
                    ReceiptNormal();
                    ReceiptJ();//海关编号进DB
                    ExpireNotice();//收费逾期信息进DB
                    break;

                default:
                    ReceiptNoChannel();
                    break;
            }
        }

        private void ReceiptNormal()
        {
            //更新报关单状态
            if (this.DecHead.CusDecStatus != "R")
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = this.DecResult.CHANNEL, }, item => item.ID == this.DecHead.ID);
                }

            }

            this.DecHead.CusDecStatus = this.DecResult.CHANNEL;

            this.DecHead.Trace(this.DecResult.NOTE, DateTime.Parse(this.DecResult.NOTICE_DATE), this.FileName, this.FilePath, this.BackupUrl);
        }

        private void ReceiptNoChannel()
        {
            this.DecHead.Trace(this.DecResult.NOTE, DateTime.Parse(this.DecResult.NOTICE_DATE), this.FileName, this.FilePath, this.BackupUrl);
        }

        //private void ReceiptL()
        //{
        //    //更新报关单相关字段
        //    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
        //    {
        //        if (this.DecHead.IsSplitDeclare)
        //        {
        //            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { EntryId = this.DecResult.ENTRY_ID}, item => item.ID == this.DecHead.ID);
        //        }
        //        else
        //        {
        //            var ddate = DateTime.Parse(this.DecResult.D_DATE);
        //            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { EntryId = this.DecResult.ENTRY_ID, IEDate = this.DecResult.I_E_DATE, DDate = ddate }, item => item.ID == this.DecHead.ID);
        //        }      
        //    }
        //}

        private void ReceiptJ()
        {
            //更新报关单相关字段
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var ddate = DateTime.Parse(this.DecResult.D_DATE);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { EntryId = this.DecResult.ENTRY_ID, IEDate = this.DecResult.I_E_DATE, DDate = ddate }, item => item.ID == this.DecHead.ID);
                //2022-08-30 报关完成后，将OrderReceipts 的updatedate 改为当前日期 收款统计要求
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { UpdateDate = DateTime.Now }, 
                                                                                item => item.OrderID == this.DecHead.OrderID&&
                                                                                item.Type==(int)Ccs.Services.Enums.OrderReceiptType.Received &&
                                                                                item.Status==(int)Ccs.Services.Enums.Status.Normal);
            }
        }

        private void HKOut()
        {
            this.DecHead.DeclareSucceess();
        }

        private void IcgooInsideAutoOut()
        {
            this.DecHead.SZWarehouseAutoOut();
        }

        private static void Declare_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Logger.Trace("报关单回执更新报关单失败");
        }

        private static void Declare_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Logger.Trace("报关单回执更新报关单成功");
        }

        public void ExpireNotice()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //判断该客户的费用类型
                string orderID = this.DecHead.OrderID;
                var orderInfo = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == orderID).FirstOrDefault();
                if (orderInfo != null)
                {
                    string clientID = orderInfo.ClientID;
                    DateTime dtReturn = ExpireDate(clientID);
                    if (DateTime.Now.Date!= dtReturn)
                    {
                        var Amount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>().Where(t => t.OrderID == orderID && t.FeeType != (int)Needs.Ccs.Services.Enums.OrderFeeType.Product).Select(t => t.Amount).Sum();
                        Needs.Ccs.Services.Models.ExpireOrder expireOrder = new Ccs.Services.Models.ExpireOrder();
                        expireOrder.OrderID = this.DecHead.OrderID;
                        expireOrder.Amount = Amount;
                        expireOrder.ExpireDate = dtReturn;
                        expireOrder.Enter();
                    }
                }
            }
        }

        private DateTime ExpireDate(string clientID)
        {
            var client = new Needs.Ccs.Services.Views.ClientsView().Where(t => t.ID == clientID).FirstOrDefault();
           
            DateTime dtReturn = DateTime.Now.Date;
            switch (client.Agreement.IncidentalFeeClause.PeriodType)
            {
                case Ccs.Services.Enums.PeriodType.AgreedPeriod:
                    dtReturn = this.DecHead.DDate.Value.AddDays(client.Agreement.IncidentalFeeClause.DaysLimit.Value);
                    break;

                case Ccs.Services.Enums.PeriodType.Monthly:
                    string month = this.DecHead.DDate.Value.AddMonths(1).ToString("yyyy-MM");
                    dtReturn = Convert.ToDateTime(month + "-" + client.Agreement.IncidentalFeeClause.MonthlyDay.Value.ToString());
                    break;

                default:
                    dtReturn = DateTime.Now.Date;
                    break;
            }

            return dtReturn;
        }
    }
}

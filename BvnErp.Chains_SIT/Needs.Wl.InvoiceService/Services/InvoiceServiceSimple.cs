using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services
{
    public class InvoiceServiceSimple
    {
        public static void Execute(DateTime? schedulerTime = null, DateTime? nextTime = null)
        {
            new Models.TaxMapJobLog
            {
                ID = Guid.NewGuid().ToString("N"),
                Type = Enums.TaxMapJobLogType.JobNormalEntry,
                CreateDate = DateTime.Now,
                SchedulerTime = schedulerTime,
                NextTime = nextTime,
            }.InsertNew();

            var unCheckedTaxManages = new Views.TaxManageOriginView().Where(t => t.InvoiceCode != null
                                                                              && t.InvoiceNo != null
                                                                              && t.InvoiceDate != null
                                                                              && t.IsVaild == Enums.InvoiceVaildStatus.UnChecked
                                                                              && t.Status == Enums.Status.Normal)
                                                                     .OrderByDescending(t => t.CreateDate)
                                                                     .ToArray();
            if (unCheckedTaxManages == null || unCheckedTaxManages.Length == 0)
            {
                return;
            }

            foreach (var unCheckedTaxManage in unCheckedTaxManages)
            {
                try
                {
                    HandleOneTaxManage(unCheckedTaxManage);

                    new Models.TaxMapJobLog
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Type = Enums.TaxMapJobLogType.JobNormalExit,
                        CreateDate = DateTime.Now,
                    }.InsertNew();
                }
                catch (Exception ex)
                {
                    Models.TaxManage.SetIsVaild(unCheckedTaxManage.ID, Enums.InvoiceVaildStatus.Invailded);

                    new Models.TaxMapJobLog
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Type = Enums.TaxMapJobLogType.JobException,
                        CreateDate = DateTime.Now,
                        Exception = ex.Message + " ||||||| " + ex.StackTrace,
                    }.InsertNew();
                }
            }
        }

        /// <summary>
        /// 处理一个 TaxManage
        /// </summary>
        /// <param name="taxManage"></param>
        private static void HandleOneTaxManage(Models.TaxManage taxManage)
        {
            var result = InvVeri.InvoiceInfoForCom.CheckInvoice(
                    taxManage.InvoiceCode, taxManage.InvoiceNo, taxManage.InvoiceDate?.ToString("yyyy-MM-dd"),
                    taxManage.Amount.ToString("0.00"));
            if (!result.Item1)
            {
                Models.TaxManage.SetIsVaild(taxManage.ID, Enums.InvoiceVaildStatus.Invailded);
                return;
            }

            string invoiceJson = result.Item3;
            InvVeri.ResultModelApi resultModelApi = JsonConvert.DeserializeObject<InvVeri.ResultModelApi>(invoiceJson);
            string newInvoiceResultID = Guid.NewGuid().ToString("N") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var invoiceResultDB = new Models.InvoiceResult();
            invoiceResultDB.ID = newInvoiceResultID;
            invoiceResultDB.RtnCode = resultModelApi.RtnCode;
            invoiceResultDB.resultCode = resultModelApi.resultCode;
            invoiceResultDB.invoicefalseCode = resultModelApi.invoicefalseCode ?? "";
            invoiceResultDB.resultMsg = resultModelApi.resultMsg;
            invoiceResultDB.invoiceName = resultModelApi.invoiceName;
            invoiceResultDB.Status = Enums.Status.Normal;
            invoiceResultDB.CreateDate = DateTime.Now;
            invoiceResultDB.UpdateDate = DateTime.Now;

            if (resultModelApi.RtnCode != "00" || resultModelApi.resultCode != "1000")
            {
                Models.TaxManage.SetIsVaild(taxManage.ID, Enums.InvoiceVaildStatus.Invailded);
                invoiceResultDB.InsertNew();
                return;
            }

            #region 插入 InvoiceResult、InvoiceResultDetail 数据到数据库

            //查询到有效的信息
            var invoiceResultApi = JsonConvert.DeserializeObject<InvVeri.InvoiceResultApi>(resultModelApi.invoiceResult);
            invoiceResultDB.invoiceTypeName = invoiceResultApi.invoiceTypeName;
            invoiceResultDB.invoiceTypeCode = invoiceResultApi.invoiceTypeCode;
            invoiceResultDB.checkDate = ConvertHelper.ToDateTime(invoiceResultApi.checkDate);
            invoiceResultDB.checkNum = ConvertHelper.ToInt32(invoiceResultApi.checkNum);
            invoiceResultDB.invoiceDataCode = invoiceResultApi.invoiceDataCode;
            invoiceResultDB.invoiceNumber = invoiceResultApi.invoiceNumber;
            invoiceResultDB.billingTime = ConvertHelper.ToDateTime(invoiceResultApi.billingTime);
            invoiceResultDB.purchaserName = invoiceResultApi.purchaserName;
            invoiceResultDB.taxpayerNumber = invoiceResultApi.taxpayerNumber;
            invoiceResultDB.taxDiskCode = invoiceResultApi.taxDiskCode;
            invoiceResultDB.taxpayerAddressOrId = invoiceResultApi.taxpayerAddressOrId;
            invoiceResultDB.taxpayerBankAccount = invoiceResultApi.taxpayerBankAccount;
            invoiceResultDB.salesName = invoiceResultApi.salesName;
            invoiceResultDB.salesTaxpayerNum = invoiceResultApi.salesTaxpayerNum;
            invoiceResultDB.salesTaxpayerAddress = invoiceResultApi.salesTaxpayerAddress;
            invoiceResultDB.salesTaxpayerBankAccount = invoiceResultApi.salesTaxpayerBankAccount;
            invoiceResultDB.totalAmount = ConvertHelper.ToDecimal(invoiceResultApi.totalAmount);
            invoiceResultDB.totalTaxNum = ConvertHelper.ToDecimal(invoiceResultApi.totalTaxNum);
            invoiceResultDB.totalTaxSum = ConvertHelper.ToDecimal(invoiceResultApi.totalTaxSum);
            invoiceResultDB.invoiceRemarks = invoiceResultApi.invoiceRemarks;
            invoiceResultDB.goodsClerk = invoiceResultApi.goodsClerk;
            invoiceResultDB.checkCode = invoiceResultApi.checkCode;
            invoiceResultDB.voidMark = invoiceResultApi.voidMark;
            invoiceResultDB.isBillMark = invoiceResultApi.isBillMark;
            invoiceResultDB.tollSign = invoiceResultApi.tollSign;
            invoiceResultDB.tollSignName = invoiceResultApi.tollSignName;

            var invoiceResultDetailsDB = new Models.InvoiceResultDetails();
            foreach (var item in invoiceResultApi.invoiceDetailData)
            {
                invoiceResultDetailsDB.Details.Add(new Models.InvoiceResultDetail
                {
                    ID = Guid.NewGuid().ToString("N"),
                    InvoiceResultID = newInvoiceResultID,
                    lineNum = ConvertHelper.ToInt32(item.lineNum),
                    goodserviceName = item.goodserviceName,
                    model = item.model,
                    unit = item.unit,
                    number = ConvertHelper.ToDecimal(item.number),
                    price = ConvertHelper.ToDecimal(item.price),
                    sum = ConvertHelper.ToDecimal(item.sum),
                    taxRate = item.taxRate,
                    tax = ConvertHelper.ToDecimal(item.tax),
                    isBillLine = item.isBillLine,
                    zeroTaxRateSign = item.zeroTaxRateSign,
                    zeroTaxRateSignName = item.zeroTaxRateSignName,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }

            invoiceResultDB.InsertNew();
            invoiceResultDetailsDB.BatchInsertNew();

            #endregion


            Models.TaxManage.SetIsVaild(taxManage.ID, Enums.InvoiceVaildStatus.Vailded);
        }
    }
}

using Layer.Data.Sqls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services
{
    public class InvoiceService
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

            var unHandledTaxs = new Views.UnHandledTaxMapView().GetUnHandledTaxs();
            if (unHandledTaxs == null || unHandledTaxs.Length == 0)
            {
                return;
            }

            foreach (var unHandledTax in unHandledTaxs)
            {
                try
                {
                    HandleOneTaxMap(unHandledTax);

                    new Models.TaxMapJobLog
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        Type = Enums.TaxMapJobLogType.JobNormalExit,
                        CreateDate = DateTime.Now,
                    }.InsertNew();
                }
                catch (Exception ex)
                {
                    Models.TaxMap.SetApiStatus(unHandledTax.ID, Enums.TaxMapApiStatus.Exception);

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
        /// 处理一个未处理的 TaxMap
        /// </summary>
        /// <param name="unHandledTax"></param>
        private static void HandleOneTaxMap(Views.UnHandledTaxMapViewModel unHandledTax)
        {
            var possibleTaxManages = new Views.PossibleTaxManageView(unHandledTax.InvoiceNoticeID).GetDatas();
            if (possibleTaxManages == null || possibleTaxManages.Length == 0)
            {
                //未查询到可能的 TaxManage
                Models.TaxMap.SetApiStatus(unHandledTax.ID, Enums.TaxMapApiStatus.UnMatchManager);
                return;
            }

            bool isCheckInvoiceSuccess = false;
            string targetTaxManageID = "";
            var invoiceResultDB = new Models.InvoiceResult();
            string invoiceResult = "";
            string newInvoiceResultID = "";
            foreach (var possibleTaxManage in possibleTaxManages)
            {
                var result = InvVeri.InvoiceInfoForCom.CheckInvoice(
                    unHandledTax.InvoiceCode, unHandledTax.InvoiceNo, unHandledTax.InvoiceDate.ToString("yyyy-MM-dd"),
                    possibleTaxManage.Amount.ToString("0.00"));
                if (!result.Item1)
                {
                    isCheckInvoiceSuccess = false;
                    continue;
                }

                string invoiceJson = result.Item3;
                InvVeri.ResultModelApi resultModelApi = JsonConvert.DeserializeObject<InvVeri.ResultModelApi>(invoiceJson);
                newInvoiceResultID = Guid.NewGuid().ToString("N") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

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
                    isCheckInvoiceSuccess = false;
                    Models.TaxMap.SetApiStatus(unHandledTax.ID, Enums.TaxMapApiStatus.Error);
                    invoiceResultDB.InsertNew();
                    continue;
                }

                //查询到发票信息
                isCheckInvoiceSuccess = true;
                targetTaxManageID = possibleTaxManage.ID;
                invoiceResult = resultModelApi.invoiceResult;
                break;
            }

            if (!isCheckInvoiceSuccess)
            {
                //查询失败
                Models.TaxMap.SetApiStatus(unHandledTax.ID, Enums.TaxMapApiStatus.Error);
                return;
            }

            //查询到发票信息

            #region 插入 InvoiceResult、InvoiceResultDetail 数据到数据库

            //查询到有效的信息
            var invoiceResultApi = JsonConvert.DeserializeObject<InvVeri.InvoiceResultApi>(invoiceResult);
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


            Models.TaxMap.SetApiStatus(unHandledTax.ID, Enums.TaxMapApiStatus.Success);
            Models.TaxMap.SetIsMapped(unHandledTax.ID, true);

            var taxManage = new Views.TaxManageOriginView()[targetTaxManageID];
            taxManage.InvoiceCode = unHandledTax.InvoiceCode;
            taxManage.InvoiceNo = unHandledTax.InvoiceNo;
            taxManage.InvoiceDate = unHandledTax.InvoiceDate;
            taxManage.VaildAmount = Convert.ToDecimal(invoiceResultApi.totalTaxNum);
            taxManage.IsVaild = Enums.InvoiceVaildStatus.Vailded;
            taxManage.InvoiceDetailID = newInvoiceResultID;
            taxManage.Enter();
        }
    }
}

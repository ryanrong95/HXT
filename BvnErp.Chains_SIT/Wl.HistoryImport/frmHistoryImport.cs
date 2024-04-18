using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.HistoryImport
{
    public partial class frmHistoryImport : Form
    {
        public frmHistoryImport()
        {
            InitializeComponent();
        }

        private void GenerateOrder()
        {
            List<string> OrderIds = new List<string>();
            //OrderIds.Add("1913A5D7C3CF4E31A81B97D9A358C19D");
            //OrderIds.Add("E12B5EEF154048A09A39C0F5F9B08218");
            //OrderIds.Add("EA7FB7CBA3DC41CE89151D6E1780CFEA");
            //OrderIds.Add("34B47D05004F4690A1F1ADFE23E6D312");

            using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
            {
                DateTime dtStart = Convert.ToDateTime("2019-04-30 17:18:00.000");
                DateTime dtEnd = Convert.ToDateTime("2019-05-29 10:09:04.560");
                var query = (from c in db.T_Declare
                             where c.CreateTime > dtStart && c.CreateTime < dtEnd
                             select c.OrderID).ToList();

                OrderIds = query;
            }

            foreach (var item in OrderIds)
            {
                GenerateOrder(item);
            }
        }

        private void GenerateOrder(string pendingOrderID)
        {
            Dictionary<int, string> CurrencyMap = getCurrencyMap();
            Dictionary<string, string> PortMap = getPortMap();
            HistoryUseOnly only = new HistoryUseOnly();
            string MemberCode;
            try
            {
                using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
                {
                    var query = (from c in db.T_Declare
                                 where c.OrderID == pendingOrderID
                                 select c).FirstOrDefault();

                    string orderid = query.OrderID;
                    only.DeclarationDate = query.DeclarationDate;
                    only.ContrNO = query.ContractNo;
                    only.OwnerCusCode = query.ConsumptionUseCode;
                    only.VoyNo = query.VoyageNumber;
                    only.BillNo = query.BillOfLoadNumber;

                    string port = PortMap[query.Port];
                    only.Port = port;

                    var t_order_declare = (from c in db.T_Order_Declare
                                           where c.OrderID == orderid
                                           select c).FirstOrDefault();

                    string Currency = CurrencyMap[t_order_declare.Currency];
                    only.TotalPacks = t_order_declare.TotalPacks.Value;

                    var t_order = (from c in db.T_Order
                                   where c.ID == orderid
                                   select c).FirstOrDefault();

                    var memberid = t_order.MemberID;
                    only.OrderCreateDate = t_order.CreateTime.Value;

                    var t_member = (from c in db.T_Member
                                    where c.ID == t_order.MemberID
                                    select c).FirstOrDefault();

                    MemberCode = t_member.MemberCode;

                    only.OrderID = orderid;
                    only.OrderNo = t_order.OrderNo;
                    only.IsLocal = t_order.IsLocal.Value;
                    only.Currency = Currency;
                    only.CustomsExchangeRate = t_order_declare.CustomsExchangeRate.Value;
                    only.RealExchangeRate = t_order_declare.RealExchangeRate.Value;
                }


                var clientView = new Needs.Ccs.Services.Views.ClientsView();
                var client = clientView.Where(t => t.ClientCode == MemberCode).FirstOrDefault();

                only.Client = client;
                only.days = CalcDays(only.DeclarationDate);

                HistoryCreateOrder CreateOrder = new HistoryCreateOrder();
                WriteLog(pendingOrderID + "开始生成\r\n");
                CreateOrder.AssemblyOrder(only);
                ImportOrderFee(only.OrderID, only.OrderNo);
                ImportFile(only.OrderID, only.OrderNo, only.OrderCreateDate.ToString("yyyyMMdd"));
                UpdateOrderStatus(only.OrderNo);
                WriteLog(pendingOrderID + "生成结束\r\n");
            }
            catch (Exception ex)
            {
                WriteLog(pendingOrderID + "导入错误:" + ex.ToString() + "\r\n");
            }
        }

        private Dictionary<int, string> getCurrencyMap()
        {
            Dictionary<int, string> CurrencyMap = new Dictionary<int, string>();
            CurrencyMap.Add(502, "USD");
            CurrencyMap.Add(110, "HKD");
            CurrencyMap.Add(300, "EUR");

            return CurrencyMap;
        }

        private Dictionary<string, string> getPortMap()
        {
            Dictionary<string, string> PortMap = new Dictionary<string, string>();
            PortMap.Add("", "5301");
            PortMap.Add("皇岗海关", "5301");
            PortMap.Add("沙头角关", "5303");
            PortMap.Add("文锦渡关", "5320");

            return PortMap;
        }

        private void ImportOrderFee(string orderid, string orderno)
        {
            try
            {
                using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
                {
                    var query = (from c in db.T_Order_Incidentals
                                 where c.OrderID == orderid
                                 select c).ToList();

                    foreach (var item in query)
                    {
                        var fee = new Needs.Ccs.Services.Models.OrderPremium();
                        fee.OrderID = orderno;
                        fee.Type = Needs.Ccs.Services.Enums.OrderPremiumType.OtherFee;
                        fee.Name = item.ChargeItem;
                        fee.Count = 1;
                        fee.UnitPrice = item.Amount.Value;
                        fee.Currency = "CNY";
                        fee.Rate = 1;
                        fee.Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("XDTAdmin");

                        fee.Enter();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(orderno + "费用导入错误!" + ex.ToString()+ "\r\n");
            }
        }

        private void ImportFile(string orderid, string orderno, string createdate)
        {
            using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
            {
                var query = (from c in db.T_Order_Attachments
                             where c.OrderID == orderid
                             select c).ToList();

                foreach (var item in query)
                {
                    int filetype = item.AttachmentType.Value;
                    if (item.AttachmentType.Value == 4)
                    {
                        filetype = 5;
                    }
                    ImportOrderFile(orderno, createdate, item.AttachmentAddress, filetype);
                }

                #region 付汇委托书
                var queryPayExchange = (from c in db.T_Order_Payment
                                        where c.OrderID == orderid
                                        && c.PayExchangeID != null && c.Status != 7
                                        select c.PayExchangeID).ToList();

                foreach(var item in queryPayExchange)
                {
                    if (item != "")
                    {
                        var querypay = (from c in db.T_Order_Attachments
                                     where c.OrderID == item
                                        select c).ToList();

                        foreach (var itempay in querypay)
                        {
                            int filetype = itempay.AttachmentType.Value;
                            if (itempay.AttachmentType.Value == 4)
                            {
                                filetype = 5;
                            }
                            ImportOrderFile(orderno, createdate, itempay.AttachmentAddress, filetype);
                        }
                    }
                }
                #endregion

              
            }
        }
        private void ImportOrderFile(string orderno, string createdate, string fileurl, int filetype)
        {
            try
            {
                #region 下载文件
                string path = @"D:/HistoryImport/Order/" + createdate.Substring(0, 6) + "/" + createdate.Substring(6, 2);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                string[] FileName = fileurl.Split('/');
                string fileName = FileName[4];

                string FileUrlPrefix = "http://old.wl.net.cn";

                HttpDownloadFile down = new HttpDownloadFile();
                down.Download(FileUrlPrefix + fileurl.Replace("~", ""), path + "/" + fileName);
                #endregion

                #region 插入DB
                string[] fileFormat = fileName.Split('.');
                string FileFormat = "image/png";

                switch (fileFormat[1])
                {
                    case "jpg":
                    case "jpeg":
                        FileFormat = "image/jpeg";
                        break;

                    case "png":
                        FileFormat = "image/png";
                        break;

                    case "pdf":
                        FileFormat = "application/pdf";
                        break;
                }

                DateTime dtCreateTime = Convert.ToDateTime(createdate.Insert(4, "-").Insert(7, "-"));

                var fileEditionTwo = new Layer.Data.Sqls.ScCustoms.OrderFiles
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile),
                    OrderID = orderno,
                    Name = fileName,
                    FileType = filetype,
                    FileFormat = FileFormat,
                    Url = "Order\\" + createdate.Substring(0, 6) + "\\" + createdate.Substring(6, 2) + "\\" + fileName,
                    FileStatus = (int)Needs.Ccs.Services.Enums.OrderFileStatus.Audited,
                    Status = 200,
                    CreateDate = dtCreateTime,
                };

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Insert(fileEditionTwo);
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(orderno + " 文件" + fileurl + "导入失败,失败原因:" + ex.ToString() + "\r\n");
            }

        }

        private void UpdateOrderStatus(string orderid)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = 7 }, item => item.ID == orderid);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                GenerateOrder();
            }));
        }

        public int CalcDays(string DeclarationDate)
        {
            int days = 0;

            int dayfirst = 0;
            DateTime dt718 = Convert.ToDateTime("2018-07-18");
            DateTime dtDeclaration = Convert.ToDateTime(DeclarationDate);
            TimeSpan timediff = dtDeclaration - dt718;
            dayfirst = timediff.Days;

            int daysecond = 0;
            DateTime dt20190509 = Convert.ToDateTime("2019-05-09");
            TimeSpan timeSecondDiff = dtDeclaration - dt20190509;
            daysecond = timeSecondDiff.Days;


            if (dayfirst < 0)
            {
                //2018-07-18 之前的
                days = -1;
            }
            else if (daysecond <= 0)
            {
                //2018-07-18 与 2019-05-09 之间
                days = 0;
            }
            else if (daysecond > 0)
            {
                //2019-05-09 之后
                days = 1;
            }

            return days;
        }

        private void btnTax_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ImportTax();
            }));
        }

        private void ImportTax()
        {
            try
            {
                var OrderView = new Needs.Ccs.Services.Views.OrdersView();

                using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
                {
                    var query = (from c in db.T_Finance_Invoice
                                 where c.ID == "d579a3b7877d443c8337d2d660265a21"
                                 select c).FirstOrDefault();

                    TaxHistoryUseOnly only = new TaxHistoryUseOnly();
                    if (query.InvoiceType == 1)
                    {
                        only.InvoiceType = Needs.Ccs.Services.Enums.InvoiceType.Full;
                    }
                    else if (query.InvoiceType == 2)
                    {
                        only.InvoiceType = Needs.Ccs.Services.Enums.InvoiceType.Service;
                    }

                    only.Adress = query.Address;
                    only.Tel = query.Phone;
                    only.BankName = query.BankName;
                    only.BankAccount = query.BankAccount;
                    only.InvoiceNo = query.InvoiceNumbers;
                    only.CreateDate = query.CreateTime.Value;

                    var FinanceDetail = (from c in db.T_Finance_InvoiceDetails
                                         where c.InvoiceID == query.ID
                                         orderby c.OrderID
                                         select c).ToList();

                    only.InvoiceTaxRate = FinanceDetail.FirstOrDefault().TaxPoint.Value;
                    only.InvoiceItems = new List<TaxItemHistoryUseOnly>();


                    TaxItemHistoryUseOnly InvoiceItem = new TaxItemHistoryUseOnly();

                    foreach (var item in FinanceDetail)
                    {
                        InvoiceItem = new TaxItemHistoryUseOnly();
                        InvoiceItem.UnitPrice = item.UnitPriceTax.Value;
                        InvoiceItem.Amount = item.AmountTax.Value;
                        InvoiceItem.Difference = item.difference == null ? 0 : item.difference.Value;


                        if (only.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full)
                        {
                            var t_order_model = (from c in db.T_Order_Model
                                                 join b in db.T_Order
                                                 on c.OrderID equals b.ID
                                                 where c.ID == item.ModelID
                                                 select new
                                                 {
                                                     Model = c.Model,
                                                     Qty = c.Quantity,
                                                     OrderNo = b.OrderNo
                                                 }).FirstOrDefault();

                            InvoiceItem.OrderNO = t_order_model.OrderNo;
                            var order = OrderView.Where(t => t.ID == t_order_model.OrderNo).FirstOrDefault();
                            var orderitem = order.Items.Where(t => t.Model == t_order_model.Model && t.Quantity == t_order_model.Qty).FirstOrDefault();
                            InvoiceItem.OrderItemID = orderitem.ID;
                        }
                        else
                        {
                            var t_order = (from c in db.T_Order
                                           where c.ID == item.OrderID
                                           select c.OrderNo).FirstOrDefault();
                            InvoiceItem.OrderNO = t_order;
                        }
                        only.InvoiceItems.Add(InvoiceItem);
                    }

                    CreateTax createTax = new CreateTax();
                    createTax.Create(only);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void WriteLog(string strs)
        {
            FileStream fs = new FileStream("D:\\HistoryImport\\log.txt", FileMode.Append, FileAccess.Write);
            //获得字节数组
            string datetime = "时间 " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+" ";
            byte[] data = System.Text.Encoding.Default.GetBytes(datetime+strs);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        private void btnImportPayExchange_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                ImportPayExchange();
            }));
        }

        private void ImportPayExchange()
        {
            try
            {
                string payExchangeid = "12605080EF25407ABFB34D12D0590B7F";
                Dictionary<int, string> CurrencyMap = getCurrencyMap();
                using (YD_LogisticsDBEntities db = new YD_LogisticsDBEntities())
                {
                    var query = (from c in db.T_Order_PayExchange
                               where c.ID == payExchangeid
                               select c).FirstOrDefault();

                    PaymentHistoryUseOnly only = new PaymentHistoryUseOnly();
                    only.SupplierName = query.SupplierName;
                    only.SupplierEnglishName = query.SupplierCompanyName;
                    only.SupplierAddress = query.SupplierAddress;
                    only.BankAccount = query.BankAccount;
                    only.BankName = query.BankName;
                    only.BankAddress = query.BankAddress;
                    only.SwiftCode = query.BankCode;
                    only.ExchangeRateType = getExchangeRateType(query.PaymentRateType);
                    only.Currency = CurrencyMap[query.Currency];
                    only.ExchangeRate = query.PaymentRate;
                    only.PaymentType = getPaymentType(query.PaymentType);
                    only.ExceptPayDate = query.ExpectedPaymentDate;
                    only.SettlementDate = query.SettlementDate;
                    only.CreateDate = query.CreateTime.Value;
                    only.PayExchangeApplyStatus = Needs.Ccs.Services.Enums.PayExchangeApplyStatus.Completed;

                    var member = (from c in db.T_Member
                                  where c.ID == query.MemberID
                                  select c).FirstOrDefault();

                    only.MemberCode = member.MemberCode;


                    var querylist = (from c in db.T_Order_Payment
                                     join d in db.T_Order on c.OrderID equals d.ID
                                     where c.PayExchangeID == payExchangeid
                                     select new PaymentItemHistoryUseOnly
                                     {
                                         OrderID = d.OrderNo,
                                         Amount = c.PaymentAmount
                                     }).ToList();

                    only.Lists = querylist;

                    CreatePayExchange create = new CreatePayExchange();
                    create.Create(only);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Needs.Ccs.Services.Enums.ExchangeRateType getExchangeRateType (int typeValue)
        {
            int returnTypeValue = 1;
            switch (typeValue)
            {
                case 1:
                    returnTypeValue = 2;
                    break;

                case 2:
                    returnTypeValue = 3;
                    break;

                case 3:
                    returnTypeValue = 1;
                    break;
            }

            return (Needs.Ccs.Services.Enums.ExchangeRateType)returnTypeValue;            
        }

        private Needs.Ccs.Services.Enums.PaymentType getPaymentType (int paymentType)
        {
            int returnPaymentValue = 1;
            switch (paymentType)
            {
                case 1:
                    returnPaymentValue = 3;
                    break;

                case 3:
                    returnPaymentValue = 1;
                    break;

                case 4:
                    returnPaymentValue = 2;
                    break;
            }
            return (Needs.Ccs.Services.Enums.PaymentType)returnPaymentValue;
        }

        private Needs.Ccs.Services.Enums.PayExchangeApplyStatus getPaymentStatus(int status)
        {
            int returnStatusValue = 1;
            switch (status)
            {
                case 1:
                    returnStatusValue = 1;
                    break;

                case 2:
                    returnStatusValue = 2;
                    break;

                case 3:
                case 5:
                    returnStatusValue = 3;
                    break;

                case 4:
                case 7:
                    returnStatusValue = 4;
                    break;

                case 6:
                    returnStatusValue = 5;
                    break;
            }
            return (Needs.Ccs.Services.Enums.PayExchangeApplyStatus)returnStatusValue;
        }
    }
}

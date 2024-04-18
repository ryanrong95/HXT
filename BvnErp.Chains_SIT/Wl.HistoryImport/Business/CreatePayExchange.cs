using Needs.Ccs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.HistoryImport
{
    public class CreatePayExchange
    {
        public void Create(PaymentHistoryUseOnly only)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string clientID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Where(item => item.ClientCode == only.MemberCode).Select(item => item.ID).FirstOrDefault();
                string payExchangeID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApply);

                //插入PayExchangeApplies
                var payExchange = new Layer.Data.Sqls.ScCustoms.PayExchangeApplies
                {
                    ID = payExchangeID,
                    ClientID = clientID,
                    SupplierName = only.SupplierName,
                    SupplierEnglishName = only.SupplierEnglishName,
                    SupplierAddress = only.SupplierAddress,
                    BankAccount = only.BankAccount,
                    BankName = only.BankName,
                    BankAddress = only.BankAddress,
                    SwiftCode = only.SwiftCode,
                    ExchangeRateType = (int)only.ExchangeRateType,
                    Currency = only.Currency,
                    ExchangeRate = only.ExchangeRate,
                    PaymentType = (int)only.PaymentType,
                    ExpectPayDate = only.ExceptPayDate,
                    SettlemenDate = only.SettlementDate,
                    PayExchangeApplyStatus = (int)only.PayExchangeApplyStatus,
                    Status = 200,
                    CreateDate = only.CreateDate,
                    UpdateDate = only.CreateDate
                };

                reponsitory.Insert(payExchange);

                //插入PayExchangeApplies
                foreach (var item in only.Lists)
                {
                    var PayExchangeApplyItems = new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                        PayExchangeApplyID = payExchangeID,
                        OrderID = item.OrderID,
                        Amount = item.Amount,
                        Status = 200,
                        CreateDate = only.CreateDate,
                        UpdateDate = only.CreateDate,
                    };

                    reponsitory.Insert(PayExchangeApplyItems);
                }


                //插入付汇文件
                foreach(var item in only.Lists)
                {
                    var files = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderFiles>().Where(t => t.OrderID == item.OrderID).ToList();
                    foreach(var file in files)
                    {
                        int filetype = file.FileType;
                        if (file.FileType == 5)
                        {
                            filetype = 11;
                        }
                        var PayExchangeApplyFiles = new Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyFile),
                            PayExchangeApplyID = payExchangeID,
                            Name = file.Name,
                            FileType = filetype,
                            FileFormat = file.FileFormat,
                            Url = file.Url,
                            Status = 200,
                            CreateDate = file.CreateDate,
                        };
                        reponsitory.Insert(PayExchangeApplyFiles);
                    }
                }

                //更改订单的付汇数量
                foreach(var item in only.Lists)
                {
                    var totalamount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(t => t.OrderID == item.OrderID).Sum(t => t.Amount);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { PaidExchangeAmount = totalamount }, t => t.ID == item.OrderID);
                }
            }
        }
    }
}

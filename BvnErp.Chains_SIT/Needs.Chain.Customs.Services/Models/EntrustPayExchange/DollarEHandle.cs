using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DollarEHandle
    {
        public DollarEquityApply Apply { get; set; }

        public List<DollarEquityMap> Maps { get; set; }

        public DollarEHandle()
        {
            this.Maps = new List<DollarEquityMap>();
        }

        public bool Handle()
        {
            bool isSuccess = false;
            DollarELock.CurrentDollarELock.IcgooDollarEBalanceLock.EnterWriteLock();
            decimal applyLeftAmouont = this.Apply.Amount; 
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
                {
                    
                    //扣除美金权益
                    var Equities = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquity>().Where(t => t.AvailableAmount>0).OrderBy(t=>t.CreateDate).ToList();
                    foreach(var item in Equities)
                    {
                        DollarEquityMap dMap = new DollarEquityMap();
                        dMap.EquityApplyID = this.Apply.ID;
                        dMap.EquityID = item.ID;
                        dMap.Currency = this.Apply.Currency;

                        decimal leftAmount = item.AvailableAmount.Value;
                        if (leftAmount > applyLeftAmouont)
                        {
                            dMap.Amount = applyLeftAmouont;
                            this.Maps.Add(dMap);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DollarEquity>(new { AvailableAmount = (item.AvailableAmount.Value - applyLeftAmouont) }, t => t.ID == item.ID);
                            isSuccess = true;
                            break;
                        }
                        else
                        {
                            dMap.Amount = leftAmount;
                            this.Maps.Add(dMap);
                            applyLeftAmouont -= leftAmount;
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.DollarEquity>(new { AvailableAmount = 0 }, t => t.ID == item.ID);
                        }
                    }

                    //更新账户余额
                    try
                    {
                        RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.EnterWriteLock();
                        var bal = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientBalance>().Where(t => t.ClientID == this.Apply.ClientID && t.Currency == this.Apply.Currency).FirstOrDefault();
                        int OriginVersion = bal.Version;
                        decimal OriginBalance = bal.Balance.Value;
                        decimal currentBalance = OriginBalance - this.Apply.Amount;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientBalance>(new
                        {
                            Balance = currentBalance,
                            Version = OriginVersion + 1
                        }, t => t.ClientID == this.Apply.ClientID && t.Currency == this.Apply.Currency);
                    }
                    finally
                    {
                        RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.ExitWriteLock();
                    }

                    //更新申请
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>(new {
                        IsPaid =true,                       
                        SeqNo = this.Apply.SeqNo,
                        PayType = (int)this.Apply.PayType,
                        FinanceVaultID = this.Apply.FinanceVaultID,
                        FinanceAccountID = this.Apply.FinanceAccountID,
                        PayerID = this.Apply.PayerID
                    }, t => t.ID == this.Apply.ID);

                    //插入匹配记录
                    foreach(var item in this.Maps)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DollarEquityMap
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            EquityApplyID = item.EquityApplyID,
                            EquityID = item.EquityID,
                            Amount = item.Amount,
                            Currency = item.Currency,
                            Status = (int)Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now                          
                        });
                    }
                   
                    reponsitory.Submit();
                }               
            }
            catch(Exception ex)
            {
                ex.CcsLog("处理转换汇失败");
            }
            finally
            {
                DollarELock.CurrentDollarELock.IcgooDollarEBalanceLock.ExitWriteLock();              
            }

           

            return isSuccess;
        }
    }

    public class DollarELock
    {
        private static DollarELock uniqueInstance = null;
        private static object locker = new object();
        public ReaderWriterLockSlim IcgooDollarEBalanceLock;
        private DollarELock()
        {
            this.IcgooDollarEBalanceLock = new ReaderWriterLockSlim();
        }

        public static DollarELock CurrentDollarELock
        {
            get
            {
                if (uniqueInstance == null)
                {
                    lock (locker)
                    {
                        if (uniqueInstance == null)
                        {
                            uniqueInstance = new DollarELock();
                        }
                    }
                }
                return uniqueInstance;
            }
        }
    }
}

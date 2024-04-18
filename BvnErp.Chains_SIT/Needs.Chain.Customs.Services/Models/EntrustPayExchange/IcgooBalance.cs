using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooBalance
    {        
        public string ClientID { get; set; }
        public string Currency { get; set; }
        /// <summary>
        /// 余额，增加余额为正，扣除余额为负
        /// </summary>
        public decimal Balance { get; set; }
        public string TriggerSource { get; set; }

        public decimal ReadBalance()
        {
            decimal balance = 0;
            RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.EnterReadLock();
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var bal = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientBalance>().Where(t => t.ClientID == this.ClientID && t.Currency == this.Currency).FirstOrDefault();
                    if (bal != null)
                    {
                        balance = bal.Balance.Value;
                    }
                    
                }
            }
            finally
            {
                RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.ExitReadLock();                
            }
            return balance;
        }

        public void UpdateBalance()
        {
            RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.EnterWriteLock();
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var bal = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientBalance>().Where(t => t.ClientID == this.ClientID && t.Currency == this.Currency).FirstOrDefault();
                    if (bal == null)
                    {

                    }
                    else
                    {
                        int OriginVersion = bal.Version;
                        decimal OriginBalance = bal.Balance.Value;
                        decimal currentBalance = OriginBalance + this.Balance;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientBalance>(new {
                            Balance = currentBalance,
                            Version = OriginVersion + 1,
                            UpdateDate = DateTime.Now
                        }, t => t.ClientID == this.ClientID && t.Currency == this.Currency);

                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientBalanceLog
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            ClientID = this.ClientID,
                            OriginBalance = OriginBalance,
                            CurrentBalance = currentBalance,
                            ChangeBalance = this.Balance,
                            OriginVersion = OriginVersion,
                            CurrentVersion = OriginVersion + 1,
                            Currency = this.Currency,
                            TriggerSource = this.TriggerSource,
                            Status = 200,
                            CreateDate = DateTime.Now,                          
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("更新IcgooRMB余额失败");
            }
            finally
            {
                RMBLock.CurrentRMBLock.IcgooRMBBalanceLock.ExitWriteLock();
            }            
        }
    }

    public class RMBLock
    {
        private static RMBLock uniqueInstance=null;
        private static object locker = new object();
        public ReaderWriterLockSlim IcgooRMBBalanceLock;
        private RMBLock()
        {
            this.IcgooRMBBalanceLock = new ReaderWriterLockSlim();
        }

        public static RMBLock CurrentRMBLock
        {
            get
            {
                if (uniqueInstance == null)
                {
                    lock (locker)
                    {
                        if (uniqueInstance == null)
                        {
                            uniqueInstance = new RMBLock();
                        }
                    }
                }
                return uniqueInstance;
            }
            
        }
    }

    public class DecHeadLock
    {
        private static DecHeadLock uniqueInstance = null;
        private static object locker = new object();
        public ReaderWriterLockSlim DecContrNoLock;
        private DecHeadLock()
        {
            this.DecContrNoLock = new ReaderWriterLockSlim();
        }

        public static DecHeadLock CurrentDecHeadLock
        {
            get
            {              
                if (uniqueInstance == null)
                {
                    lock (locker)
                    {
                        if (uniqueInstance == null)
                        {
                            uniqueInstance = new DecHeadLock();
                        }
                    }
                }
                return uniqueInstance;
            }

        }
    }
}

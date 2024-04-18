//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Needs.Underly;
//using Needs.Linq;

//namespace NtErp.Services.Models
//{
//    /// <summary>
//    /// 信用账单还款
//    /// </summary>
//    internal class ClientAccount : IClientAccount, IRefund
//    {
//        public decimal Amount { set; get; }

//        public DateTime CreateDate { set; get; }

//        public Currency Currency { set; get; }

//        public string Period { set; get; }

//        public InputSource Source { set; get; }

//        public ClientAccountType Type { set; get; }

//        internal ClientAccount()
//        {

//        }
//        string userID;
//        internal ClientAccount(string userID)
//        {
//            this.userID = userID;
//        }

//        SuccessHanlder enterSuccess;
//        ErrorHanlder enterError;
//        public event SuccessHanlder EnterSuccess
//        {
//            add { this.enterSuccess += value; }
//            remove { this.enterSuccess -= value; }
//        }
//        public event ErrorHanlder EnterError
//        {
//            add { this.enterError += value; }
//            remove { this.enterError -= value; }
//        }

//        static object locker = new object();
//        public void Enter()
//        {
//            lock (locker)
//            {
//                string message = string.Empty;
//                if (this.Amount <= 0)
//                {
//                    throw new Exception("非法操作");
//                }

//                var bill = new Views.ClientAccountsView(this.userID).CreditBills(item => item.Period == this.Period && item.Type == this.Type && item.Currency == this.Currency).FirstOrDefault();

//                if (bill == null)
//                {
//                    message = "本期账单已还清";
//                }
//                else if (this.Amount > bill.Amount - bill.Refund)
//                {
//                    message = "还款金额不可大于待还金额";
//                }

//                if (!string.IsNullOrWhiteSpace(message))
//                {
//                    if (this.enterError != null)
//                    {
//                        this.enterError(this, new ErrorEventArgs(message));
//                    }
//                    return;
//                }

//                IPayment balance = new ClientBalance(this.userID, null)
//                {
//                    Type = ClientAccountType.Cash,
//                    Currency = this.Currency,
//                    Amount = this.Amount
//                };
//                if (this.enterSuccess != null)
//                {
//                    balance.EnterSuccess += Balance_EnterSuccess;
//                }
//                if (this.enterError != null)
//                {
//                    balance.EnterError += this.enterError;
//                }
//                balance.Enter();
//            }
//        }

//        private void Balance_EnterSuccess(object sender, SuccessEventArgs e)
//        {
//            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
//            {
//                //另外记录到账单表
//                repository.Insert(new Layer.Data.Sqls.BvOrders.UserAccounts
//                {
//                    UserID = this.userID,
//                    Amount = this.Amount,
//                    Code = this.Period,
//                    CreateDate = DateTime.Now,
//                    Currency = (int)this.Currency,
//                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserAccount),
//                    Source = (int)this.Source,
//                    Type = (int)this.Type
//                });
//                //通过在Input表增加值使额度恢复
//                repository.Insert(new Layer.Data.Sqls.BvOrders.UserInputs
//                {
//                    UserID = this.userID,
//                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserInput),
//                    Amount = this.Amount,
//                    Code = this.Period,
//                    CreateDate = DateTime.Now,
//                    Currency = (int)this.Currency,
//                    Source = (int)this.Source,
//                    Type = (int)this.Type
//                });
//            }
//            if (this.enterSuccess != null)
//            {
//                this.enterSuccess(this, new SuccessEventArgs());
//            }
//        }
//    }
//}

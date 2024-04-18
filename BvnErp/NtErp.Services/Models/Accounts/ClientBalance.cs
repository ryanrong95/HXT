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
//    /// 用户余额
//    /// </summary>
//    internal class ClientBalance : IClientBalance, IPayment, IRecharge
//    {
//        public string InputID { get; internal set; }
//        public Needs.Underly.ClientAccountType Type { set; get; }
//        public Needs.Underly.Currency Currency { set; get; }
//        public decimal Amount { set; get; }
//        public DateTime CreateDate { internal set; get; }

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

//        Needs.Underly.InputSource source;
//        string userID;
//        string orderID;
//        string code;


//        internal ClientBalance()
//        {
//        }
//        internal ClientBalance(string userID)
//        {
//            this.userID = userID;
//        }

//        /// <summary>
//        /// 充值,操作Inputs表
//        /// </summary>
//        /// <param name="userID"></param>
//        /// <param name="source"></param>
//        /// <param name="code"></param>
//        internal ClientBalance(string userID, Needs.Underly.InputSource source, string code = "") : this(userID)
//        {
//            this.source = source;
//            this.code = code;
//        }
//        /// <summary>
//        /// 支出,操作Outputs表
//        /// </summary>
//        /// <param name="userID"></param>
//        /// <param name="orderID"></param>
//        internal ClientBalance(string userID, string orderID) : this(userID)
//        {
//            this.orderID = orderID;
//        }
//        /// <summary>
//        /// 充值
//        /// </summary>
//        void IRecharge.Enter()
//        {
//            if (string.IsNullOrWhiteSpace(this.userID) || this.source == 0 || this.Amount <= 0 || this.Currency == 0 || this.Type == 0)
//            {
//                throw new Exception("非法操作");
//            }

//            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
//            {
//                repository.Insert(new Layer.Data.Sqls.BvOrders.UserInputs
//                {
//                    Amount = this.Amount,
//                    Code = this.code,
//                    Currency = (int)this.Currency,
//                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserInput),
//                    Source = (int)this.source,
//                    Type = (int)this.Type,
//                    UserID = this.userID,
//                    CreateDate = DateTime.Now
//                });
//            }

//            if (this.enterSuccess != null)
//            {
//                this.enterSuccess(this, new SuccessEventArgs());
//            }

//        }
//        /// <summary>
//        /// 支付
//        /// </summary>
//        void IPayment.Enter()
//        {
//            string message = string.Empty;
//            if (string.IsNullOrWhiteSpace(this.userID) || this.Amount <= 0 || this.Currency == 0 || this.Type == 0)
//            {
//                throw new Exception("非法操作");
//            }

//            var balances = new Views.ClientBalanceView(this.userID).Balances(this.Currency, this.Type);
//            if (balances.Sum(item => item.Amount) < this.Amount)
//            {
//                message = "账户余额不足";
//            }

//            if (!string.IsNullOrWhiteSpace(message))
//            {
//                if (this.enterError != null)
//                {
//                    this.enterError(this, new ErrorEventArgs(message));
//                }
//                return;
//            }

//            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
//            {
//                Action<decimal, IClientBalance> action = (ammout, banlance) =>
//                {
//                    repository.Insert(new Layer.Data.Sqls.BvOrders.UserOutputs
//                    {
//                        Amount = ammout,
//                        Currency = (int)banlance.Currency,
//                        CreateDate = DateTime.Now,
//                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
//                        OrderID = this.orderID,
//                        Type = (int)banlance.Type,
//                        UserID = this.userID,
//                        UserInputID = banlance.InputID
//                    });
//                };
//                var total = this.Amount;
//                foreach (var item in balances.OrderBy(item => item.CreateDate))
//                {
//                    if (item.Amount >= total)
//                    {
//                        action(total, item);
//                        break;
//                    }
//                    else
//                    {
//                        total = total - item.Amount;
//                        action(item.Amount, item);
//                    }
//                }
//            }

//            if (this.enterSuccess != null)
//            {
//                this.enterSuccess(this, new SuccessEventArgs());
//            }
//        }
//    }
//}

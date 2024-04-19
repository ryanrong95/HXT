
using Needs.Overall;
using NtErp.Wss.Sales.Services.Extends;
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model
{
    /// <summary>
    /// 用户账户支出
    /// </summary>
    public class UserOutput 
    {
        public UserOutput()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        public string ID { get; set; }

        string userInputID;
        /// <summary>
        /// 收入 ID
        /// </summary>
        public string UserInputID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.userInputID))
                {

                    return "";
                }
                return this.userInputID;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(this.userInputID))
                {
                    this.userInputID = value;
                    return;
                }
                throw new NotSupportedException("Do not support multiple assignment!");
            }
        }
        /// <summary>
        /// 订单 ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 账户类型
        /// </summary>
        public UserAccountType Type { get; set; }
        /// <summary>
        /// UserID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion 


        #region 持久化
        public void Enter()
        {
            using (var repository = new Layer.Data.Sqls.BvOrdersReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Services.PKeyType.UserOutput);
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region 实现

        /// <summary>
        /// 收入平销
        /// </summary>
        /// <param name="inputid"></param>
        /// <returns></returns>
        public void Balanced()
        {
            // 支出金额较大，需要多个收入进行平销的时候会走多次
            // 第一种情况：收入需平销金额大于等于本次支出金额，只走一次
            // 第二种情况：收入需平销金额小于本次支出金额，记录支出，金额为收入需平销的金额，再进行下一收入的平销，直到收入金额大于等于支出未被平销金额
            while (this.Amount > 0)
            {
                var model = new UserOutput
                {
                    UserID = this.UserID,
                    Type = this.Type,
                    Amount = this.Amount,
                    Currency = this.Currency,
                    OrderID = this.OrderID,
                };
                var input = new UserInput
                {
                    UserID = this.UserID,
                    Type = this.Type,
                    Currency = this.Currency
                }.Balanced();
                if (input != null)
                {
                    var output = new UserOutputsView(this.UserID).Where(item => item.UserInputID == this.UserInputID).Sum(item => item.Amount as decimal?).GetValueOrDefault();

                    model.UserInputID = input.ID;
                    decimal balance = input.Amount - output;

                    if (balance >= this.Amount)
                    {
                        model.Enter();
                    }
                    else
                    {
                        model.Amount = balance;
                        model.Enter();
                    }

                    this.Amount -= balance;
                }
                else
                {
                    throw new Exception("not sufficient funds");
                }
            }
        }

        #endregion
    }
}

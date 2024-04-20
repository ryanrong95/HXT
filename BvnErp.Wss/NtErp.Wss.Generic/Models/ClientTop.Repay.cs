using Needs.Linq;
using Needs.Underly;
using NtErp.Wss.Oss.Services;
using System;
using NtErp.Wss.Services.Generic.Extends;
using NtErp.Wss.Oss.Services.Models;

namespace NtErp.Wss.Services.Generic.Models
{
    public partial class ClientTop : IUnique
    {
        public event SuccessHanlder RepaySuccess;

        public event ErrorHanlder NotEnough;
        public event ErrorHanlder NotDebt;
        public event ErrorHanlder RepayError;


        /// <summary>
        /// 债务还款(信用)
        /// </summary>
        /// <param name="price">额度</param>
        public void Repay(Currency currency, int dateIndex, decimal? price = null)
        {
            var debt = this.GetDebt(currency, dateIndex);
            decimal value = price ?? debt.Debt;
            if (value > debt.Debt)
            {
                if (this != null && this.RepayError != null)
                {
                    this.RepayError(this, new ErrorEventArgs());
                }
                return;
            }

            if (debt.Debt == 0)
            {
                if (this != null && this.NotDebt != null)
                {
                    this.NotDebt(this, new ErrorEventArgs());
                }
                return;
            }

            if (this.GetBalance(currency, UserAccountType.Cash) < value)
            {
                if (this != null && this.NotEnough != null)
                {
                    this.NotEnough(this, new ErrorEventArgs());
                }
                return;
            }



            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                    ClientID = this.ID,
                    Type = (int)UserAccountType.Cash,
                    From = (int)OutputTo.Repay,
                    OrderID = null,
                    UserInputID = null,
                    Currency = (int)currency,
                    Amount = Math.Abs(value),
                    DateIndex = dateIndex,
                    CreateDate = DateTime.Now
                });

                reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                {
                    ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                    ClientID = this.ID,
                    Type = (int)UserAccountType.Credit,
                    From = (int)OutputTo.Repay,
                    OrderID = null,
                    UserInputID = null,
                    Currency = (int)currency,
                    Amount = 0 - Math.Abs(value),
                    DateIndex = dateIndex,
                    CreateDate = DateTime.Now
                });
            }

            if (this != null && this.RepaySuccess != null)
            {
                this.RepaySuccess(this, new SuccessEventArgs());
            }
        }

        /// <summary>
        /// 债务还款(信用)
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="price"></param>
        public void Repay(decimal price, Currency currency)
        {
            //总还款按钮
            //我们可能有的客户的账期是多个期，
            foreach (var item in this.GetDebts(currency))
            {
                if (item.Debt > price || item.Debt == price)
                {
                    using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                            ClientID = this.ID,
                            Type = (int)UserAccountType.Credit,
                            From = (int)OutputTo.Repay,
                            OrderID = "",
                            UserInputID = null,
                            Currency = (int)currency,
                            Amount = 0 - Math.Abs(price),
                            DateIndex = item.DateIndex,
                            CreateDate = DateTime.Now
                        });
                    }
                    price = 0;
                    return;
                }
                else
                {
                    using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                        {
                            ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                            ClientID = this.ID,
                            Type = (int)UserAccountType.Credit,
                            From = (int)OutputTo.Repay,
                            OrderID = "",
                            UserInputID = null,
                            Currency = (int)currency,
                            Amount = 0 - Math.Abs(item.Debt),
                            DateIndex = item.DateIndex,
                            CreateDate = DateTime.Now
                        });
                    }
                    price = price - item.Debt;
                }
            }
            if (price > 0)
            {
                using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
                {
                    reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
                        UserInputID = "",
                        ClientID = this.ID,
                        Type = (int)UserAccountType.Cash,
                        From = (int)OutputTo.Repay,
                        Currency = (int)currency,
                        Amount = 0 - Math.Abs(price),
                        CreateDate = DateTime.Now,
                        DateIndex = null,
                        AdminID = Needs.Erp.Generic.Inner.Current.ID,
                    });
                }
            }
            if (this != null && this.RepaySuccess != null)
            {
                this.RepaySuccess(this, new SuccessEventArgs());
            }
        }

    }
}

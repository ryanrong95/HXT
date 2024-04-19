using Needs.Linq;
using Needs.Underly;
using NtErp.Wss.Oss.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Models
{
    /// <summary>
    /// 视图 Model
    /// </summary>

    public partial class ClientTop : Needs.Linq.IUnique
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
    }
}


namespace NtErp.Wss.Services.Models
{
    ///// <summary>
    ///// 视图 Model
    ///// </summary>

    //public partial class ClientTop  //: Needs.Linq.IUnique
    //{
    //    public event SuccessHanlder RepaySuccess;

    //    public event ErrorHanlder NotEnough;
    //    public event ErrorHanlder NotDebt;
    //    public event ErrorHanlder RepayError;


    //    /// <summary>
    //    /// 债务还款(信用)
    //    /// </summary>
    //    /// <param name="price">额度</param>
    //    public void Repay(Currency currency, int dateIndex, decimal? price = null)
    //    {
    //        var debt = this.GetDebt(currency, dateIndex);
    //        decimal value = price ?? debt.Debt;
    //        if (value > debt.Debt)
    //        {
    //            if (this != null && this.RepayError != null)
    //            {
    //                this.RepayError(this, new ErrorEventArgs());
    //            }
    //            return;
    //        }

    //        if (debt.Debt == 0)
    //        {
    //            if (this != null && this.NotDebt != null)
    //            {
    //                this.NotDebt(this, new ErrorEventArgs());
    //            }
    //            return;
    //        }

    //        if (this.GetBalance(currency, UserAccountType.Cash) < value)
    //        {
    //            if (this != null && this.NotEnough != null)
    //            {
    //                this.NotEnough(this, new ErrorEventArgs());
    //            }
    //            return;
    //        }

    //        using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
    //        {
    //            reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
    //            {
    //                ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
    //                ClientID = this.ID,
    //                Type = (int)UserAccountType.Cash,
    //                From = (int)OutputFrom.Repay,
    //                OrderID = null,
    //                UserInputID = null,
    //                Currency = (int)currency,
    //                Amount = 0 - Math.Abs(value),
    //                DateIndex = dateIndex,
    //                CreateDate = DateTime.Now
    //            });

    //            reponsitory.Insert(new Layer.Data.Sqls.CvOss.UserOutputs
    //            {
    //                ID = Needs.Overall.PKeySigner.Pick(PKeyType.UserOutput),
    //                ClientID = this.ID,
    //                Type = (int)UserAccountType.Credit,
    //                From = (int)OutputFrom.Repay,
    //                OrderID = null,
    //                UserInputID = null,
    //                Currency = (int)currency,
    //                Amount = 0 - Math.Abs(value),
    //                DateIndex = dateIndex,
    //                CreateDate = DateTime.Now
    //            });
    //        }

    //        if (this != null && this.RepaySuccess != null)
    //        {
    //            this.RepaySuccess(this, new SuccessEventArgs());
    //        }
    //    }
    //}
}

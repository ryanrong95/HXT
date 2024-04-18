using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PaymentTypeTransfer
    {
        static object locker = new object();
        private PaymentTypeTransfer() { }

        static private PaymentTypeTransfer current;
        static public PaymentTypeTransfer Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new PaymentTypeTransfer();
                        }
                    }
                }

                return current;
            }
        }

        public int L2CTransfer(PaymentType paymentType)
        {
            int type = Convert.ToInt16(paymentType);
            switch (paymentType)
            {
                case PaymentType.Check:
                    type = 2;
                    break;

                case PaymentType.Cash:
                    type = 3;
                    break;

                case PaymentType.TransferAccount:
                    type = 1;
                    break;

                default:
                    break;
            }

            return type;
        }

        public int C2LTransfer(CenterPaymentType paymentType)
        {
            int type = Convert.ToInt16(paymentType);
            switch (paymentType)
            {
                case CenterPaymentType.BankTransfer:
                    type = 3;
                    break;

                case CenterPaymentType.Cheque:
                    type = 1;
                    break;

                case CenterPaymentType.Cash:
                    type = 2;
                    break;

                default:
                    break;
            }

            return type;
        }
    }
}

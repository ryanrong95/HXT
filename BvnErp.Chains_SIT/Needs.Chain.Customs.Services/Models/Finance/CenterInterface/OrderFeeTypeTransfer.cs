using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class OrderFeeTypeTransfer
    {
        static object locker = new object();
        private OrderFeeTypeTransfer() { }

        static private OrderFeeTypeTransfer current;

        static public OrderFeeTypeTransfer Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new OrderFeeTypeTransfer();
                        }
                    }
                }

                return current;
            }
        }

        public string L2CInTransfer(int feeTypeValue)
        {
            OrderFeeType feetype = (OrderFeeType)feeTypeValue;
            string feeTypeName = feetype.GetDescription();
            switch (feeTypeName)
            {
                case "货款":
                    feeTypeName = "报关货款";
                    break;

                case "关税":
                    feeTypeName = "关税";
                    break;

                case "增值税":
                    feeTypeName = "增值税";
                    break;

                case "服务费":
                    feeTypeName = "报关服务费";
                    break;

                case "消费税":
                    feeTypeName = "消费税";
                    break;

                default:
                    break;
            }
            string feeID = AccountCatalogsAlls.Current.catalogIDIn(feeTypeName);
            return feeID;
        }
    }
}

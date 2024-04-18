using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Needs.Ccs.Services.Models
{
    public class DBSConstConfig
    {
        public class DBSConstError
        {
            /// <summary>
            /// 加密错误
            /// </summary>
            public const string Error001 = "error001";

            /// <summary>
            /// 解密错误
            /// </summary>
            public const string Error002 = "error002";

            /// <summary>
            /// 提交信息错误
            /// </summary>
            public const string Error003 = "error003";

            /// <summary>
            /// 星展返回错误
            /// </summary>
            public const string DBSRJCT = "RJCT";

            /// <summary>
            /// 报价 锁定汇率 返回错误
            /// </summary>
            public const string FXError = "ERROR";

            /// <summary>
            /// 正确
            /// </summary>
            public const string OKStatus = "OK";
        }

        public class DBSConstConfiguration
        {
            public const string Ctry = "CN";

            public const string HKCtry = "HK";

            public const string ContentType = "text/plain";

            public const string ABETxtType = "BLE";

            public const string ACTTxnType = "ACT";

            public const string CNAPSTxnType = "RTGS";

            public const string TTTxnType = "TT";

            public const string CNYAccountNo = "30015588588";

            public const string USDAccountNo = "30015589288";

            public const string CNY = "CNY";

            public const string USD = "USD";

            public const int TimeOut = 90000;

            public const string ResponseTypeACK2 = "ACK2";

            public const string ResponseTypeACK3 = "ACK3";
        }

        public class DBSConstTransName
        {
            public const string ABE = "ABE";

            public const string FXPricing = "FXPricing";

            public const string FXBooking = "FXBooking";

            public const string ACT = "ACT";

            public const string CNAPS = "CNAPS";

            public const string ARE = "ARE";

            public const string TT = "TT";

            public const string UnknownTransName = "Unknown";
        }
    }
}
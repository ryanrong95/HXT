using NtErp.Wss.Oss.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Extends
{
    static public class Class1
    {

        static public TransportMode ToNew(this TransportTerm transportTerm)
        {
            switch (transportTerm)
            {
                case TransportTerm.SelfPickUp:
                    return TransportMode.CustomPick;

                case TransportTerm.UPS:
                    return TransportMode.Ups;

                case TransportTerm.FedEx:
                    return TransportMode.FedEx;

                case TransportTerm.DHL:
                    return TransportMode.DHL;
                case TransportTerm.SF:
                    return TransportMode.Shunfeng;
                case TransportTerm.Other:
                default:
                    return TransportMode.Other;
            }
        }
    }
}

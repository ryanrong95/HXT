using Needs.Ccs.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services
{
    public static class OrderTraceExtends
    {
        public static void Trace(this Interfaces.IOrder order, Admin admin, OrderTraceStep step,string summary)
        {
            OrderTrace trace = new OrderTrace();
            trace.OrderID = order.ID;
            trace.Admin = admin;
            trace.Step = step;
            trace.Summary = summary;
            trace.Enter();
        }

        public static void Trace(this Interfaces.IOrder order, User user, OrderTraceStep step, string summary)
        {
            OrderTrace trace = new OrderTrace();
            trace.OrderID = order.ID;
            trace.User = user;
            trace.Step = step;
            trace.Summary = summary;
            trace.Enter();
        }

        public static void Trace(this Interfaces.IOrder order, OrderTraceStep step, string summary)
        {
            OrderTrace trace = new OrderTrace();
            trace.OrderID = order.ID;
            trace.Step = step;
            trace.Summary = summary;
            trace.Enter();
        }
    }
}

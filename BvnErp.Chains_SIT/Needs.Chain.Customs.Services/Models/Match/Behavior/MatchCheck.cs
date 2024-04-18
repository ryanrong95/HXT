using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Views.Alls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MatchCheck
    {
        public string OrderID { get; set; }

        public MatchCheck(string orderID)
        {
            this.OrderID = orderID;
        }

        public bool CanMatch()
        {
            bool result = false;
            var productList = new PD_OrderItemChangeNoticesAll();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState == ProcessState.UnProcess;

            Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == this.OrderID;
            lamdas.Add(lambda1);

            var products = productList.GetAlls(expression, lamdas.ToArray());

            if (products.Count() == 0)
            {
                result = true;
            }

            return result;
        }
    }
}

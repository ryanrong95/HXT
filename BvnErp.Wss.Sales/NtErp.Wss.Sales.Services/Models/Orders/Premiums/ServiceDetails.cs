
using NtErp.Wss.Sales.Services.Underly.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model.Orders
{
    public class ServiceDetails : Alters<ServiceDetail>
    {
        Order father;

        public ServiceDetails()
        {

        }

        //public override void Remove(Func<ServiceDetail, bool> predicate)
        //{
        //    decimal old = this.Total;
        //    base.Remove(predicate);
        //    ChangeEventArgs ea;
        //    //Tutopo.Fire(this.father, ea = new ChangeEventArgs(old, this.Total));
        //}

        public override IEnumerator<ServiceDetail> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public decimal Total
        {
            get
            {
                if (this.Count == 0)
                {
                    return 0;
                }
                return this.Where(item => item.Status == AlterStatus.Normal).Sum(item => item.SubTotal);
            }
        }
    }
}

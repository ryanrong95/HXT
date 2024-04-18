using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Erp
{
    /// <summary>
    /// 供应链 Salses
    /// </summary>
    public class Sales
    {
        IGenericAdmin Admin;

        public Sales(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public SalesBill Bill
        {
            get { return new SalesBill(this.Admin); }
        }
    }

    public class SalesBill
    {
        IGenericAdmin Admin;

        public SalesBill(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        //public object MyView
        //{
        //    get
        //    {
        //        return Linq.Adapter<Chain.Sales.Services.Models.Changings.ISalesOrder, SalesOrderAlls>.Current;
        //    }
        //}

        void ddsds()
        {
            try
            {
                try
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
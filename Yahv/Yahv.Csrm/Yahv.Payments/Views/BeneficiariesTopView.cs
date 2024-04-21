using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 受益人视图
    /// </summary>
    public class BeneficiariesTopView : UniqueView<Beneficiary, PvbCrmReponsitory>
    {
        private Business type;
        public BeneficiariesTopView(Business type)
        {
            this.type = type;
        }

        public BeneficiariesTopView(PvbCrmReponsitory reponsitory, Business type) : base(reponsitory)
        {
            this.type = type;
        }

        public BeneficiariesTopView()
        {

        }

        public BeneficiariesTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Beneficiary> GetIQueryable()
        {
            if (type == Business.Trading)
            {
                return new Services.Views.TradingBeneficiariesTopView<PvbCrmReponsitory>();
            }
            else if (type == Business.WarehouseServicing)
            {
                return new Services.Views.BeneficiariesTopView<PvbCrmReponsitory>(Business.WarehouseServicing);
            }
            else
            {
                return new Yahv.Services.Views.BeneficiariesTopView<PvbCrmReponsitory>();
            }
        }

        //public Beneficiary this[string id]
        //{
        //    get { return this.SingleOrDefault(item => item.ID == id); }
        //}
    }
}

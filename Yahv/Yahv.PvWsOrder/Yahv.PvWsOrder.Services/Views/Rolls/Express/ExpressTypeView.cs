using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    public class ExpressTypeView : UniqueView<ExpressTypeViewModel, PvWsOrderReponsitory>
    {
        public ExpressTypeView()
        {
        }

        internal ExpressTypeView(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ExpressTypeViewModel> GetIQueryable()
        {
            var expressTypes = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ExpressTypes>();

            return from expressType in expressTypes
                   select new ExpressTypeViewModel
                   {
                       ID = expressType.ID,
                       //ExpressCompany = expressCompany,
                       ExpressCompanyID = expressType.ExpressCompanyID,
                       TypeName = expressType.TypeName,
                       TypeValue = expressType.TypeValue,
                       Status = (GeneralStatus)expressType.Status,
                   };
        }
    }

    public class ExpressTypeViewModel : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 快递方式名称
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 快递公司ID
        /// </summary>
        public string ExpressCompanyID { get; set; }

        /// <summary>
        /// 快递方式名称
        /// </summary>
        public int TypeValue { get; set; }

        public GeneralStatus Status { get; set; }
    }
}

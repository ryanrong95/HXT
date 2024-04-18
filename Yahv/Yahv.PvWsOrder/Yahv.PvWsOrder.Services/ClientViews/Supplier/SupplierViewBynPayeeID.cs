using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class SupplierViewBynPayeeID : UniqueView<SupplierViewBynPayeeIDModel, PvbCrmReponsitory>
    {
        public SupplierViewBynPayeeID()
        {

        }

        protected override IQueryable<SupplierViewBynPayeeIDModel> GetIQueryable()
        {
            var nPayees = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nPayees>();
            var wsnSuppliersTopView = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.wsnSuppliersTopView>();

            var linq = from nPayee in nPayees
                       join wsnSupplier in wsnSuppliersTopView on nPayee.nSupplierID equals wsnSupplier.ID
                       select new SupplierViewBynPayeeIDModel
                       {
                           ID = nPayee.ID,
                           Bank = nPayee.Bank,
                           BankAddress = nPayee.BankAddress,
                           nSupplierID = nPayee.nSupplierID,
                           RealEnterpriseID = wsnSupplier.RealEnterpriseID,
                           RealEnterpriseName = wsnSupplier.RealEnterpriseName,
                       };

            return linq;
        }
    }

    public class SupplierViewBynPayeeIDModel : IUnique
    {
        /// <summary>
        /// nPayeeID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// nPayee 中 Bank
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// nPayee 中 BankAddress
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// nSupplierID
        /// </summary>
        public string nSupplierID { get; set; }

        /// <summary>
        /// wsnSuppliersTopView 中 ChineseName
        /// </summary>
        public string RealEnterpriseID { get; set; }

        /// <summary>
        /// wsnSuppliersTopView 中 EnglishName
        /// </summary>
        public string RealEnterpriseName { get; set; }
    }
}

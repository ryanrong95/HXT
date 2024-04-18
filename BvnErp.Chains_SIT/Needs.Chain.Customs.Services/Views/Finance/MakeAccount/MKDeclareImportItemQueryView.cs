using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{ 
    public class MKDeclareImportItemQueryView : UniqueView<Models.ReImportItemModel, ScCustomsReponsitory>
    {
        public MKDeclareImportItemQueryView()
        {

        }

        internal MKDeclareImportItemQueryView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ReImportItemModel> GetIQueryable()
        {


            var result = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceiptImportItems>()
                         where item.Status == (int)Enums.Status.Normal
                         select new Models.ReImportItemModel
                         {
                             ID = item.ID,
                             ImportID = item.ImportID,
                             FinanceRepID = item.FinanceRepID,
                             Seq = item.Seq,
                             USD = item.USD,
                             RMB = item.RMB,
                             DeclareRate = item.DeclareRate,
                             Currency = item.Currency,
                             Status = (Enums.Status)item.Status,
                             CreateDate = item.CreateDate,
                             UpdateDate = item.UpdateDate
                         };
            return result;
        }
    }
}

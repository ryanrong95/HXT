using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    public class PreProductInvoicesView : UniqueView<Models.MainOrderFile, ScCustomsReponsitory>
    {
        private string productUnionCode;
        public PreProductInvoicesView(string productUnionCode)
        {
            this.productUnionCode = productUnionCode;
        }

        protected override IQueryable<MainOrderFile> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on item.OrderID equals order.ID
                   //join file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrderFiles>() on order.MainOrderId equals file.MainOrderID
                   join file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CenterLinkXDTFilesTopView>() on order.MainOrderId equals file.MainOrderID
                   where item.ProductUniqueCode == this.productUnionCode && file.FileType == (int)Enums.FileType.OriginalInvoice
                   select new Models.MainOrderFile
                   {
                       ID = file.ID,
                       MainOrderID = file.MainOrderID,
                       Name = file.Name,
                       //FileFormat = file.FileFormat,
                       Url = file.Url
                   };
        }
    }
}

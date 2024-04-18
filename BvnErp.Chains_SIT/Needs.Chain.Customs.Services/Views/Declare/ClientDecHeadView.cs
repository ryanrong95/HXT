using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientDecHeadView : UniqueView<ClientDecHead, ScCustomsReponsitory>
    {
        public ClientDecHeadView()
        {

        }

        protected override IQueryable<ClientDecHead> GetIQueryable()
        {
            var fileView = new DecHeadFilesView(this.Reponsitory);
            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on head.OrderID equals order.ID
                   join file in fileView on head.ID equals file.DecHeadID into files
                   join declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on head.ID equals declist.DeclarationID into declists
                   select new ClientDecHead
                   {
                       OrderID=head.OrderID,
                       EntryId=head.EntryId,
                       ID =head.ID,
                       ContrNo=head.ContrNo,
                       DDate=head.DDate,
                       TotalDeclarePrice = declists.Count() == 0 ? 0 : declists.Sum(item=>item.DeclTotal) ,
                       files=files,
                       ClientID=order.ClientID,
                       UserID=order.UserID,
                       CreateTime=head.CreateTime,
                       IsSuccess=head.IsSuccess,
                   };
        }
    }
}

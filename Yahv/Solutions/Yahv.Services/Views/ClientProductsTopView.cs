using Layers.Data.Sqls;
using Layers.Data.Sqls.PvWsOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Http;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 客户产品通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class ClientProductsTopView<TReponsitory> : UniqueView<ClientProduct, TReponsitory>
   where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ClientProductsTopView()
        {

        }
        public ClientProductsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientProduct> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClientProducts>()
                   select new ClientProduct
                   {
                       ID = item.ID,
                       ClientID = item.ClientID,
                       Manufacturer = item.Manufacturer,
                       Model = item.Model,
                       Batch = item.Batch,
                       Name = item.Name,
                       Status = (GeneralStatus)item.Status,
                       CreateDate = item.CreateDate,
                       UpdateDate = item.UpdateDate
                   };
        }

    }
}

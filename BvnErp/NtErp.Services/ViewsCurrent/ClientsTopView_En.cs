using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Views
{
    /// <summary>
    /// Client
    /// </summary>
    public class ClientsTopView_En<T> : UniqueView<Models.ClientTop, T> where T : Layer.Linq.IReponsitory, IDisposable, new()
    {
        public ClientsTopView_En()
        {

        }

        public ClientsTopView_En(T reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientTop> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.ClientsTopView_En>()
                   select new Models.ClientTop
                   {
                       ID = entity.ID,
                       UserName = entity.UserName,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       //Password = entity.Password,
                       CreateDate = entity.CreateDate,
                       //UpdateDate = entity.UpdateDate,
                       Status = (Needs.Erp.Generic.Status)entity.Status,
                   };
        }
    }
}

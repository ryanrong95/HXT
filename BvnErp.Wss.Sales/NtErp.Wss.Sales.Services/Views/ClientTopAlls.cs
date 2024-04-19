using Layer.Data.Sqls;
using NtErp.Wss.Sales.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Views
{
    public class ClientTopAlls : Needs.Linq.UniqueView<Models.ClientTop, Layer.Data.Sqls.CvOssReponsitory>
    {
        public ClientTopAlls()
        {

        }
        internal ClientTopAlls(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientTop> GetIQueryable()
        {
            return from top in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.ClientsTopView>()
                   select new ClientTop
                   {
                       ID = top.ID,
                       UserName = top.UserName,
                       Email = top.Email,
                       Mobile = top.Mobile,
                   };
        }
    }
}


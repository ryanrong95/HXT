using NtErp.Wss.Oss.Services.Models;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// Client
    /// </summary>
    public class ClientsTopView : UniqueView<ClientTop, CvOssReponsitory>, Needs.Underly.IFkoView<ClientTop>
    {
        public ClientsTopView()
        {

        }
        internal ClientsTopView(CvOssReponsitory reponsitory) : base(reponsitory)
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

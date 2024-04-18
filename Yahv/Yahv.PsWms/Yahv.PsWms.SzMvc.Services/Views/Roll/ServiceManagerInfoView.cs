using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    /// <summary>
    /// 根据 ClientID 查询业务员的信息
    /// </summary>
    public class ServiceManagerInfoView
    {
        private string _clientID { get; set; }

        public ServiceManagerInfoView(string clientID)
        {
            this._clientID = clientID;
        }

        public ServiceManagerInfoViewModel GetTheServiceManagerInfo()
        {
            using (var PsOrderRepository = new PsOrderRepository())
            using (var PvbErmReponsitory = new PvbErmReponsitory())
            {
                var clients = PsOrderRepository.ReadTable<Layers.Data.Sqls.PsOrder.Clients>();
                string serviceManagerID = clients.Where(t => t.ID == this._clientID).Select(t => t.ServiceManagerID).FirstOrDefault();

                if (string.IsNullOrEmpty(serviceManagerID))
                {
                    return null;
                }

                var admins = PvbErmReponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>();
                var personals = PvbErmReponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Personals>();
                var serviceManagerInfo = (from admin in admins
                                          join personal in personals on admin.StaffID equals personal.ID into personals2
                                          from personal in personals2.DefaultIfEmpty()
                                          where admin.ID == serviceManagerID
                                          select new ServiceManagerInfoViewModel
                                          {
                                              AdminID = admin.ID,
                                              RealName = admin.RealName,
                                              Mobile = personal != null ? personal.Mobile : "",
                                          }).FirstOrDefault();

                return serviceManagerInfo;
            }
        }
    }

    public class ServiceManagerInfoViewModel
    {
        public string AdminID { get; set; }

        public string RealName { get; set; }

        public string Mobile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using Yahv.Underly.Logs;

namespace Yahv.Systematic
{
    /// <summary>
    /// 物流管理
    /// </summary>
    public class PvRoute : IAction
    {
        IErpAdmin admin;
        public PvRoute(IErpAdmin admin)
        {
            this.admin = admin;
        }

        public Yahv.PvRoute.Services.Views.Rolls.Logs_TransportsRoll TransportLogs
        {
            get
            {
                return new Yahv.PvRoute.Services.Views.Rolls.Logs_TransportsRoll();
            }
        }

        public Yahv.PvRoute.Services.Views.Rolls.TransportConsigneesRoll TransportConsignees
        {
            get
            {
                return new Yahv.PvRoute.Services.Views.Rolls.TransportConsigneesRoll();
            }
        }

        public Yahv.PvRoute.Services.Views.Rolls.BillsRoll Bills
        {
            get
            {
                return new Yahv.PvRoute.Services.Views.Rolls.BillsRoll();
            }
        }


        public void Logs_Error(Logs_Error log)
        {
            throw new NotImplementedException();
        }
    }
}

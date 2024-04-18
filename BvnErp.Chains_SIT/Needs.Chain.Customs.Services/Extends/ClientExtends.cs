using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class ClientExtends
    {
        /// <summary>
        /// 写入客户日志
        /// </summary>
        /// <param name="client"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.Client client, string summary)
        {
            ClientLog log = new ClientLog();
            log.ClientID = client.ID;
            log.Admin = client.Admin;
            log.ClientRank = client.ClientRank;
            log.Summary = summary;
            log.Enter();
        }
    }
}

using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户日志
    /// </summary>
    public class ClientLog : IUnique, IPersist
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public Admin Admin { get; set; }

        public Enums.ClientRank ClientRank { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public ClientLog()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientLogs
                    {
                        ID = this.ID,
                        AdminID = this.Admin.ID,
                        ClientID = this.ClientID,
                        ClientRank = (int)this.ClientRank,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientLogs
                    {
                        ID = this.ID,
                        AdminID = this.Admin.ID,
                        ClientID = this.ClientID,
                        ClientRank = (int)this.ClientRank,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}
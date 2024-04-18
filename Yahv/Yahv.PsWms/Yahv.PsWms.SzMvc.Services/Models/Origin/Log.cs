using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Log : IUnique
    {
        public string ID { get; set; }

        public string ActionName { get; set; }

        public string MainID { get; set; }

        public string Url { get; set; }

        public string Content { get; set; }

        public DateTime CreateDate { get; set; }

        public void Insert()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                repository.Insert(new Layers.Data.Sqls.PsOrder.Logs
                {
                    ID = this.ID,
                    ActionName = this.ActionName,
                    MainID = this.MainID,
                    Url = this.Url,
                    Content = this.Content,
                    CreateDate = this.CreateDate,
                });
            }
        }
    }
}
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class WxToken : IUnique
    {
        public string ID { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public DateTime CreateTime { get; set; }

        public void Insert()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.WxTokens
                {
                    ID = this.ID,
                    Type = this.Type,
                    Value = this.Value,
                    CreateTime = this.CreateTime,
                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Extends;
using Yahv.Linq;
using Yahv.Linq.Persistence;

namespace Wms.Services.Models
{
    public class Summaries: IUnique,IPersisting
    {
        public string  ID { get; set; }
        public int Otype { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public void Abandon()
        {
            throw new NotSupportedException("不支持！");
        }

        public void Enter()
        {
            using (var repository=new Layers.Data.Sqls.PvWmsRepository())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvWms.Summaries>().Where(item => item.ID == this.ID).Single() != null)
                {
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }
            }
        }
    }
}

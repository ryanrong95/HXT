using Layer.Data.Sqls;
using Needs.Overall.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Overall.Models
{
    class Version : IVersion, Linq.IUnique
    {
        public string ID { get; set; }
        public string Name { get { return this.ID; } set { this.ID = value; } }
        public string Code { get; set; }
        public DateTime LastGenerationDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public Version()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        static object locker = new object();

        public void Enter()
        {
            lock (locker)
            {
                using (var reponsitory = new BvOverallsReponsitory())
                {
                    var old = reponsitory.ReadTable<Layer.Data.Sqls.BvOveralls.Versions>()
                        .SingleOrDefault(item => item.ID == this.ID);
                    if (old == null)
                    {
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        this.UpdateDate = DateTime.Now;
                        reponsitory.Update(new Layer.Data.Sqls.BvOveralls.Versions
                        {
                            LastGenerationDate = this.LastGenerationDate,
                            Code = this.Code,
                            UpdateDate = this.UpdateDate,
                        }, item => item.ID == this.ID);
                    }
                }
            }
        }
    }
}

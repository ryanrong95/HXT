using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Models
{
    public class WaybillBox
    {
        /// <summary>
        /// 
        /// </summary>
        public string BoxID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Specs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Weight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShelveID { get; set; }

        public void Enter()
        {
            using (var rep = new PvWmsRepository())
            {
                if (rep.ReadTable<Layers.Data.Sqls.PvWms.WaybillBoxes>().Any(item => item.BoxID == this.BoxID))
                {
                    rep.Update<Layers.Data.Sqls.PvWms.WaybillBoxes>(new
                    {
                        this.BoxID,
                        this.ShelveID,
                        this.Specs,
                        this.Weight
                    }, item => item.BoxID == this.BoxID);
                }
                else
                {
                    rep.Insert(new Layers.Data.Sqls.PvWms.WaybillBoxes
                    {
                        BoxID = this.BoxID,
                        ShelveID = this.ShelveID,
                        Specs = this.Specs,
                        Weight = this.Weight
                    });
                }
            }
        }


    }
}

using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class GetIcgooPI
    {
        public List<string> IDS { get; set; }
        public string IcgooPayExchangeSupplier { get; set; }

        public GetIcgooPI()
        {
            IDS = new List<string>();
        }

        public GetIcgooPI(List<string> ids)
        {
            this.IDS = ids;
        }

        public void getPI()
        {
            try
            {
                foreach (var ID in this.IDS)
                {
                    var message = new Needs.Ccs.Services.Views.MessageView()[ID];
                    PartNoReceive model = JsonConvert.DeserializeObject<PartNoReceive>(message.PostData);
                    this.IcgooPayExchangeSupplier = model.PayExchangeSupplier;
                    List<string> OrderIDs = new List<string>();
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        OrderIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => t.IcgooOrder == model.refNo).Select(t => t.OrderID).ToList();
                    }

                    if (this.IcgooPayExchangeSupplier==null||this.IcgooPayExchangeSupplier.Trim().Equals("ICGOO ELECTRONICS LIMITED"))
                    {
                        if (OrderIDs.Count() > 0)
                        {
                            var icgoopi = new IcgooPIRequest(model.refNo, OrderIDs);
                            icgoopi.Process();
                        }                        
                    }
                    else
                    {
                        var icgooMultipi = new IcgooMultiPIRequest(model.refNo, OrderIDs);
                        icgooMultipi.Process();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void getDyjPI()
        {
            try
            {
                foreach (var ID in this.IDS)
                {
                    var pi = new DyjPIRequest(ID);
                    pi.Process();
                }
            }
            catch(Exception ex)
            {

            }
           
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvData.LinqToSolr.Attributes;

namespace Yahv.PvData.SolrService.Models
{
    [SolrCollection("standardproducts")]
    public class StandardProduct : LinqToSolr.ISolr
    {
        #region 属性

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("partnumber")]
        public string PartNumber { get; set; }

        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        [JsonProperty("packagecase")]
        public string PackageCase { get; set; }

        [JsonProperty("packaging")]
        public string Packaging { get; set; }

        [JsonProperty("createdate")]
        public DateTime CreateDate { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            var service = new LinqToSolr.SolrService();
            service.Save(this);
        }

        public void Delete()
        {
            var service = new LinqToSolr.SolrService();
            service.Delete<StandardProduct>(sp => sp.ID == this.ID);
        }

        #endregion
    }
}

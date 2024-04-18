using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentItem : IUnique,IPersist
    {
        /// <summary>
        /// （提运单ID+GoodsSeqNo ）MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ManifestConsignmentID, this.GoodsSeqNo).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        public string ManifestConsignmentID { get; set; }
        public int GoodsSeqNo { get; set; }
        public int GoodsPackNum { get; set; }
        public string GoodsPackType { get; set; }
        public decimal GoodsGrossWt { get; set; }
        public string GoodsBriefDesc { get; set; }
        public string UndgNo { get; set; }
        public string HsCode { get; set; }
        public string GoodsDetailDesc { get; set; }

        public void Enter()
        {
            throw new NotImplementedException();
        }
    }
}

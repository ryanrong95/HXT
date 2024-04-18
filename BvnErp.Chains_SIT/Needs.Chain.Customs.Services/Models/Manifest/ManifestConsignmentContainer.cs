using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ManifestConsignmentContainer :IUnique,IPersist
    {
        /// <summary>
        /// (ManifestConsignmentID + ContainerNo)MD5
        /// </summary>
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ManifestConsignmentID, this.ContainerNo).MD5();
            }
            set
            {
                this.id = value;
            }
        }
        public string ManifestConsignmentID { get; set; }
        public string ContainerNo { get; set; }

        public void Enter()
        {
            throw new NotImplementedException();
        }
    }
}

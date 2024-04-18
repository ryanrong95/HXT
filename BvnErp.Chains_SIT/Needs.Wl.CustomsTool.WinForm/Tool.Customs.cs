using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public partial class Tool
    {
        public Customs Customs
        {
            get
            {
                return new Customs();
            }
        }
    }

    public class Customs
    {
        public Customs()
        {

        }

        /// <summary>
        /// 报关单
        /// </summary>
        public WinForm.Views.DecHeadsView DecHeads
        {
            get
            {
                return new WinForm.Views.DecHeadsView();
            }
        }

        /// <summary>
        /// 报关单未上传
        /// </summary>
        public WinForm.Views.UnUploadDecHeadsListView UnUploadDecHeadsList
        {
            get
            {
                return new WinForm.Views.UnUploadDecHeadsListView();
            }
        }

        /// <summary>
        /// 舱单
        /// </summary>
        public WinForm.Views.ManifestConsignmentsView Manifests
        {
            get
            {
                return new WinForm.Views.ManifestConsignmentsView();
            }
        }
    }
}

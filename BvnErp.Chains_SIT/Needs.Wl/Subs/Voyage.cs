using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Models
{
    public partial class Admin
    {
        public Voyage Voyage
        {
            get
            {
                return new Voyage(this);
            }
        }
    }

    public class Voyage
    {
        IGenericAdmin Admin;

        public Voyage(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 运输批次
        /// </summary>
        public Needs.Ccs.Services.Views.VoyagesView ManifestSure
        {
            get
            {
                return new Needs.Ccs.Services.Views.VoyagesView();
            }
        }

        /// <summary>
        /// 运输批次附件
        /// </summary>
        public Needs.Ccs.Services.Views.VoyageFilesView VoyageFiles
        {
            get
            {
                return new Needs.Ccs.Services.Views.VoyageFilesView();
            }
        }

        public Needs.Ccs.Services.Views.OrderListVoyagesView SingleVoyages
        {
            get
            {
                return new Needs.Ccs.Services.Views.OrderListVoyagesView();
            }
        }

        /// <summary>
        /// 获取单独运输有效航次号列表-报关单录单用
        /// </summary>
        public Needs.Ccs.Services.Views.OrderVoyagesOriginView OrderVoyageNo
        {
            get
            {
                return new Ccs.Services.Views.OrderVoyagesOriginView();
            }
        }

        /// <summary>
        /// 获取有效航次号列表-报关单录单用
        /// </summary>
        public Needs.Ccs.Services.Views.VoyageNosView VoyageNos
        {
            get
            {
                return new Ccs.Services.Views.VoyageNosView();
            }
        }

        /// <summary>
        /// 运输批次明细
        /// </summary>
        public Needs.Ccs.Services.Views.VoyageDetailsView VoyageDetails
        {
            get
            {
                return new Ccs.Services.Views.VoyageDetailsView();
            }
        }

        /// <summary>
        /// 运输批次的报关单
        /// </summary>
        public Needs.Ccs.Services.Views.VoyageDecHeadsView VoyageDecHeads
        {
            get
            {
                return new Ccs.Services.Views.VoyageDecHeadsView();
            }
        }

        public Needs.Ccs.Services.Views.DeliveryAgentProxiesView DeliveryAgentProxies
        {
            get
            {
                return new Ccs.Services.Views.DeliveryAgentProxiesView();
            }
        }

        /// <summary>
        /// 运输批次列表页面
        /// </summary>
        public Needs.Wl.Logistics.Services.Views.ManifestVoyageListView ManifestVoyageListView
        {
            get
            {
                return new Needs.Wl.Logistics.Services.Views.ManifestVoyageListView();
            }
        }

    }
}

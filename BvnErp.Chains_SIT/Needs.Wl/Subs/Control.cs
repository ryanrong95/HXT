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
        public Control Control
        {
            get
            {
                return new Control(this);
            }
        }
    }

    public class Control
    {
        IGenericAdmin Admin;

        public Control(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 北京总部管控
        /// </summary>
        public Views.MyHQControlsView MyHQControls
        {
            get
            {
                return new Views.MyHQControlsView(this.Admin);
            }
        }

        /// <summary>
        /// 北京总部管控记录
        /// </summary>
        public Views.MyHQControlRecordsView MyHQControlRecords
        {
            get
            {
                return new Views.MyHQControlRecordsView(this.Admin);
            }
        }

        /// <summary>
        /// 跟单员管控
        /// </summary>
        public Views.MyMerchandiserControlsView MyMerchandiserControls
        {
            get
            {
                return new Views.MyMerchandiserControlsView(this.Admin);
            }
        }

    

        /// <summary>
        /// 跟单员管控 (订单已经取消挂起，但是还没有处理。 超过垫款上限，但是允许制单，会把订单取消挂起)
        /// </summary>
        public Views.MyMerchandiserControlsNotHangUpView MyMerchandiserControlsNotHangUp
        {
            get
            {
                return new Views.MyMerchandiserControlsNotHangUpView(this.Admin);
            }
        }

        /// <summary>
        /// 风控审批
        /// </summary>
        public Views.RiskControlApprovalView RiskControlApproval
        {
            get
            {
                return new Views.RiskControlApprovalView(this.Admin);
            }
        }

        /// <summary>
        /// 风控审批
        /// </summary>
        public Views.RiskControlApprovalNotHangUpView RiskControlApprovalNotHangUp
        {
            get
            {
                return new Views.RiskControlApprovalNotHangUpView(this.Admin);
            }
        }
        /// <summary>
        /// 风控审批(垫款上限，垫资超期)
        /// </summary>
        public Views.RiskControlApprovalNotHangUpView1 RiskControlApprovalNotHangUp1
        {
            get
            {
                return new Views.RiskControlApprovalNotHangUpView1(this.Admin);
            }
        }
        /// <summary>
        /// 跟单员管控记录
        /// </summary>
        public Views.MyMerchandiserControlRecordsView MyMerchandiserControlRecords
        {
            get
            {
                return new Views.MyMerchandiserControlRecordsView(this.Admin);
            }
        }

        /// <summary>
        /// 待审批预归类产品
        /// </summary>
        public Views.MyPreProductControlsView MyPreProductControls
        {
            get
            {
                return new Views.MyPreProductControlsView(this.Admin);
            }
        }

        /// <summary>
        /// 预归类产品管控审批记录
        /// </summary>
        public Views.MyPreProductControlRecordsView MyPreProductControlRecords
        {
            get
            {
                return new Views.MyPreProductControlRecordsView(this.Admin);
            }
        }
    }
}

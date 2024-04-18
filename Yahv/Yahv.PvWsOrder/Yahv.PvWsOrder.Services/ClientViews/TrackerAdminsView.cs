using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Spire.Pdf.Exporting.XPS.Schema;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class TrackerAdminsView : AdminsInfoTopView<PvWsOrderReponsitory>
    {
        //当前客户ID
        private string clientid;
        private MapsType mapstype;

        private TrackerAdminsView()
        {

        }

        private TrackerAdminsView(MapsType type)
        {
            this.mapstype = type;
        }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="ClientID">客户ID</param>
        public TrackerAdminsView(MapsType type, string ClientID) : this(type)
        {
            this.clientid = ClientID;
        }

        protected override IQueryable<AdminInfo> GetIQueryable()
        {
            var maps = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsTrackerTopView>().Where(item => item.Bussiness == (int)Business.WarehouseServicing);

            var realid = string.Concat(PvClientConfig.CompanyID, this.clientid).MD5();

            return from admin in base.GetIQueryable()
                   join maptracker in maps on admin.ID equals maptracker.AdminID
                   where maptracker.Type == (int)mapstype
                   where maptracker.RealID == realid
                   select admin;
        }
    }
}

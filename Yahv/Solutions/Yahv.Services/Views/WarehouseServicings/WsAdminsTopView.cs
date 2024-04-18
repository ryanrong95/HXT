using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 代仓储客户(芯达通客户)业务员或跟单员
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class WsAdminsTopView<TReponsitory> : AdminsAll<TReponsitory> // PvbErmReponsitory
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        string RealID;
        MapsType Type;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientid">客户ID</param>
        /// <param name="type">MapsType.ServiceManager业务员; MapsType.Merchandiser跟单员</param>
        /// <param name="companyid">内部公司ID:深圳市芯达通供应链管理有限公司</param>
        public WsAdminsTopView(string clientid, MapsType type, string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A")
        {
            this.RealID = string.Join("", companyid, clientid).MD5();
            this.Type = type;
        }
        public WsAdminsTopView(TReponsitory reponsitory, string clientid, MapsType type, string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A") : base(reponsitory)
        {
            this.RealID = string.Join("", companyid, clientid).MD5();
            this.Type = type;
        }
        protected override IQueryable<Admin> GetIQueryable()
        {
            var mapsView = from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>()
                           where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)this.Type && map.RealID == this.RealID
                           select map;
            var linq = from admins in base.GetIQueryable()
                       join map in mapsView on admins.ID equals map.AdminID
                       select admins;
            return linq;
        }
    }
}

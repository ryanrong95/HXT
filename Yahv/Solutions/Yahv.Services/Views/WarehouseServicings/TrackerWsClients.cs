using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 某管理员（业务员或跟单员）的代仓储客户
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class TrackerWsClients<TReponsitory> : UniqueView<WsClient, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin Admin;
        string companyid;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyid">内部公司ID:深圳市芯达通供应链管理有限公司</param>
        public TrackerWsClients(IErpAdmin admin, string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A")
        {
            this.Admin = admin;
            this.companyid = companyid;
        }
        public TrackerWsClients(TReponsitory reponsitory, IErpAdmin admin, string companyid = "DBAEAB43B47EB4299DD1D62F764E6B6A") : base(reponsitory)
        {
            this.Admin = admin;
            this.companyid = companyid;
        }


        protected override IQueryable<WsClient> GetIQueryable()
        {
            var wsclientids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsClientsTopView>().Select(item => item.ID).ToArray();

            List<MapsView> list = new List<MapsView>();
            for (int i = 0; i < wsclientids.Length; i++)
            {
                list.Add(new MapsView
                {
                    RealID = string.Join("", companyid, wsclientids[i]).MD5(),
                    ClientID = wsclientids[i]
                });
            }

            var realids = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTrackerTopView>().Where(item => item.AdminID == this.Admin.ID).Select(item => item.RealID).ToArray();
            var ids = list.Where(li => realids.Contains(li.RealID)).Select(item => item.ClientID);

            var linq = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.WsClientsTopView>().Where(item => ids.ToArray().Contains(item.ID)).Select(item => new WsClient
            {
                ID = item.ID,
                Name = item.Name,
                RegAddress = item.RegAddress,
                Corporation = item.Corporation,
                CustomsCode = item.CustomsCode,
                EnterCode = item.EnterCode,
                Uscc = item.Uscc,
                Grade = (ClientGrade)item.Grade,
                Vip = item.Vip,
                CreateDate = item.CreateDate,
                Status = (ApprovalStatus)item.Status,
                ServiceType = (ServiceType)item.ServiceType,
                IsDeclaretion = item.IsDeclaretion,
                IsStorageService = item.IsStorageService,
                StorageType = (WsIdentity)item.StorageType
            });
            return linq;
        }
    }
    class MapsView
    {
        public string RealID { set; get; }
        public string ClientID { set; get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class MyWsClientsRoll : Origins.WsClientsOrigin
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyWsClientsRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<WsClient> GetIQueryable()
        {
            //报关系统管理员和超级管理员可查看所有客户
            if (this.admin.IsSuper || this.admin.Role.Name == "报关系统总经理")
            {
                return base.GetIQueryable();
            }
            else
            {
                var wsclients = base.GetIQueryable().ToList();
                var linq1 = from maps in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsTracker>().ToList()
                            join wsclient in wsclients
                              on maps.RealID equals wsclient.MapsID
                            where maps.AdminID == this.admin.ID
                            select wsclient;
                return linq1.AsQueryable();
            }

        }
        public IQueryable<WsClient> this[Enterprise Company]
        {
            get
            {
                return this.Where(item => item.Company.ID == Company.ID);
            }
        }
        public WsClient this[string companyid, string clientid]
        {
            get
            {
                return this.FirstOrDefault(item => item.Company.ID == companyid && item.Enterprise.ID == clientid);
            }
        }
    }

    public class WsClientsRoll : Origins.WsClientsOrigin
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WsClientsRoll()
        {

        }
        protected override IQueryable<WsClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }
        public IQueryable<WsClient> this[Enterprise Company]
        {
            get
            {
                return this.Where(item => item.Company.ID == Company.ID);
            }
        }
        public WsClient this[string companyid, string clientid]
        {
            get
            {
                return this.FirstOrDefault(item => item.Company.ID == companyid && item.Enterprise.ID == clientid);
            }
        }
    }
}

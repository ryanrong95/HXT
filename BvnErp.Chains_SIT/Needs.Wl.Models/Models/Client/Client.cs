using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Wl.Logs.Services;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户信息
    /// </summary>
    [Serializable]
    public class Client : ModelBase<Layer.Data.Sqls.ScCustoms.Clients, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return this.id ?? this.Company.ID.MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company { get; set; }

        /// <summary>
        /// 业务经理
        /// </summary>
        public Admin ServiceManager { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public Admin Merchandiser { get; set; }

        /// <summary>
        /// 客户编号/入仓号
        /// </summary>
        public string ClientCode { get; set; }

        public bool? IsValid { get; set; }

        /// <summary>
        /// 客户等级
        /// </summary>
        public Enums.ClientRank ClientRank { get; set; }

        /// <summary>
        /// 客户信息添加人\操作人
        /// </summary>
        public Admin Admin { get; set; }

        public string AdminID { get; set; }
        /// <summary>
        /// 客户状态
        /// </summary>
        public Enums.ClientStatus ClientStatus { get; set; }

        /// <summary>
        /// 注册年，用于订单利润核算
        /// </summary>
        public string RegisterYear
        {
            get
            {
                return this.CreateDate.Year.ToString();
            }
        }

        /// <summary>
        /// 当客户等级改变时发生
        /// </summary>
        public event Hanlders.ClientRankChangedHanlder RankChanged;

        public Client()
        {
            this.RankChanged += Client_RankChanged;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public override void Enter()
        {
            this.Company.Reponsitory = this.Reponsitory;
            this.Company.Enter();

            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.Reponsitory.Insert(this.ToLinq());
                //新增会员，业务员默认为新增的人
                ClientAdmin clientAdmin = new ClientAdmin();
                clientAdmin.Reponsitory = this.Reponsitory;
                clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
                clientAdmin.Admin = this.Admin;
                clientAdmin.ClientID = this.ID;
                clientAdmin.Summary = "新增会员";
                clientAdmin.Enter();
            }
            else
            {
                var logchange = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Where(item => item.ID == this.ID).Select(item => new { item.ClientRank }).FirstOrDefault().ClientRank;
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 分配业务员
        /// </summary>
        /// <param name="admin"></param>
        public void SetServiceManager(Admin admin, string Summary)
        {
            ClientAdmin clientAdmin = new ClientAdmin();
            clientAdmin.Type = Enums.ClientAdminType.ServiceManager;
            clientAdmin.Admin = admin;
            clientAdmin.ClientID = this.ID;
            clientAdmin.Summary = Summary;
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
            if (count == 0)
            {
                this.Log(this.Admin.ID, "管理员[" + this.Admin.RealName + "]分配了业务员:" + admin.RealName);
            }

            clientAdmin.Enter();
        }

        /// <summary>
        /// 分配跟单员
        /// </summary>
        /// <param name="admin"></param>
        public void SetMerchandiser(Admin admin, string Summary)
        {
            ClientAdmin clientAdmin = new ClientAdmin();
            clientAdmin.Type = Enums.ClientAdminType.Merchandiser;
            clientAdmin.Admin = admin;
            clientAdmin.ClientID = this.ID;
            clientAdmin.Summary = Summary;
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == clientAdmin.ID);
            if (count == 0)
            {
                this.Log(this.Admin.ID, "管理员[" + this.Admin.RealName + "]分配了跟单员:" + admin.RealName);
            }
            clientAdmin.Enter();
        }

        /// <summary>
        /// 会员信息全部完善
        /// </summary>
        public void Confirm(string Summary)
        {
            var message = string.Empty;
            int count = 0;

            //补充协议
            count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ClientID == this.ID
             && item.Status == (int)Enums.Status.Normal);
            if (count == 0)
            {
                message += "未添加补充协议 ";
            }

            //开票信息
            count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
             && item.Status == (int)Enums.Status.Normal);
            if (count == 0)
            {
                message += "未添加开票信息 ";
            }

            if (string.IsNullOrEmpty(message))
            {
                this.SetServiceManager(this.ServiceManager, Summary);
                this.SetMerchandiser(this.Merchandiser, Summary);
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed }, item => item.ID == this.ID);
                this.OnEnter();
            }
            else
            {
                this.EnterError(this, new ErrorEventArgs(message));
            }
        }

        public string ClientConfirm(string Summary)
        {
            var message = string.Empty;
            int count = 0;

            //补充协议
            count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ClientID == this.ID
             && item.Status == (int)Enums.Status.Normal);
            if (count == 0)
            {
                message += "未添加补充协议 ";
            }

            //开票信息
            count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoices>().Count(item => item.ClientID == this.ID
             && item.Status == (int)Enums.Status.Normal);
            if (count == 0)
            {
                message += "未添加开票信息 ";
            }

            if (string.IsNullOrEmpty(message))
            {
                this.SetServiceManager(this.ServiceManager, Summary);
                this.SetMerchandiser(this.Merchandiser, Summary);
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientStatus = Enums.ClientStatus.Confirmed }, item => item.ID == this.ID);
                this.OnEnter();
            }

            return message;
        }

        /// <summary>
        /// 修改客户等级
        /// </summary>
        public void ChangeRank(Enums.ClientRank oldrank, Enums.ClientRank newrank)
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Clients>(new { ClientRank = (int)this.ClientRank }, item => item.ID == this.ID);

            this.OnRankChanged(oldrank, newrank);
        }

        virtual protected void OnRankChanged(Enums.ClientRank oldrank, Enums.ClientRank newrank)
        {
            if (this != null && this.RankChanged != null)
            {
                this.RankChanged(this, new Hanlders.ClientRankChangedEventArgs(this, oldrank, newrank));
            }
        }

        private void Client_RankChanged(object sender, Hanlders.ClientRankChangedEventArgs e)
        {
            var client = e.Client;
            this.Log(this.Admin.ID, "管理员[" + this.Admin.RealName + "]将会员等级从[" + e.OldRank.GetDescription() + "]更改到[" + e.NewRank.GetDescription() + "]");
        }
    }
}
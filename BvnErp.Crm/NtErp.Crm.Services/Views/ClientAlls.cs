using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ClientAlls : UniqueView<Client, BvCrmReponsitory>, Needs.Underly.IFkoView<Client>
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ClientAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public ClientAlls(BvCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 获取客户数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Client> GetIQueryable()
        {
            return from Client in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>()
                   join clientshower in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ClientShowers>()
                           on Client.ID equals clientshower.ClientID
                   select new Client
                   {
                       ID = Client.ID,
                       Name = Client.Name,
                       IsSafe = (IsProtected)Convert.ToInt16(Client.IsSafe),
                       Status = (ActionStatus)Client.Status,
                       CreateDate = Client.CreateDate,
                       UpdateDate = Client.UpdateDate,
                       Summary = Client.Summary,
                       NTextString = clientshower.NTextString,
                       AdminCode = Client.AdminCode,
                       CUSCC = Client.CUSCC,
                       IndustryInvolved = Client.IndustryInvolved,
                   };
        }


        /// <summary>
        /// 关系绑定
        /// </summary>
        /// <param name="ClintID">客户</param>
        /// <param name="admin">管理员</param>
        /// <param name="industry">行业</param>
        /// <param name="Manufacture">品牌</param>
        public void Binding(string ClientID, AdminTop admin, Industry industry, Company Manufacture)
        {
            var Client = this[ClientID];
            if (Client == null)
            {
                return;
            }
            this.Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsClient
            {
                ID = string.Concat(Client.ID, admin?.ID, industry?.ID, Manufacture?.ID).MD5(),
                ClientID = Client.ID,
                AdminID = admin?.ID,
                IndustryID = industry?.ID,
                ManufacturerID = Manufacture?.ID,
            });
        }

        /// <summary>
        /// 绑定销售
        /// </summary>
        /// <param name="ClientID">客户ID</param>
        /// <param name="adminids">销售ID</param>
        public void BindingSales(string ClientID, string[] adminids)
        {
            var Client = this[ClientID];
            if (Client == null)
            {
                return;
            }
            foreach (var adminid in adminids)
            {
                var admin = Extends.AdminExtends.GetTop(adminid);
                string Id = string.Concat(Client.ID, admin.ID).MD5();
                int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsClient>().Count(item => item.ID == Id);
                if (count == 0)
                {
                    this.Reponsitory.Insert(new Layer.Data.Sqls.BvCrm.MapsClient
                    {
                        ID = Id,
                        ClientID = Client.ID,
                        AdminID = admin.ID,
                        IndustryID = null,
                        ManufacturerID = null,
                    });
                }
            }
        }

        /// <summary>
        /// 解除所有绑定管理员
        /// </summary>
        /// <param name="ClientID">客户Id</param>
        public void DeleteBindingAdmin(string ClientID)
        {
            var Client = this[ClientID];
            if (Client == null)
            {
                return;
            }
            Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsClient>(item => item.ClientID == Client.ID && item.AdminID != null);
        }

        /// <summary>
        /// 解除指定销售
        /// </summary>
        /// <param name="ClientID">客户Id</param>
        /// <param name="saleids">销售ids</param>
        public void DeleteBindingSales(string ClientID, string[] saleids)
        {
            var Client = this[ClientID];
            if (Client == null)
            {
                return;
            }
            if (saleids == null || saleids.Count() == 0)
            {
                return;
            }
            Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsClient>(item => item.ClientID == Client.ID && saleids.Contains(item.AdminID));
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="ClientID">客户id</param>
        public void DeleteBinding(string ClientID)
        {
            var Client = this[ClientID];
            if (Client == null)
            {
                return;
            }
            Reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsClient>(item => item.ClientID == Client.ID && item.AdminID == null);
        }
    }
}

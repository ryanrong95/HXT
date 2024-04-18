using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 芯达通客户收货地址
    /// </summary>
    public class ClientConsigeeView : UniqueView<ClientConsignee, ScCustomReponsitory>
    {
        private string xdtClientID;

        /// <summary>
        /// 芯达通客户ID
        /// </summary>
        /// <param name="XDTClientID"></param>
        public ClientConsigeeView(string XDTClientID)
        {
            this.xdtClientID = XDTClientID;
        }

        protected override IQueryable<ClientConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientConsignees>()
                   join contact in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Contacts>() on consignee.ContactID equals contact.ID
                   where consignee.Status == (int)GeneralStatus.Normal && consignee.ClientID == this.xdtClientID
                   select new ClientConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name = consignee.Name,
                       Address = consignee.Address,
                       ContactID = contact.ID,
                       Contact = new Contact()
                       {
                           ID = contact.ID,
                           Name = contact.Name,
                           Email = contact.Email,
                           Fax = contact.Fax,
                           Mobile = contact.Mobile,
                           Tel = contact.Tel,
                           QQ = contact.QQ,
                           Summary = contact.Summary,
                           Status = contact.Status,
                           CreateDate = contact.CreateDate
                       },
                       IsDefault = consignee.IsDefault,
                       Status = consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="consignees"></param>
        public void Enter(ClientConsignee consignees)
        {
            consignees.Contact.Enter(this.Reponsitory);
            consignees.Enter(this.Reponsitory);
        }

        /// <summary>
        /// 设为默认地址
        /// </summary>
        public bool SetDefault(string ID)
        {
            var consignee = this[ID];
            if (consignee == null)
            {
                return false;
            }
            //数据持久化
            this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientConsignees>(new
            {
                IsDefault = false
            }, item => item.ClientID == consignee.ClientID);
            this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientConsignees>(new
            {
                IsDefault = true
            }, item => item.ID == consignee.ID);
            return true;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool Delete(string ID)
        {
            var consignee = this[ID];
            if (consignee == null)
            {
                return false;
            }
            this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientConsignees>(new
            {
                Status = (int)GeneralStatus.Closed,
            }, item => item.ID == consignee.ID);
            return true;
        }
    }

    /// <summary>
    /// 客户收货地址
    /// </summary>
    public class ClientConsignee : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string Name { get; set; }

        public string ContactID { get; set; }

        public Contact Contact { get; set; }

        public bool IsDefault { get; set; }

        public string Address { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientConsignee()
        {
            CreateDate = UpdateDate = DateTime.Now;
            this.Status = (int)GeneralStatus.Normal;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        internal void Enter(ScCustomReponsitory reponsitory)
        {
            if (this.IsDefault)
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientConsignees>(new
                {
                    IsDefault = false
                }, item => item.ClientID == this.ClientID);
            }
            //是否存在
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ClientConsignees
                {
                    ID = this.ID,
                    Name = this.Name,
                    ClientID = this.ClientID,
                    ContactID = this.Contact.ID,
                    Address = this.Address,
                    IsDefault = this.IsDefault,
                    Status = this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientConsignees>(new
                {
                    Name = this.Name,
                    ContactID = this.Contact.ID,
                    Address = this.Address,
                    IsDefault = this.IsDefault,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
        }
    }


    public class Contact
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Fax { get; set; }

        public string QQ { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }

        public Contact()
        {
            CreateDate = DateTime.Now;
            Status = (int)GeneralStatus.Normal;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        internal void Enter(ScCustomReponsitory reponsitory)
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.Contacts
                {
                    ID = this.ID,
                    Name = this.Name,
                    Mobile = this.Mobile,
                    Email = this.Email,
                    Status = this.Status,
                    CreateDate = DateTime.Now,
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.Contacts>(new
                {
                    Name = this.Name,
                    Mobile = this.Mobile,
                    Email = this.Email,
                }, item => item.ID == this.ID);
            }
        }
    }
}

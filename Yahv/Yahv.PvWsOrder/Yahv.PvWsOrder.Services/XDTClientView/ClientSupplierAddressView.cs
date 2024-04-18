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
    public class ClientSupplierAddressView : UniqueView<ClientSupplierAddress, ScCustomReponsitory>
    {
        public ClientSupplierAddressView()
        {

        }

        public ClientSupplierAddressView(ScCustomReponsitory reponsitory, IQueryable<ClientSupplierAddress> iQuery) : base(reponsitory, iQuery)
        {

        }

        protected override IQueryable<ClientSupplierAddress> GetIQueryable()
        {
            return from address in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientSupplierAddresses>()
                   join contact in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Contacts>() on address.ContactID equals contact.ID
                   where address.Status == (int)GeneralStatus.Normal
                   select new ClientSupplierAddress
                   {
                       ID = address.ID,
                       ClientSupplierID = address.ClientSupplierID,
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
                       Address = address.Address,
                       ZipCode = address.ZipCode,
                       IsDefault = address.IsDefault,
                       Status = address.Status,
                       CreateDate = address.CreateDate,
                       UpdateDate = address.UpdateDate,
                       Summary = address.Summary
                   };
        }

        /// <summary>
        /// 根据供应商ID查询数据
        /// </summary>
        /// <param name="supplierid"></param>
        /// <returns></returns>
        public ClientSupplierAddressView SearchBySupplierID(string supplierid)
        {
            var linq = this.IQueryable.Where(item => item.ClientSupplierID == supplierid);

            return new ClientSupplierAddressView(this.Reponsitory, linq);
        }

        /// <summary>
        /// 持久化
        /// </summary>
        /// <param name="bank"></param>
        public void Enter(ClientSupplierAddress address)
        {
            ///联系人保存
            address.Contact.Enter(this.Reponsitory);
            //默认地址
            if (address.IsDefault)
            {
                this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new
                {
                    IsDefault = false
                }, a => a.ClientSupplierID == address.ClientSupplierID);
            }
            if (string.IsNullOrWhiteSpace(address.ID))
            {
                address.ID = Guid.NewGuid().ToString();
                Reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ClientSupplierAddresses
                {
                    ID = address.ID,
                    ClientSupplierID = address.ClientSupplierID,
                    ContactID = address.Contact.ID,
                    Address = address.Address,
                    ZipCode = address.ZipCode,
                    IsDefault = address.IsDefault,
                    Status = address.Status,
                    CreateDate = address.CreateDate,
                    UpdateDate = address.UpdateDate,
                    Summary = address.Summary
                });
            }
            else
            {
                Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new
                {
                    ContactID = address.Contact.ID,
                    address.Address,
                    address.IsDefault,
                    address.Summary,
                    UpdateDate = DateTime.Now,
                }, a => a.ID == address.ID);
            }
        }


        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool Delete(string ID)
        {
            var address = this[ID];
            if (address == null)
            {
                return false;
            }
            this.Reponsitory.Update<Layers.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new
            {
                Status = (int)GeneralStatus.Closed,
            }, item => item.ID == address.ID);
            return true;
        }
    }

    public class ClientSupplierAddress : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        public string ClientSupplierID { get; set; }

        public Contact Contact { get; set; }

        public string Address { get; set; }

        public bool IsDefault { get; set; }

        public string ZipCode { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientSupplierAddress()
        {
            this.Status = (int)GeneralStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
    }
}

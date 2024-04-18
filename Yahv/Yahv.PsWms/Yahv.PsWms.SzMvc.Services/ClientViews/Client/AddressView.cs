using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.ClientModels;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.ClientViews
{
    public class AddressView : QueryView<Address, PsOrderRepository>
    {
        private string clientid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private AddressView()
        {

        }

        /// <summary>
        /// 当前客户的收货人
        /// </summary>
        /// <param name="ClientID">客户ID</param>
        public AddressView(string ClientID)
        {
            this.clientid = ClientID;
        }

        /// <summary>
        /// 查
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Address> GetIQueryable()
        {
            return from address in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>()
                   where address.ClientID == this.clientid
                   select new Address
                   {
                       ID = address.ID,
                       Type = (AddressType)address.Type,
                       ClientID = address.ClientID,
                       Title = address.Title,
                       Contact = address.Contact,
                       ClientAddress = address.Address,
                       Phone = address.Phone,
                       Email = address.Email,
                       Status = (Underly.GeneralStatus)address.Status,
                       CreateDate = address.CreateDate,
                       ModifyDate = address.ModifyDate,
                       IsDefault=address.IsDefault,
                   };
        }
    }
}

using Needs.Wl.Models;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的收件地址
    /// </summary>
    public class ClientConsigneesView : Needs.Wl.Models.Views.ClientConsigneesView
    {
        private string ClientID;

        public ClientConsigneesView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<Needs.Wl.Models.ClientConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientConsignees>()
                   join contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>() on consignee.ContactID equals contact.ID
                   where consignee.Status == (int)Needs.Wl.Models.Enums.Status.Normal && consignee.ClientID == this.ClientID
                   orderby consignee.IsDefault ascending
                   orderby consignee.CreateDate descending
                   select new Needs.Wl.Models.ClientConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name = consignee.Name,
                       Address = consignee.Address,
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
                           Status = (Needs.Wl.Models.Enums.Status)contact.Status,
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
        /// 获取默认地址
        /// </summary>
        /// <returns></returns>
        public Needs.Wl.Models.ClientConsignee GetDefault()
        {
            return this.GetIQueryable().Where(item => item.IsDefault == true).FirstOrDefault();
        }
    }
}
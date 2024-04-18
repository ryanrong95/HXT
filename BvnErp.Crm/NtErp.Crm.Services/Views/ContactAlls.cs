using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Views
{
    public class ContactAlls : UniqueView<Contact, BvCrmReponsitory>, Needs.Underly.IFkoView<Contact>
    {
        //人员对象
        IGenericAdmin Admin;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public ContactAlls()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="admin">人员对象</param>
        public ContactAlls(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bv">数据库实体</param>
        public ContactAlls(BvCrmReponsitory bv) : base(bv) { }

        /// <summary>
        /// 获取联系人数据集合
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Contact> GetIQueryable()
        {
            //客户视图
            ClientAlls ClientView = new ClientAlls(this.Reponsitory);

            return from contact in base.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Contacts>()
                   join client in ClientView
                   on contact.ClientID equals client.ID
                   where client.Status != ActionStatus.Delete && contact.Status != (int)Enums.Status.Delete
                   select new Contact
                   {
                       ID = contact.ID,
                       Name = contact.Name,
                       ClientID = contact.ClientID,
                       Clients = client,
                       Types = (ConsigneeType)contact.Type,
                       CompanyID = contact.CompanyID,
                       Position = contact.Position,
                       Email = contact.Email,
                       Mobile = contact.Moblie,
                       Tel = contact.Tel,
                       Detail = contact.Detail,
                       Status = (Enums.Status)contact.Status,
                       CreateDate = contact.CreateDate,
                       UpdateDate = contact.UpdateDate,
                   };
        }

        /// <summary>
        /// 附件删除
        /// </summary>
        /// <param name="ContactId"></param>
        public void DeleteFiles(string ContactId)
        {
            var Contact = this[ContactId];
            if (Contact != null)
            {
                Reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                {
                    Status = Enums.Status.Delete
                }, item => item.ContactID == Contact.ID);
            }
        }
    }
}

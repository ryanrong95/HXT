using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 联系人
    /// </summary>
    [Serializable]
    public class Contact : Needs.Linq.IUnique, Needs.Linq.IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.CreateDate, this.Email, this.Mobile).MD5();

            }
            set
            {
                this.id = value;
            }
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Tel { get; set; }

        public string Fax { get; set; }

        public string QQ { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        public Contact()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Contacts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        Tel = this.Tel,
                        Fax = this.Fax,
                        QQ = this.QQ,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Contacts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        Tel = this.Tel,
                        Fax = this.Fax,
                        QQ = this.QQ,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    },item=>item.ID==this.id);
                }
            }
        }
    }
}

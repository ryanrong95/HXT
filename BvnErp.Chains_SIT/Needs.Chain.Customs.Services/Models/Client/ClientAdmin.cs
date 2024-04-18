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
    /// 客户的客服 
    /// 业务经理、跟单员
    /// </summary>
    public class ClientAdmin : IUnique
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ClientID, this.Type.GetHashCode(), this.Admin.ID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }

        /// <summary>
        /// 类型：业务经理，跟单员
        /// </summary>
        public Enums.ClientAdminType Type { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string DepartmentID { get; set; }


        public ClientAdmin()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == this.ID);

                //先把记录删除，再添加新记录
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAdmins>(new { Status = (int)Enums.Status.Delete },
                    item => item.ClientID == this.ClientID && item.Type == (int)this.Type);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientAdmins
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        AdminID = this.Admin.ID,
                        ClientID = this.ClientID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientAdmins
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        AdminID = this.Admin.ID,
                        ClientID = this.ClientID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

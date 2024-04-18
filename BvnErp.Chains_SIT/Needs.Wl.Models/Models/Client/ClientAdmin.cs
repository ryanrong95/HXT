using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户的客服 
    /// 业务经理、跟单员
    /// </summary>
    public class ClientAdmin : ModelBase<Layer.Data.Sqls.ScCustoms.Clients, ScCustomsReponsitory>, IUnique
    {
        string id;
        public new string ID
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

        public ClientAdmin()
        {

        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>().Count(item => item.ID == this.ID);

            //先把记录删除，再添加新记录
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAdmins>(new { Status = (int)Enums.Status.Delete },
                item => item.ClientID == this.ClientID && item.Type == (int)this.Type);

            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientAdmins
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
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientAdmins
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


            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
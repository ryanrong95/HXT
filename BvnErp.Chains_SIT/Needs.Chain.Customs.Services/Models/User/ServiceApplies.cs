using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 注册申请
    /// </summary>
    public class ServiceApplies : IUnique, IPersist
    {
        public string ID { get; set; }

        public string Email { get; set; }

        public string CompanyName { get; set; }

        public string Address { get; set; }

        public string Contact { get; set; }

        public string Mobile { get; set; }

        public string Tel { get; set; }

        public Admin Admin { get; set; }

        public Enums.HandleStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ServiceApplies()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.HandleStatus.Pending;
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

        private void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ServiceApplies>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.ServiceApply);
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ServiceApplies
                    {
                        ID = this.ID,
                        Email = this.Email,
                        CompanyName = this.CompanyName,
                        Address = this.Address,
                        Contact = this.Contact,
                        Mobile = this.Mobile,
                        Tel = this.Tel,
                        //AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ServiceApplies
                    {
                        ID = this.ID,
                        Email = this.Email,
                        CompanyName = this.CompanyName,
                        Address = this.Address,
                        Contact = this.Contact,
                        Mobile = this.Mobile,
                        Tel = this.Tel,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                   }, item => item.ID == this.ID);
                }
            }
        }
    }
}

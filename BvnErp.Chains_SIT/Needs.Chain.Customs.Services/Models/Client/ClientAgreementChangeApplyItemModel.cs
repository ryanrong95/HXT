using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientAgreementChangeApplyItemModel : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string ApplyID { get; set; }

        public Enums.Status Status { get; set; }

        public int ChangeType { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }


        public ClientAgreementChangeApplyItemModel()
        {
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.AgreementChangeApplyItems
                {
                    ID = this.ID,
                    ApplyID = this.ApplyID,
                    ChangeType = this.ChangeType,
                    Status = (int)this.Status,
                    OldValue = this.OldValue,
                    NewValue = this.NewValue
                });
            }

            this.OnEnterSuccess();
        }

        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

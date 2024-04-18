using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   public class ProductClassifyLock: IUnique, IPersist, IFulError, IFulSuccess
    {

        public string ID { get; set;}

        public bool IsLocked { get; set; }
        public DateTime LockDate { get; set; }

        public Enums.Status Status { get; set; }

        public Admin Admin { get; set; }

        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ProductClassifyLock()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ProductClassifyLocks
                    {
                        ID = this.ID,
                        IsLocked = true,
                        LockDate = DateTime.Now,
                        AdminID = AdminID,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>(new
                    {
                        ID = this.ID,
                        IsLocked = true,
                        LockDate = DateTime.Now,
                        AdminID = AdminID,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }


    }
}

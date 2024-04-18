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
   ///快递方式
   /// </summary>
    public class ExpressType : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 快递公司
        /// </summary>
        public ExpressCompany ExpressCompany { get; set; }

        /// <summary>
        /// 快递方式名称
        /// </summary>
        public string  TypeName { get; set; }

        /// <summary>
        /// 快递公司ID
        /// </summary>
        public string ExpressCompanyID { get; set; }

        /// <summary>
        /// 快递方式名称
        /// </summary>
        public int TypeValue { get; set; }

        public Enums.Status Status { get; set; }

        public ExpressType()
        {
            this.Status = Enums.Status.Normal;
        }


        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressTypes>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = ChainsGuid.NewGuidUp();
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExpressTypes>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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
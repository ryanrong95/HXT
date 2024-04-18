using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    public class PayExchangeSensitiveArea : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// 类型
        /// </summary>
        public Enums.PayExchangeSensitiveAreaType Type { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        public string Name { get; set; }

        #endregion

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        #region 构造函数

        public PayExchangeSensitiveArea()
        {

        }

        #endregion

        #region 持久化

        override public void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.ID = Guid.NewGuid().ToString("N");
                this.Reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>(new Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas
                {
                    ID = this.ID,
                    Type = (int)this.Type,
                    Name = this.Name,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                });
            }
            else
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>(new
                {
                    ID = this.ID,
                    Type = (int)this.Type,
                    Name = this.Name,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                }, item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        override public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveAreas>(new
                {
                    Status = (int)Enums.Status.Delete,
                    UpdateDate = this.UpdateDate,
                }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #endregion
    }
}

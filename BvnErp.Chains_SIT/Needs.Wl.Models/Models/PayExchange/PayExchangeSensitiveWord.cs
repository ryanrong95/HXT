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
    public class PayExchangeSensitiveWord : ModelBase<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        /// <summary>
        /// AreaID
        /// </summary>
        public string AreaID { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        #endregion

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        #region 构造函数

        public PayExchangeSensitiveWord()
        {

        }

        #endregion

        #region 持久化

        override public void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.ID = Guid.NewGuid().ToString("N");
                this.Reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>(new Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords
                {
                    ID = this.ID,
                    AreaID = this.AreaID,
                    Content = this.Content,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                });
            }
            else
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>(new
                {
                    ID = this.ID,
                    AreaID = this.AreaID,
                    Content = this.Content,
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeSensitiveWords>(new
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

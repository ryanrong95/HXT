using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Layers.Data.Sqls;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Suggestion : Yahv.Linq.IUnique
    {
        #region 属性
        public string ID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public Suggestion()
        {
            this.CreateDate = DateTime.Now;
            this.Status = (int)GeneralStatus.Normal;
        }

        public virtual event SuccessHanlder EnterSuccess;

        #region 持久化
        /// <summary>
        /// 数据插入
        /// </summary>
        public virtual void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Suggestions
                {
                    ID = this.ID,
                    Name = this.Name,
                    Phone = this.Phone,
                    Status = this.Status,
                    CreateDate = this.CreateDate,
                    Summary = this.Summary,
                });
            }
            this.OnEnterSuccess();
        }

        /// <summary>
        /// 成功触发事件
        /// </summary>
        public virtual void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}

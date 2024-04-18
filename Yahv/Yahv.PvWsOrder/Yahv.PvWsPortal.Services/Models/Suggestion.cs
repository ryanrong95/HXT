using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsPortal.Services.Models
{
    public class Suggestion : Yahv.Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 建议
        /// </summary>
        public string Summary { get; set; }
        #endregion

        public Suggestion()
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.Status = (int)GeneralStatus.Normal;
        }

        public virtual event SuccessHanlder EnterSuccess;

        #region 持久化
        /// <summary>
        /// 数据插入
        /// </summary>
        public virtual void Enter()
        {
            using (Layers.Data.Sqls.ScCustomReponsitory reponsitory = new Layers.Data.Sqls.ScCustomReponsitory())
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.Suggestions
                {
                    ID = this.ID,
                    Name = this.Name,
                    Phone = this.Phone,
                    City = this.Place,
                    Status = this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
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

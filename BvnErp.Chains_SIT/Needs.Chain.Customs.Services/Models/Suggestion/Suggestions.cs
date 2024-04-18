using Needs.Ccs.Services.Hanlders;
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
    /// 投诉与建议
    /// </summary>
    public class Suggestions : IUnique, IPersist
    {
        public string ID { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public Suggestions()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ID= ChainsGuid.NewGuidUp();
                reponsitory.Insert(this.ToLinq());
            }

            this.OnEnter();
        }

    }
}

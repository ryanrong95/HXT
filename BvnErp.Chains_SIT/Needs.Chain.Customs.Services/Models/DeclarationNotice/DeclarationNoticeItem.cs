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
    /// 报关通知项
    /// </summary>
    [Serializable]
    public class DeclarationNoticeItem : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }

        public string DeclarationNoticeID { get; set; }

        /// <summary>
        /// 到货信息以及装箱信息
        /// </summary>
        public Sorting Sorting { get; set; }

        /// <summary>
        /// 报关通知项状态：未制单、已制单
        /// </summary>
        public Enums.DeclareNoticeItemStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public DeclarationNoticeItem()
        {
            this.Status = Enums.DeclareNoticeItemStatus.UnMake;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>().Count(item => item.ID == this.ID);

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
    }
}

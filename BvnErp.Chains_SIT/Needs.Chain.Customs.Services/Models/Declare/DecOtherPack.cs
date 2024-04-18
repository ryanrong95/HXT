using Needs.Ccs.Services.Enums;
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
    /// 报关单-其它包装
    /// </summary>
    [Serializable]
    public class DecOtherPack :IUnique,IPersist
    {
        /// <summary>
        /// 主键ID（DeclarationID+包装种类）
        /// </summary>       
        public string ID { get; set; }       

        /// <summary>
        /// 报关单ID
        /// </summary>
        public string DeclarationID { get; set; }

        /// <summary>
        /// 包装件数
        /// </summary>
        public decimal PackQty { get; set; }

        /// <summary>
        /// 包装材料种类
        /// </summary>
        public string PackType { get; set; }

       
        public DecOtherPack()
        {
            //TODO：设置默认值
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecOtherPacks>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
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

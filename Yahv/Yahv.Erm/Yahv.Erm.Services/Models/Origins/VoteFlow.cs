using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 审批流
    /// </summary>
    public class VoteFlow : IUnique
    {
        #region 属性

        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationType Type{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ModifyID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ModifyDate { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 请假天数上限
        /// </summary>
        public decimal? UpperLimit { get; set; }

        /// <summary>
        /// 请假天数下限
        /// </summary>
        public decimal? LowerLimit { get; set; }

        #endregion

        #region 构造器

        internal VoteFlow()
        {
        }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        #endregion

        #region 持久化

        //public void Enter()
        //{
        //    using (var repository = new PvbErmReponsitory())
        //    {
        //        if (!repository.GetTable<Layers.Data.Sqls.PvbErm.VoteFlows>().Any(item => item.ID == this.ID))
        //        {
        //            this.ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.VoteFlow);
        //            repository.Insert(new Layers.Data.Sqls.PvbErm.VoteFlows
        //            {
        //                ID = this.ID,
        //                Name = this.Name,
        //                Type = (int)this.Type,
        //                CreatorID = this.CreatorID,
        //                ModifyID = this.ModifyID,
        //                CreateDate = DateTime.Now,
        //                ModifyDate = DateTime.Now
        //            });
        //        }
        //        else
        //        {
        //            repository.Update<Layers.Data.Sqls.PvbErm.VoteFlows>(new
        //            {
        //                Name = this.Name,
        //                Type = (int)this.Type,
        //                ModifyID = this.ModifyID,
        //                ModifyDate = DateTime.Now
        //            }, item => item.ID == this.ID);
        //        }
        //        if (this != null && this.EnterSuccess != null)
        //        {
        //            this.EnterSuccess(this, new SuccessEventArgs(this));
        //        }
        //    }
        //}

        #endregion
    }
}

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
    /// 审批步骤
    /// </summary>
    public class VoteStep : IUnique
    {
        #region 属性

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 所属审批流
        /// </summary>
        public string VoteFlowID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderIndex { get; set; }

        /// <summary>
        /// 固定审批人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string PositionID { get; set; }

        /// <summary>
        /// 审批页面
        /// </summary>
        public string Uri { get; set; }

        #endregion

        #region 构造器

        internal VoteStep()
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
        //        if (!repository.GetTable<Layers.Data.Sqls.PvbErm.VoteSteps>().Any(item => item.ID == this.ID))
        //        {
        //            this.ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.VoteStep);
        //            repository.Insert(new Layers.Data.Sqls.PvbErm.VoteSteps
        //            {
        //                ID = this.ID,
        //                Name = this.Name,
        //                VoteFlowID = this.VoteFlowID,
        //                CreateDate = DateTime.Now,
        //                OrderIndex = this.OrderIndex,
        //                AdminID = this.AdminID,
        //                PositionID = this.PositionID,
        //                Uri = this.Uri
        //            });
        //        }
        //        else
        //        {
        //            repository.Update<Layers.Data.Sqls.PvbErm.VoteSteps>(new
        //            {
        //                Name = this.Name,
        //                VoteFlowID = this.VoteFlowID,
        //                OrderIndex = this.OrderIndex,
        //                AdminID = this.AdminID,
        //                PositionID = this.PositionID,
        //                Uri = this.Uri
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

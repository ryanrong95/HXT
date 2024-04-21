using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins.Rolls
{
    public class ProtectApply : ApplyTask
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string ApplyerName { set; get; }
        #endregion

        #region 事件
        public override event SuccessHanlder EnterSuccess;
        #endregion
        public void Approve(string approveid)
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.ApplyTasks>(new
                    {
                        ApproverID = approveid,
                        ApproveDate = DateTime.Now,
                        Status = (int)this.Status
                    }, item => item.ID == this.ID);
                    if (this.Status == Underly.ApplyStatus.Allowed)
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.Clients>(new
                        {
                            Owner = this.ApplierID
                        }, item => item.ID == this.MainID);
                    }
                }
                tran.Commit();
            }
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));

        }



    }
}

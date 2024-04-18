using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 运单
    /// </summary>
    public class WayCost : Yahv.Services.Models.WayCost
    {
        public WayCost()
        {
            this.CreateDate =  DateTime.Now;
            this.EnterSuccess += Waybill_EnterSuccess;
        }

        #region 事件

        public virtual event SuccessHanlder EnterSuccess;

        private void Waybill_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var waybill = (WayCost)e.Object;
        }
        
        #endregion

        #region 持久化

        public virtual void Enter()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                //保存运单费用
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayCosts>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Underly.PKeyType.WayCost);
                    Reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }

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

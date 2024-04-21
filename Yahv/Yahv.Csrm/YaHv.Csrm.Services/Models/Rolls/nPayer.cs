using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Rolls
{
    public class nPayer : Models.Origins.nPayer
    {

        public event ErrorHanlder Repeat;
        
        #region 持久化
        override public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {

                var payers = new Views.Rolls.nPayersRoll(this.nSupplierID).ToArray();
                //收款人是否已存在，唯一性判断：供应商ID，支付方式，币种，银行账号
                var exsitor = payers.FirstOrDefault(item => item.EnterpriseID == this.EnterpriseID
                  && item.RealID == this.RealID
                  && item.nSupplierID == this.nSupplierID
                  && item.Account == this.Account
                  && item.Methord == this.Methord
                  && item.Currency == this.Currency);
                if (string.IsNullOrEmpty(this.ID))
                {
                    if (exsitor != null)
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        this.ID = PKeySigner.Pick(PKeyType.nPayee);
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    if (exsitor.ID == this.ID)
                    {
                        repository.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                    else
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                        }
                    }
                }
                this.Fire(new SuccessEventArgs(this));
            }
        }
        override public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update(new Layers.Data.Sqls.PvbCrm.nPayees
                {
                    Status = (int)GeneralStatus.Deleted
                }, item => item.ID == this.ID);
                this.Fire(new AbandonedEventArgs(this));
            }

        }

        #endregion
    }
}

using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    /// <summary>
    /// 佣金，供应商返款
    /// </summary>
    public class Commission : Yahv.Linq.IUnique
    {
        public Commission()
        {
            this.Status = DataStatus.Normal;
        }
        #region 属性
        public string ID { set; get; }
        public string SupplierID { set; get; }
        /// <summary>
        /// 我方公司ID
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 方式：返点，折扣
        /// </summary>

        public CommissionType Type { set; get; }
        /// <summary>
        /// 返款类型
        /// </summary>
        public CommissionMethod Methord { set; get; }
        /// <summary>
        /// 返款比例
        /// </summary>
        public decimal? Radio { set; get; }
        /// <summary>
        /// 金额最小值
        /// </summary>
        public decimal Msp { set; get; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public DataStatus Status { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        public int? Months { set; get; }
        public int? Days { set; get; }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.Commission);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.Commissions
                    {
                        ID = this.ID,
                        SupplierID = this.SupplierID,
                        CompanyID = this.CompanyID,
                        Type = (int)this.Type,
                        Methord = (int)this.Methord,
                        Currency=(int)this.Currency,
                        Ratio=this.Radio,
                        Msp=this.Msp,
                        Status=(int)this.Status,
                        CreatorID=this.CreatorID,
                        Months=this.Months,
                        Days=this.Days
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.Commissions>(new
                    {
                        CompanyID = this.CompanyID,
                        Type = (int)this.Type,
                        Methord = (int)this.Methord,
                        Currency = (int)this.Currency,
                        Ratio = this.Radio,
                        Msp = this.Msp,
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        Months = this.Months,
                        Days = this.Days
                    },item=>item.ID==this.ID);
                }
                this.EnterError?.Invoke(this, new ErrorEventArgs());
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvdCrm.Commissions>(new
                {
                    Status = (int)DataStatus.Closed,
                }, item => item.ID == this.ID);
                this.AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}

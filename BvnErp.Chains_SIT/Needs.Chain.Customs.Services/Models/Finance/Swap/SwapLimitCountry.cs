using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SwapLimitCountry : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// 银行名
        /// </summary>
        public string BankName { get; set; }

        internal Admin Admin { get; set; }

        /// <summary>
        /// 国家地区名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        public string Code { get; set; }

        
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public void SetAdmin(Admin admin) {
            this.Admin = admin;
        }

        #endregion
        public SwapLimitCountry()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.EnterSuccess += Limit_EnterSuccess;
            this.AbandonSuccess+= Limit_AbandonSuccess;
        }

        private void Limit_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var limit = (SwapLimitCountry)e.Object;
            limit.Log(limit.Admin, "管理员[" + limit.Admin.RealName + "]删除了受限国家地区[" + limit.Name + "]。");
        }

        private void Limit_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var limit = (SwapLimitCountry)e.Object;
            limit.Log(limit.Admin, "管理员[" + limit.Admin.RealName + "]新增了受限国家地区[" + limit.Name + "]。");
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 日志
        /// </summary>
        private SwapLimitCountryLogs logs;

        public SwapLimitCountryLogs Logs
        {
            get
            {
                if (logs == null)
                {
                    using (var view = new Views.SwapLimitCountryLogsView())
                    {
                        var query = view.Where(item => item.BankID == this.BankID);
                        this.Logs = new SwapLimitCountryLogs(query);
                    }
                }
                return this.logs;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.logs = new SwapLimitCountryLogs(value, new Action<SwapLimitCountryLog>(delegate (SwapLimitCountryLog item)
                {
                    item.BankID = this.BankID;
                }));
            }
        }


        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapLimitCountries>().Count(item => item.ID == this.ID);
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
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.SwapLimitCountries>(
                        new
                        {
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }



    }
}

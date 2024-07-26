using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 华芯通管理员的角色
    /// </summary>
    public class AdminRoles : IUnique, IPersist
    {
        public string ID { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public Admin Admin { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }

        public AdminRoles()
        {
            this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;


        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminRoles>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //主键ID（FinanceVault +8位年月日+6位流水号）
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.AdminRole);
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
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 管理员（客服）
    /// </summary>
    [Needs.Underly.FactoryView(typeof(Views.AdminsTopView))]
    [Serializable]
    public class Admin : IUnique, IPersist
    {
        public string ID { get; set; }

        /// <summary>
        /// 用户名/账号/登录名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string DepartmentID { get; set; }

        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LoginDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int Status { get; set; }

        public string Summary { get; set; }

        public string OriginID { get; set; }

        public string ErmAdminID { get; set; }

        #region

        /// <summary>
        /// 
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 大赢家公司编号
        /// </summary>
        public string DyjCompanyCode { get; set; }

        /// <summary>
        /// 大赢家公司
        /// </summary>
        public string DyjCompany { get; set; }

        /// <summary>
        /// 大赢家部门编号
        /// </summary>
        public string DyjDepartmentCode { get; set; }

        /// <summary>
        /// 大赢家部门
        /// </summary>
        public string DyjDepartment { get; set; }

        /// <summary>
        ///系统用户别名
        /// </summary>
        public string ByName { get; set; }

        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public Admin()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                bool exsit = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>().
                    Any(item => item.AdminID == this.ID);
                if (exsit)
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.AdminWl
                    {
                        AdminID = this.ID,
                        Tel = this.Tel,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        DepartmentID = this.DepartmentID,
                        UpdateDate = DateTime.Now,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary
                    }, item => item.AdminID == this.ID);
                }
                else
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.AdminWl
                    {
                        AdminID = this.ID,
                        Tel = this.Tel,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        DepartmentID = this.DepartmentID,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                        CreateDate = DateTime.Now
                    });
                }

                this.OnEnter();
            }
        }
    }
}

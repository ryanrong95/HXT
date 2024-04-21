using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.VcCsrm.Service.Extends;

namespace YaHv.VcCsrm.Service.Models
{
    public class Enterprise : Yahv.Linq.IUnique
    {
        public Enterprise()
        {
            this.Status = ApprovalStatus.Normal;
        }
        #region 属性
        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID
        {
            get
            {
                return this.id ?? this.Name.MD5();

            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 管理编码
        /// </summary>
        /// <chenhan>保障局部唯一</chenhan>
        public string AdminCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        ///// <summary>
        ///// 地域、地区
        ///// </summary>
        public string District { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 企业法人
        /// </summary>

        public string Corporation { set; get; }
        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion


        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                if (repository.GetTable<Layers.Data.Sqls.PvcCrm.Enterprises>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvcCrm.Enterprises>(new
                    {
                        AdminCode = this.AdminCode,
                        District = this.District,
                        Status = this.Status,
                        RegAddress = this.RegAddress,
                        Corporation = this.Corporation,
                        Uscc = this.Uscc
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(this.ToLinq());
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }

            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvcCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvcCrm.Enterprises>(new
                {
                    Status = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }
        #endregion

    }
}

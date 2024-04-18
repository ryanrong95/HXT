using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.MyNoticeView))]
    public class Notice : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get;set;
        }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// 公告内容
        /// </summary>
        public string Context
        {
            get;set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public WarningStatus Status
        {
            get;set;
        }

        /// <summary>
        /// 发布人
        /// </summary>
        public AdminTop Admin
        {
            get;set;
        }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime CreateDate
        {
            get;set;
        }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        #region 持久化
        /// <summary>
        /// 数据删除触发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.MapsNotice>(item => item.NoticeID == this.ID);
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.Files>(item => item.NoticeID == this.ID);
            }
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.Notices>(item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 数据保存触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判断是否为新增
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Notices
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Context = this.Context,
                        CreateDate = this.CreateDate,
                        AdminID = this.Admin.ID,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Notices
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Context = this.Context,
                        CreateDate = this.CreateDate,
                        AdminID = this.Admin?.ID,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}

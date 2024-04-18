using Needs.Erp.Generic;
using Needs.Linq;
using System;
using NtErp.Crm.Services.Extends;
using Needs.Underly;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Collections.Generic;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Converters;
using Needs.Overall;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.ClientAlls))]
    public partial class Client : Needs.Underly.Document, IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ID
        {
            get
            {
                return this[nameof(ID)] as string;
            }
            set
            {
                this[nameof(ID)] = value;
            }
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name
        {
            get
            {
                return this[nameof(Name)] as string;
            }
            set
            {
                this[nameof(Name)] = value;
            }
        }
        /// <summary>
        /// 是否保护
        /// </summary>
        public IsProtected IsSafe
        {
            get
            {
                return this[nameof(IsSafe)];
            }
            set
            {
                this[nameof(IsSafe)] = value;
            }
        }
        /// <summary>
        /// 客户状态
        /// </summary>
        public ActionStatus Status
        {
            get
            {
                return this[nameof(Status)];
            }
            set
            {
                this[nameof(Status)] = value;
            }
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get
            {
                return this[nameof(CreateDate)];
            }
            set
            {
                this[nameof(CreateDate)] = value;
            }
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get
            {
                return this[nameof(UpdateDate)];
            }
            set
            {
                this[nameof(UpdateDate)] = value;
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get
            {
                return this[nameof(Summary)] as string;
            }
            set
            {
                this[nameof(Summary)] = value;
            }
        }
        /// <summary>
        /// 管理员编码
        /// </summary>
        public string AdminCode
        {
            get
            {
                return this[nameof(AdminCode)] as string;
            }
            set
            {
                this[nameof(AdminCode)] = value;
            }
        }
        /// <summary>
        /// 社会信用统一代码
        /// </summary>
        public string CUSCC
        {
            get
            {
                return this[nameof(CUSCC)] as string;
            }
            set
            {
                this[nameof(CUSCC)] = value;
            }
        }
        /// <summary>
        /// 区域ID
        /// </summary>
        public string AreaID
        {
            get
            {
                return this[nameof(AreaID)] as string;
            }
            set
            {
                this[nameof(AreaID)] = value;
            }
        }
        /// <summary>
        /// 主要产品
        /// </summary>
        public string IndustryInvolved
        {
            get
            {
                return this[nameof(IndustryInvolved)] as string;
            }
            set
            {
                this[nameof(IndustryInvolved)] = value;
            }
        }

        /// <summary>
        /// 其他客户属性
        /// </summary>
        public string NTextString
        {
            get
            {
                return this[nameof(NTextString)];
            }
            set
            {
                this[nameof(NTextString)] = value;
            }
        }

        #endregion

        public Client()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = ActionStatus.Auditing;
        }

        public event ErrorHanlder AbandonError;
        public event ErrorHanlder Renaming;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        /// <summary>
        /// 数据插入
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

        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>().Count(item => item.Name == this.Name &&
                    item.Status != (int)ActionStatus.Delete && item.Status != (int)ActionStatus.Reject);
                    if (count > 0)
                    {
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.Renaming(this, new ErrorEventArgs("公司名称不能重复！"));
                            return;
                        }
                    }
                    this.ID = PKeySigner.Pick(PKeyType.Client);
                    reponsitory.Insert(this.ToLinq());
                    reponsitory.Insert(this.ToShower());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    reponsitory.Update(this.ToShower(), item => item.ClientID == this.ID);
                }
            }
        }

        /// <summary>
        /// 删除
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
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Clients>(new
                {
                    Status = ActionStatus.Delete
                }, item => item.ID == this.ID);
            }
        }
        public void SingleUpdateEnter()
        {
            this.OnSingleUpdateEnter();
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        public void OnSingleUpdateEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Clients>().Count(item => item.ID == this.ID);

                if (count == 1)
                {
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.Clients>(new
                    {
                        IsSafe = Convert.ToBoolean(this.IsSafe),
                        Status = this.Status,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
            }
        }
    }

}

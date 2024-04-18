using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Overall;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;
using NtErp.Crm.Services.Models.Generic;
using NtErp.Crm.Services.Views.Generic;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.ProjectAlls))]
    public partial class Project : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public ProjectType Type
        {
            get; set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 产品全称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public string ClientID
        {
            get; set;
        }
        /// <summary>
        /// 公司
        /// </summary>
        public string CompanyID
        {
            get; set;
        }
        /// <summary>
        /// 货币
        /// </summary>
        public CurrencyType Currency
        {
            get; set;
        }
        /// <summary>
        /// 项目估值
        /// </summary>
        public decimal? Valuation
        {
            get; set;
        }

        /// <summary>
        /// 人员对象
        /// </summary>
        public string AdminID
        {
            get; set;
        }
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate
        {
            get; set;
        }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate
        {
            get; set;
        }

        /// <summary>
        /// 原型日期
        /// </summary>
        public DateTime? ModelDate { get; set; }

        /// <summary>
        /// 量产日期
        /// </summary>
        public DateTime? ProductDate { get; set; }

        /// <summary>
        /// 月产量
        /// </summary>
        public int? MonthYield { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contactor { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }
        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }
        /// <summary>
        /// 行动状态
        /// </summary>
        public ActionStatus Status
        {
            get; set;
        }
        /// <summary>
        /// 项目摘要描述
        /// </summary>
        public string Summary
        {
            get; set;
        }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 所属行业
        /// </summary>
        public Industry Industry
        {
            get; set;
        }


        public string AdminName
        {
            get
            {
                return AdminExtends.GetTop(AdminID).RealName;
            }
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName
        {
            get
            {
                return new Views.ClientAlls()[this.ClientID].Name;
            }
        }


        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName
        {
            get
            {
                return new Views.CompanyAlls()[this.CompanyID].Name;
            }
        }

        decimal expectTotal;
        /// <summary>
        /// 预计成交总额
        /// </summary>
        public decimal ExpectTotal
        {
            get
            {
                if (this.expectTotal == 0)
                {
                    using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
                    {
                        var linqs = from map in reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsProject>()
                                    join item in reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.ProductItems>() on map.ProductItemID equals item.ID
                                    where map.ProjectID == this.ID
                                    select new
                                    {
                                        ExpectTotal = item.ExpectTotal.GetValueOrDefault(0)
                                    };
                        this.expectTotal = linqs.ToArray().Sum(item => item.ExpectTotal);
                    }
                }
                return this.expectTotal;
            }
            set
            {
                this.expectTotal = value;
            }
        }

        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public Project()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ActionStatus.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;

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
        /// 数据逻辑删除
        /// </summary>
        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Projects>(new
                {
                    Status = ActionStatus.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 保存数据触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 数据保存数据库
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Project);
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Projects
                    {
                        ID = this.ID,
                        Name = this.Name,
                        ProductName = this.ProductName,
                        Type = (int)this.Type,
                        ClientID = this.ClientID,
                        CompanyID = this.CompanyID,
                        Valuation = this.Valuation.GetValueOrDefault(),
                        Currency = (int)this.Currency,
                        AdminID = this.AdminID,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        ModelDate = this.ModelDate,
                        ProductDate = this.ProductDate,
                        MonthYield = this.MonthYield,
                        Contactor = this.Contactor,
                        Phone = this.Phone,
                        Address = this.Address,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        ExpectTotal = this.ExpectTotal
                    });
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.Projects>(new
                    {
                        Name = this.Name,
                        ProductName = this.ProductName,
                        Type = (int)this.Type,
                        ClientID = this.ClientID,
                        CompanyID = this.CompanyID,
                        Valuation = this.Valuation.GetValueOrDefault(),
                        Currency = (int)this.Currency,
                        AdminID = this.AdminID,
                        StartDate = this.StartDate,
                        EndDate = this.EndDate,
                        ModelDate = this.ModelDate,
                        ProductDate = this.ProductDate,
                        MonthYield = this.MonthYield,
                        Contactor = this.Contactor,
                        Phone = this.Phone,
                        Address = this.Address,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Status = (int)this.Status,
                        Summary = this.Summary,
                        ExpectTotal = this.ExpectTotal
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}

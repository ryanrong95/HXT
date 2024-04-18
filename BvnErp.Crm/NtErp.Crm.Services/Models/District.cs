using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.DistrictAlls))]
    public partial class District : IUnique
    {
        #region 属性
        string id;
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 父类节点ID
        /// </summary>
        public string FatherID
        {
            get;set;
        }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// 区域级别
        /// </summary>
        public int? Level
        {
            get;set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get;set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get;set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get;set;
        }
        #endregion

        public event SuccessHanlder EnterSuccess;

        #region 持久化

        /// <summary>
        /// 保存数据触发事件
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
        /// 数据保存
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定是否为新增数据
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Districts>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Districts
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        Name = this.Name,
                        Level = this.Level,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)Status.Normal,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Districts
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        Name = this.Name,
                        Level = this.Level,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}

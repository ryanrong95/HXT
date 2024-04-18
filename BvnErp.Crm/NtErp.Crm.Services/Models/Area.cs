using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Utils.Converters;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.AreaAlls))]
    public class Area : IUnique, IPersist
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
        /// 父节点ID
        /// </summary>
        public string FatherID
        {
            get;set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
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
        #endregion

        //public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;


        #region  持久化
        /// <summary>
        /// 持久化触发方法
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
        /// 保存
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定数据是否为新增
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = string.Concat(this.FatherID, this.Name).MD5();
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Areas
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        Name = this.Name,
                        CreateDate = DateTime.Now,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Areas
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        Name = this.Name,
                        CreateDate = this.CreateDate,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}

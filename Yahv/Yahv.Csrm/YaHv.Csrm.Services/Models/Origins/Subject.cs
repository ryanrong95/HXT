using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbCrm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Views.Rolls;

namespace YaHv.Csrm.Services.Models.Origins
{
    /// <summary>
    /// 财务科目
    /// </summary>
    public class _Subject : IUnique
    {
        #region 属性

        public string ID { get; set; }
        public string FatherID { get; set; }
        public SubjectType Type { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public SubjectSubs Subs { get { return new SubjectSubs(this); } }
        #endregion

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            //using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            //{
            //    //财务科目是否已存在
            //    if (!repository.ReadTable<Layers.Data.Sqls.PvbCrm._Subjects>().Any(item => item.ID == this.ID))
            //    {
            //        repository.Insert(new _Subjects()
            //        {
            //            Status = (int)Status.Normal,
            //            FatherID = this.FatherID,
            //            Name = this.Name,
            //            Type = (int)this.Type,
            //            ID = PKeySigner.Pick(PKeyType.Subject),
            //        });
            //    }
            //    else
            //    {
            //        repository.Update<_Subjects>(new
            //        {
            //            Name = this.Name,
            //            Type = (int)this.Type,
            //            Status = (int)this.Status
            //        }, item => item.ID == this.ID);
            //    }
            //}
        }

        /// <summary>
        /// 假删除
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Subjects>(new
                {
                    Status = (int)Status.Deleted
                }, item => item.ID == this.ID);
            }
        }
        #endregion
    }

    /// <summary>
    /// 科目管理
    /// </summary>
    public class Subject : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 科目类型（应收、应付）
        /// </summary>
        public Yahv.Underly.SubjectType Type { get; set; }

        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// 科目名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 是否需要录入个数
        /// </summary>
        public bool IsCount { get; set; }

        /// <summary>
        /// 是否需要流转给客户
        /// </summary>
        public bool IsToCustomer { get; set; }

        /// <summary>
        /// json格式存储 后续选择步骤。如果本次段不为null 就表示有后续步骤
        /// </summary>
        public string Steps { get; set; }
        #endregion
    }
}

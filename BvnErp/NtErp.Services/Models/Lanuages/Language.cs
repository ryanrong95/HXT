using Needs.Linq;
using NtErp.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 语言
    /// </summary>
    public class Language :  IPersistence, IUnique
    {
        public string ID
        {
            get
            {
                return this.ShortName;
            }
            set
            {
                this.ShortName = value;
            }
        }
        /// <summary>
        /// 短名称
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 英文名称（国际名称）
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataName { get; set; }

        #region 持久化

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public void Enter()
        {
            using (var reponsitory = new Layer.Data.Sqls.BvOverallsReponsitory())
            {
                Expression<Func<Layer.Data.Sqls.BvOveralls.Languages, bool>> predicate = item => item.ID == this.ID;
                var entity = reponsitory.GetTable<Layer.Data.Sqls.BvOveralls.Languages>().SingleOrDefault(predicate);
                if (entity == null)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), predicate);
                }
            }
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Abandon()
        {
            using (var repository = new Layer.Data.Sqls.BvOverallsReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvOveralls.Languages>(item => item.ID == this.ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #endregion 

    }

}

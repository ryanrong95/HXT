using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 海关申报要素默认值
    /// </summary>
    public class ElementsDefault : IUnique
    {
        #region 属性

        string id;
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID
        {
            get
            {
                //编码规则：税则ID+申报要素名称的MD5
                return this.id ?? string.Concat(this.TariffID, this.ElementName).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 海关税则ID
        /// </summary>
        public string TariffID { get; set; }

        /// <summary>
        /// 申报要素名称
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder DeleteSuccess;

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加
                if (!repository.ReadTable<Layers.Data.Sqls.PvData.ElementsDefaults>().Any(t => t.ID == this.ID))
                {
                    repository.Insert(new Layers.Data.Sqls.PvData.ElementsDefaults()
                    {
                        ID = this.ID,
                        TariffID = this.TariffID,
                        ElementName = this.ElementName,
                        DefaultValue = this.DefaultValue,
                        CreateDate = DateTime.Now
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvData.ElementsDefaults>(new
                    {
                        DefaultValue = this.DefaultValue
                    }, a => a.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Delete()
        {
            using (var repository = LinqFactory<PvDataReponsitory>.Create())
            {
                repository.Delete<Layers.Data.Sqls.PvData.ElementsDefaults>(item => item.ID == this.ID);
            }

            if (this != null && this.DeleteSuccess != null)
            {
                this.DeleteSuccess(this, new SuccessEventArgs(this));
            }
        }

        #endregion
    }
}

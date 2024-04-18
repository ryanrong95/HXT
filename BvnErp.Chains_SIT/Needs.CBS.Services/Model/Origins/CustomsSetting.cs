using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Model.Origins
{
    /// <summary>
    /// 海关基础数据配置
    /// </summary>
    class CustomsSetting : IUnique
    {
        #region 属性
        private string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Type).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public Enums.BaseType Type { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public string Summary { get; set; }

        #endregion

        #region 事件
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsSettings>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.Customs.CustomsSettings
                    {
                        ID = this.ID,
                        Code = this.Code,
                        EnglishName = this.EnglishName,
                        Name = this.Name,
                        Summary=this.Summary,
                        Type=(int)this.Type,
                    });
                }

                this.OnEnter();
            }
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        #endregion

    }
}

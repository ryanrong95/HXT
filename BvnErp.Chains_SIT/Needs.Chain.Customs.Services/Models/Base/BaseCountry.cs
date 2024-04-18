using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 国家与地区
    /// </summary>
    public class Country : IUnique
    {
        private string id;

        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string Code { get; set; }

        public string Name { get; set; }

        public string EnglishName { get; set; }

        public string EditionOneCode { get; set; }

        /// <summary>
        ///区域类型（用于付汇）
        /// </summary>
        public RegionalType Type { get; set; }

        public int? TypeInt { get; set; }

        /// <summary>
        /// 优普税率
        /// </summary>
        public string Preferential { get; set; }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCountries>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.BaseCountries
                    {
                        ID = this.ID,
                        Code = this.Code,
                        EnglishName = this.EnglishName,
                        Name = this.Name,
                        Type=(int)this.Type
                    
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
    }
}

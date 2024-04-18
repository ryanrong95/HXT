using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 申报要素默认值
    /// </summary>
    public class ElementsDefault : IUnique, IPersist
    {
        public string ID { get; set; }

        public string CustomsTariffID { get; set; }

        public string ElementName { get; set; }

        public string DefaultValue { get; set; }

        public DateTime CreateDate { get; set; }

        public ElementsDefault()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsElementsDefaults>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.CustomsElementsDefaults
                    {
                        ID = this.ID,
                        CustomsTariffID = this.CustomsTariffID,
                        ElementName = this.ElementName,
                        DefaultValue = this.DefaultValue,
                        CreateDate = this.CreateDate,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.CustomsElementsDefaults
                    {
                        ID = this.ID,
                        CustomsTariffID = this.CustomsTariffID,
                        ElementName = this.ElementName,
                        DefaultValue = this.DefaultValue,
                        CreateDate = this.CreateDate,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.CustomsElementsDefaults>(item => item.ID == this.ID);
            }
        }
    }
}
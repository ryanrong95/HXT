using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Linq;
using Yahv.Utils.Serializers;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 税率管理
    /// </summary>
    public class TaxRate : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// json名称
        /// </summary>
        public string JsonName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 枚举值
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifierID { get; set; }
        #endregion

        #region 拓展属性
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreatorName { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.TaxRates>().Any(item => item.ID == this.ID))
                {
                    this.ID = this.ID ?? PKeySigner.Pick(PKeyType.TaxRates);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.TaxRates()
                    {
                        ID = this.ID,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        Name = this.Name,
                        Code = this.Code,
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                        Rate = this.Rate,
                        JsonName = this.JsonName,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.TaxRates>(new
                    {
                        ModifierID = this.ModifierID,
                        ModifyDate = DateTime.Now,
                        Rate = this.Rate,
                        JsonName = this.JsonName,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
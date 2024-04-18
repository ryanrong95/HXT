using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace YaHv.PvData.Services.Views.Alls
{
    /// <summary>
    /// 税务归类历史记录
    /// </summary>
    public class Logs_ClassifiedTaxView : UniqueView<Models.Log_ClassifiedTax, PvDataReponsitory>
    {
        string Name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">报关品名</param>
        public Logs_ClassifiedTaxView(string name)
        {
            this.Name = name.Trim();
        }

        protected override IQueryable<Models.Log_ClassifiedTax> GetIQueryable()
        {
            //1.查询产品税务归类历史记录
            var logs_ClassifiedPartNumber = new Origins.Logs_ClassifiedPartNumberOrigin(this.Reponsitory)
                                            .Where(cpn => cpn.Name.Contains(this.Name) && cpn.TaxCode != null && cpn.TaxName != null);
            if (logs_ClassifiedPartNumber.Any())
            {
                return logs_ClassifiedPartNumber.Select(item => new Models.Log_ClassifiedTax
                {
                    ID = item.ID,
                    Name = item.Name,
                    TaxCode = item.TaxCode,
                    TaxName = item.TaxName,
                    CreateDate = item.CreateDate,
                    //OrderIndex = item.Name == this.Name ? 1 : 2
                });
            }

            //2.如果没有产品税务归类历史记录，则查询税务分类基础信息
            var taxRules = new Origins.TaxRulesOrigin(this.Reponsitory).Where(tr => tr.TaxSecondCategory.Contains(this.Name));
            return taxRules.Select(item => new Models.Log_ClassifiedTax
            {
                ID = item.ID,
                Name = item.TaxSecondCategory,
                TaxCode = item.TaxCode,
                TaxName = item.TaxSecondCategory,
                CreateDate = item.CreateDate,
                //OrderIndex = item.TaxSecondCategory == this.Name ? 1 : 2
            });
        }
    }
}

using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InsidePreProduct : IcgooPreProduct
    {
        public string HSCode { get; set; }
        public string Elements { get; set; }
        public string TaxName { get; set; }
        public string TaxCode { get; set; }

        public event GetPreInsideSuccessHanlders GetInsideSuccess;
        public InsidePreProduct()
        {
            this.ClientID = System.Configuration.ConfigurationManager.AppSettings["InsideClientID"];
            this.GetInsideSuccess += ClassifyDefault;
        }

        public void PreEnter()
        {
            this.Enter();
            this.OnInsideGetSuccess(new GetPreInsideSuccessEventArgs(this));
        }

        /// <summary>
        /// Icgoo型号获取成功调用事件-生成待归类记录
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnInsideGetSuccess(GetPreInsideSuccessEventArgs args)
        {
            this.GetInsideSuccess?.Invoke(this, args);
        }

        private void ClassifyDefault(object sender, GetPreInsideSuccessEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var category = new Layer.Data.Sqls.ScCustoms.PreProductCategories();
                category.PreProductID = e.insidePreProduct.ID;
                category.Model = e.insidePreProduct.partno;
                category.Manufacture = e.insidePreProduct.mfr;
                category.UpdateDate = category.CreateDate = DateTime.Now;
                category.Status = (int)Enums.Status.Normal;
                category.ClassifyStatus = (int)Enums.ClassifyStatus.First;
                category.HSCode = e.insidePreProduct.HSCode;
                category.TaxCode = e.insidePreProduct.TaxCode;
                category.TaxName = e.insidePreProduct.TaxName;
                category.Elements = e.insidePreProduct.Elements;

                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>().Count(item => item.Model == e.insidePreProduct.partno);
                if (count != 0)
                {
                    var defaultResult = new Needs.Ccs.Services.Views.DefaultClassifyResultView().Where(item => item.Model == e.insidePreProduct.partno).FirstOrDefault();
                    if (defaultResult != null)
                    {
                        category.ProductName = defaultResult.ProductName;                        
                        category.TariffRate = defaultResult.TariffRate;
                        category.AddedValueRate = defaultResult.AddedValueRate;                      
                        category.ClassifyType = defaultResult.Type == null ? (int)ItemCategoryType.Normal : (int)defaultResult.Type;
                        category.InspectionFee = defaultResult.InspectionFee;
                        category.Unit1 = defaultResult.Unit1;
                        category.Unit2 = defaultResult.Unit2;
                        category.CIQCode = defaultResult.CIQCode;                        
                        category.ClassifyFirstOperator = Icgoo.DefaultCreator;
                        category.ClassifyStatus = (int)Enums.ClassifyStatus.First;
                    }
                }

                int precount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Count(item => item.PreProductID == e.insidePreProduct.ID);
                if (precount != 0)
                {
                    //reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(category, item => item.PreProductID == e.icgooPreProduct.ID);
                }
                else
                {
                    category.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(category);
                }
            }
        }
    }
}

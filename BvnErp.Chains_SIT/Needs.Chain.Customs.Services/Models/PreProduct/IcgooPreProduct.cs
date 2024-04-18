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
    public class IcgooPreProduct : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string sale_orderline_id { get; set; }
        public string partno { get; set; }
        public string mfr { get; set; }
        public decimal price { get; set; }
        public string currency_code { get; set; }
        public string supplier { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Status { get; set; }
        public string Creater { get; set; }
        public string Updater { get; set; }
        public Client Client { get; set; }
        public CompanyTypeEnums CompanyType { get; set; }
        /// <summary>
        /// 内单 产地
        /// </summary>
        public string AraeOfProduction { get; set; }
        /// <summary>
        /// 内单 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string Pack { get; set; }
        /// <summary>
        /// 内单 封装
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 内单 用途
        /// </summary>
        public string UseFor { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event GetPreProductSuccessHanlders GetSuccess;

        public IcgooPreProduct()
        {

        }

        public IcgooPreProduct(CompanyTypeEnums companyType)
        {
            if (companyType != CompanyTypeEnums.Inside)
            {
                this.GetSuccess += ClassifyDefault;
            }
        }

        public void OnEnter()
        {

        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProducts>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new { Status = Enums.Status.Delete }, item => item.PreProductID == this.ID);
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>().Count(item => item.ProductUnionCode == this.sale_orderline_id);
                var OldId = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>().Where(item => item.ProductUnionCode == this.sale_orderline_id).FirstOrDefault();
                if (count == 0)
                {
                    this.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.ID = OldId.ID;
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.PreProducts
                    {
                        Model = this.partno,
                        Manufacturer = this.mfr,
                        Price = this.price,
                        Currency = this.currency_code,
                        Supplier = this.supplier,
                        UpdateDate = DateTime.Now,
                        Status = (int)this.Status,
                        CompanyType = (int)this.CompanyType,
                    }, item => item.ProductUnionCode == this.sale_orderline_id);
                }
            }

            this.OnEnter();
            this.OnGetSuccess(new GetPreProductSuccessEventArgs(this));
        }


        /// <summary>
        /// Icgoo型号获取成功调用事件-生成待归类记录
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnGetSuccess(GetPreProductSuccessEventArgs args)
        {
            this.GetSuccess?.Invoke(this, args);
        }

        private void ClassifyDefault(object sender, GetPreProductSuccessEventArgs e)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var category = new Layer.Data.Sqls.ScCustoms.PreProductCategories();
                category.PreProductID = e.icgooPreProduct.ID;
                category.Model = e.icgooPreProduct.partno;
                category.Manufacture = e.icgooPreProduct.mfr;
                category.UpdateDate = category.CreateDate = DateTime.Now;
                category.Status = (int)Enums.Status.Normal;
                category.ClassifyStatus = (int)Enums.ClassifyStatus.Unclassified;

#pragma warning disable
#if PvData
                var preProductCategory = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().FirstOrDefault(item => item.PreProductID == e.icgooPreProduct.ID);
                if (preProductCategory == null)
                {
                    category.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(category);
                }
                else
                {
                    category.ID = preProductCategory.ID;
                }

                try
                {
                    //自动归类
                    var preClassifyProduct = new Views.Alls.PreClassifyProductsAll(reponsitory)[category.ID];
                    var autoCategory = new AutoPreClassify(preClassifyProduct);
                    autoCategory.DoClassify();
                }
                catch (Exception ex)
                {
                }
#else
               int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>().Count(item => item.Model == e.icgooPreProduct.partno);
                if (count != 0)
                {
                    var defaultResult = new Needs.Ccs.Services.Views.DefaultClassifyResultView().Where(item => item.Model == e.icgooPreProduct.partno).FirstOrDefault();
                    if (defaultResult != null)
                    {
                        category.ProductName = defaultResult.ProductName;
                        category.HSCode = defaultResult.HSCode;
                        category.TariffRate = defaultResult.TariffRate / 100;
                        category.AddedValueRate = defaultResult.AddedValueRate / 100;
                        category.TaxCode = defaultResult.TaxCode;
                        category.TaxName = defaultResult.TaxName;
                        category.ClassifyType = defaultResult.ClassifyType==null? (int)IcgooClassifyTypeEnums.Normal: (int)defaultResult.ClassifyType;
                        category.Type = defaultResult.Type == null ? (int)ItemCategoryType.Normal : (int)defaultResult.Type;
                        category.InspectionFee = defaultResult.InspectionFee;
                        category.Unit1 = defaultResult.Unit1;
                        category.Unit2 = defaultResult.Unit2;
                        category.CIQCode = defaultResult.CIQCode;
                        category.Elements = defaultResult.Elements;
                        category.ClassifyFirstOperator = Icgoo.DefaultCreator;
                        category.ClassifyStatus = (int)Enums.ClassifyStatus.First;
                    }
                }

                int precount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Count(item => item.PreProductID == e.icgooPreProduct.ID);
                if (precount != 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(category, item => item.PreProductID == e.icgooPreProduct.ID);
                }
                else
                {
                    category.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(category);
                }
#endif
#pragma warning restore
            }
        }
    }
}

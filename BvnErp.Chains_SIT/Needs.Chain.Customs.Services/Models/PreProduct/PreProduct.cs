using Layer.Data.Sqls;
using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Serializers;
using System;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 预归类标准产品
    /// </summary>
    public class PreProduct : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public string ProductUnionCode { get; set; }

        public Client Client { get; set; }

        //public string CusProductID { get; set; }
        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public decimal Price { get; set; }

        public decimal? Qty { get; set; }

        public string Currency { get; set; }

        public string Supplier { get; set; }

        public Enums.CompanyTypeEnums CompanyType { get; set; }

        public string BatchNo { get; set; }

        public string Description { get; set; }

        public string Pack { get; set; }

        public string AreaOfProduction { get; set; }

        public string UseFor { get; set; }
        public string Source { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        public DateTime? DueDate { get; set; }
        public PreProductUserType UseType { get; set; }
        public string Unit { get; set; }
        public string AdminID { get; set; }
        public string IcgooAdmin { get; set; }
        #endregion

        #region 大赢家预归类扩展属性
        public string TaxCode { get; set; }
        public string TaxName { get; set; }
        public string HSCode { get; set; }
        public string Elements { get; set; }
        #endregion

        #region 自动归类扩展属性
        //  public bool
        #endregion
        /// <summary>
        /// 构造函数
        /// </summary>
        public PreProduct()
        {
            this.Status = Enums.Status.Normal;
            this.UseType = Enums.PreProductUserType.Pre;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.GetPreProductSuccessed += ClassifyDefault;
        }

        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event PreCategoryHanlder GetPreProductSuccessed;

        #region 持久化

        /// <summary>
        /// 数据删除触发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProducts>(new
                {
                    Status = Enums.Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        public void Enter()
        {
            this.OnEnter();
            this.OnGetSuccessed(new PreCategoryEventArgs(this));
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var oldID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>().Where(item => item.ProductUnionCode == this.ProductUnionCode).FirstOrDefault();

                //判断是否为新增
                if (oldID == null)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PreProducts
                    {
                        ID = this.ID = ChainsGuid.NewGuidUp(),
                        ClientID = this.ClientID,
                        ProductUnionCode = this.ProductUnionCode,
                        Model = this.Model,
                        Manufacturer = this.Manufacturer,
                        Qty = this.Qty,
                        Price = this.Price,
                        Currency = this.Currency,
                        Supplier = this.Supplier,
                        CompanyType = (int)this.CompanyType,
                        BatchNo = this.BatchNo,
                        Description = this.Description,
                        Pack = this.Pack,
                        AreaOfProduction = this.AreaOfProduction,
                        UseFor = this.UseFor,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        DueDate = this.DueDate,
                        UseType = (int)this.UseType,
                        Source = this.Source,
                        Unit = this.Unit,
                        AdminID = this.AdminID,
                        IcgooAdmin = this.IcgooAdmin
                    });
                }
                else
                {
                    this.ID = oldID.ID;
                }
            }
        }

        public virtual void OnGetSuccessed(PreCategoryEventArgs args)
        {
            this.GetPreProductSuccessed?.Invoke(this, args);
        }

        private void ClassifyDefault(object sender, PreCategoryEventArgs e)
        {
            using (ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var category = new Layer.Data.Sqls.ScCustoms.PreProductCategories();
                category.Type = (int)ItemCategoryType.Normal;
                category.PreProductID = e.icgooPreProduct.ID;
                category.Model = e.icgooPreProduct.Model;
                category.Manufacture = e.icgooPreProduct.Manufacturer;
                category.UpdateDate = category.CreateDate = DateTime.Now;
                category.Status = (int)Enums.Status.Normal;

                if (this.CompanyType == CompanyTypeEnums.Inside && !string.IsNullOrEmpty(category.HSCode))
                {
                    category.ClassifyStatus = (int)Enums.ClassifyStatus.First;
                    category.HSCode = e.icgooPreProduct.HSCode;
                    category.TaxCode = e.icgooPreProduct.TaxCode;
                    category.TaxName = e.icgooPreProduct.TaxName;
                    category.Elements = e.icgooPreProduct.Elements;
                    category.ClassifyFirstOperator = Icgoo.DefaultCreator;
                }
                else
                    category.ClassifyStatus = (int)Enums.ClassifyStatus.Unclassified;

#pragma warning disable
#if PvData
                int precount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Count(item => item.PreProductID == e.icgooPreProduct.ID);
                if (precount == 0)
                {
                    category.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(category);

                    try
                    {
                        //自动归类
                        var preClassifyProduct = new Views.Alls.PreClassifyProductsAll(reponsitory)[category.ID];
                        var autoCategory = new AutoPreClassify(preClassifyProduct);
                        autoCategory.DoClassify();
                    }
                    catch (Exception ex)
                    {
                        //记录异常日志
                        var log = new Models.ClassifyApiLogs()
                        {
                            ClassifyProductID = category.ID,
                            ResponseContent = new { ex.Message, ex.StackTrace }.Json(),
                            Summary = "产品预归类 - 自动归类异常"
                        };
                        log.Enter();
                    }
                }
#else
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategoriesDefaults>().Count(item => item.Model == e.icgooPreProduct.Model);
                if (count != 0)
                {
                    var defaultResult = new Views.DefaultClassifyResultView().Where(item => item.Model == e.icgooPreProduct.Model).FirstOrDefault();
                    if (defaultResult != null)
                    {
                        category.ProductName = defaultResult.ProductName;
                        category.TariffRate = defaultResult.TariffRate / 100;
                        category.AddedValueRate = defaultResult.AddedValueRate / 100;
                        category.Type = defaultResult.Type == null ? (int)ItemCategoryType.Normal : (int)defaultResult.Type;
                        category.InspectionFee = defaultResult.InspectionFee;
                        category.Unit1 = defaultResult.Unit1;
                        category.Unit2 = defaultResult.Unit2;
                        category.CIQCode = defaultResult.CIQCode;
                        category.ClassifyFirstOperator = Icgoo.DefaultCreator;

                        //内单的以下归类信息，采用产品预归类接口提交过来的数据，不用自动归类数据
                        if (this.CompanyType != CompanyTypeEnums.Inside)
                        {
                            category.HSCode = defaultResult.HSCode;
                            category.TaxCode = defaultResult.TaxCode;
                            category.TaxName = defaultResult.TaxName;
                            category.Elements = defaultResult.Elements;
                        }

                        if ((!string.IsNullOrWhiteSpace(defaultResult.TaxCode)) && (!string.IsNullOrWhiteSpace(defaultResult.TaxName)))  //历史数据里面可能没有税务名称和税务编码，不让它自动归类
                        {
                            var productCategories = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductCategories>().Where(item => item.Model == e.icgooPreProduct.Model && item.CreateDate >= DateTime.Now.AddMonths(-3));//三个月之前的历史记录
                            if (productCategories.Count() == 0)
                            {
                                category = AutoClassify(category, reponsitory);
                            }
                            else
                            {
                                var vagPrice = productCategories.Average(item => item.UnitPrice.Value); //三个月单价平均值
                                var percentValue = System.Math.Abs(this.Price - vagPrice) / vagPrice;  //增长率
                                if (percentValue < (decimal)0.1)
                                {
                                    category = AutoClassify(category, reponsitory);
                                }
                            }
                        }
                    }
                    else
                    {
                        category.Type = (int)ItemCategoryType.Normal;
                    }
                }

                int precount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Count(item => item.PreProductID == e.icgooPreProduct.ID);

                if (precount == 0)
                {
                    category.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(category);
                    if (category.ClassifyStatus == (int)Enums.ClassifyStatus.First)  //自动归类添加日志
                    {
                        var product = new Views.Alls.PreClassifyProductsAll(reponsitory)[category.ID];
                        product.Admin = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                        var step1 = new PreClassifyStep1(product);
                        step1.DoClassify();
                    }
                }
#endif
#pragma warning restore
            }
        }

        /// <summary>
        /// 自动归类
        /// </summary>
        /// <param name="category"></param>
        /// <param name="reponsitory"></param>
        /// <returns></returns>
        private PreProductCategories AutoClassify(PreProductCategories category, ScCustomsReponsitory reponsitory)
        {
            category.ClassifyStatus = (int)Enums.ClassifyStatus.First;
            var tariff = new Views.CustomsTariffsView(reponsitory).Where(item => item.HSCode == category.HSCode.Trim()).FirstOrDefault();
            if (tariff != null)
            {
                var elements = tariff.Elements;
                var elementArr = elements.Split(';');
                for (int i = 0; i < elementArr.Length; i++)
                {
                    var arr = elementArr[i].Split(':');
                    if (arr[1] == "品牌")
                    {
                        var categoryElementArr = category.Elements.Split('|');
                        categoryElementArr[i] = this.Manufacturer + "牌";
                        category.Elements = string.Join("|", categoryElementArr);
                    }
                }
            }
            return category;
        }

        #endregion
    }
}

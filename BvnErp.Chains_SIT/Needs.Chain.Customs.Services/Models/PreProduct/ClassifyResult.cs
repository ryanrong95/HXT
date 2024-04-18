using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClassifyResult : ProductCategoriesDefault, IUnique, IPersist, IFulError, IFulSuccess, Interfaces.IProduct
    {
        public IcgooPreProduct PreProduct;
        public string PreProductID { get; set; }
        public ClassifyStatus ClassifyStatus { get; set; }
        public string ClassifyFirstOperatorID { get; set; }
        public Admin ClassifyFirstOperator { get; set; }
        public string ClassifySecondOperatorID { get; set; }
        public Admin ClassifySecondOperator { get; set; }     

        /// <summary>
        /// 为了提速IFClassifyResultView，冗余
        /// </summary>
        public string DeclarantID { get; set; }

        /// <summary>
        /// 为了速度IFClassifyResultView，冗余
        /// </summary>
        public string PreProductUnicode { get; set; }

        /// <summary>
        /// 当前归类是否已经锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 锁定人
        /// </summary>
        public Admin Locker { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime? LockDate { get; set; }
        /// <summary>
        /// 预处理一还是预处理二
        /// </summary>
        public string FirstOrSecond { get; set; }
        /// <summary>
        /// 冗余
        /// </summary>
        public CompanyTypeEnums CompanyType { get; set; }
        public string InsidePostUrl { get; set; }
        public string InsideKey { get; set; }
        public string InsideElementsPostUrl { get; set; }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event IcgooClassifyHanlder IcgooDefaultClassify;
        public event IcgooClassifyHanlder IcgooClassifyChange;
        public event PostIcgooHandler PostIcgoo;
        public event IcgooClassifyLockHanlder ProductLock;
        public event PostIcgooHandler PostInside;
     
        public ClassifyResult()
        {
            IcgooClassifyChange += ClassifyResult_Changed;
            IcgooDefaultClassify += SetDefaultClassify;           
            PostIcgoo += PostIcgooClassify;
            ProductLock += ClassifyResult_Locked;
            PostInside += PostInsideClassify;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProductCategories>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 一次归类
        /// </summary>
        public void ClassifyFirst()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ClassifyStatus = ClassifyStatus.First;
                this.UpdateDate = DateTime.Now;
                reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            OnClassified(new IcgooClassifyEventArgs(this));
        }

        /// <summary>
        /// 二次归类
        /// </summary>
        public void ClassifySecond()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.ClassifyStatus = ClassifyStatus.Done;
                this.UpdateDate = DateTime.Now;
                reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            OnClassified(new IcgooClassifyEventArgs(this));
            OnSecondClassified(new IcgooClassifyEventArgs(this));
        }

        public virtual void OnClassified(IcgooClassifyEventArgs args)
        {
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this.ID));
            this.IcgooClassifyChange?.Invoke(this, args);
            this.IcgooDefaultClassify?.Invoke(this, args);
            //解除锁定
            this.UnLock();
        }

        public virtual void OnSecondClassified(IcgooClassifyEventArgs args)
        {
            switch (this.CompanyType)
            {
                case CompanyTypeEnums.Icgoo:
                case CompanyTypeEnums.FastBuy:
                    this.PostIcgoo?.Invoke(this, args);
                    break;

                case CompanyTypeEnums.Inside:
                    this.PostInside?.Invoke(this, args);
                    break;
            }            
        }

        /// <summary>
        /// 设置归类默认值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetDefaultClassify(object sender, IcgooClassifyEventArgs e)
        {
           
            var model = this.Model;
            var catDefault = new Views.ProductCategoriesDefaultsView().Where(item => item.Model == model).FirstOrDefault();

            ProductCategoriesDefault defaultValue = new ProductCategoriesDefault();
            defaultValue.Model = e.classifyResult.Model;
            defaultValue.Manufacturer = e.classifyResult.Manufacturer;
            defaultValue.ProductName = e.classifyResult.ProductName;
            defaultValue.HSCode = e.classifyResult.HSCode;
            defaultValue.TariffRate = e.classifyResult.TariffRate;
            defaultValue.AddedValueRate = e.classifyResult.AddedValueRate;
            defaultValue.TaxCode = e.classifyResult.TaxCode;
            defaultValue.TaxName = e.classifyResult.TaxName;
            defaultValue.ClassifyType = e.classifyResult.ClassifyType;
            defaultValue.InspectionFee = e.classifyResult.InspectionFee;
            defaultValue.Unit1 = e.classifyResult.Unit1;
            defaultValue.Unit2 = e.classifyResult.Unit2;
            defaultValue.CIQCode = e.classifyResult.CIQCode;
            defaultValue.Elements = e.classifyResult.Elements;
            defaultValue.Status = e.classifyResult.Status;
            defaultValue.CreateDate = e.classifyResult.CreateDate;
            defaultValue.UpdateDate = e.classifyResult.UpdateDate;
            defaultValue.Summary = e.classifyResult.Summary;

            //同步订单归类结果
            if (catDefault != null)
            {
                switch (this.ClassifyType)
                {
                    case IcgooClassifyTypeEnums.Inspection:
                        defaultValue.Type |= ItemCategoryType.Inspection;
                        defaultValue.InspectionFee = e.classifyResult.InspectionFee;
                        break;

                    case IcgooClassifyTypeEnums.CCC:
                        defaultValue.Type |= ItemCategoryType.CCC;
                        break;

                    default:
                        break;
                }
            }

            defaultValue.DefaultEnter();
        }

        /// <summary>
        /// 向Icgoo提交归类结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PostIcgooClassify(object sender, IcgooClassifyEventArgs e)
        {
            IcgooPost icgooPost = new IcgooPost(e.classifyResult.CompanyType);
            icgooPost.id = e.classifyResult.ID;
            icgooPost.sale_orderline_id = e.classifyResult.PreProduct.sale_orderline_id;
            icgooPost.partno = e.classifyResult.Model;
            icgooPost.supplier = e.classifyResult.PreProduct.supplier;
            icgooPost.mfr = e.classifyResult.Manufacturer;
            icgooPost.brand = e.classifyResult.Manufacturer;
            icgooPost.origin = "";
            icgooPost.customs_rate = e.classifyResult.TariffRate;
            icgooPost.add_rate = e.classifyResult.AddedValueRate;
            icgooPost.product_name = e.classifyResult.ProductName;
            icgooPost.category = "";
            icgooPost.type = (int)e.classifyResult.ClassifyType;
            icgooPost.hs_code = e.classifyResult.HSCode;
            icgooPost.tax_code = e.classifyResult.TaxCode;

            icgooPost.PostData();
        }

        private void ClassifyResult_Changed(object sender, IcgooClassifyEventArgs e)
        {
            var classifyProduct = (ClassifyResult)e.classifyResult;
            var model = classifyProduct.Model;
            var catDefault = new Views.ProductCategoriesDefaultsView().Where(item => item.Model == model).FirstOrDefault();

            if (catDefault == null)
            {
                catDefault = new ProductCategoriesDefault();
            }
            else
            {
                var declarent = this.ClassifyFirstOperator == null ? this.ClassifySecondOperator: ClassifyFirstOperator;

                #region 产品归类变更日志

                if (classifyProduct.Manufacturer != catDefault.Manufacturer)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的品牌由【" + catDefault.Manufacturer + "】修改为【" + classifyProduct.Manufacturer + "】");
                }
                if (classifyProduct.ProductName != catDefault.ProductName)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的报关品名由【" + catDefault.ProductName + "】修改为【" + classifyProduct.ProductName + "】");
                }
                if (classifyProduct.HSCode != catDefault.HSCode)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的海关编码由【" + catDefault.HSCode + "】修改为【" + classifyProduct.HSCode + "】");
                }
                if (classifyProduct.TariffRate != catDefault.TariffRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的关税率由【" + catDefault.TariffRate / 100 + "】修改为【" + classifyProduct.TariffRate + "】");
                }
                if (classifyProduct.AddedValueRate != catDefault.AddedValueRate / 100)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的增值税率由【" + catDefault.AddedValueRate / 100 + "】修改为【" + classifyProduct.AddedValueRate + "】");
                }
                if (classifyProduct.TaxCode != catDefault.TaxCode)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的税务编码由【" + catDefault.TaxCode + "】修改为【" + classifyProduct.TaxCode + "】");
                }
                if (classifyProduct.TaxName != catDefault.TaxName)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的税务名称由【" + catDefault.TaxName + "】修改为【" + classifyProduct.TaxName + "】");
                }
                if (classifyProduct.Unit1 != catDefault.Unit1)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的法定第一单位由【" + catDefault.Unit1 + "】修改为【" + classifyProduct.Unit1 + "】");
                }
                if (classifyProduct.Unit2 != catDefault.Unit2)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的法定第二单位由【" + catDefault.Unit2 + "】修改为【" + classifyProduct.Unit2 + "】");
                }
                if (classifyProduct.CIQCode != catDefault.CIQCode)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的检验检疫编码由【" + catDefault.CIQCode + "】修改为【" + classifyProduct.CIQCode + "】");
                }
                if (classifyProduct.Elements != catDefault.Elements)
                {
                    classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的申报要素由【" + catDefault.Elements + "】修改为【" + classifyProduct.Elements + "】");
                }
                if (catDefault.ClassifyType != null)
                {
                    if (classifyProduct.ClassifyType != catDefault.ClassifyType)
                    {
                        classifyProduct.Log("报关员【" + declarent.RealName + "】将型号【" + model + "】的类型由【" + catDefault.ClassifyType.GetDescription() + "】修改为【" + classifyProduct.ClassifyType.GetDescription() + "】");
                    }                 
                }

                #endregion
            }

        }

        private void Log(string summary)
        {
            ProductClassifyChangeLog log = new ProductClassifyChangeLog();
            log.Model = this.Model;
            log.Manufacturer = this.Manufacturer;
            log.Declarant = this.ClassifyFirstOperator == null ? this.ClassifySecondOperator : this.ClassifySecondOperator;
            log.Summary = summary;
            log.Enter();
        }

        /// <summary>
        /// 归类锁定
        /// </summary>
        public void Lock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ProductClassifyLocks
                    {
                        ID = this.PreProduct.ID,
                        IsLocked = true,
                        LockDate = DateTime.Now,
                        AdminID = FirstOrSecond==Icgoo.First?this.ClassifyFirstOperator.ID:this.ClassifySecondOperator.ID,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                    OnLocked(new IcgooClassifyLockEventArgs(this,"锁定"));
                }
            }
        }

        /// <summary>
        /// 解除归类锁定
        /// </summary>
        public void UnLock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>(item => item.ID == this.PreProduct.ID);
                OnLocked(new IcgooClassifyLockEventArgs(this,"解锁"));
            }
        }

        private void OnLocked(IcgooClassifyLockEventArgs args)
        {
            this.ProductLock?.Invoke(this, args);
        }

        private void ClassifyResult_Locked(object sender, IcgooClassifyLockEventArgs e)
        {
            ProductClassifyLog pLog = new ProductClassifyLog();
            pLog.ClassifyProductID = e.classifyResult.PreProduct.ID;
            pLog.LogType = LogTypeEnums.Lock;
            pLog.Declarant = FirstOrSecond == Icgoo.First ? this.ClassifyFirstOperator : this.ClassifySecondOperator;
            string firstOrSecond = FirstOrSecond == Icgoo.First ? "预处理一" : "预处理二";
            pLog.OperationLog = "报关员【" + pLog.Declarant.RealName + "】" + e.Operation + "了"+firstOrSecond+"。";                             
            pLog.Enter();
        }

        public void PostInsideClassify(object sender, IcgooClassifyEventArgs e)
        {
            bool IsInsp = (e.classifyResult.Type & ItemCategoryType.Inspection) > 0 ? true : false;
            bool IsHighValue = (e.classifyResult.Type & ItemCategoryType.HighValue) > 0 ? true : false;
            bool IsCCC = (e.classifyResult.Type & ItemCategoryType.CCC) > 0 ? true : false;
            bool IsOrigin = (e.classifyResult.Type & ItemCategoryType.OriginProof) > 0 ? true : false;

            bool IsSpecial = false;
            if (IsInsp || IsHighValue || IsCCC|| IsOrigin)
            {
                IsSpecial = true;
            }

            string PostResult = new
            {
                id = e.classifyResult.PreProduct.sale_orderline_id.UrlEncoding(),
                MFC = e.classifyResult.Manufacturer.Replace("&", "").UrlEncoding(),
                Area = e.classifyResult.PreProduct.AraeOfProduction.UrlEncoding(),
                BatchNo = e.classifyResult.PreProduct.BatchNo.Replace("&", "").UrlEncoding(),
                Pack = e.classifyResult.PreProduct.Pack.UrlEncoding(),
                Description = e.classifyResult.PreProduct.Description.Replace("&", "").UrlEncoding(),
                GoodName = e.classifyResult.TaxName.UrlEncoding(),
                UseFor = e.classifyResult.PreProduct.UseFor.UrlEncoding(),
                CustomsCode = e.classifyResult.ProductName.UrlEncoding(),
                IsBusinessCheck = (IsInsp ? "1" : "0").UrlEncoding(),
                IsSpecialPack = (IsSpecial ? "1" : "0").UrlEncoding()
            }.Json();


            string url = this.InsidePostUrl + "?json=" + PostResult + "&key=" + this.InsideKey;
            bool issuccess = false;
            string result = HttpRequest.GetRequest(url, ref issuccess);


            ///Elements 提交
            string[] elements = e.classifyResult.Elements.Split('|');
            List<InsideEletmentsPost> lists = new List<InsideEletmentsPost>();
            int i = 0;
            foreach(var ele in elements)
            {
                InsideEletmentsPost p = new InsideEletmentsPost();
                p.objid = e.classifyResult.HSCode + "_" + i.ToString();
                p.objname = ele;               
                lists.Add(p);
                i++;
            }
            string PostElementsResult = new
            {
                partno = e.classifyResult.Model,
                mfc = e.classifyResult.Manufacturer,               
                list = lists,

            }.Json().UrlEncoding(); 

            string ElementsUrl = this.InsideElementsPostUrl + "?json=" + PostElementsResult + "&key=" + this.InsideKey;           
            string ElementsResult = HttpRequest.GetRequest(url, ref issuccess);


            if (issuccess)
            {
                InsidePreProducts pro = JsonConvert.DeserializeObject<InsidePreProducts>(result);
                PostBack p = new PostBack();
                p.ID = ChainsGuid.NewGuidUp();
                p.id = e.classifyResult.PreProduct.ID;
                p.status = pro.isSuccess;
                p.msg = pro.message;
                p.CreateDate = DateTime.Now;
                p.RecordStatus = Status.Normal;
                p.Enter();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类产品
    /// </summary>
    public sealed partial class ClassifyProduct : OrderItem, Interfaces.IClassifyProduct
    {
        #region 属性

        /// <summary>
        /// 订单类型
        /// </summary>
        public Enums.OrderType OrderType { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 归类异常原因
        /// </summary>
        public string AnomalyReason { get; set; }

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
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 型号，加这个是为了解决Icgoo 同一个ProductUniqueCode，所有信息都一样，就只有箱号不一样
        /// 按照原来的方式，推送给库房，只推送一个箱号问题
        /// </summary>
        public string CaseNo { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否禁运
        /// </summary>
        public bool IsForbid { get; set; }

        /// <summary>
        /// 是否原产地证明
        /// </summary>
        public bool IsOriginProof { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsInsp { get; set; }

        /// <summary>
        /// 是否高价值产品
        /// </summary>
        public bool IsHighValue { get; set; }

        /// <summary>
        /// 系统判定是否3C
        /// </summary>
        public bool IsSysCCC { get; set; }

        /// <summary>
        /// 系统判定是否禁运
        /// </summary>
        public bool IsSysForbid { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        public Enums.ClassifyStep ClassifyStep { get; set; }

        #endregion

        #region 操作人/报关员

        public Admin Admin { get; set; }

        /// <summary>
        /// 归类一操作者 ID
        /// </summary>
        public string ClassifyFirstOperator { get; set; } = string.Empty;

        /// <summary>
        /// 归类一操作者 姓名
        /// </summary>
        public string ClassifyFirstOperatorName { get; set; } = string.Empty;

        /// <summary>
        /// 归类二操作者 ID
        /// </summary>
        public string ClassifySecondOperator { get; set; } = string.Empty;

        /// <summary>
        /// 归类一操作者 姓名
        /// </summary>
        public string ClassifySecondOperatorName { get; set; } = string.Empty;

        #endregion

        #region 事件

        public event ProductClassifiedHanlder Classified;
        public event ProductLockedHanlder Locked;
        public event ProductLockedHanlder UnLocked;
        public event ProductClassifiedHanlder Anomalied;

        #endregion

        public void DoClassify()
        {
            ComClassify();
            this.OnClassified();
        }

        private void ComClassify()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 订单项
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                {
                    Name = this.Name,
                    Model = this.Model,
                    Manufacturer = this.Manufacturer,
                    Batch = this.Batch,
                    Unit = this.Unit,
                    ClassifyStatus = (int)this.ClassifyStatus
                }, item => item.ID == this.ID);
                #endregion

                #region 归类类型
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Count(item => item.ID == this.Category.ID);
                if (count == 0)
                {
                    this.Category.OrderItemID = this.ID;
                    reponsitory.Insert(this.Category.ToLinq());
                }
                else
                {
                    //reponsitory.Update(this.Category.ToLinq(), item => item.ID == this.Category.ID);
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                    {
                        ID = this.Category.ID,
                        OrderItemID = this.Category.OrderItemID,
                        ////AdminID = this.Category.Declarant.ID,
                        //ClassifyFirstOperator = this.Category.ClassifyFirstOperator.ID,
                        //ClassifySecondOperator = this.Category.ClassifySecondOperator?.ID,
                        Type = (int)this.Category.Type,
                        TaxCode = this.Category.TaxCode,
                        TaxName = this.Category.TaxName,
                        HSCode = this.Category.HSCode,
                        Unit1 = this.Category.Unit1,
                        Unit2 = this.Category.Unit2,
                        Name = this.Category.Name,
                        Elements = this.Category.Elements,
                        Qty1 = this.Category.Qty1,
                        Qty2 = this.Category.Qty2,
                        CIQCode = this.Category.CIQCode,
                        Status = (int)this.Category.Status,
                        CreateDate = this.Category.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Category.Summary
                    }, item => item.ID == this.Category.ID);
                }
                #endregion

                #region 关税率
                count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Count(item => item.ID == this.ImportTax.ID);
                if (count == 0)
                {
                    this.ImportTax.OrderItemID = this.ID;
                    reponsitory.Insert(this.ImportTax.ToLinq());
                }
                else
                {
                    //reponsitory.Update(this.ImportTax.ToLinq(), item => item.ID == this.ImportTax.ID);
                    decimal ImportTaxValue = this.ImportTax.Value == null ? 0 : this.ImportTax.Value.Value ;
                    reponsitory.Update< Layer.Data.Sqls.ScCustoms.OrderItemTaxes> (new
                    {
                        ID = this.ImportTax.ID,
                        OrderItemID = this.ImportTax.OrderItemID,
                        Type = (int)this.ImportTax.Type,
                        Rate = this.ImportTax.Rate,
                        ReceiptRate = this.ImportTax.ReceiptRate,
                        Value = ImportTaxValue,
                        Status = (int)this.ImportTax.Status,
                        CreateDate = this.ImportTax.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.ImportTax.Summary
                    }, item => item.ID == this.ImportTax.ID);
                }
                #endregion

                #region 增值税率
                count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Count(item => item.ID == this.AddedValueTax.ID);
                if (count == 0)
                {
                    this.AddedValueTax.OrderItemID = this.ID;
                    reponsitory.Insert(this.AddedValueTax.ToLinq());
                }
                else
                {
                    //reponsitory.Update(this.AddedValueTax.ToLinq(), item => item.ID == this.AddedValueTax.ID);
                    decimal AddedValueTaxValue = this.AddedValueTax.Value == null ? 0: this.AddedValueTax.Value.Value ;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Rate = this.AddedValueTax.Rate,
                        ReceiptRate = this.AddedValueTax.ReceiptRate,
                        Value = AddedValueTaxValue,
                        Status = (int)this.AddedValueTax.Status,
                        UpdateDate = DateTime.Now,
                        Summary = this.AddedValueTax.Summary
                    }, item => item.ID == this.AddedValueTax.ID);
                }
                #endregion

                #region 消费税率
                count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Count(item => item.ID == this.ExciseTax.ID);
                if (count == 0)
                {
                    this.ExciseTax.OrderItemID = this.ID;
                    reponsitory.Insert(this.ExciseTax.ToLinq());
                }
                else
                {
                    //reponsitory.Update(this.ExciseTax.ToLinq(), item => item.ID == this.ExciseTax.ID);
                    decimal ExciseTaxValue = this.ExciseTax.Value == null ? 0 : this.ExciseTax.Value.Value ;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Rate = this.ExciseTax.Rate,
                        ReceiptRate = this.ExciseTax.ReceiptRate,
                        Value = ExciseTaxValue,
                        Status = (int)this.ExciseTax.Status,
                        UpdateDate = DateTime.Now,
                        Summary = this.ExciseTax.Summary
                    }, item => item.ID == this.ExciseTax.ID);
                }
                #endregion

                #region 商检费
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPremiums>(item => item.OrderItemID == this.ID &&
                            (Enums.OrderPremiumType)item.Type == Enums.OrderPremiumType.InspectionFee);
                if (this.IsInsp)
                {
                    var premium = new OrderPremium
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                        OrderID = this.OrderID,
                        OrderItemID = this.ID,
                        Type = Enums.OrderPremiumType.InspectionFee,
                        Count = 1,
                        UnitPrice = this.InspectionFee.GetValueOrDefault(),
                        Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                        Rate = 1,
                        Admin = this.Admin ?? Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator)
                    };
                    reponsitory.Insert(premium.ToLinq());
                }
                #endregion
            }
        }

        public void QuickClassify()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                {
                    ClassifyStatus = (int)this.ClassifyStatus,
                    UpdateDate = DateTime.Now
                }, item => item.ID == this.ID);

                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                {
                    ClassifySecondOperator = this.Admin.ID,//AdminID = this.Admin.ID,
                    UpdateDate = DateTime.Now
                }, item => item.OrderItemID == this.ID);
            }

            this.OnClassified();
        }

        void OnClassified()
        {
            if (this != null && this.Classified != null)
            {
                this.Classified(this, new ProductClassifiedEventArgs(this));
            }
        }

        public void Lock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ProductClassifyLocks
                    {
                        ID = this.ID,
                        IsLocked = true,
                        LockDate = DateTime.Now,
                        AdminID = this.Admin.ID,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
            }

            this.OnProductLocked();
        }

        void OnProductLocked()
        {
            if (this != null && this.Locked != null)
            {
                this.Locked(this, new ProductLockedEventArgs(this, this.ClassifyStep));
            }
        }

        public void UnLock()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ProductClassifyLocks>(item => item.ID == this.ID);
            }

            this.OnProductUnLocked();
        }

        void OnProductUnLocked()
        {
            if (this != null && this.UnLocked != null)
            {
                this.UnLocked(this, new ProductLockedEventArgs(this, this.ClassifyStep));
            }
        }

        public void Anomaly()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                {
                    ClassifyStatus = Enums.ClassifyStatus.Anomaly,
                    Summary = this.AnomalyReason
                }, item => item.ID == this.ID);
            }

            this.OnAnomalied();
        }

        void OnAnomalied()
        {
            if (this != null && this.Anomalied != null)
            {
                this.Anomalied(this, new ProductClassifiedEventArgs(this));
            }
        }
    }

    /// <summary>
    /// 用于处理会员中心预归类产品快捷下单
    /// </summary>
    public partial class ClassifyProduct
    {
        /// <summary>
        /// 当预归类产品快捷下单时使用
        /// </summary>
        public event ProductClassifiedHanlder PreClassified;

        public ClassifyProduct()
        {
            this.PreClassified += Product_PreClassified;
        }

        public void DoPreClassify()
        {
            ComClassify();
            this.OnPreClassified();
        }

        void OnPreClassified()
        {
            if (this != null && this.PreClassified != null)
            {
                this.PreClassified(this, new ProductClassifiedEventArgs(this));
            }
        }

        /** 预处理产品快捷下单之后需要做的操作
         * 1.同步订单特殊类型(OrderVoyage)
         * 2.进行产品管控
         */
        private void Product_PreClassified(object sender, ProductClassifiedEventArgs e)
        {
            var classifyProduct = (ClassifyProduct)e.Product;

            //快捷下单后，在这里更新 OrderItemCategories 中的 ClassifyFirstOperator、ClassifySecondOperator
            //不在 ComClassify 中更新是因为 ComClassify 是很多归类共用的，不是都要更新两个归类人，否则会误把其中一个抹掉
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                {
                    ClassifyFirstOperator = this.Category?.ClassifyFirstOperator?.ID,
                    ClassifySecondOperator = this.Category?.ClassifySecondOperator?.ID,
                }, item => item.ID == this.Category.ID);
            }  


            SetOrderVoyage(classifyProduct);
            DoProductControl(classifyProduct);
        }

        private void SetOrderVoyage(ClassifyProduct classifyProduct)
        {
            var orderID = classifyProduct.OrderID;

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC Begin

            OrderItem[] orderItems = new Order() { ID = orderID, }.Items.ToArray();
            OrderItem[] orderItemsHasCategory = orderItems.Where(t => t.Category != null).ToArray();
            Dictionary<OrderSpecialType, ItemCategoryType> dicCheckOrderSpecialType = new Dictionary<OrderSpecialType, ItemCategoryType>();
            dicCheckOrderSpecialType.Add(OrderSpecialType.HighValue, ItemCategoryType.HighValue);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Inspection, ItemCategoryType.Inspection);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Quarantine, ItemCategoryType.Quarantine);
            dicCheckOrderSpecialType.Add(OrderSpecialType.CCC, ItemCategoryType.CCC);

            foreach (var dic in dicCheckOrderSpecialType)
            {
                if (orderItemsHasCategory != null && orderItemsHasCategory.Any())
                {
                    bool isTheType = orderItemsHasCategory.Any(t => (t.Category.Type.GetHashCode() & dic.Value.GetHashCode()) > 0);
                    var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage();
                    orderVoyage.Order = new Order() { ID = orderID, };
                    orderVoyage.Type = dic.Key;
                    if (isTheType)
                    {
                        orderVoyage.Enter();
                    }
                    else
                    {
                        orderVoyage.Abandon();
                    }
                }
            }

            //该订单的型号归类完成后，判断该订单的特殊类型 高价值、商检、检疫、CCC End
        }

        private void DoProductControl(ClassifyProduct classifyProduct)
        {
            if (classifyProduct.IsCCC)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.CCC);
            }
            else if (classifyProduct.IsSysCCC)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.CCC, Enums.OrderControlStep.Headquarters);
            }

            if (classifyProduct.IsOriginProof)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.OriginCertificate);
            }

            if (classifyProduct.IsSysForbid || classifyProduct.IsForbid)
            {
                classifyProduct.OrderHangUp(Enums.OrderControlType.Forbid, Enums.OrderControlStep.Headquarters);
            }
        }
    }

    public partial class ClassifyProduct
    {
        /// <summary>
        /// 获取产品变更通知
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderItemChangeNotice> GetChangeNotices()
        {
            return new Views.Origins.OrderItemChangeNoticesOrigin().Where(item => item.OrderItemID == this.ID && item.Status == Status.Normal).ToArray();
        }

        /// <summary>
        /// 获取产品变更日志
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderItemChangeLog> GetChangeLogs()
        {
            return new Views.Origins.OrderItemChangeLogsOrigin()
                .Where(t => t.OrderItemID == this.ID)
                .OrderByDescending(t => t.CreateDate)
                .ToArray();
        }
    }
}

using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Hanlders;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类产品
    /// </summary>
    public sealed partial class PD_ClassifyProduct : OrderItem, Interfaces.PD_IClassifyProduct
    {
        #region 属性

        /// <summary>
        /// 订单类型
        /// </summary>
        public Enums.OrderType OrderType { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderedDate { get; set; }

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
        /// 是否属于海关验估编码
        /// </summary>
        public bool IsCustomsInspection { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        public Enums.ClassifyStep ClassifyStep { get; set; }

        #endregion

        #region 操作人/报关员

        public Admin Admin { get; set; }

        #endregion

        #region 事件

        public event ProductClassifiedHanlder Classified;

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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        ID = this.ImportTax.ID,
                        OrderItemID = this.ImportTax.OrderItemID,
                        Type = (int)this.ImportTax.Type,
                        Rate = this.ImportTax.Rate,
                        ReceiptRate = this.ImportTax.ReceiptRate,
                        Value = this.ImportTax.Value,
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Rate = this.AddedValueTax.Rate,
                        ReceiptRate = this.AddedValueTax.ReceiptRate,
                        Value = this.AddedValueTax.Value,
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
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Rate = this.ExciseTax.Rate,
                        ReceiptRate = this.ExciseTax.ReceiptRate,
                        Value = this.ExciseTax.Value,
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
                        Admin = this.Admin ?? this.Category.Declarant
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

        public JMessage Lock()
        {
            var setting = new PvDataApiSetting();
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.Lock;

            return Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                itemId = this.ID,
                creatorId = this.Admin.ID,
                creatorName = this.Admin.ByName,
                step = this.ClassifyStep
            });
        }

        public JMessage UnLock()
        {
            var setting = new PvDataApiSetting();
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.UnLock;

            return Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                itemId = this.ID,
                creatorId = this.Admin.ID,
                creatorName = this.Admin.ByName,
                step = this.ClassifyStep
            });
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
        }
    }
}

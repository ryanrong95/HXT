using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using YaHv.PvData.Services.Extends;
using YaHv.PvData.Services.Handlers;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 已下单的产品：代仓储、代报关
    /// </summary>
    public partial class OrderedProduct : Interfaces.IClassifyProduct, Interfaces.IProductConstraint
    {
        #region 属性

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// OrderID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderedDate { get; set; }

        /// <summary>
        /// 合同发票
        /// </summary>
        public string PIs { get; set; }

        /// <summary>
        /// 回发子系统的地址
        /// </summary>
        public string CallBackUrl { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 品牌/制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 客户自定义品名
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string TariffName { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 成交单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string LegalUnit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string LegalUnit2 { get; set; }

        decimal vatRate;

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal VATRate
        {
            get
            {
                return this.vatRate;
            }
            set
            {
                this.vatRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        decimal importPreferentialTaxRate;

        /// <summary>
        /// 进口优惠税率
        /// </summary>
        public decimal ImportPreferentialTaxRate
        {
            get
            {
                return this.importPreferentialTaxRate;
            }
            set
            {
                this.importPreferentialTaxRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        decimal originATRate;

        /// <summary>
        /// 产地加征税率
        /// </summary>
        public decimal OriginATRate
        {
            get
            {
                return this.originATRate;
            }
            set
            {
                this.originATRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        decimal exciseTaxRate;

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal ExciseTaxRate
        {
            get
            {
                return this.exciseTaxRate;
            }
            set
            {
                this.exciseTaxRate = decimal.Parse(value.ToString("0.#######"));
            }
        }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 是否需要3C认证
        /// </summary>
        public bool Ccc { get; set; }

        /// <summary>
        /// 是否是禁运产品
        /// </summary>
        public bool Embargo { get; set; }

        /// <summary>
        /// 是否是香港管制
        /// </summary>
        public bool HkControl { get; set; }

        /// <summary>
        /// 是否需要原产地证明
        /// </summary>
        public bool Coo { get; set; }

        /// <summary>
        /// 是否需要商检
        /// </summary>
        public bool CIQ { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal CIQprice { get; set; }

        /// <summary>
        /// 是否是高价值产品
        /// </summary>
        public bool IsHighPrice { get; set; }

        /// <summary>
        /// 是否需要消毒/检疫
        /// </summary>
        public bool IsDisinfected { get; set; }

        /// <summary>
        /// 是否系统判定3C
        /// </summary>
        public bool IsSysCcc { get; set; }

        /// <summary>
        /// 是否系统判定禁运
        /// </summary>
        public bool IsSysEmbargo { get; set; }

        /// <summary>
        /// 是否属于海关验估编码
        /// </summary>
        public bool IsCustomsInspection { get; set; }

        #endregion

        public string SupervisionRequirements { get; set; }

        public string CIQC { get; set; }


        #region 扩展属性

        public string CreatorID { get; set; }

        public string CreatorName { get; set; }

        public ClassifyStep Step { get; set; }

        public DeclarantRole Role { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        #endregion

        #region 归类完返回给子系统的数据

        /// <summary>
        /// 标准产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 税则归类结果ID
        /// </summary>
        public string CpnID { get; set; }

        #endregion

        #region 事件

        public event ProductLockedHandler Locked;
        public event ProductLockedHandler UnLocked;
        public event ProductClassifiedHandler Classified;
        public event ProductReturnedHandler Returned;

        #endregion

        public void DoClassify()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                this.ProductEnter(this, reponsitory);
                this.Log_ClassifyModifiedEnter(this, reponsitory);
                this.ClassifiedPartNumberEnter(this, reponsitory);
                this.OtherEnter(this, reponsitory);
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
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                //添加锁定
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Locks_Classify>().Any(t => t.MainID == this.ID))
                {
                    var lockId = Utils.GuidUtil.NewGuidUp();
                    reponsitory.Insert(new Layers.Data.Sqls.PvData.Locks_Classify()
                    {
                        ID = lockId,
                        MainID = this.ID,
                        LockDate = DateTime.Now,
                        LockerID = this.CreatorID,
                        LockerName = this.CreatorName,
                        Status = (int)GeneralStatus.Normal
                    });
                }
            }

            this.OnLocked();
        }

        void OnLocked()
        {
            if (this != null && this.Locked != null)
            {
                this.Locked(this, new ProductLockedEventArgs(this));
            }
        }

        public void UnLock()
        {
            using (var reponsitory = LinqFactory<PvDataReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvData.Locks_Classify>(item => item.MainID == this.ID);
            }

            this.OnUnLocked();
        }

        void OnUnLocked()
        {
            if (this != null && this.UnLocked != null)
            {
                this.UnLocked(this, new ProductLockedEventArgs(this));
            }
        }

        public void Return()
        {
            throw new NotImplementedException();
        }

        #region 产品归类业务逻辑处理

        /// <summary>
        /// 标准产品持久化
        /// </summary>
        /// <param name="op"></param>
        /// <param name="reponsitory"></param>
        private void ProductEnter(OrderedProduct op, PvDataReponsitory reponsitory)
        {
            var productId = this.GenOtherID();

            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Any(p => p.ID == productId))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.Products()
                {
                    ID = productId,
                    PartNumber = this.PartNumber,
                    Manufacturer = this.Manufacturer,
                    CreateDate = DateTime.Now
                });
            }

            op.ProductID = productId;
        }

        /// <summary>
        /// 归类变更日志持久化
        /// </summary>
        /// <param name="op">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        private void Log_ClassifyModifiedEnter(OrderedProduct op, PvDataReponsitory reponsitory)
        {
            var cpn = new Views.Alls.ClassifiedPartNumbersAll(reponsitory)[op.PartNumber, op.Manufacturer];
            if (cpn != null && op.CreatorID != null)
            {
                var histClassified = cpn.FillHistoryClassified();

                #region 记录日志

                //归类已完成列表导出Excel要求单独包含海关编码的变更记录，所以HSCode单独记日志
                if (op.HSCode != histClassified.HSCode)
                    op.Log("报关员【" + op.CreatorName + "】将型号【" + op.PartNumber + "】的海关编码由【" + histClassified.HSCode + "】修改为【" + op.HSCode + "】", reponsitory);

                StringBuilder sbLog = new StringBuilder();

                if (op.Manufacturer != histClassified.Manufacturer)
                    sbLog.Append("品牌由【" + histClassified.Manufacturer + "】修改为【" + op.Manufacturer + "】、");
                if (op.TariffName != histClassified.TariffName)
                    sbLog.Append("报关品名由【" + histClassified.TariffName + "】修改为【" + op.TariffName + "】、");
                if (op.TaxCode != histClassified.TaxCode)
                    sbLog.Append("税务编码由【" + histClassified.TaxCode + "】修改为【" + op.TaxCode + "】、");
                if (op.TaxName != histClassified.TaxName)
                    sbLog.Append("税务名称由【" + histClassified.TaxName + "】修改为【" + op.TaxName + "】、");
                if (op.ImportPreferentialTaxRate != histClassified.ImportPreferentialTaxRate)
                    sbLog.Append("优惠税率由【" + histClassified.ImportPreferentialTaxRate.ToString("#0.0000") + "】修改为【" + op.ImportPreferentialTaxRate + "】、");
                if (op.VATRate != histClassified.VATRate)
                    sbLog.Append("增值税率由【" + histClassified.VATRate.ToString("#0.0000") + "】修改为【" + op.VATRate + "】、");
                if (op.ExciseTaxRate != histClassified.ExciseTaxRate)
                    sbLog.Append("消费税率由【" + histClassified.ExciseTaxRate.ToString("#0.0000") + "】修改为【" + op.ExciseTaxRate + "】、");
                if (op.LegalUnit1 != histClassified.LegalUnit1)
                    sbLog.Append("法定第一单位由【" + histClassified.LegalUnit1 + "】修改为【" + op.LegalUnit1 + "】、");
                if (op.LegalUnit2 != null && op.LegalUnit2 != histClassified.LegalUnit2)
                    sbLog.Append("法定第二单位由【" + histClassified.LegalUnit2 + "】修改为【" + op.LegalUnit2 + "】、");
                if (op.CIQCode != histClassified.CIQCode)
                    sbLog.Append("检验检疫编码由【" + histClassified.CIQCode + "】修改为【" + op.CIQCode + "】、");
                if (op.Elements != histClassified.Elements)
                    sbLog.Append("申报要素由【" + histClassified.Elements + "】修改为【" + op.Elements + "】、");
                if (op.Ccc != histClassified.Ccc)
                    sbLog.Append("CCC认证由【" + (histClassified.Ccc ? "是" : "否") + "】修改为【" + (op.Ccc ? "是" : "否") + "】、");
                if (op.Coo != histClassified.Coo)
                    sbLog.Append("原产地证明由【" + (histClassified.Coo ? "是" : "否") + "】修改为【" + (op.Coo ? "是" : "否") + "】、");
                if (op.Embargo != histClassified.Embargo)
                    sbLog.Append("是否禁运由【" + (histClassified.Embargo ? "是" : "否") + "】修改为【" + (op.Embargo ? "是" : "否") + "】、");
                if (op.HkControl != histClassified.HkControl)
                    sbLog.Append("是否香港管制由【" + (histClassified.HkControl ? "是" : "否") + "】修改为【" + (op.HkControl ? "是" : "否") + "】、");
                if (op.CIQ != histClassified.CIQ)
                    sbLog.Append("是否商检由【" + (histClassified.CIQ ? "是" : "否") + "】修改为【" + (op.CIQ ? "是" : "否") + "】、");
                if (op.CIQprice != histClassified.CIQprice)
                    sbLog.Append("商检费由【" + histClassified.CIQprice.ToString("#0.0000") + "】修改为【" + op.CIQprice + "】、");

                if (sbLog.Length > 0)
                {
                    op.Log("报关员【" + op.CreatorName + "】将型号【" + op.PartNumber + "】的" + sbLog.ToString().TrimEnd('、'), reponsitory);
                }

                #endregion
            }
        }



        /// <summary>
        /// 税则归类结果持久化
        /// </summary>
        /// <param name="op"></param>
        /// <param name="reponsitory"></param>
        private void ClassifiedPartNumberEnter(OrderedProduct op, PvDataReponsitory reponsitory)
        {
            var cpnId = this.GenID();
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Any(t => t.ID == cpnId))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                {
                    ID = cpnId,
                    PartNumber = op.PartNumber,
                    Manufacturer = op.Manufacturer,
                    HSCode = op.HSCode,
                    Name = op.TariffName,
                    LegalUnit1 = op.LegalUnit1,
                    LegalUnit2 = op.LegalUnit2,
                    VATRate = op.VATRate,
                    ImportPreferentialTaxRate = op.ImportPreferentialTaxRate,
                    ExciseTaxRate = op.ExciseTaxRate,
                    Elements = op.Elements,
                    CIQCode = op.CIQCode,
                    TaxCode = op.TaxCode,
                    TaxName = op.TaxName,
                    CreateDate = DateTime.Now,
                    OrderDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>(new { OrderDate = DateTime.Now }, a => a.ID == cpnId);
            }

            op.CpnID = cpnId;
        }

        /// <summary>
        /// 归类特殊类型持久化
        /// </summary>
        /// <param name="op">产品</param>
        /// <param name="reponsitory">数据库连接</param>
        private void OtherEnter(OrderedProduct op, PvDataReponsitory reponsitory)
        {
            var id = this.GenOtherID();

            //添加
            if (!reponsitory.ReadTable<Layers.Data.Sqls.PvData.Others>().Any(t => t.PartNumber == op.PartNumber && t.Manufacturer == op.Manufacturer))
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvData.Others()
                {
                    ID = id,
                    PartNumber = op.PartNumber,
                    Manufacturer = op.Manufacturer,
                    Ccc = op.Ccc,
                    Embargo = op.Embargo,
                    HkControl = op.HkControl,
                    Coo = op.Coo,
                    CIQ = op.CIQ,
                    CIQprice = op.CIQprice,
                    CreateDate = DateTime.Now,
                    OrderDate = DateTime.Now
                });
            }
            //修改
            else
            {
                reponsitory.Update<Layers.Data.Sqls.PvData.Others>(new
                {
                    Ccc = op.Ccc,
                    Embargo = op.Embargo,
                    HkControl = op.HkControl,
                    Coo = op.Coo,
                    CIQ = op.CIQ,
                    CIQprice = op.CIQprice,
                    OrderDate = DateTime.Now
                }, a => a.PartNumber == op.PartNumber && a.Manufacturer == op.Manufacturer);
            }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Overall;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    public partial class ProductItem : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 标准产品ID
        /// </summary>
        public string StandardID
        {
            get; set;
        }

        /// <summary>
        /// 样品
        /// </summary>
        public Sample Sample { get; set; }

        /// <summary>
        /// 询价信息
        /// </summary>
        public IEnumerable<Enquiry> Enquiries { get; set; }

        public Enquiry Enquiry
        {
            get
            {
                return this.Enquiries?.OrderByDescending(item => item.UpdateDate).FirstOrDefault();
            }
        }
        /// <summary>
        /// 标准产品对象
        /// </summary>
        public StandardProduct standardProduct
        {
            get; set;
        }

        /// <summary>
        /// 竞争产品ID
        /// </summary>
        public string CompeteID { get; set; }

        /// <summary>
        /// 竞争产品对象
        /// </summary>
        public CompeteProduct CompeteProduct
        {
            get; set;
        }

        /// <summary>
        /// 单机用量
        /// </summary>
        public int RefUnitQuantity
        {
            get; set;
        }

        /// <summary>
        /// 项目用量
        /// </summary>
        public int RefQuantity
        {
            get; set;
        }

        /// <summary>
        /// 参考单价
        /// </summary>
        public decimal RefUnitPrice
        {
            get; set;
        }

        /// <summary>
        /// 预计成交概率
        /// </summary>
        public int ExpectRate
        {
            get; set;
        }

        /// <summary>
        /// 预计成交数量
        /// </summary>
        public int? ExpectQuantity { get; set; }

        public decimal expectTotal;
        /// <summary>
        /// 预计成交额
        /// </summary>
        public decimal ExpectTotal
        {
            get
            {
                if (expectTotal == 0)
                {
                    int q = ExpectQuantity ?? 0;
                    this.expectTotal = RefUnitPrice * q * 1000;
                }
                return this.expectTotal;
            }
            set
            {
                this.expectTotal = value;
            }
        }
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status
        {
            get; set;
        }

        /// <summary>
        /// 报备状态
        /// </summary>
        public bool? IsReport
        {
            get; set;
        }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime? ReportDate
        {
            get; set;
        }

        /// <summary>
        /// 预计成交日期
        /// </summary>
        public DateTime? ExpectDate
        {
            get; set;
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }

        /// <summary>
        /// 文件凭证
        /// </summary>
        public ProductItemFile[] Files
        {
            get; set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        #region  管理人员信息
        /// <summary>
        /// PM
        /// </summary>
        public string PMAdminID
        {
            get; set;
        }
        public string PMAdminName
        {
            get
            {
                return AdminExtends.GetTop(this.PMAdminID)?.RealName;
            }
        }

        /// <summary>
        /// FAE
        /// </summary>
        public string FAEAdminID
        {
            get; set;
        }
        public string FAEAdminName
        {
            get
            {
                return AdminExtends.GetTop(this.FAEAdminID)?.RealName;
            }
        }

        /// <summary>
        /// 销售
        /// </summary>
        public string SaleAdminID { get; set; }

        public string SaleAdminName
        {
            get
            {
                return AdminExtends.GetTop(this.SaleAdminID)?.RealName;
            }
        }

        /// <summary>
        /// 采购
        /// </summary>
        public string PurchaseAdminID { get; set; }

        public string PurchaseAdminName
        {
            get
            {
                return AdminExtends.GetTop(this.PurchaseAdminID)?.RealName;
            }
        }

        /// <summary>
        /// 销售助理
        /// </summary>
        public string AssistantAdiminID { get; set; }

        public string AssistantAdiminName
        {
            get
            {
                return AdminExtends.GetTop(this.AssistantAdiminID)?.RealName;
            }
        }
        #endregion


        #region 废弃字段
        /// <summary>
        /// 实际单价
        /// </summary>
        //public decimal? UnitPrice
        //{
        //    get; set;
        //}
        /// <summary>
        /// 实际用量
        /// </summary>
        //public int? Quantity
        //{
        //    get; set;
        //}

        /// <summary>
        /// 送样数量
        /// </summary>
        //public int? Count
        //{
        //    get; set;
        //}

        /// <summary>
        /// 原厂注册批复号
        /// </summary>
        //public string OriginNumber
        //{
        //    get; set;
        //}

        /// <summary>
        /// 参考总金额
        /// </summary>
        //public decimal RefTotalPrice
        //{
        //    get
        //    {
        //        return (this.RefUnitPrice * this.RefQuantity) / 10000;
        //    }
        //}

        /// <summary>
        /// 实际总金额
        /// </summary>
        //public decimal? TotalPrice
        //{
        //    get
        //    {
        //        return (this.Quantity * this.UnitPrice) / 10000;
        //    }
        //}
        #endregion

        /// <summary>
        /// 是否审批
        /// </summary>
        public bool IsApr
        {
            get; set;
        }

        public bool IsSample
        {
            get
            {
                if (this.Sample != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        public ProductItem()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;


        /// <summary>
        /// 删除出发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 逻辑删除数据
        /// </summary>
        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItems>(new
                {
                    Status = ProductStatus.DL
                }, item => item.ID == this.ID);
            }
        }


        /// <summary>
        /// 保存触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 产品项插入逻辑
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory(false))
            {
                this.HandleProduct(reponsitory);
                this.itemsEnter(reponsitory);
                this.HandleItemsExtends(reponsitory);
                this.FilesEnter(reponsitory);
                //提交修改
                reponsitory.Submit();
            }
        }

        /// <summary>
        /// 销售状态更新
        /// </summary>
        public void StatusEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItems>(new
                {
                    Status = this.Status,
                    ExpectRate = this.ExpectRate,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
        }



        #region 持久化方法
        /// <summary>
        /// 产品数据处理
        /// </summary>
        /// <param name="reponsitory"></param>
        private void HandleProduct(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            this.StandardEnter(reponsitory);
            this.StandardID = this.standardProduct.ID;
            if (!string.IsNullOrWhiteSpace(this.CompeteProduct.Name))
            {
                this.CompeteEnter(reponsitory);
                this.CompeteID = this.CompeteProduct.ID;
            }
            else if (!string.IsNullOrWhiteSpace(this.CompeteProduct.ID))
            {
                reponsitory.Delete<Layer.Data.Sqls.BvCrm.CompeteProducts>(item => item.ID == this.CompeteProduct.ID);
                this.CompeteID = null;
            }
        }

        /// <summary>
        /// 产品项拓展处理
        /// </summary>
        /// <param name="reponsitory"></param>
        private void HandleItemsExtends(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            if (this.Sample != null)
            {
                if (this.Sample.ID == null)
                {
                    this.Sample.ProductItemID = this.ID;
                }
                this.Sample.Enter();
            }
        }

        /// <summary>
        /// 标准产品录入
        /// </summary>
        /// <param name="reponsitory"></param>
        internal void StandardEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            this.standardProduct.ID = this.standardProduct.ID ?? string.Concat(standardProduct.Name + standardProduct.Manufacturer.Name).MD5();
            int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>().Count(item => item.ID == standardProduct.ID);
            if (count == 0)
            {
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.StandardProducts
                {
                    ID = this.standardProduct.ID,
                    Name = this.standardProduct.Name,
                    Origin = this.standardProduct.Origin,
                    ManufacturerID = this.standardProduct.Manufacturer.ID,
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.StandardProducts>(new
                {
                    this.standardProduct.Name,
                    ManufacturerID = this.standardProduct.Manufacturer.ID,
                    this.standardProduct.Origin,
                }, item => item.ID == this.standardProduct.ID);
            }
        }

        /// <summary>
        /// 竞争产品持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void CompeteEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            if (string.IsNullOrWhiteSpace(this.CompeteProduct.ID))
            {
                this.CompeteProduct.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.CompeteProducts
                {
                    ID = this.CompeteProduct.ID,
                    Name = this.CompeteProduct.Name,
                    Origin = this.CompeteProduct.Origin,
                    ManufacturerID = this.CompeteProduct.ManufacturerID ?? "",
                    Packaging = this.CompeteProduct.Packaging,
                    PackageCase = this.CompeteProduct.PackageCase,
                    Batch = this.CompeteProduct.Batch,
                    DateCode = this.CompeteProduct.DateCode,
                    CreateDate = DateTime.Now,
                    Currency = (int?)this.CompeteProduct.Currency,
                    UnitPrice = this.CompeteProduct.UnitPrice ?? 0m,
                    OriginNumber = this.CompeteProduct.OriginNumber,
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.CompeteProducts>(new
                {
                    Name = this.CompeteProduct.Name,
                    ManufacturerID = this.CompeteProduct.ManufacturerID ?? "",
                    UnitPrice = this.CompeteProduct.UnitPrice ?? 0,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 产品项主表持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void itemsEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                this.ID = PKeySigner.Pick(PKeyType.Product);
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItems
                {
                    ID = this.ID,
                    StandardID = this.StandardID,
                    CompeteID = this.CompeteID,
                    RefUnitQuantity = this.RefUnitQuantity,
                    RefQuantity = this.RefQuantity,
                    RefUnitPrice = this.RefUnitPrice,
                    ExpectRate = this.ExpectRate,
                    ExpectQuantity = this.ExpectQuantity,
                    ExpectDate = this.ExpectDate,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    PMAdmin = this.PMAdminID,
                    FAEAdmin = this.FAEAdminID,
                    SaleAdmin = this.SaleAdminID,
                    PurChaseAdmin = this.PurchaseAdminID,
                    AssistantAdmin = this.AssistantAdiminID,
                    Summary = this.Summary,
                    ExpectTotal = this.ExpectTotal
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItems>(new
                {
                    ID = this.ID,
                    StandardID = this.StandardID,
                    CompeteID = this.CompeteID,
                    RefUnitQuantity = this.RefUnitQuantity,
                    RefQuantity = this.RefQuantity,
                    RefUnitPrice = this.RefUnitPrice,
                    ExpectRate = this.ExpectRate,
                    ExpectQuantity = this.ExpectQuantity,
                    ExpectDate = this.ExpectDate,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    PMAdmin = this.PMAdminID,
                    FAEAdmin = this.FAEAdminID,
                    SaleAdmin = this.SaleAdminID,
                    PurChaseAdmin = this.PurchaseAdminID,
                    AssistantAdmin = this.AssistantAdiminID,
                    Summary = this.Summary,
                    ExpectTotal = this.ExpectTotal
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 样本拓展表持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        private void SampleEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            if (string.IsNullOrWhiteSpace(this.Sample.ID))
            {
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItemSamples
                {
                    ID = this.ID,
                    Type = (int)this.Sample.Type,
                    UnitPrice = this.Sample.UnitPrice,
                    Quantity = this.Sample.Quantity,
                    TotalPrice = this.Sample.TotalPrice,
                    Date = this.Sample.Date,
                    Contactor = this.Sample.Contactor,
                    Phone = this.Sample.Phone,
                    Address = this.Sample.Address,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItemSamples>(new
                {
                    Type = (int)this.Sample.Type,
                    UnitPrice = this.Sample.UnitPrice,
                    Quantity = this.Sample.Quantity,
                    TotalPrice = this.Sample.TotalPrice,
                    Date = this.Sample.Date,
                    Contactor = this.Sample.Contactor,
                    Phone = this.Sample.Phone,
                    Address = this.Sample.Address,
                    UpdateDate = DateTime.Now,
                }, item => item.ID == this.Sample.ID);
            }
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="reponsitory"></param>
        private void FilesEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            foreach (var file in Files)
            {
                if (string.IsNullOrWhiteSpace(file.ID))
                {
                    file.ID = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItemFiles
                    {
                        ID = file.ID,
                        ProductItemID = this.ID,
                        Type = (int)file.Type,
                        Name = file.Name,
                        Url = file.Url,
                        CreateDate = file.CreateDate,
                        Status = (int)file.Status,
                        AdminID = file.AdminID,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItemFiles>(new
                    {
                        Type = (int)file.Type,
                        Name = file.Name,
                        Url = file.Url,
                        CreateDate = DateTime.Now,
                        AdminID = file.AdminID,
                    }, item => item.ID == file.ID);
                }
            }
        }
        #endregion
    }


    public class ProductExtends
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string VendorID { get; set; }
        public decimal RefUnitPrice { get; set; }
        public int RefUnitQuantity { get; set; }
        public int RefQuantity { get; set; }
        public int ExpectRate { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public ProductStatus Status { get; set; }
        public int? Count { get; set; }
        public DateTime? ExpectDate { get; set; }
        public string CompeteManu { get; set; }
        public string CompeteModel { get; set; }
        public decimal? CompetePrice { get; set; }
        public string OriginNumber { get; set; }
        public string PMAdminID { get; set; }
        public string FAEAdminID { get; set; }
    }

}

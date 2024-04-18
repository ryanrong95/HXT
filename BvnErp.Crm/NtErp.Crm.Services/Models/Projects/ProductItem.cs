using Needs.Linq;
using Needs.Overall;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Projects
{
    /// <summary>
    /// 产品型号
    /// </summary>
    public class ProductItem : IUnique, IPersistence
    {

        //public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder AbandonError;
        //public event SuccessHanlder AbandonSuccess;

        public ProductItem()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        internal ProductItem(string projectID) : this()
        {
            this.ProjectID = projectID;
        }

        #region 属性

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 标准产品ID
        /// </summary>
        public string StandardID { get; set; }

        /// <summary>
        /// 竞争产品ID
        /// </summary>
        public string CompeteID { get; set; }

        /// <summary>
        /// 单机用量
        /// </summary>
        public int RefUnitQuantity { get; set; }

        /// <summary>
        /// 项目用量
        /// </summary>
        public int RefQuantity { get; set; }

        /// <summary>
        /// 参考单价
        /// </summary>
        public decimal RefUnitPrice { get; set; }

        /// <summary>
        /// 预计成交概率
        /// </summary>
        public int ExpectRate { get; set; }

        /// <summary>
        /// 预计成交数量
        /// </summary>
        public int? ExpectQuantity { get; set; }
        /// <summary>
        /// 预计成交日期
        /// </summary>
        public DateTime? ExpectDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// PM
        /// </summary>
        public string PMAdminID { get; set; }
        /// <summary>
        /// FAE
        /// </summary>
        public string FAEAdminID { get; set; }

        /// <summary>
        /// 销售
        /// </summary>
        public string SaleAdminID { get; set; }
        /// <summary>
        /// 采购
        /// </summary>
        public string PurchaseAdminID { get; set; }
        /// <summary>
        /// 销售助理
        /// </summary>
        public string AssistantAdiminID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
        #endregion

        #region 扩展属性

        decimal expectTotal;
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
        /// 样品
        /// </summary>
        public Sample Sample { get; set; }

        /// <summary>
        /// 标准产品
        /// </summary>
        public StandardProduct StandardProduct { get; set; }

        /// <summary>
        /// 竞争产品
        /// </summary>
        public CompeteProduct CompeteProduct { get; set; }


        #region 人员信息
        /// <summary>
        /// 销售
        /// </summary>
        public AdminTop SaleAdmin { get; set; }
        /// <summary>
        /// FEA
        /// </summary>
        public AdminTop FAEAdmin { get; set; }
        /// <summary>
        /// PM
        /// </summary>
        public AdminTop PMAdmin { get; set; }
        /// <summary>
        /// 采购
        /// </summary>
        public AdminTop PurChaseAdmin { get; set; }
        /// <summary>
        /// 销售助理
        /// </summary>
        public AdminTop AssistantAdmin { get; set; }

        #endregion

        /// <summary>
        /// 是否待审批
        /// </summary>
        public bool IsApr
        {
            get
            {
                using (var reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
                {
                    return reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>().Where(item => item.MainID == this.ID && item.Status == (int)ApplyStatus.Audting).Count() > 0;
                }
            }
        }

        /// <summary>
        /// 是否已送样
        /// </summary>
        public bool IsSample { get; set; }

        /// <summary>
        /// 询价集合
        /// </summary>
        public IEnumerable<Enquiry> Enquires { get; set; }

        /// <summary>
        /// 最新一条询价
        /// </summary>
        public Enquiry Enquiry
        {
            get
            {
                return Enquires.OrderByDescending(item => item.CreateDate).FirstOrDefault();
            }
        }

        /// <summary>
        /// 凭证
        /// </summary>
        public Models.ProductItemFile Voucher { get; set; }

        /// <summary>
        /// 销售机会ID
        /// </summary>
        public string ProjectID { get; internal set; }
        #endregion

        #region 持久化

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            throw new Exception("不支持调用");
            //if (string.IsNullOrWhiteSpace(this.ID))
            //{
            //    if (this != null && this.AbandonError != null)
            //    {
            //        this.AbandonError(this, new ErrorEventArgs("主键ID不能为空"));
            //    }
            //}

            ////this.OnAbandon();

            //if (this != null && this.AbandonSuccess != null)
            //{
            //    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            //}
        }

        /// <summary>
        /// 保存持久化
        /// </summary>
        public void Enter()
        {
            throw new Exception("不支持调用");
            //using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory(false))
            //{
            //    if (string.IsNullOrEmpty(this.ID))
            //    {
            //        this.StardandProductEnter(reponsitory);
            //        this.CompeteProductEnter(reponsitory);

            //        this.ID = PKeySigner.Pick(PKeyType.Product);
            //        reponsitory.Insert(new Layer.Data.Sqls.BvCrm.ProductItems
            //        {
            //            ID = this.ID,
            //            StandardID = this.StandardID,
            //            CompeteID = this.CompeteID,
            //            RefUnitQuantity = this.RefUnitQuantity,
            //            RefQuantity = this.RefQuantity,
            //            RefUnitPrice = this.RefUnitPrice,
            //            ExpectRate = this.ExpectRate,
            //            ExpectQuantity = this.ExpectQuantity,
            //            ExpectDate = this.ExpectDate,
            //            Status = (int)this.Status,
            //            CreateDate = this.CreateDate,
            //            UpdateDate = this.UpdateDate,
            //            PMAdmin = this.PMAdminID,
            //            FAEAdmin = this.FAEAdminID,
            //            SaleAdmin = this.SaleAdminID,
            //            PurChaseAdmin = this.PurchaseAdminID,
            //            AssistantAdmin = this.AssistantAdiminID,
            //            Summary = this.Summary,
            //            ExpectTotal = this.ExpectTotal
            //        });

            //        this.SampleEnter(reponsitory);
            //        this.ProductFileEnter(this.Voucher, reponsitory);
            //    }
            //    else
            //    {
            //        reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItems>(new
            //        {
            //            ID = this.ID,
            //            StandardID = this.StandardID,
            //            CompeteID = this.CompeteID,
            //            RefUnitQuantity = this.RefUnitQuantity,
            //            RefQuantity = this.RefQuantity,
            //            RefUnitPrice = this.RefUnitPrice,
            //            ExpectRate = this.ExpectRate,
            //            ExpectQuantity = this.ExpectQuantity,
            //            ExpectDate = this.ExpectDate,
            //            Status = (int)this.Status,
            //            CreateDate = this.CreateDate,
            //            UpdateDate = DateTime.Now,
            //            PMAdmin = this.PMAdminID,
            //            FAEAdmin = this.FAEAdminID,
            //            SaleAdmin = this.SaleAdminID,
            //            PurChaseAdmin = this.PurchaseAdminID,
            //            AssistantAdmin = this.AssistantAdiminID,
            //            Summary = this.Summary,
            //            ExpectTotal = this.ExpectTotal
            //        }, item => item.ID == this.ID);
            //    }

            //}
            //if (this != null && this.EnterSuccess != null)
            //{
            //    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            //}
        }

        /// <summary>
        /// 标准产品持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        protected void StardandProductEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            this.StandardID = this.StandardProduct.ID = this.StandardProduct.ID ?? string.Concat(StandardProduct.Name + StandardProduct.Manufacturer.Name).MD5();
            int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.StandardProducts>().Count(item => item.ID == StandardProduct.ID);
            if (count == 0)
            {
                reponsitory.Insert(new Layer.Data.Sqls.BvCrm.StandardProducts
                {
                    ID = this.StandardProduct.ID,
                    Name = this.StandardProduct.Name,
                    Origin = this.StandardProduct.Origin,
                    ManufacturerID = this.StandardProduct.Manufacturer.ID,
                    CreateDate = DateTime.Now
                });
            }
            else
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.StandardProducts>(new
                {
                    this.StandardProduct.Name,
                    ManufacturerID = this.StandardProduct.Manufacturer.ID,
                    this.StandardProduct.Origin,
                }, item => item.ID == this.StandardProduct.ID);
            }
        }

        /// <summary>
        /// 竞争产品持久化
        /// </summary>
        protected void CompeteProductEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
        {
            if (string.IsNullOrEmpty(this.CompeteProduct.Name))
            {
                if (string.IsNullOrEmpty(this.CompeteProduct.ID))
                {
                    reponsitory.Delete<Layer.Data.Sqls.BvCrm.CompeteProducts>(item => item.ID == this.CompeteProduct.ID);
                    this.CompeteID = null;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.CompeteProduct.ID))
                {
                    this.CompeteID = this.CompeteProduct.ID = Guid.NewGuid().ToString();
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

        }

        /// <summary>
        /// 送样信息持久化
        /// </summary>
        /// <param name="reponsitory"></param>
        protected void SampleEnter(Layer.Data.Sqls.BvCrmReponsitory reponsitory)
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

            //if (this.Sample.Type != null)
            //{
            //}
            //else if (!string.IsNullOrWhiteSpace(this.Sample.ID))
            //{
            //    reponsitory.Delete<Layer.Data.Sqls.BvCrm.ProductItemSamples>(item => item.ID == this.Sample.ID);
            //}
        }

        /// <summary>
        /// 文件信息持久阿虎
        /// </summary>
        /// <param name="file"></param>
        /// <param name="reponsitory"></param>
        protected void ProductFileEnter(ProductItemFile file, Layer.Data.Sqls.BvCrmReponsitory reponsitory)
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


        #endregion

        #region 业务方法

        public void Save()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory(false))
            {
                // 备注更改
                this.UpdateDate = DateTime.Now;
                reponsitory.Update<Layer.Data.Sqls.BvCrm.ProductItems>(new
                {
                    Summary = this.Summary,
                    UpdateDate = this.UpdateDate
                }, item => item.ID == this.ID);

                // 送样信息
                this.SampleEnter(reponsitory);

                reponsitory.Submit();
            }
        }

        #endregion
    }
}

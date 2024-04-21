using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.CrmPlus.Service.Extends;
using Layers.Data;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class StandardPartNumber : IUnique
    {
        public StandardPartNumber()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { set; get; }
        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { set; get; }
        /// <summary>
        /// 标准品牌ID
        /// </summary>
        public string BrandID { set; get; }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { internal set; get; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { set; get; }

        /// <summary>
        /// 封装
        /// </summary>
        public string PackageCase { set; get; }
        /// <summary>
        /// 包装
        /// </summary>
        public string Packaging { set; get; }
        /// <summary>
        /// 最小起订量
        /// </summary>
        public int Moq { set; get; }
        /// <summary>
        /// 包装数
        /// </summary>
        public int Mpq { set; get; }
        /// <summary>
        /// 税收分类
        /// </summary>
        public string TaxCode { set; get; }
        /// <summary>
        /// ECCN编码
        /// </summary>
        public string Eccn { set; get; }
        /// <summary>
        /// 关税率
        /// </summary>
        public decimal? TariffRate { set; get; }
        /// <summary>
        /// 是否3C
        /// </summary>
        public bool? Ccc { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { set; get; }
        /// <summary>
        /// 数据状态
        /// </summary>
        public DataStatus Status { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 型号分类
        /// </summary>
        public string Catalog { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { set; get; }
        public PcFile[] files { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        /// <summary>
        /// Repeat
        /// </summary>
        public event ErrorHanlder Repeat;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = reponsitory.OpenTransaction())
            {
                var entity = reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>().FirstOrDefault(item => item.PartNumber == this.PartNumber && item.BrandID == this.BrandID);
                if (string.IsNullOrWhiteSpace(this.ID) && entity == null)
                {
                    this.ID = PKeySigner.Pick(PKeyType.StandardPartNumber);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        if (this != null && this.Repeat != null)
                        {
                            this.Repeat(this, new ErrorEventArgs());
                            return;
                        }

                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>(new  {
                            PartNumber = this.PartNumber,
                            Brand = this.Brand,
                            ProductName = this.ProductName,
                            PackageCase = this.PackageCase,
                            Packaging = this.Packaging,
                            Moq = this.Moq,
                            Mpq = this.Mpq,
                            TaxCode = this.TaxCode,
                            Eccn = this.Eccn,
                            TariffRate = this.TariffRate,
                            Ccc = this.Ccc,
                            ModifyDate = DateTime.Now,
                            Status = (int)this.Status,
                            Summary = this.Summary,
                            Catalog = this.Catalog
                        }, item => item.ID == this.ID);
                    }
                }
                if (this.files != null && this.files.Length > 0)
                {
                    //清空附件
                    //reponsitory.Delete<Layers.Data.Sqls.PvdCrm.PcFiles>(item => item.MainID == this.ID);
                    reponsitory.Insert<Layers.Data.Sqls.PvdCrm.PcFiles>(files.Select(item => new Layers.Data.Sqls.PvdCrm.PcFiles()
                    {
                        ID = PKeySigner.Pick(PKeyType.PcFile),
                        Type = (int)item.Type,
                        MainID = this.ID,
                        AdminID = item.CreatorID,
                        Url = item.Url,
                        CreateDate = DateTime.Now,
                        CustomName = item.CustomName,
                    }));
                }
                tran.Commit();
                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
                this.EnterError?.Invoke(this, new ErrorEventArgs());
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.StandardPartNumbers>(new
                {
                    Status = (int)DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        #endregion
    }

}

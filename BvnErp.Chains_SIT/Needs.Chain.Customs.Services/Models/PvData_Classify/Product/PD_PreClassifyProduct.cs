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
    /// 预归类产品
    /// </summary>
    public sealed class PD_PreClassifyProduct : Interfaces.PD_IClassifyProduct
    {
        #region 属性

        public string ID { get; set; }

        /// <summary>
        /// 预归类标准产品
        /// </summary>
        public string PreProductID { get; set; }
        public PreProduct PreProduct { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal? AddedValueRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? ExciseTaxRate { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 归类类型
        /// </summary>
        public Enums.ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 归类状态：未归类、首次归类、归类完成，归类异常
        /// </summary>
        public Enums.ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

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

        public string Summary { get; set; }

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
        /// 
        /// </summary>
        public string ProductUnionCode { get; set; }
        /// <summary>
        /// 来源 大赢家字段
        /// </summary>
        public string Source { get; set; }

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
        /// 香港管控
        /// </summary>
        public bool IsHKForbid { get; set; }

        /// <summary>
        /// 是否属于海关验估编码
        /// </summary>
        public bool IsCustomsInspection { get; set; }

        /// <summary>
        /// 归类阶段
        /// </summary>
        public Enums.ClassifyStep ClassifyStep { get; set; }

        /// <summary>
        /// 是否需要推送状态提醒
        /// </summary>
        public bool IsPushStatusWarning { get; set; } = false;

        #endregion

        #region 操作人/报关员

        public Admin Admin { get; set; }
        /// <summary>
        /// 咨询归类录入人
        /// </summary>
        public string Register { get; set; }
        public string RegisterName { get; set; } = string.Empty;
        public string IcgooAdminName { get; set; }
        #endregion

        #region 事件

        public event ProductClassifiedHanlder Classified;

        #endregion

        public void DoClassify()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
                {
                    Model = this.Model,
                    Manufacture = this.Manufacturer,
                    ProductName = this.ProductName,
                    HSCode = this.HSCode,
                    TariffRate = this.TariffRate,
                    AddedValueRate = this.AddedValueRate,
                    ExciseTaxRate = this.ExciseTaxRate,
                    TaxCode = this.TaxCode,
                    TaxName = this.TaxName,
                    Type = (int)this.Type,
                    InspectionFee = this.InspectionFee,
                    Unit1 = this.Unit1,
                    Unit2 = this.Unit2,
                    CIQCode = this.CIQCode,
                    Elements = this.Elements,
                    ClassifyStatus = (int)this.ClassifyStatus,
                    Status = (int)this.Status,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary,
                }, item => item.ID == this.ID);
            }

            this.OnClassified();
        }

        public void QuickClassify()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
                {
                    ClassifySecondOperator = this.Admin.ID,
                    ClassifyStatus = (int)this.ClassifyStatus,
                    UpdateDate = DateTime.Now,
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
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.PreLock;

            return Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                mainId = this.ID,
                creatorId = this.Admin.ID,
                creatorName = this.Admin.ByName,
                step = this.ClassifyStep
            });
        }

        public JMessage UnLock()
        {
            var setting = new PvDataApiSetting();
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.PreUnLock;

            return Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                mainId = this.ID,
                creatorId = this.Admin.ID,
                creatorName = this.Admin.ByName,
                step = this.ClassifyStep
            });
        }

        public void Return()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProductCategories>(new
                {
                    ClassifyStatus = (int)Enums.ClassifyStatus.Anomaly,
                }, item => item.ID == this.ID);
            }

            var setting = new PvDataApiSetting();
            string apiurl = ConfigurationManager.AppSettings[setting.ApiName] + setting.PreReturn;

            Needs.Utils.Http.ApiHelper.Current.JPost<JMessage>(apiurl, new
            {
                mainId = this.ID,
                creatorId = this.Admin.ID,
                creatorName = this.Admin.ByName,
                step = this.ClassifyStep,
                summary = this.Summary
            });
        }

        public void Delete()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                string PreProductID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>().Where(t => t.ProductUnionCode == this.PreProduct.ProductUnionCode).FirstOrDefault().ID;
                if (!string.IsNullOrEmpty(PreProductID))
                {
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PreProductCategories>(item => item.PreProductID == PreProductID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PreProducts>(item => item.ID == PreProductID);
                }
            }
        }
    }
}

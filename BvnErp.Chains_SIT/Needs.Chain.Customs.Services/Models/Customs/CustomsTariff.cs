using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 海关税则
    /// </summary>
    [Needs.Underly.FactoryView(typeof(Views.CustomsTariffsView))]
    public class CustomsTariff : IUnique, IPersist, IFulError, IFulSuccess
    {
        string id;
        public string ID
        {
            get
            {
                ////主键编码规则：（HSCode）.MD5
                return this.id ?? string.Concat(this.HSCode).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 最惠国税率
        /// </summary>
        public decimal MFN { get; set; }

        /// <summary>
        /// 普通税率
        /// </summary>
        public decimal General { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal AddedValue { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? Consume { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 监管代码
        /// </summary>
        public string RegulatoryCode { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位（可空）
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 检验检疫编码（3位）
        /// </summary>
        public string CIQCode { get; set; }

        private IEnumerable<CustomsOriginTariff> originTariffs;

        /// <summary>
        /// 原产地税则
        /// </summary>
        public IEnumerable<CustomsOriginTariff> OriginTariffs
        {
            get
            {
                if (originTariffs == null)
                {
                    using (var view = new Views.CustomsOriginTariffsView())
                    {
                        this.originTariffs = view.Where(item => item.CustomsTariffID == this.ID && item.Status == Enums.Status.Normal).ToList();
                    }
                }

                return this.originTariffs;
            }
            set
            {
                this.originTariffs = value;
            }
        }

        private IEnumerable<ElementsDefault> elementsDefault;

        /// <summary>
        /// 申报要素默认值
        /// </summary>
        public IEnumerable<ElementsDefault> ElementsDefaults
        {
            get
            {
                if (elementsDefault == null)
                {
                    using (var view = new Views.CustomsElementsDefaultsView())
                    {
                        this.elementsDefault = view.Where(item => item.CustomsTariffID == this.ID).ToList();
                    }
                }

                return this.elementsDefault;
            }
            set
            {
                this.elementsDefault = value;
            }
        }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string InspectionCode { get; set; }

        public CustomsTariff()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CustomsTariffs>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.CustomsTariffs>(new {
                        ID = this.ID,
                        HSCode = this.HSCode,
                        Name = this.Name,
                        MFN = this.MFN,
                        General = this.General,
                        AddedValue = this.AddedValue,
                        Consume = this.Consume,
                        Elements = this.Elements,
                        RegulatoryCode = this.RegulatoryCode,
                        Unit1 = this.Unit1,
                        Unit2 = this.Unit2,
                        CIQCode = this.CIQCode,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                        InspectionCode = this.InspectionCode,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CustomsTariffs>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Model.Origins
{
    /// <summary>
    /// 原产地税率
    /// </summary>
    public class CustomsOriginTariff : IUnique, IPersist, IPersistence
    {
        public string ID { get; set; }

        public string CustomsTariffID { get; set; }

        /// <summary>
        /// 海关编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 原产地名称
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// 海关税则类型
        /// </summary>
        public Enums.CustomsRateType Type { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal Rate { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime StartDate { get; set; }

        public Nullable<DateTime> EndDate { get; set; }

        public string Summary { get; set; }

        public CustomsOriginTariff()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.Customs.CustomsOriginTariffs>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.Customs.CustomsOriginTariffs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            CustomsTariffID = this.CustomsTariffID,
                            Origin = this.Origin,
                            Type = (int)this.Type,
                            Rate = this.Rate,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            StartDate = this.StartDate,
                            EndDate = this.EndDate,
                            Summary = this.Summary
                        });
                    }
                    else
                    {
                        reponsitory.Update(new Layer.Data.Sqls.Customs.CustomsOriginTariffs
                        {
                            ID = this.ID,
                            CustomsTariffID = this.CustomsTariffID,
                            Origin = this.Origin,
                            Type = (int)this.Type,
                            Rate = this.Rate,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            StartDate = this.StartDate,
                            EndDate = this.EndDate,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                    }
                }
                this.OnEnter();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.CustomsOriginTariffs>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
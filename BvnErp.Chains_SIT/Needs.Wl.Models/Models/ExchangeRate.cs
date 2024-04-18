using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using Needs.Wl.Models.Hanlders;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 币种汇率
    /// </summary>
    public abstract class ExchangeRate : ModelBase<Layer.Data.Sqls.ScCustoms.ExchangeRates, ScCustomsReponsitory>, IUnique, IPersist
    {
        private string id;

        public new string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Code, this.Type.GetHashCode()).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 币种代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 币种名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 汇率类型：海关汇率、实时汇率
        /// </summary>
        public Enums.ExchangeRateType Type { get; set; }

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 当删除成功后发生
        /// </summary>
        public event SuccessHanlder AbandonSuccessed;

        /// <summary>
        /// 当删除失败后发生
        /// </summary>
        public event ErrorHanlder AbandonErrored;

        /// <summary>
        /// 当新增或编辑失败后发生
        /// </summary>
        public event ErrorHanlder EnterErrored;

        /// <summary>
        /// 当新增或编辑成功后发生
        /// </summary>
        public event SuccessHanlder EnterSuccessed;

        /// <summary>
        /// 当新增或编辑成功前发生
        /// </summary>
        public event SuccessHanlder Entering;

        /// <summary>
        /// 修改汇率前发生
        /// </summary>
        public event ExchangeRateChangingHanlder Changing;

        public ExchangeRate()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public ExchangeRate(Enums.ExchangeRateType type) : this()
        {
            this.Type = type;
        }

        public override void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExchangeRates>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ExchangeRates
                    {
                        ID = this.ID,
                        Code = this.Code,
                        Rate = this.Rate,
                        Type = (int)this.Type,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    });
                }
                else
                {
                    this.OnEntering();

                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ExchangeRates
                    {
                        ID = this.ID,
                        Code = this.Code,
                        Rate = this.Rate,
                        Type = (int)this.Type,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEntered();
        }

        virtual protected void OnEntered()
        {
            if (this != null && this.EnterSuccessed != null)
            {
                //成功后触发事件
                this.EnterSuccessed(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnEntering()
        {
            if (this != null && this.Entering != null)
            {
                //成功后触发事件
                this.Entering(this, new SuccessEventArgs(this));
            }
        }

        public override void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ExchangeRates>(item => item.ID == this.ID);
            }

            this.OnAbandoned();
        }

        virtual protected void OnAbandoned()
        {
            if (this != null && this.AbandonSuccessed != null)
            {
                this.AbandonSuccessed(this, new SuccessEventArgs(this));
            }
        }

        public void OnChanging()
        {
            if (this != null && this.Changing != null)
            {
                this.Changing(this, new ExchangeRateChangingEventArgs(this));
            }
        }
    }
}
using Needs.Linq;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 承运商
    /// </summary>
    [Serializable]
    public class Carrier : IUnique, IPersist
    {
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 联系人 ID
        /// </summary>
        public string ContactID { get; set; } = string.Empty;

        /// <summary>
        /// 承运商类型
        /// </summary>
        public Enums.CarrierType CarrierType { get; set; }

        /// <summary>
        /// 承运商名称(简称)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 查询标记
        /// </summary>
        public string QueryMark { get; set; } = string.Empty;

        /// <summary>
        /// 承运商名称(全称)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;
        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Carriers>(new Layer.Data.Sqls.ScCustoms.Carriers
                    {
                        ID = this.ID,
                        ContactID = this.ContactID,
                        CarrierType = (int)this.CarrierType,
                        Code = this.Code,
                        QueryMark = this.QueryMark,
                        Name = this.Name,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        Address = this.Address,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Carriers>(new
                    {
                        ID = this.ID,
                        ContactID = this.ContactID,
                        CarrierType = (int)this.CarrierType,
                        Code = this.Code,
                        QueryMark = this.QueryMark,
                        Name = this.Name,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        Address = this.Address,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Carriers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}

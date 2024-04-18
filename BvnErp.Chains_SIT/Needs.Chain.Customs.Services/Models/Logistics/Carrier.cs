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
    /// 承运商
    /// </summary>
    [Serializable]
    public class Carrier : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Code).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 承运商名称(简称)
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 查询标记
        /// </summary>
        public string QueryMark { get; set; }

        /// <summary>
        /// 承运商名称(全称)
        /// </summary>
        public string Name { get; set; }

        public Enums.CarrierType CarrierType { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        public string Summary { get; set; }

        public string Address { get; set; }

        public Carrier()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public void Enter()
        {
            this.Contact.Enter();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Carriers
                    {
                        ID = this.ID,
                        //  ContactID =string.IsNullOrWhiteSpace(this.Contact.ID)?ChainsGuid.NewGuidUp():this.Contact.ID,
                        ContactID = this.Contact.ID,
                        Code = this.Code,
                        QueryMark=this.QueryMark,
                        Name = this.Name,
                        CarrierType = (int)this.CarrierType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Address=this.Address,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Carriers
                    {
                        ID = this.ID,
                        ContactID = this.Contact.ID,
                        Name = this.Name,
                        Code = this.Code,
                        QueryMark=this.QueryMark,
                        CarrierType = (int)this.CarrierType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Address = this.Address,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Carriers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
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

        virtual public void OnEnterSuccess()
        {

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
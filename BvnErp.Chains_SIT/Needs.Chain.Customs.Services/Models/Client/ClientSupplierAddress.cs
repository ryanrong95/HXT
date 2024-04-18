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
    /// 客户供应商地址
    /// </summary>
    public class ClientSupplierAddress : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ClientSupplierID, this.Contact.ID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientSupplierID { get; set; }

        /// <summary>
        /// 是否默认提货地址
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string Place { get; set; }

        public ClientSupplierAddress()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        private void OnEnter()
        {
            this.Contact.Enter();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>().Count(item => item.ID == this.ID);
                //默认地址
                if (this.IsDefault)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new { IsDefault = false }, item => item.ClientSupplierID == this.ClientSupplierID);
                }
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new {

                        ID = this.ID,
                        ClientSupplierID = this.ClientSupplierID,
                        ContactID = this.Contact.ID,
                        Address = this.Address,
                        ZipCode = this.ZipCode,
                        IsDefault = this.IsDefault,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        Place=this.Place

                    }, item => item.ID == this.ID);
                }
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            //判定ID不能为空
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    //失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSupplierAddresses>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }
    }
}
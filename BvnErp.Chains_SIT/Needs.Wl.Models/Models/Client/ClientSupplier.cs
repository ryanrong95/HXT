using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户供应商
    /// </summary>
    public class ClientSupplier : ModelBase<Layer.Data.Sqls.ScCustoms.ClientSuppliers, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return this.id ?? Guid.NewGuid().ToString("N");
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        public ClientSupplier()
        {

        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

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
        public override void Abandon()
        {
            //判定ID不能为空
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                throw new Exception("未将对象设置对象的实例");
            }
            else
            {
                int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>().Count(item => item.ClientSupplierID == this.ID);
                if (count > 0)
                {
                    throw new Exception("不可删除已经下单的供应商信息");
                }

                count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>().Count(item => item.ClientSupplierID == this.ID);
                if (count > 0)
                {
                    throw new Exception("不可删除已经下单的供应商信息");
                }

                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSuppliers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                this.OnAbandonSuccess();
            }
        }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientSuppliers
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ClientID = this.ClientID,
                    Name = this.Name,
                    ChineseName = this.ChineseName,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                });
            }
            else
            {
                this.UpdateDate = DateTime.Now;
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientSuppliers
                {
                    ID = this.ID,
                    ClientID = this.ClientID,
                    Name = this.Name,
                    ChineseName = this.ChineseName,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary
                }, item => item.ID == this.ID);
            }

            this.OnEnterSuccess();
        }

        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
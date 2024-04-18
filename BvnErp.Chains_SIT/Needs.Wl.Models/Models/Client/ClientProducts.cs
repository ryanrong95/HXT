using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户产品
    /// </summary>
    public class ClientProducts : ModelBase<Layer.Data.Sqls.ScCustoms.ClientInvoices, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                return this.id ?? string.Concat(this.ClientID, this.Name, this.Model).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 产品品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 产品批次
        /// </summary>
        public string Batch { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        public ClientProducts()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = (int)Enums.Status.Normal;
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
                if (this != null && this.AbandonError != null)
                {
                    //失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientProducts>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);

            this.OnAbandonSuccess();
        }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProducts>().Count(item => item.ID == this.ID);

            if (count == 0)
            {
                this.Reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientProducts
                {
                    ID = this.ID,
                    ClientID = this.ClientID,
                    Name = this.Name,
                    Model = this.Model,
                    Manufacturer = this.Manufacturer,
                    Batch = this.Batch,
                    Status = (int)this.Status,
                    CreateDate = DateTime.Now,
                    UpdateDate = this.UpdateDate,
                });
            }
            else
            {
                this.UpdateDate = DateTime.Now;
                this.Reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientProducts
                {
                    ID = this.ID,
                    ClientID = this.ClientID,
                    Name = this.Name,
                    Model = this.Model,
                    Manufacturer = this.Manufacturer,
                    Batch = this.Batch,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
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

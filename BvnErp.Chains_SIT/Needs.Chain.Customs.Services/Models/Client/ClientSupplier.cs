using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户供应商
    /// </summary>
    public class ClientSupplier : IUnique, IPersist
    {
        string id;
        public string ID
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

        /// <summary>
        /// 供应商级别
        /// </summary>
        public Enums.SupplierGrade SupplierGrade { get; set; }

        /// <summary>
        /// 所属地区
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// PlaceType
        /// </summary>
        public int? PlaceType { get; set; }

        public bool IsShowClient { get; set; }

        /// <summary>
        /// 供应商银行账号
        /// </summary>
        public IEnumerable<ClientSupplierBank> Banks
        {
            get
            {
                using (var view = new Views.ClientSupplierBanksView())
                {
                    return view.Where(item => item.ClientSupplierID == this.ID && item.Status == Enums.Status.Normal).ToList();
                }
            }
        }

        /// <summary>
        /// 供应商提货地址
        /// </summary>
        public IEnumerable<ClientSupplierAddress> Addresses
        {
            get
            {
                using (var view = new Views.ClientSupplierAddressesView())
                {
                    return view.Where(item => item.ClientSupplierID == this.ID && item.Status == Enums.Status.Normal).ToList();
                }
            }
        }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientSupplier()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.IsShowClient = true;
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
            else
            {
                if (new Views.OrderConsigneesView().Any(t => t.ClientSupplier.ID == this.ID))
                {
                    //失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("该供应商已被订单使用，无法删除！"));
                }
                else
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientSuppliers>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                    }
                    this.OnAbandonSuccess();
                }
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientSuppliers
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        ClientID = this.ClientID,
                        Name = this.Name,
                        ChineseName = this.ChineseName,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Grade=(int)this.SupplierGrade,
                        Place=this.Place,
                        IsShowClient = this.IsShowClient,
                        Summary = this.Summary
                    });
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientSuppliers
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        Name = this.Name,
                        ChineseName = this.ChineseName,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        Grade = (int)this.SupplierGrade,
                        Place = this.Place,
                        IsShowClient = this.IsShowClient
                    }, item => item.ID == this.ID);
                }
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
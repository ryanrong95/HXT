using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.OrderAlls))]
    public class Order : IUnique, IPersistence
    {
        public string Address
        {
            get; set;
        }
        public AdminTop Admin
        {
            get; set;
        }

        public string AdminID
        {
            get
            {
                return this.Admin.ID;
            }
            set
            {
                this.Admin.ID = value;
            }
        }

        public Beneficiaries Beneficiaries
        {
            get; set;
        }

        public string BeneficiaryID
        {
            get
            {
                return this.Beneficiaries.ID;
            }
            set
            {
                this.Beneficiaries.ID = value;
            }
        }


        public string CatalogueID
        {
            get; set;
        }
        public Client Client
        {
            get; set;
        }
        public string ClientID
        {
            get
            {
                return this.Client.ID;
            }
            set
            {
                this.Client.ID = value;
            }
        }
        public Contact Contact
        {
            get; set;
        }
        public string ConsigneeID
        {
            get
            {
                return this.Contact.ID;
            }
            set
            {
                this.Contact.ID = value;
            }
        }

        public DateTime CreateDate
        {
            get; set;
        }

        public CurrencyType Currency
        {
            get; set;
        }

        public string DeliveryAddress
        {
            get; set;
        }

        public string ID
        {
            get; set;
        }

        public Status Status
        {
            get; set;
        }

        public DateTime UpdateDate
        {
            get; set;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        public Order()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }
        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Orders>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs(this.ID));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //创建订单分类
                    var catelogues = new Layer.Data.Sqls.BvCrm.Catalogues();
                    this.CatalogueID = catelogues.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Catelogues);
                    catelogues.CreateDate = catelogues.UpdateDate = DateTime.Now;
                    catelogues.Summary = "订单分类";
                    reponsitory.Insert(catelogues);
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Orders
                    {
                        ID = PKeySigner.Pick(PKeyType.Order),
                        CatalogueID=catelogues.ID,
                        ClientID = this.ClientID,
                        Currency = (int)this.Currency,
                        BeneficiaryID = this.BeneficiaryID,
                        DeliveryAddress = this.DeliveryAddress,
                        Address = this.Address,
                        ConsigneeID = this.ConsigneeID,
                        CreateDate = this.CreateDate,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Orders
                    {
                        ID = this.ID,
                        CatalogueID = this.CatalogueID,
                        ClientID = this.ClientID,
                        Currency = (int)this.Currency,
                        BeneficiaryID = this.BeneficiaryID,
                        DeliveryAddress = this.DeliveryAddress,
                        Address = this.Address,
                        ConsigneeID = this.ConsigneeID,
                        CreateDate = this.CreateDate,
                        AdminID = this.AdminID,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

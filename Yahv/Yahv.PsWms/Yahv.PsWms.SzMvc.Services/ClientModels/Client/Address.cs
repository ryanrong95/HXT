using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.ClientModels
{
    /// <summary>
    /// 客户收货/提货地址
    /// </summary>
    public class Address : IUnique
    {
        public Address()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = Underly.GeneralStatus.Normal;
        }

        #region 属性
        public string ID { get; set; }

        public AddressType Type { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 收货单位
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 联系人 收货人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 地址:”省\s市\s县\sAddress” 
        /// </summary>
        public string ClientAddress { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        public string Email { get; set; }

        public Underly.GeneralStatus Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }
        public bool IsDefault { get; set; }

        /// <summary>
        /// 省市区
        /// </summary>
        public string AddressCity { get; set; }
        /// <summary>
        /// 具体楼道
        /// </summary>
        public string AddressDetail { get; set; }
        #endregion

        //新增/编辑收货地址
        public void Enter()
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                int count = repository.ReadTable<Layers.Data.Sqls.PsOrder.Addresses>().Where(t => t.ID == this.ID).Count();
                if (count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Services.Enums.PKeyType.Address);

                    repository.Insert(new Layers.Data.Sqls.PsOrder.Addresses
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        ClientID = this.ClientID,
                        Title = this.Title,
                        Contact = this.Contact,
                        Address = this.ClientAddress,
                        Phone = this.Phone,
                        Email = this.Email,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        IsDefault = this.IsDefault,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsOrder.Addresses>(new
                    {
                        Title = this.Title,
                        Contact = this.Contact,
                        Address = this.ClientAddress,
                        Phone = this.Phone,
                        Email = this.Email,
                        ModifyDate = DateTime.Now,
                        IsDefault = this.IsDefault,
                    }, t => t.ID == this.ID);
                }
            }

        }

        public void Delete(string id)
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Update<Layers.Data.Sqls.PsOrder.Addresses>(new
                {
                    Status = Underly.GeneralStatus.Deleted,
                    ModifyDate = DateTime.Now,
                }, t => t.ID == id);

            }
        }

        /// <summary>
        /// 设置默认地址
        /// </summary>
        public void SetDefaultAddress()
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                repository.Update<Layers.Data.Sqls.PsOrder.Addresses>(new
                {
                    IsDefault = false,
                }, item => item.ClientID == this.ClientID && item.Type == (int)this.Type);

                repository.Update<Layers.Data.Sqls.PsOrder.Addresses>(new
                {
                    IsDefault = true,
                }, item => item.ID == this.ID);
            }
        }

    }
}

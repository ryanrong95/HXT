using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Extends;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class FixedSupplier : Yahv.Linq.IUnique
    {
        public FixedSupplier()
        {
            this.CreateDate = DateTime.Now;
        }
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 截单时间
        /// </summary>
        public TimeSpan? CutoffTime { set; get; }
        /// <summary>
        /// 询价议价方式
        /// </summary>
        public QuoteMethod QuoteMethod { set; get; }
        /// <summary>
        /// 发货地
        /// </summary>
        public string DeliveryPlace { set; get; }
        /// <summary>
        /// 货期
        /// </summary>
        public string DeliveryTime { set; get; }
        /// <summary>
        /// 批号确认方式
        /// </summary>
        public string BatchMethod { set; get; }
        /// <summary>
        /// 运费负担方
        /// </summary>
        public FreightPayer FreightPayer { set; get; }
        /// <summary>
        /// 最小起订金额
        /// </summary>
        public decimal? Mop { set; get; }
        /// <summary>
        /// 下单公司
        /// </summary>
        public string CompanyID { set; get; }
        /// <summary>
        /// 发票是否体现产地
        /// </summary>
        public bool? IsOriginPi { set; get; }
        /// <summary>
        /// 发票是否体现运单号
        /// </summary>
        public bool? IsWaybillPi { set; get; }
        /// <summary>
        /// 是否有发货通知单
        /// </summary>
        public bool? IsNotcieShiped { set; get; }
        /// <summary>
        /// 是否委托付款
        /// </summary>
        public bool? IsDelegatePay { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { internal set; get; }
        /// <summary>
        /// 运单号来源
        /// </summary>
        public string WaybillFrom { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { set; get; }
        public List<CallFile> PriceRules { set; get; }
        #endregion

        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            {

                if (repository.ReadTable<Layers.Data.Sqls.PvdCrm.FixedSuppliers>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvdCrm.FixedSuppliers>(new
                    {
                        CutoffTime = this.CutoffTime,
                        DeliveryPlace = this.DeliveryPlace,
                        DeliveryTime = this.DeliveryTime,
                        QuoteMethod = (int)this.QuoteMethod,
                        BatchMethod = this.BatchMethod,
                        FreightPayer = (int)this.FreightPayer,
                        Mop = this.Mop,
                        CompanyID = this.CompanyID,
                        IsDelegatePay = this.IsDelegatePay,
                        IsNotcieShiped = this.IsNotcieShiped,
                        IsOriginPi = this.IsOriginPi,
                        IsWaybillPi = this.IsWaybillPi,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        WaybillFrom = this.WaybillFrom
                    }, item => item.ID == this.ID);
                }
                else
                {
                    repository.Insert(new Layers.Data.Sqls.PvdCrm.FixedSuppliers
                    {
                        ID = this.ID,
                        CutoffTime = this.CutoffTime,
                        DeliveryPlace = this.DeliveryPlace,
                        DeliveryTime = this.DeliveryTime,
                        QuoteMethod = (int)this.QuoteMethod,
                        BatchMethod = this.BatchMethod,
                        FreightPayer = (int)this.FreightPayer,
                        Mop = this.Mop,
                        CompanyID = this.CompanyID,
                        IsDelegatePay = this.IsDelegatePay,
                        IsNotcieShiped = this.IsNotcieShiped,
                        IsOriginPi = this.IsOriginPi,
                        IsWaybillPi = this.IsWaybillPi,
                        CreateDate = this.CreateDate,
                        CreatorID = this.CreatorID,
                        WaybillFrom = this.WaybillFrom
                    });
                }
                if (this.PriceRules?.Count() > 0)
                {
                    List<FilesDescription> listFile = new List<FilesDescription>();
                    foreach (var item in this.PriceRules)
                    {
                        var file = new FilesDescription
                        {
                            EnterpriseID = this.ID,
                            SubID = this.ID,
                            CustomName = item.FileName,
                            Url = item.CallUrl,
                            Type = CrmFileType.PricingRules,
                            CreatorID = this.CreatorID
                        };
                        listFile.Add(file);
                    }
                    listFile.Enter();
                }

                tran.Commit();
            }
            this.EnterError?.Invoke(this, new ErrorEventArgs());
            this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        #endregion
    }
}

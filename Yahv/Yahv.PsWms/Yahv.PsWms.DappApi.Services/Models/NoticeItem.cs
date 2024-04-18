using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class NoticeItem : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 通知来源 NoticeSource: Keeper	库房 1 库管, Tracker	跟单 2 客服 
        /// </summary>
        public NoticeSource Source { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 自定义编码
        /// </summary>
        public string CustomCode { get; set; }

        /// <summary>
        /// 盘点类型
        /// </summary>
        public StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 最小包装量, 实际录入的如果是按个点数默认1
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 件数 实际录入的, 如果是按个点数默认, Total
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 总数, 计算的或是实际录入的
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额, 默认0
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string Supplier { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 所属单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 所属单据ID
        /// </summary>
        public string FormItemID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 库位
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 分拣人
        /// </summary>
        public string SorterID { get; set; }

        /// <summary>
        /// 捡货人
        /// </summary>
        public string PickerID { get; set; }

        /// <summary>
        /// 异常
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        #endregion

        public NoticeItem()
        {
            this.CreateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Enums.PKeyType.NoticeItem);

                    repository.Insert(new Layers.Data.Sqls.PsWms.NoticeItems()
                    {
                        ID = this.ID,
                        NoticeID = this.NoticeID,
                        Source = (int)this.Source,
                        ProductID = this.ProductID,
                        InputID = this.InputID,
                        CustomCode = this.CustomCode,
                        StocktakingType = (int)this.StocktakingType,
                        Mpq = this.Mpq,
                        PackageNumber = this.PackageNumber,
                        Total = this.Total,
                        Currency = (int)this.Currency,
                        UnitPrice = this.UnitPrice,
                        Supplier = this.Supplier,
                        ClientID = this.ClientID,
                        FormID = this.FormID,
                        FormItemID = this.FormItemID,
                        StorageID = this.StorageID,
                        ShelveID = this.ShelveID,
                        SorterID = this.SorterID,
                        PickerID = this.PickerID,
                        CreateDate = this.CreateDate,
                        Exception = this.Exception,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.NoticeItems>(new
                    {
                        this.NoticeID,
                        Source = (int)this.Source,
                        this.ProductID,
                        this.InputID,
                        this.CustomCode,
                        StocktakingType = (int)this.StocktakingType,
                        this.Mpq,
                        this.PackageNumber,
                        this.Total,
                        Currency = (int)this.Currency,
                        this.UnitPrice,
                        this.Supplier,
                        this.ClientID,
                        this.FormID,
                        this.FormItemID,
                        this.StorageID,
                        this.ShelveID,
                        this.SorterID,
                        this.PickerID,
                        this.Exception,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}

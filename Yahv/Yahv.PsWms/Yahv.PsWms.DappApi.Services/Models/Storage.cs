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
    public class Storage : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 通知ItemID
        /// </summary>
        public string NoticeItemID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 进项ID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// 库存类型 StorageType: Store 库存库 1, Flow 流水库 2, Park 暂存库 3, Ordering 在途库	4 订购期间物资, 在途物资, Scrap 报废库 100	
        /// </summary>
        public StorageType Type { get; set; }

        /// <summary>
        /// 所止库存, 库存锁止后，不能以任何理由操作
        /// </summary>
        public bool Islock { get; set; }

        /// <summary>
        /// 盘点类型 StocktakingType: Single 按个 1, MinPackage 最小包装 2
        /// </summary>
        public StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 最小包装量, 实际录入的如果是按个点数默认1
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 件数, 实际录入的, 如果是按个点数默认, Total
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 入库分拣人ID
        /// </summary>
        public string SorterID { get; set; }

        /// <summary>
        /// 入库单ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 入库单ItemID
        /// </summary>
        public string FormItemID { get; set; }

        /// <summary>
        /// 入库币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 入库单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 存储库位
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 异常备注
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 信息备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Unique { get; set; }

        #endregion

        public Storage()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.Storages>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Enums.PKeyType.Storage);

                    repository.Insert(new Layers.Data.Sqls.PsWms.Storages()
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        NoticeID = this.NoticeID,
                        NoticeItemID = this.NoticeItemID,
                        ProductID = this.ProductID,
                        InputID = this.InputID,
                        Type = (int)this.Type,
                        Islock = this.Islock,
                        StocktakingType = (int)this.StocktakingType,
                        Mpq = this.Mpq,
                        PackageNumber = this.PackageNumber,
                        Total = this.Total,
                        SorterID = this.SorterID,
                        FormID = this.FormID,
                        FormItemID = this.FormItemID,
                        Currency = (int)this.Currency,
                        UnitPrice = this.UnitPrice,
                        ShelveID = this.ShelveID,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Exception = this.Exception,
                        Summary = this.Summary,
                        Unique = this.ID,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {
                        this.ClientID,
                        this.NoticeID,
                        this.NoticeItemID,
                        this.ProductID,
                        this.InputID,
                        Type = (int)this.Type,
                        this.Islock,
                        StocktakingType = (int)this.StocktakingType,
                        this.Mpq,
                        this.PackageNumber,
                        this.Total,
                        this.SorterID,
                        this.FormID,
                        this.FormItemID,
                        Currency = (int)this.Currency,
                        this.UnitPrice,
                        this.ShelveID,
                        this.ModifyDate,
                        this.Exception,
                        this.Summary,
                        this.Unique,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}

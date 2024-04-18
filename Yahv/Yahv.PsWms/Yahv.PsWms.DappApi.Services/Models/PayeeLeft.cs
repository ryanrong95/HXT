using System;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    /// <summary>
    /// 应收信息
    /// </summary>
    public class PayeeLeft : IUnique
    {
        #region
        /// <summary>
        /// 唯一码 pick(time,4)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 发生地 AccountSource,   AccountSource: Keeper 库房 1, Tracker 跟单 2
        /// </summary>
        public AccountSource Source { get; set; }

        /// <summary>
        /// 付款公司ID, ClientID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 收款公司ID, 内部来自内部公司 默认：芯达通
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 业务, 枚举 Conduct, 本期默认业务为仓储
        /// </summary>
        public Conduct Conduct { get; set; }

        /// <summary>
        /// 科目分类, 参考《联创杰国内仓储服务协议2020-12-17》-附件：收费标准
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 币种, 枚举Currency
        /// </summary>
        public Underly.Currency Currency { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 总额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime CutDate { get; set; }

        /// <summary>
        /// 结算期号, 月结账单提取的依据 [202012]
        /// </summary>
        public int CutDateIndex { get; set; }

        /// <summary>
        /// 发生通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 发生单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 发生运单ID
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }
        #endregion

        public PayeeLeft()
        {
            this.CreateDate = this.CutDate = DateTime.Now;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.PayeeLefts>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(Enums.PKeyType.PayeeLeft);
                    repository.Insert(new Layers.Data.Sqls.PsWms.PayeeLefts()
                    {
                        ID = this.ID,
                        Source = (int)this.Source,
                        PayerID = this.PayerID,
                        PayeeID = this.PayeeID,
                        Conduct = (int)this.Conduct,
                        Subject = this.Subject,
                        Currency = (int)this.Currency,
                        Quantity = this.Quantity,
                        UnitPrice = this.UnitPrice,
                        Unit = this.Unit,
                        Total = this.Total,
                        CreateDate = this.CreateDate,
                        CutDate = this.CutDate,
                        CutDateIndex = this.CutDateIndex,
                        NoticeID = this.NoticeID,
                        FormID = this.FormID,
                        WaybillCode = this.WaybillCode,
                        AdminID = this.AdminID,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.PayeeLefts>(new
                    {
                        Source = (int)this.Source,
                        this.PayerID,
                        this.PayeeID,
                        Conduct = (int)Conduct,
                        this.Subject,
                        Currency = (int)this.Currency,
                        this.Quantity,
                        this.UnitPrice,
                        this.Unit,
                        this.Total,
                        this.CreateDate,
                        this.CutDate,
                        this.CutDateIndex,
                        this.NoticeID,
                        this.FormID,
                        this.WaybillCode,
                        this.AdminID,
                    }, t => t.ID == this.ID);
                }
            }
        }

        public void Abandon()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                repository.Delete<Layers.Data.Sqls.PsWms.PayeeLefts>(t => t.ID == this.ID);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    sealed public class Notice : IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 所属客户
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 内部公司，所属公司
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 通知类型: Inbound 入库 1, Outbound 出库 2, InAndOut 即入即出 3,
        /// </summary>
        public NoticeType NoticeType { get; set; }

        /// <summary>
        /// 来自的订单ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 交货人信息ID
        /// </summary>
        public string ConsignorID { get; set; }

        /// <summary>
        /// 收货人信息ID
        /// </summary>
        public string ConsigneeID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 通知状态
        /// </summary>
        public NoticeStatus Status { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 跟单
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 异常备注
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 信息备注
        /// </summary>
        public string Summary { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 交货人（交货人 + 提送货信息）
        /// </summary>
        public NoticeTransport Consignor { get; set; }

        /// <summary>
        /// 收货人（收货人 + 提送货信息）
        /// </summary>
        public NoticeTransport Consignee { get; set; }

        #endregion

        public Notice()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.Status = NoticeStatus.Processing;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsWmsRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsWms.Notices>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Notice);

                    repository.Insert(new Layers.Data.Sqls.PsWms.Notices()
                    {
                        ID = this.ID,
                        WarehouseID = this.WarehouseID,
                        ClientID = this.ClientID,
                        CompanyID = this.CompanyID,
                        NoticeType = (int)this.NoticeType,
                        FormID = this.FormID,
                        ConsignorID = this.ConsignorID,
                        ConsigneeID = this.ConsigneeID,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        Status = (int)this.Status,
                        WaybillID = this.WaybillID,
                        TrackerID = this.TrackerID,
                        Exception = this.Exception,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsWms.Notices>(new
                    {
                        this.WarehouseID,
                        this.ClientID,
                        this.CompanyID,
                        NoticeType = (int)this.NoticeType,
                        this.FormID,
                        this.ConsignorID,
                        this.ConsigneeID,
                        Status = (int)this.Status,
                        this.WaybillID,
                        this.TrackerID,
                        this.Exception,
                        this.Summary,
                        ModifyDate = DateTime.Now,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}

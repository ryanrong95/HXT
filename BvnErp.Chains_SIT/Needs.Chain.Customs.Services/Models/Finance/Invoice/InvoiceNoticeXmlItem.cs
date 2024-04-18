using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceNoticeXmlItem : IUnique, IFulError, IFulSuccess
    {
        public string ID { get; set; }
        /// <summary>
        /// 开票通知Xml ID
        /// </summary>
        public string InvoiceNoticeXmlID { get; set; }
        /// <summary>
        /// 订单项ID
        /// </summary>
        public string InvoiceNoticeItemID { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Xh { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string Spmc { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Ggxh { get; set; }
        /// <summary>
        /// 计量单位
        /// </summary>
        public string Jldw { get; set; }
        /// <summary>
        /// 商品编码
        /// </summary>
        public string Spbm { get; set; }
        /// <summary>
        /// 企业商品编码
        /// </summary>
        public string Qyspbm { get; set; }
        /// <summary>
        /// 优惠政策标识
        /// </summary>
        public string Syyhzcbz { get; set; }
        /// <summary>
        /// 零税率标识
        /// </summary>
        public string Lslbz { get; set; }
        /// <summary>
        /// 优惠政策说明
        /// </summary>
        public string Yhzcsm { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Dj { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Sl { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Je { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public decimal Slv { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal Se { get; set; }
        /// <summary>
        /// 扣除额
        /// </summary>
        public decimal? Kce { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public InvoiceNoticeXmlItem()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 数据插入
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeXmlItem);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems
                        {
                            ID = this.ID,
                            InvoiceNoticeXmlID = this.InvoiceNoticeXmlID,
                            InvoiceNoticeItemID = this.InvoiceNoticeItemID,
                            Xh = this.Xh,
                            Spmc = this.Spmc,
                            Ggxh = this.Ggxh,
                            Jldw = this.Jldw,
                            Spbm = this.Spbm,
                            Qyspbm = this.Qyspbm,
                            Syyhzcbz = this.Syyhzcbz,
                            Lslbz = this.Lslbz,
                            Yhzcsm = this.Yhzcsm,
                            Dj = this.Dj,
                            Sl = this.Sl,
                            Je = this.Je,
                            Slv = this.Slv,
                            Se = this.Se,
                            Kce = this.Kce,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,                           
                        });
                    }
                    else
                    {                       
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>(new
                        {
                            InvoiceNoticeXmlID = this.InvoiceNoticeXmlID,
                            InvoiceNoticeItemID = this.InvoiceNoticeItemID,
                            Xh = this.Xh,
                            Spmc = this.Spmc,
                            Ggxh = this.Ggxh,
                            Jldw = this.Jldw,
                            Spbm = this.Spbm,
                            Qyspbm = this.Qyspbm,
                            Syyhzcbz = this.Syyhzcbz,
                            Lslbz = this.Lslbz,
                            Yhzcsm = this.Yhzcsm,
                            Dj = this.Dj,
                            Sl = this.Sl,
                            Je = this.Je,
                            Slv = this.Slv,
                            Se = this.Se,
                            Kce = this.Kce,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                        }, item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }

    public class InvoiceNoticeXmlItems : BaseItems<InvoiceNoticeXmlItem>
    {
        internal InvoiceNoticeXmlItems(IEnumerable<InvoiceNoticeXmlItem> enums) : base(enums)
        {
        }

        internal InvoiceNoticeXmlItems(IEnumerable<InvoiceNoticeXmlItem> enums, Action<InvoiceNoticeXmlItem> action) : base(enums, action)
        {
        }

        public override void Add(InvoiceNoticeXmlItem item)
        {
            base.Add(item);
        }
    }
}

using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 开票通知明细项
    /// </summary>
    public class InvoiceNoticeItem : IUnique, IFulError, IFulSuccess
    {
        #region 数据库属性

        public string ID { get; set; }

        /// <summary>
        /// 开票通知ID
        /// </summary>
        public string InvoiceNoticeID { get; set; }

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        public OrderItem OrderItem { get; set; }

        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 含税总额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 差额
        /// </summary>
        public decimal Difference { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 开票日期
        /// </summary>
        public DateTime? InvoiceDate { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
        #endregion

        #region 扩展属性

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName
        {
            get { return this.OrderItem?.Category.TaxName; }
        }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode
        {
            get { return this.OrderItem?.Category.TaxCode; }
        }

        /// <summary>
        /// 开票税率
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        public Client Client { get; set; }

        /// <summary>
        /// 单价（不含税）
        /// </summary>
        public decimal SalesUnitPrice
        {
            get
            {
                if (this.OrderItem == null)
                {
                    return this.SalesTotalPrice;
                }
                return (this.SalesTotalPrice / this.OrderItem.Quantity).ToRound(4);
            }
        }

        /// <summary>
        /// 总额(不含税)
        /// </summary>
        public decimal SalesTotalPrice
        {
            get
            {
                return (this.Amount / (1 + this.InvoiceTaxRate)).ToRound(4);
            }
        }


        /// <summary>
        ///(不含税)开票明细总额
        /// </summary>
        public decimal DetailSalesTotalPrice
        {
            get
            {
                return ((this.Amount + this.Difference) / (1 + this.InvoiceTaxRate)).ToRound(4);
            }
        }

        /// <summary>
        /// （不含税）明细的单价
        /// </summary>
        public decimal DetailSalesUnitPrice
        {
            get
            {
                if (this.OrderItem == null)
                {
                    return this.DetailSalesTotalPrice;
                }
                return (this.DetailSalesTotalPrice / this.OrderItem.Quantity).ToRound(4);
            }
        }
        /// <summary>
        /// （含税）明细总额
        /// </summary>
        public decimal DetailAmount
        {
            get
            {
                return (this.Amount + this.Difference).ToRound(4);
            }
        }

        /// <summary>
        /// （含税）明细单价
        /// </summary>
        public decimal DetailUnitPrice
        {
            get
            {

                if (this.OrderItem == null)
                {
                    return this.DetailAmount;
                }
                return (this.DetailAmount / this.OrderItem.Quantity).ToRound(4);
            }
        }


        public  string  UnitName { get; set; }

        public DateTime? InvoiceTime { get; set; }

        public string ClientID { get; set; } = string.Empty;

        public decimal KaiPiaoJinE { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal Tax { get; set; }
        /// <summary>
        /// 不含税金额
        /// </summary>
        public decimal TaxFreeAmout { get; set; }
        public decimal InvoiceQty { get; set; }

        #endregion

        public InvoiceNoticeItem()
        {
            this.Difference = 0M;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
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
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeItem);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        //reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>(new
                        {
                            ID = this.ID,
                            InvoiceNoticeID = this.InvoiceNoticeID,
                            OrderID = this.OrderID,
                            OrderItemID = this.OrderItem?.ID,
                            UnitPrice = this.UnitPrice,
                            Amount = this.Amount,
                            Difference = this.Difference,
                            InvoiceCode = this.InvoiceCode,
                            InvoiceNo = this.InvoiceNo,
                            InvoiceDate = this.InvoiceDate,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary,
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

    public class InvoiceNoticeItems : BaseItems<InvoiceNoticeItem>
    {
        internal InvoiceNoticeItems(IEnumerable<InvoiceNoticeItem> enums) : base(enums)
        {
        }

        internal InvoiceNoticeItems(IEnumerable<InvoiceNoticeItem> enums, Action<InvoiceNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(InvoiceNoticeItem item)
        {
            base.Add(item);
        }
    }
}

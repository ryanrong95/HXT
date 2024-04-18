using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceNoticeXml : IUnique, IPersistence
    {
        public string ID { get; set; }
        /// <summary>
        /// 开票通知ID
        /// </summary>
        public string InvoiceNoticeID { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string Djh { get; set; }
        /// <summary>
        /// 购方名称
        /// </summary>
        public string Gfmc { get; set; }
        /// <summary>
        /// 购方税号
        /// </summary>
        public string Gfsh { get; set; }
        /// <summary>
        /// 购方银行账号
        /// </summary>
        public string Gfyhzh { get; set; }
        /// <summary>
        /// 购方地址电话
        /// </summary>
        public string Gfdzdh { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Bz { get; set; }
        /// <summary>
        /// 复核人
        /// </summary>
        public string Fhr { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Skr { get; set; }
        /// <summary>
        /// 商品编码版本号
        /// </summary>
        public string Spbmbbh { get; set; }
        /// <summary>
        /// 含税标志
        /// </summary>
        public string Hsbz { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
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
        /// <summary>
        /// 创建人
        /// </summary>
        public Admin Admin { get; set; }
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool? InCreSta { get; set; }
        public string InCreWord { get; set; }
        public string InCreNo { get; set; }
        public InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票通知明细项
        /// </summary>
        InvoiceNoticeXmlItems items;
        public InvoiceNoticeXmlItems InvoiceXmlItems
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.InvoiceNoticeXmlItemView())
                    {
                        var query = view.Where(item => item.InvoiceNoticeXmlID == this.ID);
                        this.InvoiceXmlItems = new InvoiceNoticeXmlItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.items = new InvoiceNoticeXmlItems(value, new Action<InvoiceNoticeXmlItem>(delegate (InvoiceNoticeXmlItem item)
                {
                    item.InvoiceNoticeXmlID = this.ID;
                }));
            }
        }

        public InvoiceNoticeXml()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(
                        new
                        {
                            UpdateDate = DateTime.Now,
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.InvoiceNoticeXml);
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls
                        {
                            ID = this.ID,
                            InvoiceNoticeID = this.InvoiceNoticeID,
                            Djh = this.Djh,
                            Gfmc = this.Gfmc,
                            Gfsh = this.Gfsh,
                            Gfyhzh = this.Gfyhzh,
                            Gfdzdh = this.Gfdzdh,
                            Bz = this.Bz,
                            Fhr = this.Fhr,
                            Skr = this.Skr,
                            Spbmbbh = this.Spbmbbh,
                            Hsbz = this.Hsbz,
                            FilePath = this.FilePath,
                            InvoiceCode = this.InvoiceCode,
                            InvoiceNo = this.InvoiceNo,
                            InvoiceDate = this.InvoiceDate,
                            AdminID = this.Admin.ID,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                        });
                    }
                    else
                    {
                        UpdateDate = DateTime.Now;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(new {
                            InvoiceNoticeID = this.InvoiceNoticeID,
                            Djh = this.Djh,
                            Gfmc = this.Gfmc,
                            Gfsh = this.Gfsh,
                            Gfyhzh = this.Gfyhzh,
                            Gfdzdh = this.Gfdzdh,
                            Bz = this.Bz,
                            Fhr = this.Fhr,
                            Skr = this.Skr,
                            Spbmbbh = this.Spbmbbh,
                            Hsbz = this.Hsbz,
                            FilePath = this.FilePath,
                            InvoiceCode = this.InvoiceCode,
                            InvoiceNo = this.InvoiceNo,
                            InvoiceDate = this.InvoiceDate,
                            AdminID = this.Admin.ID,
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
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }
    }

    public class InvoiceNoticeXmlCre : InvoiceNoticeXml
    {
        public decimal Je { get; set; }
        public decimal Se { get; set; }
    }
}

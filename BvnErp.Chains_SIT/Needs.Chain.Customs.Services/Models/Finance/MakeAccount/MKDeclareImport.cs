using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class MKDeclareImport : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }
        public string RequestID { get; set; }
        public string TemplateCode { get; set; }
        public string SchemeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Type { get; set; }
        public int InvoiceType { get; set; }
        public string DeclareDate { get; set; }
        public int Tian { get; set; }
        public decimal Jinkou { get; set; }
        public decimal? Huokuan { get; set; }
        public decimal? Yunbaoza { get; set; }

        public decimal Guanshui { get; set; }
        public decimal GuanshuiShijiao { get; set; }
        public decimal Xiaofeishui { get; set; }
        public decimal XiaofeishuiShijiao { get; set; }
        public decimal? Shui { get; set; }
        public decimal? Jinxiangshui { get; set; }
        public decimal HuiduiSanfang { get; set; }
        public string Sanfang { get; set; }
        public decimal? HuiduiWofang { get; set; }
        public decimal Huilv { get; set; }
        public decimal? YingfuSanfang { get; set; }
        public string Wuliufang { get; set; }
        public decimal? YingfuWofang { get; set; }
        public string Currency { get; set; }
        public string PingzhengZi { get; set; }
        public string PingzhengHao { get; set; } 
        public Enums.Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

        #endregion

        public MKDeclareImport()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MKDeclareImport>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {

                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.MKDeclareImport
                        {
                            ID = this.ID,
                            TemplateCode = this.TemplateCode,
                            SchemeCode = this.SchemeCode,
                            Type = this.Type,
                            InvoiceType = this.InvoiceType,
                            DeclareDate = this.DeclareDate,
                            Tian = this.Tian,
                            Jinkou = this.Jinkou,
                            Huokuan = this.Huokuan,
                            Yunbaoza = this.Yunbaoza,
                            Guanshui = this.Guanshui,
                            GuanshuiShijiao = this.GuanshuiShijiao,
                            Xiaofeishui = this.Xiaofeishui,
                            XiaofeishuiShijiao = this.XiaofeishuiShijiao,
                            Shui = this.Shui,
                            Jinxiangshui = this.Jinxiangshui,
                            HuiduiSanfang = this.HuiduiSanfang,
                            Sanfang = this.Sanfang,
                            HuiduiWofang = this.HuiduiWofang,
                            Huilv = this.Huilv,
                            YingfuSanfang = this.YingfuSanfang,
                            Wuliufang = this.Wuliufang,
                            YingfuWofang = this.YingfuWofang,
                            Currency = this.Currency,
                            PingzhengZi = this.PingzhengZi,
                            PingzhengHao = this.PingzhengHao,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary
                        });
                    }
                    else
                    {
                        reponsitory.Update(new Layer.Data.Sqls.ScCustoms.MKDeclareImport
                        {
                            ID = this.ID,
                            TemplateCode = this.TemplateCode,
                            SchemeCode = this.SchemeCode,
                            Type = this.Type,
                            InvoiceType = this.InvoiceType,
                            DeclareDate = this.DeclareDate,
                            Tian = this.Tian,
                            Jinkou = this.Jinkou,
                            Huokuan = this.Huokuan,
                            Yunbaoza = this.Yunbaoza,
                            Guanshui = this.Guanshui,
                            GuanshuiShijiao = this.GuanshuiShijiao,
                            Xiaofeishui = this.Xiaofeishui,
                            XiaofeishuiShijiao = this.XiaofeishuiShijiao,
                            Shui = this.Shui,
                            Jinxiangshui = this.Jinxiangshui,
                            HuiduiSanfang = this.HuiduiSanfang,
                            Sanfang = this.Sanfang,
                            HuiduiWofang = this.HuiduiWofang,
                            Huilv = this.Huilv,
                            YingfuSanfang = this.YingfuSanfang,
                            Wuliufang = this.Wuliufang,
                            YingfuWofang = this.YingfuWofang,
                            Currency = this.Currency,
                            PingzhengZi = this.PingzhengZi,
                            PingzhengHao = this.PingzhengHao,
                            Status = (int)this.Status,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Summary = this.Summary
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
        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.MKDeclareImport>(
                        new
                        {
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
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}

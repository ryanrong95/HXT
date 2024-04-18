using Needs.Ccs.Services.Enums;
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
    /// 客户自定义的产品税务归类
    /// </summary>
    public class ClientProductTaxCategory : IUnique, IPersist, Interfaces.IProduct
    {
        #region 属性

        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.TaxCode, this.TaxName).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 客户编号
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 税务状态
        /// </summary>
        public ProductTaxStatus TaxStatus { get; set; }

        /// <summary>
        /// 数据状态: 正常、删除
        /// </summary>
        public Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion
    
        public ClientProductTaxCategory()
        {
            Status = Enums.Status.Normal;
            this.TaxStatus = ProductTaxStatus.Auditing;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            //判定ID不能为空
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    //失败触发事件
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        public void Approve()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>(new { TaxStatus = Enums.ProductTaxStatus.Audited}, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 审核未通过
        /// </summary>
        public void Reject()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>(new { TaxStatus = Enums.ProductTaxStatus.NotPass }, item => item.ID == this.ID);
            }
        }
    }
}
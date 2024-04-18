using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户自定义的产品税务归类
    /// </summary>
    public class ClientProductTaxCategory : ModelBase<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories, ScCustomsReponsitory>, IUnique, IPersist
    {
        #region 属性

        string id;
        public new string ID
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
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 品名
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
        public Needs.Wl.Models.Enums.ProductTaxStatus TaxStatus { get; set; }

        #endregion

        public ClientProductTaxCategory()
        {
            Status = (int)Enums.Status.Normal;
            this.TaxStatus = Enums.ProductTaxStatus.Auditing;
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
        public override void Enter()
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
        public override void Abandon()
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
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientProductTaxCategories>(new { TaxStatus = Enums.ProductTaxStatus.Audited }, item => item.ID == this.ID);
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

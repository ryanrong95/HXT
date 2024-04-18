using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单项商品归类
    /// </summary>
    public class OrderItemCategory : IUnique, IPersist, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                //主键ID（OrderItemID）.MD5 -- 一个OrderItem对应一条归类信息
                return this.id ?? string.Concat(this.OrderItemID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string OrderItemID { get; set; }

        /// <summary>
        /// 报关员
        /// </summary>
        public string DeclarantID { get; set; }
        public Admin Declarant { get; set; }

        /// <summary>
        /// 预归类一人员
        /// </summary>
        public string ClassifyFirstOperatorID { get; set; }
        public Admin ClassifyFirstOperator { get; set; }

        /// <summary>
        /// 预归类二人员
        /// </summary>
        public string ClassifySecondOperatorID { get; set; }
        public Admin ClassifySecondOperator { get; set; }

        /// <summary>
        /// 产品归类：普通，商检，3C，原产地证明、禁运、检疫等
        /// </summary>
        public Enums.ItemCategoryType Type { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 海关编码\商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 报关品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位（可空）
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 法定第一数量（必填）
        /// </summary>
        public decimal? Qty1 { get; set; }

        /// <summary>
        /// 法定第二数量（非必填）
        /// </summary>
        public decimal? Qty2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public OrderItemCategory()
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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Count(item => item.ID == this.ID);

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
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 产品特殊类型
        /// </summary>
        /// <returns></returns>
        public string GetSpecialTypeForClassify()
        {
            if (this.Type == Enums.ItemCategoryType.Normal)
            {
                return "--";
            }
            else
            {
                StringBuilder specialType = new StringBuilder();
                foreach (Enums.ItemCategoryType type in Enum.GetValues(typeof(Enums.ItemCategoryType)))
                {
                    //20191113 与魏晓毅确认，归类预处理二列表界面特殊类型只显示归类界面上有的类型
                    if (type == Enums.ItemCategoryType.Quarantine)
                        continue;

                    if ((this.Type & type) > 0)
                    {
                        specialType.Append(type.GetDescription() + "|");
                    }
                }
                if (specialType.Length > 0)
                    return specialType.ToString().TrimEnd('|');
                else
                    return "--";
            }
        }

        /// <summary>
        /// 产品特殊类型
        /// </summary>
        /// <returns></returns>
        public string GetSpecialTypeForHKWarehouse()
        {
            if (this.Type == Enums.ItemCategoryType.Normal)
            {
                return "--";
            }
            else
            {
                StringBuilder specialType = new StringBuilder();
                foreach (Enums.ItemCategoryType type in Enum.GetValues(typeof(Enums.ItemCategoryType)))
                {
                    if ((this.Type & type) > 0)
                    {
                        specialType.Append(type.GetDescription() + "|");
                    }
                }
                return specialType.ToString().TrimEnd('|');
            }
        }

    }
}

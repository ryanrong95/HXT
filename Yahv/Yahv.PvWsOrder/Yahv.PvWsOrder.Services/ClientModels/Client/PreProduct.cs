using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.ClientModels.Client
{
    /// <summary>
    /// 预归类产品
    /// </summary>
    public class PreProduct : Yahv.Linq.IUnique
    {
        public PreProduct()
        {
            this.EnterSuccess += On_EnterSuccess;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = (int)GeneralStatus.Normal;
        }

        #region 扩展属性
        /// <summary>
        /// 预归类ID
        /// </summary>
        public string PreProductCategoryID { get; set; }

        /// <summary>
        /// 归类状态
        /// </summary>
        public ClassifyStatus ? ClassifyStatus { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }
        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder AbandonError;

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_EnterSuccess(object sender, SuccessEventArgs e)
        {

        }

        #endregion

        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 物料号
        /// </summary>
        public string ProductUnionCode { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Qty { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        public string Supplier { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 数据状态
        /// //TODO:需要在框架中定义枚举
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 描述，备注
        /// </summary>
        public string Summary { get; set; }

        public CompanyTypeEnums CompanyType { get; set; }

        public string Description { get; set; }

        public string Pack { get; set; }

        public string AreaOfProduction { get; set; }

        public string UseFor { get; set; }

        public DateTime? DueDate { get; set; }

        public PreProductUserType UseType { get; set; }

        #endregion

        public enum PreProductUserType
        {
            [Description("预归类")]
            Pre = 1,

            [Description("咨询")]
            Consult = 2
        }

        #region 持久化

        /// <summary>
        /// 数据删除触发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layers.Data.Sqls.ScCustomReponsitory reponsitory = new Layers.Data.Sqls.ScCustomReponsitory())
            {
                reponsitory.Update<Layers.Data.Sqls.ScCustoms.PreProducts>(new
                {
                    Status = GeneralStatus.Deleted
                }, item => item.ID == this.ID);
            }
        }
        #endregion
    }

    /// <summary>
    /// 香港交货方式
    /// </summary>
    public enum CompanyTypeEnums
    {
        /// <summary>
        /// 内单
        /// </summary>
        [Description("内单")]
        Inside = 1,

        /// <summary>
        /// Icgoo等公司
        /// </summary>
        [Description("Icgoo")]
        Icgoo = 2,

        /// <summary>
        /// 外单
        /// </summary>
        [Description("外单")]
        OutSide = 3,

        /// <summary>
        /// Icgoo等公司
        /// </summary>
        [Description("快包")]
        FastBuy = 4,
    }
}

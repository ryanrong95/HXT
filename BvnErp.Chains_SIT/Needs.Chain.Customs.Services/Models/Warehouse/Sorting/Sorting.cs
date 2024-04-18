using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 分拣结果、到货确认、库存
    /// </summary>
    [Serializable]
    public class Sorting : IUnique, IPersist, IFulError, IFulSuccess
    {
        public string ID { get; set; }

        /// <summary>
        /// 分拣人
        /// </summary>
        public string AdminID { get; set; }

        public string OrderID { get; set; }

        public string EntryNoticeItemID { get; set; }

        public Enums.WarehouseType WarehouseType { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public string WrapType { get; set; }

        /// <summary>
        /// 进项
        /// </summary>
        public OrderItem OrderItem { get; set; }

        /// <summary>
        /// 流转箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal GrossWeight { get; set; }

        /// <summary>
        /// 报关状态
        /// </summary>
        public Enums.SortingDecStatus DecStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 深圳出库装箱日期 表中新增字段-20180420
        /// </summary>
        public DateTime? SZPackingDate { get; set; }

        /// <summary>
        /// 20190408添加
        /// </summary>
        public Interfaces.IOrder Order { get; set; }

        #region 扩展字段

        //TODO:@董健，这里是Sorting对象，如果要使用，不要这样拓展！
        //把Sorting中的库位查询拿出去！尤其是View中的

        ///// <summary>
        ///// 客户编号
        ///// </summary>
        //[Obsolete]
        //public string ClientCode { get; set; }

        ///// <summary>
        ///// 币种
        ///// </summary>
        //[Obsolete]
        //public string Currency { get; set; }

        #endregion

        public Sorting()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.DecStatus = Enums.SortingDecStatus.No;
            this.Status = Enums.Status.Normal;
        }

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Sorting);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }

        /// <summary>
        /// 去持久化
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess();
            }
            catch(Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
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

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 修改箱号
        /// </summary>
        /// <param name="boxIndex"></param>
        public void ChangeBoxIndex(string boxIndex)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Sortings
                {
                    UpdateDate = DateTime.Now,
                    BoxIndex = boxIndex,
                }, item => item.ID == this.ID);

                //更新库存中的箱号
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.StoreStorages
                {
                    UpdateDate = DateTime.Now,
                    BoxIndex = boxIndex,
                }, t => t.SortingID == this.ID);
            }
        }
    }
}
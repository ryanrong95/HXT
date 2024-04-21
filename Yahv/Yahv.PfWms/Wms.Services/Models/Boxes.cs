using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Wms.Services.Views;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;

namespace Wms.Services.Models
{
    /// <summary>
    /// 装箱单
    /// </summary>
    public class Boxes : IUnique, IPersisting
    {

        public Boxes()
        {

        }

        IErpAdmin admin;
        public Boxes(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #region 属性
        /// <summary>
        /// 编号：年(4)+月(2)+日(2)+4位流水
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// 箱号：前辍+6位流水（每天都从1开始）
        /// </summary>
        public string Code { get; internal set; }


        /// <summary>
        /// 创建人编号
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 创建库房编号
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; internal set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 状态：200正常，400 删除
        /// </summary>
        public BoxesStatus Status { get; set; }



        #endregion

        #region 扩展属性

        public bool _disabled
        {
            get { return this.Status == BoxesStatus.Declared; }
        }

        /// <summary>
        /// 箱号前辍
        /// </summary>
        public string CodePrefix { get; set; }


        /// <summary>
        /// 日期前辍(yyyy-MM-dd)
        /// </summary>
        public string DateStr { get; set; }

        /// <summary>
        /// Status的枚举描述
        /// </summary>
        public string StatusDescription
        {
            get
            {
                return this.Status.GetDescription();
            }
        }

        public string AdminName
        {
            get
            {
                return new AdminsView().Where(item => item.ID == AdminID).FirstOrDefault().RealName;
            }
        }

        #endregion

        #region 废弃
        public void Abandon()
        {
            using (var repository = new PvWmsRepository())
            {
                this.Status = BoxesStatus.Deleted;
                repository.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        #endregion

        #region 事件
        public event SuccessHanlder BoxesSuccess;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        #endregion

        #region 持久化
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    var IDPrefix = string.Concat(this.CodePrefix, this.WarehouseID, this.DateStr.Replace("-", ""));
                    this.ID = PKeySigner.Pick(PkeyType.Boxes, IDPrefix);
                    this.AdminID = admin.ID;
                    this.Status = BoxesStatus.WaitingApply;
                    this.Code = string.Concat(this.CodePrefix, this.DateStr.Replace("-", "").Substring(2), this.ID.Replace(IDPrefix, ""));
                    this.CreateDate = DateTime.Now;

                    repository.Insert(this.ToLinq());
                }

                BoxesSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch (Exception ex)
            {
                EnterError?.Invoke(this, new ErrorEventArgs(ex.Message, ErrorType.System));
            }
        }
        #endregion
    }

    public class BoxesPickingNotices
    {

        public string TinyOrderID { get; set; }

        public BoxesNotices[] BoxesNotices { get; set; }
    }

    public class BoxesNotices
    {
        public string ClientID { get; set; }
        public string ClientName { get; set; }

        public Boxes Boxes { get; set; }

        public CustomPickingNotice[] PickingNotice { get; set; }
        //public BoxOrder[] Order { get; set; }
    }

    //public class BoxOrder
    //{
    //    //public string OrderID { get; set; }
    //    //public Yahv.Services.Models.PickingNotice[] PickingNotice { get; set; }
    //    public CustomPickingNotice[] PickingNotice { get; set; }
    //}

    public class CustomPickingNotice : Notice
    {
        //public CenterProduct Product { get; set; }
        public Output Output { get; set; }

    }

    public class BoxesSortingNotices
    {
        public Yahv.Services.Models.SortingNotice[] SortingNotice { get; set; }
    }
}

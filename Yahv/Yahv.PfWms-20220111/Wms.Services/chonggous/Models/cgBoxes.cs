using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.chonggous.Extends;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;
using Yahv.Linq;
using Yahv.Linq.Persistence;

namespace Wms.Services.chonggous.Models
{
    public class cgBoxes : IUnique, IPersisting
    {
        #region 构造函数

        IErpAdmin admin;
        public cgBoxes()
        {
        }

        public cgBoxes(IErpAdmin admin)
        {
            this.admin = admin;
        }

        #endregion

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

        /// <summary>
        /// 创建人
        /// </summary>
        public string AdminName
        {
            get
            {
                return new AdminsView().Where(item => item.ID == AdminID).FirstOrDefault().RealName;
            }
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

        #region 废弃
        public void Abandon()
        {
            //throw new NotImplementedException();
            using (var repository = new PvWmsRepository())
            {
                this.Status = BoxesStatus.Deleted;
                repository.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
        }
        #endregion
    }

    /// <summary>
    /// 装箱
    /// </summary>
    public class CgBoxesSortingNotices
    {
        public CgSortingNotice[] SortingNotice { get; set; }
    }
}

using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms.Services.Enums;
using Wms.Services.Extends;
using Wms.Services.Views;
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Underly;
using Yahv.Usually;

namespace Wms.Services.Models
{
    /// <summary>
    /// 货架类（库区、货架（卡板）、库位）
    /// </summary>

    public abstract class BaseShelves : IUnique
    {
        #region 属性

        /// <summary>
        /// ID (库区ID+ShelvesNumber)
        /// </summary>
        public string ID { get; set; }

        private string _Name;
        /// <summary>
        /// 库房时是数据库字段，库区/货架/库位临时字段，配合写增删改查
        /// </summary>
        public string Name
        {
            get
            {
                if (this.Type == ShelvesType.Warehouse)
                {
                    return this._Name;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(this.ID))
                        return this.ID.Replace(this.FatherID, "").Replace("-", "");
                    else
                        return this._Name;
                }

            }
            set
            {
                this._Name = value;
            }
        }

        /// <summary>
        /// 地区简码：香港（HK）;深圳（SZ）;西南地区（XN）
        /// </summary>
        //public string RegionCode { get; set; }

        /// <summary>
        /// 父亲ID
        /// </summary>
        public string FatherID { get; set; }

        //public object FatherMsg
        //{
        //    get
        //    {
        //        if (this.Type == ShelvesType.Warehouse)
        //        {
        //            return null;
        //        }
        //        else if (this.Type == ShelvesType.Region)
        //        {
        //            var warehouse = new ShelvesView()[this.FatherID];
        //            return new { ID = warehouse.ID, WarehouseName = warehouse.Name };
        //        }
        //        else if (this.Type == ShelvesType.Board || this.Type == ShelvesType.Shelve)
        //        {
        //            var region = new ShelvesView()[this.FatherID];//库区信息
        //            var warehouse = new ShelvesView()[region.FatherID];//库房信息
        //            return new { ID = region.ID, RegionName = region.Name, WarehouseName = warehouse.Name };
        //        }
        //        else
        //        {
        //            var shelve = new ShelvesView()[this.FatherID];//货架信息
        //            var region = new ShelvesView()[shelve.FatherID];//库区信息
        //            var warehouse = new ShelvesView()[region.FatherID];//库房信息
        //            return new { ID = shelve.ID, ShleveName = shelve.Name, RegionName = region.Name, WarehouseName = warehouse.Name };
        //        }
        //    }
        //}

        /// <summary>
        /// 类型：库区、货架（卡板）、库位
        /// </summary>
        public ShelvesType Type { get; set; }

        /// <summary>
        /// 用途：租赁业务、代仓储业务、代报关业务、贸易业务
        /// </summary>
        public ShelvesPurpose Purpose { get; set; }

        /// <summary>
        /// 是否可添子项（这个子项中开发中要根据实际情况进行添加）
        /// </summary>
        public bool Addible { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }

        /// <summary>
        /// 货架状态：正常、停用、删除
        /// </summary>
        public ShelvesStatus Status { get; internal set; }

        /// <summary>
        /// 货架只在库位中起 作用规格
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 在库人员，AdminID(负责人)
        /// </summary>
        public string ManagerID { get; set; }

        ///// <summary>
        ///// 在库人员，AdminID(负责人)名字
        ///// </summary>
        //public string ManagerName
        //{
        //    get
        //    {
        //        return new AdminsView().Where(item => item.ID == this.ManagerID).FirstOrDefault().RealName;
        //    }
        //}

        /// <summary>
        /// 内部公司、供应商、客户、承运商(所有人)
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 内部公司、供应商、客户、承运商(所有人)名字
        /// </summary>
        public string EnterpriseName
        {
            get
            {
                return new EnterprisesTopView().Where(item => item.ID == this.EnterpriseID).FirstOrDefault().Name;
            }
        }


        /// <summary>
        /// 库位分配给的集团下的任意员工（admin）
        /// </summary>
        public string ClerkID { get; set; }

        /// <summary>
        /// 合同协议
        /// </summary>
        public string ContractID { get; set; }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 库区/货架/库位数量
        /// </summary>
        public int Count
        {
            get
            {
                return new Views.ShelvesView().Where(item => item.FatherID == this.ID).Count();
            }
        }

        /// <summary>
        /// 类型的枚举信息
        /// </summary>
        public string TypeDes
        {
            get
            {
                return this.Type.GetDescription();
            }
        }

        /// <summary>
        /// 业务的枚举信息
        /// </summary>
        public string PurposeDes
        {
            get
            {
                return this.Purpose.GetDescription();
            }
        }

        /// <summary>
        /// 状态的枚举信息
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }
        #endregion

        #region 方法
        public abstract void Enter();
        public abstract void Abandon();

        //public void Abandon()
        //{
        //    try
        //    {
        //        using (var repository = new PvWmsRepository())
        //        {
        //            //只满足item => item.FatherID==this.ID个数为0的时候给予删除的功能
        //            var shelve = new Views.ShelvesView().Where(item => item.FatherID == this.ID);
        //            if (shelve.Count() == 0)
        //            {
        //                this.Status = ShelvesStatus.Deleted;
        //                repository.Update(this.ToLinq(), item => item.ID == this.ID);
        //                //repository.Delete<Layer.Data.Sqls.PvWms.Shelves>(item => item.ID == this.ID);
        //            }
        //            else
        //            {
        //                this.AbandonFailed?.Invoke(this, new ErrorEventArgs("Delete Failed!!"));
        //                return;
        //            }
        //        }
        //        this.AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
        //    }
        //    catch
        //    {
        //        this.AbandonFailed?.Invoke(this, new ErrorEventArgs("Delete Failed!!"));
        //    }
        //}




        //private object GetFather(string fatherID)
        //{

        //}
        #endregion
    }
}

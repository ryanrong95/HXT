using Layers.Data.Sqls;
using System;
using System.Linq;
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

    public class Shelves : IUnique, IPersisting
    {

        #region 事件
        //enter成功
        public event SuccessHanlder ShelvesSuccess;
        //enter失败
        public event ErrorHanlder ShelvesFailed;
        //不支持修改
        public event ErrorHanlder NotSupportedUpdate;
        //删除成功
        public event SuccessHanlder AbandonSuccess;
        //删除失败
        public event ErrorHanlder AbandonFailed;
        //名称重复
        public event ErrorHanlder CheckNameRepeated;
        //ID不支持修改
        public event ErrorHanlder IDNotSupportModify;

        #endregion

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
                    {
                        if (this.Type == ShelvesType.Region)
                        {
                            return this.ID.Replace(this.FatherID.Substring(0, 4), "").Replace("-", "");
                        }
                        else
                        {
                            return this.ID.Replace(this.FatherID, "").Replace("-", "");
                        }
                    }
                    else
                        return this._Name;
                }

            }
            set
            {
                this._Name = value;
            }
        }

        ///// <summary>
        ///// 地区简码：香港（HK）;深圳（SZ）;西南地区（XN）
        ///// </summary>
        //public string RegionCode { get; set; }

        /// <summary>
        /// 父亲ID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 所属位置信息
        /// </summary>
        public string FatherName
        {
            get
            {
                //库房分两种情况，大库房和门牌库房，大库房没有父级信息，门牌库房有父级信息
                if (this.Type == ShelvesType.Warehouse)
                {
                    //大库房没有父级信息
                    if (string.IsNullOrWhiteSpace(this.FatherID))
                    {
                        return null;
                    }
                    //门牌库房有父级信息
                    else
                    {
                        return new WarehousesView()[this.FatherID].Name;
                    }
                }

                if (this.Type == ShelvesType.Region)
                {
                    return new WarehousesView()[this.FatherID].Name;
                }
                else if (this.Type == ShelvesType.Board || this.Type == ShelvesType.Shelve)
                {
                    var region = new ShelvesView()[this.FatherID];//获得库区信息
                    var warehouseName = new WarehousesView()[region.FatherID].Name;//获得库房信息

                    return string.Concat(warehouseName, region.Name, region.Type.GetDescription());
                }
                else
                {
                    var shelve = new ShelvesView()[this.FatherID];//获得货架信息
                    var region = new ShelvesView()[shelve.FatherID];//获得库区信息
                    var warehouseName = new WarehousesView()[region.FatherID].Name;//获得库房信息

                    return string.Concat(warehouseName, region.Name, region.Type.GetDescription(), shelve.Name, shelve.Type.GetDescription());
                }

            }
        }

        /// <summary>
        /// 所有父级信息
        /// </summary>
        public object FatherMsg
        {
            get
            {
                //库房分两种情况，大库房和门牌库房，大库房没有父级信息，门牌库房有父级信息
                if (this.Type == ShelvesType.Warehouse)
                {
                    //大库房没有父级信息
                    if (string.IsNullOrWhiteSpace(this.FatherID))
                    {
                        return null;
                    }
                    //门牌库房有父级信息
                    else
                    {
                        var fatherWarehouse = new WarehousesView()[this.FatherID];
                        return new { WarehouseName = fatherWarehouse.Name };
                    }
                }
                if (this.Type == ShelvesType.Region)
                {
                    var warehouse = new WarehousesView()[this.FatherID];
                    return new { WarehouseName = warehouse.Name };
                }
                else if (this.Type == ShelvesType.Board || this.Type == ShelvesType.Shelve)
                {
                    var region = new ShelvesView()[this.FatherID];//库区信息
                    var warehouse = new WarehousesView()[region.FatherID];//库房信息
                    return new { RegionName = region.Name, WarehouseName = warehouse.Name };
                }
                else
                {

                    var shelve = new ShelvesView()[this.FatherID];//货架信息
                    var region = new ShelvesView()[shelve.FatherID];//库区信息
                    var warehouse = new WarehousesView()[region.FatherID];//获得库房名称
                    return new { ShleveName = shelve.Name, RegionName = region.Name, WarehouseName = warehouse.Name, WarehouseID = warehouse.ID };
                }
            }
        }

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
        /// 在库人员，AdminID
        /// </summary>
        public string ManagerID { get; set; }

        /// <summary>
        ///负责人姓名
        /// </summary>
        public string ManagerName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ManagerID))
                {
                    var admin = new AdminsView().Where(item => item.ID == this.ManagerID).FirstOrDefault();

                    return admin == null ? null : admin.RealName; /*(admin ?? null).RealName;*/
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// 内部公司、供应商、客户、承运商
        /// </summary>
        public string EnterpriseID { get; set; }

        /// <summary>
        /// 内部公司、供应商、客户、承运商(所有人)名字
        /// </summary>
        public string EnterpriseName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(EnterpriseID))
                {
                    var enterprise = new EnterprisesTopView().Where(item => item.ID == this.EnterpriseID).FirstOrDefault();

                    return enterprise == null ? null : enterprise.Name;
                }
                return null;
            }
        }

        /// <summary>
        /// 库位分配给的集团下的任意员工（admin）
        /// </summary>
        public string ClerkID { get; set; }

        /// <summary>
        /// 租赁通知编号
        /// </summary>
        public string LeaseID { get; set; }

        /// <summary>
        /// 租赁人
        /// </summary>
        public string LeaseName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.LeaseID))
                {
                    //try
                    //{
                    var clientID = new Yahv.Services.Views.LsNoticeView()[LeaseID].ClientID;
                    var clientMsg = new ClientsView().Where(item => item.ID == clientID).FirstOrDefault();
                    return clientMsg == null ? null : clientMsg.Name;
                    //}
                    //catch
                    //{
                    //    return null;
                    //}

                }
                else
                {
                    return null;
                }
            }

        }

        //public Shelves[] Children { get; set; }
        #endregion

        #region 扩展属性

        ///// <summary>
        ///// 获得子集信息
        ///// </summary>
        //public Shelves[] SubShelves
        //{
        //    get
        //    {
        //        return new Views.ShelvesView().Where(item => item.FatherID == this.ID).ToArray();
        //    }
        //}

        /// <summary>
        /// 库区/货架/库位数量
        /// </summary>
        public int Count
        {
            get
            {
                if (this.Type == ShelvesType.Warehouse)
                {
                    return new Views.ShelvesView().Where(item => item.FatherID == this.ID && item.Type == ShelvesType.Region).Count();
                }
                else
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

        #region 删除（逻辑删除）持久化
        public void Abandon()
        {
            //try
            //{
            //    using (var repository = new PvWmsRepository())
            //    {
            //        //只满足item => item.FatherID==this.ID个数为0并且库位上面没有货物（即该库区/货架（卡板）/库位上面没有货物，库存里该库位的货物数量为0）,而且租赁ID为空  的时候给予删除的功能
            //        var shelve = new Views.ShelvesView().Where(item => item.FatherID == this.ID);

            //        var storage = repository.ReadTable<Layers.Data.Sqls.PvWms.StoragesTopView>().Where(item => item.ShelveID == this.ID);

            //        if (shelve.Count() == 0 && storage.Count() == 0 && string.IsNullOrWhiteSpace(this.LeaseID))
            //        {
            //            //this.Status = ShelvesStatus.Deleted;
            //            //repository.Update(this.ToLinq(), item => item.ID == this.ID);
            //            repository.Delete<Layers.Data.Sqls.PvWms.Shelves>(item => item.ID == this.ID);
            //        }
            //        else
            //        {
            //            this.AbandonFailed?.Invoke(this, new ErrorEventArgs("Delete Failed!!"));
            //            return;
            //        }
            //    }
            //    this.AbandonSuccess?.Invoke(this, new SuccessEventArgs(this));
            //}
            //catch
            //{
            //    this.AbandonFailed?.Invoke(this, new ErrorEventArgs("Delete Failed!!"));
            //}
        }

        #endregion

        #region 新增/修改持久化
        public void Enter()
        {
            try
            {
                using (var repository = new PvWmsRepository())
                {
                    #region 添加
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        //库区添加
                        if (this.Type == ShelvesType.Region)
                        {
                            this.ID = this.FatherID.Substring(0, 4) + "-" + this.Name;
                        }
                        //货架（卡板）添加
                        else if (this.Type == ShelvesType.Shelve)
                        {
                            this.ID = this.FatherID + this.Name;
                        }
                        //库位添加
                        else
                        {
                            this.ID = this.FatherID + "-" + this.Name;
                        }
                        if (CheckNameRepeated != null)
                        {
                            //ID的个数大于1是名字重复
                            if (new Views.ShelvesView()[this.ID] != null)
                            {
                                CheckNameRepeated.Invoke(this, new ErrorEventArgs("Name Repeated!!"));
                                return;
                            }
                        }
                        this.Addible = true;
                        this.CreateDate = this.UpdateDate = DateTime.Now;
                        this.Status = ShelvesStatus.Normal;
                        repository.Insert(this.ToLinq());

                    }
                    #endregion

                    #region 修改
                    else
                    {

                        var oldID = this.ID;//原有的ID
                        if (new Views.ShelvesView()[oldID] == null)
                        {
                            this.NotSupportedUpdate?.Invoke(this, new ErrorEventArgs("所要修改的数据不存在"));
                            return;
                        }
                        var newID = "";
                        if (this.Type == ShelvesType.Shelve)
                        {
                            newID = this.FatherID + this.Name;//FatherID和this.Name组合而成的编号
                        }
                        else
                        {
                            newID = this.FatherID + "-" + this.Name;//FatherID和this.Name组合而成的编号
                        }

                        if (CheckNameRepeated != null)
                        {
                            //newID的个数大于1并且newID不是this.ID是名字重复
                            if (new Views.ShelvesView()[newID] != null && newID != oldID)
                            {
                                CheckNameRepeated.Invoke(this, new ErrorEventArgs("Name Repeated!!"));
                                return;
                            }
                        }

                        //原有的ID == Shelves.FatherID的个数>0的时候不可以进行编辑ID
                        if (new Views.ShelvesView().Where(item => item.FatherID == oldID).Count() > 0)
                        {
                            //id不支持修改
                            if (newID != oldID)
                            {
                                this.IDNotSupportModify?.Invoke(this, new ErrorEventArgs("ID does not support modification!!"));
                                return;
                            }
                        }

                        this.UpdateDate = DateTime.Now;
                        repository.Update(this.ToLinq(), item => item.ID == oldID);

                    }
                    #endregion

                }
                this.ShelvesSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
            catch
            {
                this.ShelvesFailed?.Invoke(this, new ErrorEventArgs("Failed"));
            }
        }
        #endregion


        #endregion
    }
}

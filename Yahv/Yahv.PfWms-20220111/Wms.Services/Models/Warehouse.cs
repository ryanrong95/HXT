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
using Yahv.Linq;
using Yahv.Linq.Persistence;
using Yahv.Usually;

namespace Wms.Services.Models
{
    /// <summary>
    /// 库房类
    /// </summary>

    public class Warehouse : Shelves
    {

        #region 事件
        ////enter成功
        //public event SuccessHanlder ShelvesSuccess;
        ////enter失败
        //public event ErrorHanlder ShelvesFailed;
        ////不支持修改
        //public event ErrorHanlder NotSupportedUpdate;
        ////删除成功
        //public event SuccessHanlder AbandonSuccess;
        ////删除失败
        //public event ErrorHanlder AbandonFailed;
        ////名称重复
        //public event ErrorHanlder CheckNameRepeated;
        #endregion

        #region 库房专属属性

        /// <summary>
        /// 是否在途
        /// </summary>
        public bool IsOnOrder { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// crm编码
        /// </summary>
        public string CrmCode { get; set; }

        #endregion

        #region 方法


        //public override void Enter()
        //{
        //    try
        //    {
        //        using (var repository = new PvWmsRepository())
        //        {
        //            if (CheckNameRepeated != null)
        //            {
        //                //判断新增/修改的库房名称是否已经存在
        //                if (new Views.WarehousesView().Any(item => item.Name == this.Name && item.ID != (this.ID ?? "")))
        //                {
        //                    CheckNameRepeated.Invoke(this, new ErrorEventArgs("Name Repeated!!"));
        //                    return;
        //                }
        //            }

        //            var entity = new Views.WarehousesView()[this.ID];
        //            using (TransactionScope scope = new TransactionScope())
        //            {
        //                try
        //                {
        //                    //没有数据时添加
        //                    if (entity == null)
        //                    {
                               
        //                        this.CreateDate = this.UpdateDate = DateTime.Now;
        //                        this.Status = ShelvesStatus.Normal;
        //                        this.Type = ShelvesType.Warehouse;
        //                        repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
        //                        {
        //                            ID = ID,
        //                            FatherID = FatherID,
        //                            Type = (int)Type,
        //                            Purpose = (int)Purpose,
        //                            Addible = Addible,
        //                            CreateDate = CreateDate,
        //                            UpdateDate = UpdateDate,
        //                            Status = (int)Status,
        //                            SpecID = "AB01",
        //                            Summary = Summary,
        //                            ManagerID = ManagerID,
        //                            EnterpriseID = EnterpriseID,
        //                            ClerkID = ClerkID,
        //                            ContractID = ContractID,
        //                        });

        //                        repository.Insert(this.ToLinq());

        //                    }
        //                    //有数据时修改
        //                    else
        //                    {
        //                        this.UpdateDate = DateTime.Now;
        //                        this.Type = ShelvesType.Warehouse;
        //                        repository.Update(new Layers.Data.Sqls.PvWms.Shelves
        //                        {
        //                            FatherID = FatherID,
        //                            Type = (int)Type,
        //                            Purpose = (int)Purpose,
        //                            Addible = Addible,
        //                            CreateDate = CreateDate,
        //                            UpdateDate = UpdateDate,
        //                            Status = (int)Status,
        //                            SpecID = "AB01",
        //                            Summary = Summary,
        //                            ManagerID = ManagerID,
        //                            EnterpriseID = EnterpriseID,
        //                            ClerkID = ClerkID,
        //                            ContractID = ContractID,
        //                        }, item => item.ID == this.ID);

        //                        repository.Update(this.ToLinq(), item => item.ID == this.ID);

        //                    }
        //                    scope.Complete();
        //                }
        //                catch (Exception ex)
        //                {
        //                    this.ShelvesFailed?.Invoke(this, new ErrorEventArgs("Enter Failed!!"));
        //                    return;
        //                }
        //                finally
        //                {
        //                    scope.Dispose();
        //                }

        //            }
        //            this.ShelvesSuccess?.Invoke(this, new SuccessEventArgs(this));

        //        }

        //    }
        //    catch
        //    {
        //        this.ShelvesFailed?.Invoke(this, new ErrorEventArgs("Failed"));
        //    }

        //}

        //public override void Abandon()
        //{
        //    throw new NotImplementedException("不支持此方法");
        //}
        #endregion
    }
}

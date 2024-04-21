using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{
    public class WarehousesView : UniqueView<Warehouse, PvWmsRepository>
    {
        public WarehousesView()
        {

        }

        /// <summary>
        /// 同步库房
        /// </summary>
        /// <returns></returns>
        public void LoadData()
        {
            using (PvWmsRepository repository = new PvWmsRepository())
            {

                //循环添加主库房     WarehouseTopView
                var tops = new Yahv.Services.Views.WarehousesTopView<PvWmsRepository>(repository).ToArray();

                //循环添加子库放    PlateTopView
                var subs = new Yahv.Services.Views.PlateTopView<PvWmsRepository>(repository).ToArray();

                //取出所有库房ID
                var warehouseids = repository.ReadTable<Layers.Data.Sqls.PvWms.Warehouses>().Select(item => item.ID).ToArray();

                using (var trans = new TransactionScope())
                {
                    try
                    {
                        //如下的操作，一定要做到判断:添加 与 修改
                        //var counter = 0;
                        foreach (var top in tops)
                        {
                            //插入库房信息
                            //需要操作两个表： shelve 与  warehose

                            if (!warehouseids.Contains(top.WsCode))
                            {
                                //主库房视图进行操作 
                                repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
                                {
                                    ID = top.WsCode,
                                    //FatherID = null,
                                    //Type = (int)ShelvesType.Warehouse,
                                    //Purpose = (int)ShelvesPurpose.WarehouseServicing,
                                    //Addible = true,
                                    //CreateDate = DateTime.Now,
                                    //UpdateDate = DateTime.Now,
                                    //Status = (int)ShelvesStatus.Normal,
                                    //SpecID = null,
                                    //Summary = "",
                                    //ManagerID = null,
                                    //EnterpriseID = null,
                                    //ClerkID = null,
                                    LeaseID = null,
                                });

                                //主库房视图进行操作 
                                repository.Insert(new Layers.Data.Sqls.PvWms.Warehouses
                                {
                                    ID = top.WsCode,
                                    IsOnOrder = false,
                                    Name = top.Name,
                                    Address = top.Address,
                                    CrmCode = top.ID
                                });

                                //子库房（门牌）视图进行操作(主库房视图未同步过来子库房视图肯定未同步过来，直接添加)
                                foreach (var sub in subs.Where(item => item.EnterpriseID == top.ID))
                                {
                                    //插入门牌信息
                                    //需要操作两个表： shelve 与  warehose

                                    //临时处理，后期需要刘芳提供code
                                    //var id = top.WsCode + (counter++).ToString().PadLeft(2, '0');

                                    //子库房视图进行操作
                                    repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
                                    {
                                        ID = sub.Code,  //code
                                        //FatherID = top.WsCode,
                                        //Type = (int)ShelvesType.Warehouse,
                                        //Purpose = (int)ShelvesPurpose.WarehouseServicing,
                                        //Addible = true,
                                        //CreateDate = DateTime.Now,
                                        //UpdateDate = DateTime.Now,
                                        //Status = (int)ShelvesStatus.Normal,
                                        //SpecID = null,
                                        //Summary = "",
                                        //ManagerID = null,
                                        //EnterpriseID = null,
                                        //ClerkID = null,
                                        LeaseID = null,
                                    });

                                    //子库房视图进行操作
                                    repository.Insert(new Layers.Data.Sqls.PvWms.Warehouses
                                    {
                                        ID = sub.Code,
                                        IsOnOrder = false,
                                        Name = sub.Name,
                                        Address = sub.Address,
                                        CrmCode = sub.ID
                                    });


                                }

                            }

                            else
                            {
                                //主库房视图进行操作 
                                repository.Update(new Layers.Data.Sqls.PvWms.Warehouses
                                {
                                    ID = top.WsCode,
                                    IsOnOrder = false,
                                    Name = top.Name,
                                    Address = top.Address,
                                    CrmCode = top.ID
                                }, item => item.ID == top.WsCode);

                                //主库房视图进行操作 
                                repository.Update(new Layers.Data.Sqls.PvWms.Shelves
                                {
                                    //FatherID = null,
                                    //Type = (int)ShelvesType.Warehouse,
                                    //Purpose = (int)ShelvesPurpose.WarehouseServicing,
                                    //Addible = true,
                                    ////CreateDate = DateTime.Now,
                                    //UpdateDate = DateTime.Now,
                                    //Status = (int)ShelvesStatus.Normal,
                                    //SpecID = null,
                                    //Summary = "",
                                    //ManagerID = null,
                                    //EnterpriseID = null,
                                    //ClerkID = null,
                                    LeaseID = null,
                                }, item => item.ID == top.WsCode);

                                foreach (var sub in subs.Where(item => item.EnterpriseID == top.ID))
                                {
                                    //插入门牌信息
                                    //需要操作两个表： shelve 与  warehose
                                    //var id = top.WsCode + (counter++).ToString().PadLeft(2, '0');

                                    if (!warehouseids.Contains(sub.Code))
                                    {
                                        //子库房视图进行操作
                                        repository.Insert(new Layers.Data.Sqls.PvWms.Warehouses
                                        {
                                            ID = sub.Code,
                                            IsOnOrder = false,
                                            Name = sub.Name,
                                            Address = sub.Address,
                                            CrmCode = sub.ID
                                        });

                                        //子库房视图进行操作
                                        repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
                                        {
                                            ID = sub.Code,  //code
                                            //FatherID = top.WsCode,
                                            //Type = (int)ShelvesType.Warehouse,
                                            //Purpose = (int)ShelvesPurpose.WarehouseServicing,
                                            //Addible = true,
                                            //CreateDate = DateTime.Now,
                                            //UpdateDate = DateTime.Now,
                                            //Status = (int)ShelvesStatus.Normal,
                                            //SpecID = null,
                                            //Summary = "",
                                            //ManagerID = null,
                                            //EnterpriseID = null,
                                            //ClerkID = null,
                                            LeaseID = null,
                                        });
                                    }
                                    else
                                    {
                                        //子库房视图进行操作
                                        repository.Update(new Layers.Data.Sqls.PvWms.Warehouses
                                        {
                                            IsOnOrder = false,
                                            Name = sub.Name,
                                            Address = sub.Address,
                                            CrmCode = sub.ID
                                        }, item => item.ID == sub.Code);

                                        repository.Update(new Layers.Data.Sqls.PvWms.Shelves
                                        {
                                            //FatherID = top.WsCode,
                                            //Type = (int)ShelvesType.Warehouse,
                                            //Purpose = (int)ShelvesPurpose.WarehouseServicing,
                                            //Addible = true,
                                            ////CreateDate =DateTime.Now ,
                                            //UpdateDate = DateTime.Now,
                                            //Status = (int)ShelvesStatus.Normal,
                                            //SpecID = null,
                                            //Summary = "",
                                            //ManagerID = null,
                                            //EnterpriseID = null,
                                            //ClerkID = null,
                                            LeaseID = null,
                                        }, item => item.ID == sub.Code);
                                    }
                                }
                            }
                        }

                        trans.Complete();

                    }
                    catch(Exception ex)
                    {

                    }
                    finally
                    {
                        trans.Dispose();
                    }

                }

                //return this.GetIQueryable().ToArray();

                #region 暂时无用
                //var linq = from sub in subs
                //           group sub by new
                //           {
                //               sub.Code,
                //               sub.Name
                //           } into groups
                //           select new
                //           {
                //               Name = groups.Key.Name,
                //               Code = groups.Key.WsCode,
                //               Sub = groups.Select(item => new
                //               {
                //                   item.Address,
                //                   item.ConsinStatus
                //               })
                //           };



                ////同步crm库房数据
                //foreach (var top in linq)
                //{
                //    if (true)//判断会否存在
                //    {
                //        repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
                //        {
                //            ID = top.Code,
                //            FatherID = null,
                //            Type = (int)ShelvesType.Warehouse,
                //            Purpose = (int)ShelvesPurpose.WarehouseServicing,
                //            Addible = true,
                //            CreateDate = DateTime.Now,
                //            UpdateDate = DateTime.Now,
                //            Status = (int)ShelvesStatus.Normal,
                //            SpecID = "AB01",
                //            Summary = "",
                //            ManagerID = null,
                //            EnterpriseID = null,
                //            ClerkID = null,
                //            ContractID = null,
                //        });
                //    }

                //    foreach (var item in top.Sub)
                //    {
                //        repository.Insert(new Layers.Data.Sqls.PvWms.Shelves
                //        {
                //            ID = top.Code,
                //            FatherID = null,
                //            Type = (int)ShelvesType.Warehouse,
                //            Purpose = (int)ShelvesPurpose.WarehouseServicing,
                //            Addible = true,
                //            CreateDate = DateTime.Now,
                //            UpdateDate = DateTime.Now,
                //            Status = (int)ShelvesStatus.Normal,
                //            SpecID = "AB01",
                //            Summary = "",
                //            ManagerID = null,
                //            EnterpriseID = null,
                //            ClerkID = null,
                //            ContractID = null,
                //        });
                //        repository.Insert(new Layers.Data.Sqls.PvWms.Warehouses
                //        {
                //            ID = entity.ID,
                //            IsOnOrder = entity.IsOnOrder,
                //            Name = entity.Name,
                //            Address = entity.Address,
                //            CrmCode = entity.CrmCode,
                //        });
                //    }


                //    new Warehouse
                //    {
                //        ID = top.Code,
                //        Name = top.Name,
                //        Address = top.Code,
                //        EnterpriseID = "",
                //        Addible = true,
                //        ClerkID = "",
                //        ContractID = null,
                //        CrmCode = "",
                //        FatherID = null,
                //        IsOnTheWay = false,
                //        ManagerID = "",
                //        Purpose = Enums.ShelvesPurpose.WarehouseServicing,
                //        SpecID = "",
                //        Summary = ""
                //    }.Enter();
                //}

                #endregion 
            }


        }

        //static MainWarehousesView()
        //{


        //}

        protected override IQueryable<Warehouse> GetIQueryable()
        {

            #region 在途库房先不做
            ////同步关系
            //for (int i = 0; i < crmArry.Length; i++)
            //{
            //    var entity = crmArry[i];
            //    for (int j = 0; j < crmArry.Length; j++)
            //    {
            //        if (crmArry[i].ID == crmArry[j].ID)
            //        { break; }
            //        var entity1 = crmArry[j];
            //        new Shelves { Name = string.Join("-", entity.Name, entity1.Name) }.Enter();
            //    }
            //}
            #endregion

            // left join 暂时不做

            #region 左连接
            //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
            //       join warehouse in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Warehouses>() on entity.ID equals warehouse.ID into temp
            //       from tt in temp.DefaultIfEmpty()
            //       select new Warehouse
            //       {
            //           ID = entity.ID,
            //           FatherID = entity.FatherID,
            //           Type = (Enums.ShelvesType)entity.Type,
            //           Purpose = (Enums.ShelvesPurpose)entity.Purpose,
            //           Addible = entity.Addible,
            //           CreateDate = entity.CreateDate,
            //           UpdateDate = entity.UpdateDate,
            //           Status = (Enums.ShelvesStatus)entity.Status,
            //           SpecID = entity.SpecID,
            //           Summary = entity.Summary,
            //           ManagerID = entity.ManagerID,
            //           EnterpriseID = entity.EnterpriseID,
            //           ClerkID = entity.ClerkID,
            //           ContractID = entity.ContractID,
            //           Name = tt.Name ?? "",
            //           Address = tt.Address ?? "",
            //           CrmCode = tt.CrmCode ?? "",
            //           IsOnTheWay = tt == null ? false : tt.IsOnOrder
            //       };

            #endregion

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
                   join warehouse in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Warehouses>() on entity.ID equals warehouse.ID
                   select new Warehouse
                   {
                       ID = entity.ID,
                       //FatherID = entity.FatherID,
                       //Type = (Enums.ShelvesType)entity.Type,
                       //Purpose = (Enums.ShelvesPurpose)entity.Purpose,
                       //Addible = entity.Addible,
                       //CreateDate = entity.CreateDate,
                       //UpdateDate = entity.UpdateDate,
                       //Status = (Enums.ShelvesStatus)entity.Status,
                       //SpecID = entity.SpecID,
                       //Summary = entity.Summary,
                       //ManagerID = entity.ManagerID,
                       //EnterpriseID = entity.EnterpriseID,
                       //ClerkID = entity.ClerkID,
                       LeaseID = entity.LeaseID,
                       Name = warehouse.Name,
                       Address = warehouse.Address,
                       CrmCode = warehouse.CrmCode,
                       IsOnOrder = warehouse.IsOnOrder
                   };


        }

        //库房  门牌  库区  货架 （卡板）  库位

        //public Shelves[] Get(string name, string other = null)
        //{
        //    //想方设法 返回的也要是一个数据结构
        //    return null;

        //}



        //public void Enter(WsShelve entity)
        //{

        //}

        //public void Delete(string id)
        //{
        //    // 先判断删除的库位上是否有 货？
        //    // 如果没有货物，就直接删除
        //    // 否则失败
        //}
    }


    ///// <summary>
    ///// 库位
    ///// </summary>
    //public class WsShelve
    //{
    //    public int Type { get; set; }
    //}

}

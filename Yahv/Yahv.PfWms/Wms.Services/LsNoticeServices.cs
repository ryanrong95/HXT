using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Wms.Services;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Usually;

namespace Wms.Services
{
    public class LsNoticeServices
    {
        public LsNoticeServices()
        {

        }

        IErpAdmin admin;
        public LsNoticeServices(IErpAdmin admin)
        {
            this.admin = admin;
        }

        public event SuccessHanlder Success;
        public event ErrorHanlder Failed;

        ////合同编号重复
        //public event ErrorHanlder CodeRepeated;

        //库位不能被使用（已被签过合同）
        public event ErrorHanlder ShelveNotUse;

        //库位重复
        public event ErrorHanlder ShelveRepeated;

        //没有足够的库位，请联系管理员添加库位
        public event ErrorHanlder ShelveNotEnough;


        public object GetLsNotice(string key = null, string status = null, int pageIndex = 1, int pageSize = 10)
        {
            Expression<Func<Yahv.Services.Models.LsOrder.LsOrder, bool>> exp = item => item.Status != LsOrderStatus.Unpaid;

            if (!string.IsNullOrWhiteSpace(key))
            {
                exp = PredicateExtends.And(exp, item => item.ID == key || item.ClientID == key);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                exp = PredicateExtends.And(exp, item => item.Status == (LsOrderStatus)(int.Parse(status)));
            }

            #region 废弃了的
            //Expression<Func<LsNotice, bool>> exp = item => true;

            //if (!string.IsNullOrWhiteSpace(key))
            //{
            //    exp = PredicateExtends.And(exp, item => item.OrderID == key || item.ClientID == key);
            //}

            //if (!string.IsNullOrWhiteSpace(status))
            //{
            //    exp = PredicateExtends.And(exp, item => item.Status == (Yahv.Services.Enums.LsNoticeStatus)(int.Parse(key)));
            //}

            ////var data = new Yahv.Services.Views.LsNoticeView().GetLsNoticeData();
            //var lsNotices = new Yahv.Services.Views.LsNoticeView().Where(exp);
            //var groups = lsNotices.GroupBy(item => item.OrderID);

            //var returnDatas = from entity in groups
            //           select new
            //           {
            //               OrderID = entity.Key,
            //               CreateDate = entity.Select(item => item.CreateDate).FirstOrDefault(),
            //               ClientID = entity.Select(item => item.ClientID).FirstOrDefault(),
            //               Count = entity.Key.Count(),
            //               Status = entity.Select(item => item.Status).FirstOrDefault(),
            //           };



            //var lsnotices = data.GroupBy(item => item.OrderID) as groups 
            //DateTime? startDate, endDate;
            //Yahv.Utils.Converters.DateExtend.DateHandle(StartTime, EndTime, out startDate, out endDate);

            //if (startDate != null && endDate == null)
            //{
            //    exp = PredicateExtends.And(exp, item => item.StartDate >= startDate);
            //}

            //if (endDate != null && startDate == null)
            //{
            //    exp = PredicateExtends.And(exp, item => item.EndDate < endDate);
            //}

            //if (startDate != null && endDate != null)
            //{
            //    exp = PredicateExtends.And(exp, item => item.StartDate >= startDate && item.EndDate <= endDate);
            //}

            //return Json(new { obj = returnDatas }, JsonRequestBehavior.AllowGet);
            #endregion

            return new Wms.Services.Views.LsOrderTopView().Where(exp).OrderByDescending(item => item.CreateDate).Paging(pageIndex, pageSize);
        }

        public object Detail(string warehouseID, string orderID)
        {

            using (var rep = new PvWmsRepository())
            {
                var order = new Wms.Services.Views.LsOrderTopView().Where(item => item.Status != LsOrderStatus.Unpaid && item.ID == orderID).FirstOrDefault();

                var lsNotices = new Yahv.Services.Views.LsNoticeView().Where(item => item.OrderID == orderID);
                //var lsNoticeIDs = lsNotices.Select(item => item.ID).ToArray().Distinct();

                List<LsNotice> list = new List<LsNotice>();
                foreach (var item in lsNotices)
                {
                    //根据库房ID获得对应库房信息
                    var warehouse = new Wms.Services.Views.WarehousesView()[warehouseID];
                    //库位的ShelvesView的判断条件
                    Expression<Func<Shelves, bool>> exp = null;
                    if (!string.IsNullOrWhiteSpace(warehouse.FatherID))
                    {
                        var regions = new Wms.Services.Views.ShelvesView().Where(tem => tem.FatherID == warehouseID);//获得该库房的所有库区信息
                        if (regions.Count() <= 0)
                        {
                            if (this != null && this.ShelveNotEnough != null)
                            {
                                this.ShelveNotEnough(this, new ErrorEventArgs("Shelve Not Enough!!"));
                                return null;
                            }
                        }

                        var regionIDs = regions.Select(tem => tem.ID).ToArray();


                        foreach (var ids in regionIDs)
                        {
                            exp = PredicateExtends.Or(exp, tem => tem.ID.StartsWith(ids));
                        }
                    }
                    else
                    {
                        exp = PredicateExtends.And(exp, tem => tem.ID.StartsWith(warehouseID));
                    }
                    exp = PredicateExtends.And(exp, tem => tem.Purpose == Enums.ShelvesPurpose.Rent && tem.Type == Enums.ShelvesType.Position && tem.Status == Enums.ShelvesStatus.Normal);

                    var positions = new Views.ShelvesView().Where(exp);
                    if (positions.Count() <= 0)
                    {
                        if (this != null && this.ShelveNotEnough != null)
                        {
                            this.ShelveNotEnough(this, new ErrorEventArgs("Shelve Not Enough!!"));
                            return null;
                        }
                    }
                    //var regions = new Views.ShelvesView().Where(tem => tem.FatherID == warehouseID);//获得该库房的所有库区信息
                    //var regionIDs = regions.Select(tem => tem.ID).ToArray();
                    //var shelves = new Views.ShelvesView().Where(tem => regionIDs.Contains(tem.FatherID));//获得该库房的所有货架信息
                    //var shelveIDs = shelves.Select(tem => tem.ID).ToArray();
                    //var positions = new Views.ShelvesView().Where(tem => shelveIDs.Contains(tem.FatherID));//获得该库房的所有库位信息

                    //获得可用的库位信息（包括自身）
                    item.UsableShelves = positions.Where(tem => (tem.LeaseID == item.ID || tem.LeaseID.Equals(null) || tem.LeaseID.Equals("")) && tem.SpecID.Equals(item.SpecID)).Select(tem => new { ID = tem.ID }).ToArray();

                    // 接受到的库位ID的集合（已分配情况下返回出去）
                    item.ShelveIDs = positions.Where(tem => tem.LeaseID == item.ID).Select(tem => tem.ID).ToArray();

                    list.Add(item);
                }

                var returnData = new
                {
                    OrderID = order.ID,
                    CreateDate = order.CreateDate,//创建时间
                    ClientID = order.ClientID,//客户编号
                    Status = order.Status,
                    StatuDes = order.Status.GetDescription(),
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    Summary = order.Summary,
                    LsNotices = list.Select(item => new
                    {
                        item.ID,
                        item.SpecID,
                        item.Quantity,
                        item.ShelveIDs,
                        item.UsableShelves

                    }).ToArray()
                };

                return returnData;
            }

        }

        public void LsNoticeEnter(string orderID, LsNotice[] lsnotices)
        {
            using (var rep = new PvWmsRepository())
            {
                foreach (var lsnotice in lsnotices)
                {
                    if (IsRepeat(lsnotice.ShelveIDs))
                    {
                        if (this != null && this.ShelveRepeated != null)
                        {
                            this.ShelveRepeated(this, new ErrorEventArgs("Shelve Repeated!!"));
                            return;
                        }
                    }
                    foreach (var shelveID in lsnotice.ShelveIDs)
                    {
                        var leaseID = new Views.ShelvesView()[shelveID].LeaseID;
                        //判断库位是否已经有合同，有合同=已经被租赁，就无法再次签合同(排除自身)
                        if (lsnotice.ID != leaseID && !string.IsNullOrWhiteSpace(leaseID))
                        {
                            if (this != null && this.ShelveNotUse != null)
                            {
                                this.ShelveNotUse(this, new ErrorEventArgs("Shelve cannot be used!!"));
                                return;
                            }
                        }
                    }
                }

                //根据orderid查询leaseid,根据leaseID去库位表查数据，个数大于0证明已经分配过库位，是修改功能；个数=0是未分配库位

                var leases = new LsNoticeView().Where(item => item.OrderID == orderID);
                var leaseIDs = leases.Select(item => item.ID).ToArray();
                var shelves = new Views.ShelvesView().Where(item => leaseIDs.Contains(item.LeaseID)).ToArray();

                using (var trans = new TransactionScope())
                {
                    try
                    {

                        rep.Update(new Layers.Data.Sqls.PvWms.LsNotice
                        {
                            Status = (int)LsOrderStatus.Allocated
                        }, item => item.OrderID == orderID);

                        //修改中心库的租赁通知日志表之前的Iscurrent=0
                        //再添加一条新的租赁通知日志数据IsCurrent=1

                        rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{orderID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                        rep.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{orderID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Allocated}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{admin.ID}','1')");

                        #region 添加信息
                        if (shelves.Count() == 0)
                        {
                            foreach (var lsnotice in lsnotices)
                            {
                                foreach (var shelveID in lsnotice.ShelveIDs)
                                {
                                    rep.Update(new Layers.Data.Sqls.PvWms.Shelves
                                    {
                                        LeaseID = lsnotice.ID
                                    }, item => item.ID == shelveID);
                                }
                            }
                        }
                        #endregion

                        else
                        {
                            //此方法行不通
                            //rep.Update<Layers.Data.Sqls.PvWms.Shelves>(new Layers.Data.Sqls.PvWms.Shelves
                            //{
                            //    LeaseID = null
                            //}, item => leaseIDs.Contains(item.LeaseID));


                            foreach (var leaseID in leaseIDs)
                            {
                                rep.Command($"update Shelves set leaseID=null where leaseid ='{leaseID}'");
                            }

                            foreach (var lsnotice in lsnotices)
                            {
                                foreach (var shelveID in lsnotice.ShelveIDs)
                                {
                                    rep.Update(new Layers.Data.Sqls.PvWms.Shelves
                                    {
                                        LeaseID = lsnotice.ID
                                    }, item => item.ID == shelveID);
                                }
                            }
                        }
                        rep.Submit();

                        trans.Complete();

                        if (this != null && this.Success != null)
                        {
                            this.Success(this, new SuccessEventArgs("Success!!"));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this != null && this.Failed != null)
                        {
                            this.Failed(this, new ErrorEventArgs("Failed!!"));
                        }

                    }
                    finally
                    {
                        trans.Dispose();
                    }

                }

                #region 已废弃的
                //if (contract.ContractItems != null)
                //{
                //    foreach (var contractItem in contract.ContractItems)
                //    {
                //        foreach (var shelveID in contractItem.ShelveIDs.Split(','))
                //        {
                //            //判断库位是否已经有合同，有合同=已经被租赁，就无法再次签合同
                //            if (!string.IsNullOrWhiteSpace(new Views.ShelvesView()[shelveID].LeaseID))
                //            {
                //                if (this != null && this.ShelveNotUse != null)
                //                {
                //                    this.ShelveNotUse(this, new ErrorEventArgs("Shelve cannot be used!!"));
                //                    return;
                //                }
                //            }
                //        }
                //    }

                //    foreach (var item in contract.ContractItems)
                //    {
                //        var contractItemID = PKeySigner.Pick(PkeyType.ContractItem);

                //        item.ID = contractItemID;
                //    }
                //}

                ////合同编号==合同code
                //var contractID = contract.Code;
                //if (new Views.ContractsView()[contractID] != null)
                //{
                //    if (this != null && this.CodeRepeated != null)
                //    {
                //        this.CodeRepeated(this, new ErrorEventArgs("Code Repeated!!"));
                //        return;
                //    }
                //}

                //using (var trans = new TransactionScope())
                //{
                //    try
                //    {

                //        rep.Insert(new Layers.Data.Sqls.PvWms.Contracts
                //        {
                //            ID = contractID,
                //            BrotherID = contract.BrotherID,
                //            Code = contract.Code,
                //            Type = (int)contract.Type,
                //            OwnerID = contract.OwnerID,
                //            StartTime = DateTime.Now /*contract.StartTime*/,
                //            EndTime = contract.EndTime,
                //            IsCleared = contract.IsCleared
                //        });

                //        foreach (var item in contract.ContractItems)
                //        {

                //            //rep.Insert(new Layers.Data.Sqls.PvWms.ContractItems
                //            //{
                //            //    ID = item.ID,
                //            //    ContractID = contractID,
                //            //    Quantity = item.Quantity,
                //            //    ShelveIDs = item.ShelveIDs,
                //            //    SpecID = item.SpecID
                //            //});

                //            foreach (var shelveID in item.ShelveIDs.Split(','))
                //            {
                //                rep.Update(new Layers.Data.Sqls.PvWms.Shelves
                //                {
                //                    ContractID = contractID
                //                }, s => s.ID == shelveID);
                //            }
                //        }

                //        trans.Complete();

                //        if (this != null && this.Success != null)
                //        {
                //            this.Success(this, new SuccessEventArgs("Success!!"));
                //        }
                //    }
                //    catch
                //    {
                //        if (this != null && this.Failed != null)
                //        {
                //            this.Failed(this, new ErrorEventArgs("Failed!!"));
                //        }
                //    }
                //    finally
                //    {
                //        trans.Dispose();
                //    }
                //}

                #endregion


            }
        }

        public void Submit(LsNotice[] lsNotices)
        {
            using (var rep = new PvWmsRepository())
            {
                //续租需要考虑提前续租和过期续租问题
                //1.获得var fatherID=dbo.PvLsOrder.Orders.FatherID值；
                //2.根据dbo.Pvwms.LsNotices的OrderID=fatherID获得对应的租赁通知的结束时间（endtime）；
                //3.当前时间和endtime进行比较，看是提前续租还是过期续租；
                //4.提前续租：
                // 4.1 修改dbo.Pvwms.LsNotices的OrderID=fatherID的状态值，改成已关闭的状态；
                // 4.2 修改中心库的状态；
                // 4.3 添加新的dbo.Pvwms.LsNotices数据；
                // 4.4 修改dbo.Pvwms.Shelves的leaseid,(dbo.Pvwms.LsNotices.Select{ID=>new{item.ID}}.Contains(leaseID))修改成新的dbo.Pvwms.LsNotices.ID;
                //5.过期续租：
                // 5.1 添加新的dbo.Pvwms.LsNotices数据；
                // 5.2 根据dbo.Pvwms.LsNotices的OrderID=fatherID获得租赁编号；
                // 5.3 返回ShelvesID=[]，因为续租的dbo.Pvwms.Shelves的LeaseID已经被清掉了
                //处理dbo.PvLsOrder.Orders的InheritStatus和FatherID

                //同步过来的订单ID
                var orderID = lsNotices.Select(item => item.OrderID).FirstOrDefault();
                //根据订单ID获得对应的同步订单信息
                var order = new Views.LsOrderTopView().Where(item => item.ID == orderID).FirstOrDefault();


                //判断fatherID不为空为续租,续租的处理
                if (!string.IsNullOrWhiteSpace(order.FatherID))
                {
                    //根据同步过来的订单的fatherID找到以前的租赁通知
                    var preLsNotices = new LsNoticeView().Where(item => item.OrderID == order.FatherID);
                    //过期续租
                    #region 过期续租
                    if (DateTime.Now > preLsNotices.Select(item => item.EndDate).FirstOrDefault())
                    {
                        foreach (var lsnotice in lsNotices)
                        {
                            lsnotice.ID = PKeySigner.Pick(PkeyType.LsNotice);
                        }

                        using (var trans = new TransactionScope())
                        {
                            try
                            {
                                var nowdate = DateTime.Now;
                                foreach (var lsNotice in lsNotices)
                                {
                                    rep.Insert(new Layers.Data.Sqls.PvWms.LsNotice
                                    {
                                        ID = lsNotice.ID,
                                        SpecID = lsNotice.SpecID,
                                        Quantity = lsNotice.Quantity,
                                        StartDate = lsNotice.StartDate,
                                        EndDate = lsNotice.EndDate,
                                        CreateDate = nowdate,
                                        Summary = lsNotice.Summary,
                                        OrderID = lsNotice.OrderID,
                                        ClientID = lsNotice.ClientID,
                                        PayeeID = lsNotice.PayeeID,
                                        Status = (int)LsOrderStatus.UnAllocate //未分配库位
                                    });
                                }

                                rep.Submit();

                                trans.Complete();
                            }
                            finally
                            {
                                trans.Dispose();
                            };

                        }

                    }
                    #endregion

                    //提前续租
                    #region 提前续租
                    else
                    {

                        foreach (var lsNotice in lsNotices)
                        {
                            lsNotice.ID = PKeySigner.Pick(PkeyType.LsNotice);
                            //获取以前的租赁通知，根据orderID和SpecID确定以前的租赁通知唯一值
                            lsNotice.PreLsNotice = preLsNotices.Where(item => item.SpecID == lsNotice.SpecID).FirstOrDefault();
                        }

                        using (var trans = new TransactionScope())
                        {
                            try
                            {
                                //修改以前的租赁通知为已关闭状态
                                rep.Update(new Layers.Data.Sqls.PvWms.LsNotice
                                {
                                    Status = (int)LsOrderStatus.Closed
                                }, item => item.OrderID == order.FatherID);

                                //修改旧的订单的中心库的状态为5已关闭
                                rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.FatherID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                                rep.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.FatherID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Closed}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");

                                //添加新的dbo.Pvwms.LsNotices数据
                                var nowdate = DateTime.Now;
                                foreach (var lsNotice in lsNotices)
                                {
                                    rep.Insert(new Layers.Data.Sqls.PvWms.LsNotice
                                    {
                                        ID = lsNotice.ID,
                                        SpecID = lsNotice.SpecID,
                                        Quantity = lsNotice.Quantity,
                                        StartDate = lsNotice.PreLsNotice.StartDate,//开始时间是上次通知的开始时间
                                        EndDate = lsNotice.EndDate,
                                        CreateDate = nowdate,
                                        Summary = lsNotice.Summary,
                                        OrderID = lsNotice.OrderID,
                                        ClientID = lsNotice.ClientID,
                                        PayeeID = lsNotice.PayeeID,
                                        Status = (int)LsOrderStatus.Allocated //续租：默认已分配库位
                                    });

                                    //获取以前的租赁通知，根据orderID和SpecID确定以前的租赁通知唯一值
                                    //var preLsNotice = new LsNoticeView().Where(item => item.OrderID == order.FatherID && item.SpecID == lsNotice.SpecID).FirstOrDefault();

                                    //修改dbo.Pvwms.Shelves的LeaseID为新的租赁ID
                                    rep.Update(new Layers.Data.Sqls.PvWms.Shelves
                                    {
                                        LeaseID = lsNotice.ID
                                    }, item => item.LeaseID == lsNotice.PreLsNotice.ID);
                                }

                                //修改新的订单的中心库状态为3（已分配）
                                rep.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.ID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                                rep.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.ID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Allocated}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");

                                rep.Submit();

                                trans.Complete();
                            }
                            finally
                            {
                                trans.Dispose();
                            }
                        }
                    }
                    #endregion

                }

                //不续租的处理
                else
                {
                    foreach (var lsnotice in lsNotices)
                    {
                        lsnotice.ID = PKeySigner.Pick(PkeyType.LsNotice);
                    }

                    using (var trans = new TransactionScope())
                    {
                        try
                        {
                            var nowdate = DateTime.Now;
                            foreach (var lsNotice in lsNotices)
                            {
                                rep.Insert(new Layers.Data.Sqls.PvWms.LsNotice
                                {
                                    ID = lsNotice.ID,
                                    SpecID = lsNotice.SpecID,
                                    Quantity = lsNotice.Quantity,
                                    StartDate = lsNotice.StartDate,
                                    EndDate = lsNotice.EndDate,
                                    CreateDate = nowdate,
                                    Summary = lsNotice.Summary,
                                    OrderID = lsNotice.OrderID,
                                    ClientID = lsNotice.ClientID,
                                    PayeeID = lsNotice.PayeeID,
                                    Status = (int)LsOrderStatus.UnAllocate //未分配库位
                                });
                            }

                            rep.Submit();

                            trans.Complete();
                        }
                        finally
                        {
                            trans.Dispose();
                        };

                    }
                }

            }

        }


        /// <summary>
        /// Hashtable 方法
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public bool IsRepeat(string[] array)
        {
            Hashtable ht = new Hashtable();
            for (int i = 0; i < array.Length; i++)
            {
                if (ht.Contains(array[i]))
                {
                    return true;
                }
                else
                {
                    ht.Add(array[i], array[i]);
                }
            }
            return false;
        }
    }
}

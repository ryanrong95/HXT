using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Wms.Services.chonggous
{
    public class LsNoticeManage
    {
        /// <summary>
        /// 租赁通知信息
        /// </summary>
        /// <param name="lsNotices"></param>
        public void Submit(LsNotice[] lsNotices)
        {
            #region 逻辑流程
            /*
             逻辑流程：判断传过来的Order的FatherID是否为空，不为空是续租，续租分过期续租和提前续租，提前续租时注意修改旧的订单的中心库的状态为5已关闭，修改旧的订单的租赁信息为新的订单ID和结束时间，修改新的订单的中心库状态为3（已分配）。过期续租和不续租的处理是一样的，添加新的租赁信息，修改订单的中心库状态为3（已分配）
             */
            #endregion

            using (var pvwms = new PvWmsRepository())
            //using (var pvcenter = new PvCenterReponsitory())
            {
                //同步过来的订单ID
                var orderID = lsNotices.Select(item => item.OrderID).FirstOrDefault();
                //根据订单ID获得对应的同步订单信息
                var order = new Services.Views.LsOrderTopView().Where(item => item.ID == orderID).FirstOrDefault();

                //续租
                if (pvwms.ReadTable<Layers.Data.Sqls.PvWms.LsNotice>().Any(item => item.OrderID == order.FatherID))
                {
                    //根据同步过来的订单的fatherID找到以前的租赁通知
                    var preLsNotices = new LsNoticeView().Where(item => item.OrderID == order.FatherID);

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

                                //添加新的租赁信息
                                var nowdate = DateTime.Now;
                                foreach (var lsNotice in lsNotices)
                                {
                                    pvwms.Insert(new Layers.Data.Sqls.PvWms.LsNotice
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
                                        Status = (int)LsOrderStatus.Allocated //通知过来后默认已分配库位
                                    });
                                }

                                //修改新的订单的中心库状态为3（已分配）
                                pvwms.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.ID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                                pvwms.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.ID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Allocated}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");
                                //修改订单的中心库状态为3（已分配）
                                //pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvLsOrder>(new
                                //{
                                //    IsCurrent = false
                                //}, item => item.MainID == order.ID && item.Type == (int)LsOrderStatusType.MainStatus);
                                //pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder
                                //{
                                //    ID = Guid.NewGuid().ToString(),
                                //    MainID = order.ID,
                                //    Type = (int)LsOrderStatusType.MainStatus,
                                //    Status = (int)LsOrderStatus.Allocated,
                                //    CreateDate = DateTime.Now,
                                //    CreatorID = order.Creator,
                                //    IsCurrent = true
                                //});

                                pvwms.Submit();
                                trans.Complete();

                            }
                            finally
                            {

                                trans.Dispose();
                            }
                        }
                    }
                    #endregion

                    #region 提前续租
                    else
                    {
                        using (var trans = new TransactionScope())
                        {
                            try
                            {

                                //修改旧的订单的中心库的状态为5已关闭
                                pvwms.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.FatherID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                                pvwms.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.FatherID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Closed}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");
                                ////修改旧的订单的中心库的状态为5已关闭
                                //pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvLsOrder>(new
                                //{
                                //    IsCurrent = false
                                //}, item => item.MainID == order.FatherID && item.Type == (int)LsOrderStatusType.MainStatus);
                                //pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder
                                //{
                                //    ID = Guid.NewGuid().ToString(),
                                //    MainID = order.FatherID,
                                //    Type = (int)LsOrderStatusType.MainStatus,
                                //    Status = (int)LsOrderStatus.Closed,
                                //    CreateDate = DateTime.Now,
                                //    CreatorID = order.Creator,
                                //    IsCurrent = true
                                //});

                                //修改旧的订单的租赁信息为新的订单ID和新的结束时间
                                pvwms.Update<Layers.Data.Sqls.PvWms.LsNotice>(new
                                {
                                    OrderID = orderID,
                                    EndDate = lsNotices.FirstOrDefault().EndDate
                                }, item => item.OrderID == order.FatherID);

                                //修改新的订单的中心库状态为3（已分配）
                                pvwms.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.ID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                                pvwms.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.ID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Allocated}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");
                                ////修改新的订单的中心库状态为3（已分配）
                                //pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvLsOrder>(new
                                //{
                                //    IsCurrent = false
                                //}, item => item.MainID == order.ID && item.Type == (int)LsOrderStatusType.MainStatus);
                                //pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder
                                //{
                                //    ID = Guid.NewGuid().ToString(),
                                //    MainID = order.ID,
                                //    Type = (int)LsOrderStatusType.MainStatus,
                                //    Status = (int)LsOrderStatus.Allocated,
                                //    CreateDate = DateTime.Now,
                                //    CreatorID = order.Creator,
                                //    IsCurrent = true
                                //});

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
                            //添加新的租赁信息
                            var nowdate = DateTime.Now;
                            foreach (var lsNotice in lsNotices)
                            {
                                pvwms.Insert(new Layers.Data.Sqls.PvWms.LsNotice
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
                                    Status = (int)LsOrderStatus.Allocated //通知过来后默认已分配库位
                                });
                            }


                            //修改新的订单的中心库状态为3（已分配）
                            pvwms.Command($"update {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)} set {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)} = '0' where MainID='{order.ID}' and Type='{(int)LsOrderStatusType.MainStatus}'");
                            pvwms.Command($"insert into {nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView)}({nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.ID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.MainID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Type)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.Status)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreateDate)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.CreatorID)},{nameof(Layers.Data.Sqls.PvWms.Logs_PvLsOrderTopView.IsCurrent)}) values('{Guid.NewGuid().ToString()}','{order.ID}','{(int)LsOrderStatusType.MainStatus}','{(int)LsOrderStatus.Allocated}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{order.Creator}','1')");
                            ////修改订单的中心库状态为3（已分配）
                            //pvcenter.Update<Layers.Data.Sqls.PvCenter.Logs_PvLsOrder>(new
                            //{
                            //    IsCurrent = false
                            //}, item => item.MainID == order.ID && item.Type == (int)LsOrderStatusType.MainStatus);
                            //pvcenter.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder
                            //{
                            //    ID = Guid.NewGuid().ToString(),
                            //    MainID = order.ID,
                            //    Type = (int)LsOrderStatusType.MainStatus,
                            //    Status = (int)LsOrderStatus.Allocated,
                            //    CreateDate = DateTime.Now,
                            //    CreatorID = order.Creator,
                            //    IsCurrent = true
                            //});

                            pvwms.Submit();
                            trans.Complete();

                        }
                        finally
                        {
                            trans.Dispose();
                        }
                    }
                }
            }
        }
    }
}

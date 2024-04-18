using Needs.Underly;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services.Enums;
using WebApi.Models;
using Needs.Utils.Serializers;
using Needs.Ccs.Services.Models;

namespace WebApi.Controllers
{
    public class DeclarationController : MyApiController
    {
        /// <summary>
        /// 库房申请报关
        /// </summary>
        /// <param name="DecInfo"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public ActionResult DeclarationNotice(JPost DecInfo)
        {
            try
            {
                var applies = DecInfo.ToObject<DeclarationApply>();

                if (applies.Items.Count < 1)
                {

                    return Json(new JMessage()
                    {
                        code = 300,
                        success = false,
                        data = "参数Items为空!"
                    }, JsonRequestBehavior.AllowGet);
                }

                string batchID = Guid.NewGuid().ToString("N");
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    // OrderID = applies.Items[0].VastOrderID,
                    TinyOrderID = applies.Items[0].TinyOrderID,
                    RequestContent = applies.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "库房调用报关申请"
                };
                apiLog.Enter();

                var OriginAdminID = new AdminsTopView2().FirstOrDefault(t => t.ID == applies.Items[0].AdminID)?.OriginID;
                if (string.IsNullOrEmpty(OriginAdminID))
                {
                    OriginAdminID = "XDTAdmin";
                }


                var currentUnDecNoticeCount = new Needs.Ccs.Services.Views.CurrentUnDecNoticeCountView().GetCurrentUnDecNoticeCount();

                var TinyOrderIDs = new List<string>();

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    foreach (var orderid in applies.Items.GroupBy(t => t.TinyOrderID).Select(t => t.Key).Distinct())
                    {
                        //验证是否有重复
                        if (!new DeclarationNoticesView().Any(t => t.OrderID == orderid && t.Status != DeclareNoticeStatus.Cancel))
                        {
                            //生成报关通知
                            var entity = new Layer.Data.Sqls.ScCustoms.DeclarationNotices();
                            entity.ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.DeclareNotice);
                            entity.OrderID = orderid;
                            entity.AdminID = OriginAdminID;
                            entity.Status = (int)DeclareNoticeStatus.UnDec;
                            entity.CreateDate = DateTime.Now;
                            entity.UpdateDate = DateTime.Now;
                            entity.CreateDeclareAdminID = GetDeclareCreatorAdminID(currentUnDecNoticeCount);
                            reponsitory.Insert(entity);

                            //生成报关通知项
                            //此处认为分拣ID唯一，且不会出现在不同的小定单中
                            foreach (var item in applies.Items.Where(t => t.TinyOrderID == orderid).ToList())
                            {
                                var entityItem = new Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems();
                                entityItem.ID = Needs.Ccs.Services.ChainsGuid.NewGuidUp();
                                entityItem.DeclarationNoticeID = entity.ID;
                                entityItem.SortingID = item.DeclareID;
                                entityItem.Status = (int)DeclareNoticeItemStatus.UnMake;
                                entityItem.CreateDate = entity.CreateDate;
                                entityItem.UpdateDate = entity.UpdateDate;
                                reponsitory.Insert(entityItem);
                            }

                            //更改订单的DeclareFlag,只要提交过来，就改为3
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                            {
                                DeclareFlag = (int)DeclareFlagEnums.Done
                            }, item => item.ID == orderid);

                            TinyOrderIDs.Add(orderid);
                        }
                    }
                }

                //var tinyOrderIDs = applies.Items.Select(t => t.TinyOrderID).Distinct();
                //foreach (var orderid in TinyOrderIDs)
                //{
                //    NoticeLog notice = new NoticeLog();
                //    notice.ID = orderid;
                //    notice.NoticeType = SendNoticeType.DecNoticePending;
                //    notice.SendNotice();
                //}


                var json = new JMessage()
                {
                    code = 200,
                    success = true,
                    data = "提交成功"
                };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        private string GetDeclareCreatorAdminID(List<CurrentUnDecNoticeCountViewModel> listModel)
        {
            if (listModel == null || !listModel.Any())
            {
                return null;
            }

            var minCount = listModel.OrderBy(t => t.UnDecNoticeCount).FirstOrDefault().UnDecNoticeCount;
            int[] serialNos = listModel.Where(t => t.UnDecNoticeCount == minCount).Select(t => t.SerialNo).ToArray();

            Random rand = new Random();
            int arrNum = rand.Next(0, serialNos.Count() - 1);

            var theSelectedModel = listModel.Where(t => t.SerialNo == serialNos[arrNum]).FirstOrDefault();

            for (int i = 0; i < listModel.Count; i++)
            {
                if (listModel[i].SerialNo == serialNos[arrNum])
                {
                    listModel[i].UnDecNoticeCount = listModel[i].UnDecNoticeCount + 1;
                    break;
                }
            }

            return theSelectedModel.AdminID;
        }

    }
}
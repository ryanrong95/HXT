using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    public class DutiablePricePost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Run()
        {
            DateTime DateTimeFrom = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00");
            DateTime DateTimeTo = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59");

            List<string> DecHeads = new List<string>();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                DecHeads = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxs>().
                                     Where(item => item.IsUpload == (int)Needs.Ccs.Services.Enums.UploadStatus.Uploaded
                                     && item.UpdateDate >= DateTimeFrom && item.UpdateDate <= DateTimeTo).
                                     Select(item => item.ID).ToList();

                foreach (var item in DecHeads)
                {
                    try
                    {
                        var head = new Needs.Ccs.Services.Views.DecHeadsView().Where(t => t.ID == item).FirstOrDefault();
                        var Order = new Needs.Ccs.Services.Views.IcgooOrdersView().Where(t => t.ID == head.OrderID).FirstOrDefault();
                        bool ifPost = false;
                        var changeNotice = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Where(t => t.OderID == head.OrderID).FirstOrDefault();
                        if (changeNotice != null)
                        {
                            if (changeNotice.ProcessState == (int)Needs.Ccs.Services.Enums.ProcessState.Processing || changeNotice.ProcessState == (int)Needs.Ccs.Services.Enums.ProcessState.Processed)
                            {
                                ifPost = true;
                            }
                        }
                        else
                        {
                            ifPost = true;
                        }

                        if (ifPost)
                        {
                            int postCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DutiablePricePostLog>().Where(t => t.OrderID == head.OrderID && t.PostStatus == true).Count();

                            if (postCount == 0)
                            {
                                switch (Order.Type)
                                {
                                    case Needs.Ccs.Services.Enums.OrderType.Icgoo:
                                        PostIcgoo(head);
                                        break;

                                    case Needs.Ccs.Services.Enums.OrderType.Inside:
                                        PostInside(head);
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                        continue;
                    }
                }
            }
        }

        private void PostInside(DecHead head)
        {
            string result = head.PostInsideDutiablePrice();

            DutiablePriceJson model = JsonConvert.DeserializeObject<DutiablePriceJson>(result);

            string msg = "";
            bool boolIsAllSuccess = true;

            foreach (var item in model.data)
            {
                if (item.状态 == false)
                {
                    msg += Encryption.Decrypt(item.单据号) + ",";
                    boolIsAllSuccess = false;
                }
            }

            if (boolIsAllSuccess)
            {
                msg = model.msg;
            }
            else
            {
                msg += "型号提交失败";
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DutiablePricePostLog>().Where(item => item.OrderID == head.OrderID).Count();
                if (count == 0)
                {
                    DutiablePricePostBack p = new DutiablePricePostBack();
                    p.ID = ChainsGuid.NewGuidUp();
                    p.OrderID = head.OrderID;
                    p.PostStatus = model.success;
                    p.Msg = msg;
                    p.CreateDate = DateTime.Now;
                    p.UpdateDate = DateTime.Now;
                    p.Status = Status.Normal;
                    p.Enter();
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DutiablePricePostLog>(
                          new
                          {
                              UpdateDate = DateTime.Now,
                              PostStatus = model.success,
                          }, item => item.OrderID == head.OrderID);
                }
            }

        }

        private void PostIcgoo(DecHead head)
        {
            string result = head.PostIcgooDutiablePrice();

            List<IcgooDutiablePriceJson> model = JsonConvert.DeserializeObject<List<IcgooDutiablePriceJson>>(result);

            string msg = "";
            bool boolIsAllSuccess = true;

            foreach (var item in model)
            {
                if (item.status == false)
                {
                    msg += item.id + ",";
                    boolIsAllSuccess = false;
                }
            }

            if (boolIsAllSuccess)
            {
                msg = "提交成功";
            }
            else
            {
                msg += "型号提交失败";
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DutiablePricePostLog>().Where(item => item.OrderID == head.OrderID).Count();
                if (count == 0)
                {
                    DutiablePricePostBack p = new DutiablePricePostBack();
                    p.ID = ChainsGuid.NewGuidUp();
                    p.OrderID = head.OrderID;
                    p.PostStatus = boolIsAllSuccess;
                    p.Msg = msg;
                    p.CreateDate = DateTime.Now;
                    p.UpdateDate = DateTime.Now;
                    p.Status = Status.Normal;
                    p.Enter();
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DutiablePricePostLog>(
                          new
                          {
                              UpdateDate = DateTime.Now,
                              PostStatus = boolIsAllSuccess,
                          }, item => item.OrderID == head.OrderID);
                }
            }
        }
    }
}

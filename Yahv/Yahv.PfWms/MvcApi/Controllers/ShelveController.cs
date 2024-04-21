using Yahv.Utils.EventExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;

namespace MvcApp.Controllers
{
    public class ShelveController : Controller
    {

        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("名称不能为空")]
            NameIsNull = 2,
            [Description("名称不能重复")]
            NameRepeated = 3,
            [Description("名称不支持修改")]
            IDNotSupported = 4,
            [Description("所要修改的数据不存在")]
            DataIsNull = 5,
            [Description("父级编号不能为空")]
            FatherIDIsNull = 6
        }

        Message message;

        public class ShelveMessage
        {
            public string Key { get; set; }

            /// <summary>
            /// 类型
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// 编号
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 负责人编号
            /// </summary>
            public string ManagerID { get; set; }

            /// <summary>
            /// 业务类型
            /// </summary>
            public string Purpose { get; set; }
            public int PageIndex { get; set; } = 1;
            public int PageSize { get; set; } = 10;
        }


        /// <summary>
        /// 根据父亲ID分别获得库区、货架、库位的数据
        /// </summary>
        /// <param name="fatherID">父亲ID</param>
        /// <param name="managerID">负责人，在库人员</param>
        /// <param name="pageindex">当前页码</param>
        /// <param name="pagesize">每页记录数</param>
        /// <returns></returns>
        public ActionResult Index(ShelveMessage datas)
        {
            //库房数据同步
            WarehouseSync();

            Expression<Func<Shelves, bool>> exp = item => item.Status != Wms.Services.Enums.ShelvesStatus.Deleted && item.FatherID.Equals(datas.Key) && item.Type != Wms.Services.Enums.ShelvesType.Warehouse;

            //exp = PredicateExtends.And(exp, item => item.FatherID.Equals(datas.Key) && item.Type != Wms.Services.Enums.ShelvesType.Warehouse);
            if (!string.IsNullOrWhiteSpace(datas.ID))
            {
                exp = PredicateExtends.And(exp, item => item.ID == datas.ID);
            }
            if (!string.IsNullOrWhiteSpace(datas.Type))
            {
                exp = PredicateExtends.And(exp, item => item.Type == (Wms.Services.Enums.ShelvesType)(Convert.ToInt32(datas.Type)));
            }
            if (!string.IsNullOrWhiteSpace(datas.ManagerID))
            {
                exp = PredicateExtends.And(exp, item => item.ManagerID == datas.ManagerID);
            }
            if (!string.IsNullOrWhiteSpace(datas.Purpose))
            {
                exp = PredicateExtends.And(exp, item => item.Purpose == (Wms.Services.Enums.ShelvesPurpose)(Convert.ToInt32(datas.Purpose)));
            }

            #region 放弃这种StartWith的写法
            ////根据库房获得库区
            //if (datas.Key.Length == 4)
            //{
            //    exp = PredicateExtends.And(exp, item => item.ID.StartsWith(datas.Key) && item.ID.Length == 6 && item.Type == Wms.Services.Enums.ShelvesType.Region);
            //}
            ////根据库区获得货架/卡板
            //if (datas.Key.Length == 6)
            //{
            //    exp = PredicateExtends.And(exp, item => item.ID.StartsWith(datas.Key) && item.ID.Length == 8);
            //}
            ////根据货架/卡板获得库位
            //if (datas.Key.Length == 8)
            //{
            //    exp = PredicateExtends.And(exp, item => item.ID.StartsWith(datas.Key) && item.ID.Length == 13);
            //}
            //exp = PredicateExtends.And(exp, item => item.FatherID == datas.Key.ToUpper());
            #endregion

            return Json(new { obj = new Wms.Services.Views.ShelvesView().Where(exp).Paging(datas.PageIndex, datas.PageSize) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Index(Shelves datas)
        {
            try
            {
                //foreach (var item in datas)
                //{
                datas.Name = datas.Name.ToUpper() ?? "";
                //var name = datas.Name.ToUpper() ?? "";
                if (string.IsNullOrWhiteSpace(datas.Name))
                {
                    return Json(new { val = (int)Message.NameIsNull, msg = Message.NameIsNull.GetDescription() });
                }
                if (string.IsNullOrWhiteSpace(datas.FatherID))
                {
                    return Json(new { val = (int)Message.FatherIDIsNull, msg = Message.FatherIDIsNull.GetDescription() });
                }
                datas.AddEvent("ShelvesSuccess", new SuccessHanlder(Datas_ShelvesSuccess))
                    .AddEvent("ShelvesFailed", new ErrorHanlder(Datas_ShelvesFailed))
                    .AddEvent("CheckNameRepeated", new ErrorHanlder(Datas_CheckNameRepeated))
                    .AddEvent("IDNotSupportModify", new ErrorHanlder(Datas_IDNotSupportModify))
                    .AddEvent("NotSupportedUpdate", new ErrorHanlder(Datas_NotSupportedUpdate))
                    .Enter();
                //}
                return Json(new { val = (int)message, msg = message.GetDescription() });

            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }
        }

        public ActionResult Delete(string shelveID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(shelveID))
                {
                    return Json(new { val = (int)Message.NameIsNull, msg = Message.NameIsNull.GetDescription() });
                }

                Shelves data = new Wms.Services.Models.Shelves() { ID = shelveID };
                data.AddEvent("AbandonSuccess", new SuccessHanlder(Data_AbandonSuccess))
                      .AddEvent("AbandonFailed", new ErrorHanlder(Data_AbandonFailed))
                      .Abandon();

                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
        }

        private void Data_AbandonFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Data_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }

        public ActionResult GetWarehouse(ShelveMessage datas)
        {
            //库房数据同步
            WarehouseSync();
            Expression<Func<Shelves, bool>> exp = item => item.Type == Wms.Services.Enums.ShelvesType.Warehouse && item.Status != Wms.Services.Enums.ShelvesStatus.Deleted;

            //获得某库房下的门牌库房数据
            if (!string.IsNullOrWhiteSpace(datas.Key))
            {
                exp = PredicateExtends.And(exp, item => item.FatherID == datas.Key);
            }
            //获取某库房数据
            else
            {
                exp = PredicateExtends.And(exp, item => item.FatherID.Equals(null));
            }

            if (!string.IsNullOrWhiteSpace(datas.ID))
            {
                exp = PredicateExtends.And(exp, item => item.ID == datas.ID);
            }
            if (!string.IsNullOrWhiteSpace(datas.Type))
            {
                exp = PredicateExtends.And(exp, item => item.Type == (Wms.Services.Enums.ShelvesType)(Convert.ToInt32(datas.Type)));
            }
            if (!string.IsNullOrWhiteSpace(datas.ManagerID))
            {
                exp = PredicateExtends.And(exp, item => item.ManagerID == datas.ManagerID);
            }
            if (!string.IsNullOrWhiteSpace(datas.Purpose))
            {
                exp = PredicateExtends.And(exp, item => item.Purpose == (Wms.Services.Enums.ShelvesPurpose)(Convert.ToInt32(datas.Purpose)));
            }


            return Json(new
            {
                obj = new Wms.Services.Views.WarehousesView().Where(exp).Paging(datas.PageIndex, datas.PageSize)
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证是否名称重复
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsNameRepeated(string id)
        {
            return new Wms.Services.Views.ShelvesView()[id] == null;
        }

        /// <summary>
        /// 获取可以使用的库位接口（除了没有被租赁出去的还有自己已经租赁的也属于自己可以使用的库位）
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUsableShelves(string whid, string clientID = null)
        {
            if (string.IsNullOrWhiteSpace(whid))
            {
                return null;
            }

            //根据库房ID获得对应库房信息
            var warehouse = new Wms.Services.Views.WarehousesView()[whid];
            //库位的ShelvesView的判断条件
            Expression<Func<Shelves, bool>> exp = null;

            string[] leaseIDs = null;//客户编号不为空的时候，找出对应的已租赁的编号
            if (!string.IsNullOrWhiteSpace(clientID))
            {
                leaseIDs = new Yahv.Services.Views.LsNoticeView().Where(item => item.ClientID == clientID).Select(item => item.ID).ToArray();
            }

            if (!string.IsNullOrWhiteSpace(warehouse.FatherID))
            {
                var regions = new Wms.Services.Views.ShelvesView().Where(tem => tem.FatherID == whid);//获得该库房的所有库区信息
                if (regions.Count() <= 0)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var regionIDs = regions.Select(tem => tem.ID).ToArray();

                foreach (var ids in regionIDs)
                {
                    exp = PredicateExtends.Or(exp, item => item.ID.StartsWith(ids));
                }

                if (leaseIDs != null)
                {
                    exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("") || leaseIDs.Contains(item.LeaseID)) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status == Wms.Services.Enums.ShelvesStatus.Normal);
                }
                else
                {
                    exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("")) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status == Wms.Services.Enums.ShelvesStatus.Normal);
                }

                return Json(new
                {
                    obj = new Wms.Services.Views.ShelvesView().Where(exp).Select(item => new { ID = item.ID })
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (leaseIDs != null)
                {
                    exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("") || leaseIDs.Contains(item.LeaseID)) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status != Wms.Services.Enums.ShelvesStatus.Deleted && item.Type != Wms.Services.Enums.ShelvesType.Warehouse && item.ID.StartsWith(whid));
                }
                else
                {
                    exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("")) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status != Wms.Services.Enums.ShelvesStatus.Deleted && item.Type != Wms.Services.Enums.ShelvesType.Warehouse && item.ID.StartsWith(whid));
                }

                if (leaseIDs != null)
                {
                    exp = PredicateExtends.And(exp, item => leaseIDs.Contains(item.ID));
                }

                return Json(new
                {
                    obj = new Wms.Services.Views.ShelvesView().Where(exp).Select(item => new
                    {
                        item.ID
                    })
                }, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult GetShelves(string whid)
        //{
        //    //根据库房ID获得对应库房信息
        //    var warehouse = new Wms.Services.Views.WarehousesView()[whid];
        //    //库位的ShelvesView的判断条件
        //    Expression<Func<Shelves, bool>> exp = null;

        //    exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("")) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status == Wms.Services.Enums.ShelvesStatus.Normal);

        //    //List<Shelves> list = new Wms.Services.Views.ShelvesView().Where(exp).ToList();
        //    List<Shelves> list = new List<Shelves>();
        //    if (!string.IsNullOrWhiteSpace(warehouse.FatherID))
        //    {
        //        var regions = new Wms.Services.Views.ShelvesView().Where(tem => tem.FatherID == whid);//获得该库房的所有库区信息
        //        if (regions.Count() <= 0)
        //        {
        //            return Json(null, JsonRequestBehavior.AllowGet);
        //        }

        //        foreach (var region in regions)
        //        {
        //            region.Children = new Wms.Services.Views.ShelvesView().Where(tem => tem.ID.StartsWith(region.ID) && (tem.Type == Wms.Services.Enums.ShelvesType.Shelve || tem.Type == Wms.Services.Enums.ShelvesType.Shelve)).ToArray();
        //            if (region.Children.Count() <= 0)
        //            {
        //                break;
        //            }

        //            foreach (var shelve in region.Children)
        //            {
        //                shelve.Children= new Wms.Services.Views.ShelvesView().Where(tem => tem.ID.StartsWith(region.ID) && tem.Type == Wms.Services.Enums.ShelvesType.Position).ToArray();
        //            }
        //        }

        //        return Json(new
        //        {
        //            obj = list.Select(item => new { ID = item.ID })
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        exp = PredicateExtends.And(exp, item => (item.LeaseID.Equals(null) || item.LeaseID.Equals("")) && item.Purpose == Wms.Services.Enums.ShelvesPurpose.Rent && item.Status != Wms.Services.Enums.ShelvesStatus.Deleted && item.Type != Wms.Services.Enums.ShelvesType.Warehouse && item.ID.StartsWith(whid));

        //        return Json(new
        //        {
        //            obj = new Wms.Services.Views.ShelvesView().Where(exp).Select(item => new
        //            {
        //                item.ID
        //            })
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// 库位是否存在
        /// </summary>
        /// <param name="shelveID"></param>
        /// <returns></returns>
        public bool ShelveIsExits(string warehouseID, string shelveID)
        {
            var warehouse = new Wms.Services.Views.WarehousesView()[warehouseID];
            //库位的ShelvesView的判断条件
            Expression<Func<Shelves, bool>> exp = null;
            if (!string.IsNullOrWhiteSpace(warehouse.FatherID))
            {
                var regions = new Wms.Services.Views.ShelvesView().Where(tem => tem.FatherID == warehouseID);//获得该库房的所有库区信息
                if (regions.Count() <= 0)
                {
                    return false;
                }

                var regionIDs = regions.Select(tem => tem.ID).ToArray();

                foreach (var ids in regionIDs)
                {
                    exp = PredicateExtends.Or(exp, item => item.ID.StartsWith(ids));
                }
            }
            else
            {
                exp = PredicateExtends.And(exp, item => item.ID.StartsWith(warehouseID) && item.Type != Wms.Services.Enums.ShelvesType.Warehouse);
            }
            var shelves = new Wms.Services.Views.ShelvesView().Where(exp);

            var shelve = shelves.Where(item => item.ID == shelveID);
            if (shelve.Count() > 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 同步库房数据
        /// </summary>
        /// <returns></returns>
        private void WarehouseSync()
        {
            new Wms.Services.Views.WarehousesView().LoadData();
        }

        #region 事件

        private void Datas_NotSupportedUpdate(object sender, ErrorEventArgs e)
        {
            message = Message.DataIsNull;
        }

        private void Datas_IDNotSupportModify(object sender, ErrorEventArgs e)
        {
            message = Message.IDNotSupported;
        }

        private void Datas_CheckNameRepeated(object sender, ErrorEventArgs e)
        {
            message = Message.NameRepeated;
        }

        private void Datas_ShelvesFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_ShelvesSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }

        #endregion



    }
}
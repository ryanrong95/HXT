using Layers.Data.Sqls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Yahv.Linq.Extends;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Wms.Services.chonggous
{


    public class ShelveInfo
    {
        /// <summary>
        /// 库位编号
        /// </summary>
        public string ShelveID { get; set; }

        /// <summary>
        /// 库位对应的总产品件数（总种类产品的数量）（一个库位理论上不能超过五种产品）
        /// </summary>
        public int TotalPackage { get; set; }
    }

    public delegate void ShelveLeasedEventHandler(object sender, EventArgs e);

    public class ShelveManage : IEnumerable<string>
    {

        /// <summary>
        /// 库房
        /// </summary>
        //ConcurrentBag<string> places;
        SortedSet<string> places;
        ShelveManage()
        {
            //this.places = new ConcurrentBag<string>();

            this.places = new SortedSet<string>();
            //this.Init();
        }

        #region 一、单例+事件 返回删除失败事件(暂时采用第二种)

        //object objectLock = new Object();

        //event ShelveLeasedEventHandler shelveLeased;
        ///// <summary>
        ///// 已经被租赁事件
        ///// </summary>
        //public event ShelveLeasedEventHandler ShelveLeased
        //{
        //    add
        //    {
        //        lock (objectLock)
        //        {

        //            System.Reflection.EventInfo eventInfo = this.GetType().GetEvent(nameof(this.ShelveLeased));

        //            if (shelveLeased != null)
        //            {
        //                foreach (Delegate dele in shelveLeased.GetInvocationList())
        //                {
        //                    shelveLeased -= (dele as ShelveLeasedEventHandler);
        //                    eventInfo.RemoveEventHandler(this, dele);
        //                }

        //            }
        //            shelveLeased += value;
        //        }
        //    }
        //    remove
        //    {
        //        lock (objectLock)
        //        {
        //            shelveLeased -= value;
        //        }
        //    }
        //}

        #endregion


        /// <summary>
        /// 返回库位以及对应的产品件数（??如果一个库房库位特别多，如何保证效率）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <remarks>
        /// 获取可用库位的时候一定要把EnterCode拿到，租赁了库位的人一定需要有个入仓号
        /// </remarks>
        public IEnumerable<ShelveInfo> this[string whid]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(whid))
                {
                    throw new ArgumentNullException($@"{nameof(whid)}", "不能为空！");
                }

                //如果要是增加 entercode
                //读取 租赁的数据
                //返回  自己租赁与其他没有被租赁的数据


                //查询每个库位上的产品件数

                using (var repository = new PvWmsRepository())
                {

                    //取出库房下的所有有库存的库位

                    //如果一个库位被使用，在一定条件下，我们不允许使用这个
                    //可以在获取库位的时候完成对库位使用的验证

                    var linq = from storage in repository.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                               where storage.WareHouseID == GetWhCode(whid) && storage.Quantity > 0
                               group storage by storage.ShelveID into groups
                               select new
                               {
                                   ShelveID = groups.Key,
                                   Count = groups.Count()
                               };
                    var arry = linq.ToArray();

                    var shelves = (from shelve in repository.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
                                   select new
                                   {
                                       shelve.ID
                                   }).ToArray();

                    return from shelve in shelves
                           join storage in arry on shelve.ID equals storage.ShelveID into storages
                           from storage in storages.DefaultIfEmpty()
                           where shelve.ID.StartsWith(("P" + GetWhCode(whid)))
                           orderby shelve.ID ascending
                           select new ShelveInfo
                           {
                               ShelveID = shelve.ID,
                               TotalPackage = storage?.Count ?? 0
                           };

                    //某些库位已经有了客户的货了，就尽量把这些库位置顶!
                }
            }
        }

        static ShelveManage current;
        static object locker = new object();

        /// <summary>
        /// 当前引用
        /// </summary>
        /// <remarks>
        /// 利用单利的锁返回一致的数据，多个会话
        /// </remarks>
        static public ShelveManage Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new ShelveManage();
                        }
                    }
                }
                return current;
            }
        }

        /// <summary>
        /// 设置库区
        /// </summary>
        /// <param name="whCode">指定库房</param>
        /// <param name="region">指定库区</param>
        /// <param name="name">库区名称</param>
        public void SetRegion(string whCode, string region, string name = null)
        {
            if (string.IsNullOrWhiteSpace(whCode) || string.IsNullOrWhiteSpace(region))
            {
                throw new ArgumentNullException($@"{ nameof(whCode)}\{nameof(region)})", "不能为空！");
            }

            var newregion = string.Concat(GetWhCode(whCode), region);

            if (newregion.Length != 4)
            {
                //返回格式错误
                throw new Exception("库区格式有误！！");
                //return;
            }
            using (var rep = new PvWmsRepository())
            {
                if (!this.places.Contains(newregion))
                {
                    this.places.Add(newregion);
                }

                //添加的数据是否已经存在，存在即修改
                if (rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Any(item => item.ID == newregion))
                {
                    rep.Update<Layers.Data.Sqls.PvWms.Shelves>(new
                    {
                        Name = name,
                    }, item => item.ID == newregion);
                }
                //否则为添加
                else
                {
                    rep.Insert(new Layers.Data.Sqls.PvWms.Shelves
                    {
                        ID = newregion,
                        Name = name,
                        WhCode = GetWhCode(whCode),
                        DoorCode = "",
                        RegionCode = region,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)ShelvesStatus.Normal,
                        Summary = ""
                    });
                }
            }
        }

        /// <summary>
        /// 设置库位
        /// </summary>
        /// <param name="whCode">库房编号</param>
        /// <param name="place">位（前台拼好库区+货架+位置）</param>
        public bool SetPlace(string whCode, string place)
        {
            //自动检查  库区是否存在？
            //如果不存在 也增加相应的 SetRegion(whCode , region)
            //var regionCode = string.Concat(whCode, place.Substring(0, 2));

            //验证库位是否合法？
            //合法就加入，不合法就报错！
            //P开头
            var newPlace = string.Concat("P", GetWhCode(whCode), place);

            Regex regex = new Regex(@"P[A-Z]{2}[0][A-Z]{1}\d{3,}", RegexOptions.None);

            if (!regex.IsMatch(newPlace))
            {
                throw new Exception("该库位不符合规则！！");
            }

            using (var rep = new PvWmsRepository())
            {
                //添加的数据是否已经存在
                var exsit = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Any(item => item.ID == newPlace);
                if (exsit)
                {
                    return false;
                }
                else
                {
                    if (!this.places.Contains(newPlace))
                    {
                        this.places.Add(newPlace);
                    }

                    //添加库位
                    rep.Insert(new Layers.Data.Sqls.PvWms.Shelves
                    {
                        ID = newPlace,
                        WhCode = GetWhCode(whCode),
                        DoorCode = "",
                        RegionCode = GetWhCode(whCode) + place.Substring(0, 2),
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)ShelvesStatus.Normal,
                        Summary = ""
                    });
                    return true;
                }
            }

            //验证库位是否合法？
            //合法就加入，不合法就报错！
            //P

            //自动检查  库区是否存在？
            //如果不存在 也增加相应的 SetRegion(whCode , region)

            //this.places.AddRange(places);
        }

        /// <summary>
        /// 卡板设置
        /// </summary>
        /// <param name="whCode">库房编号</param>
        /// <param name="places">卡板编号</param>
        public bool SetPallet(string whCode, string pallet)
        {
            //验证卡板设是否合法？
            //PA开头
            //合法就加入，不合法就报错！
            //卡板：库房+编号（不用要区号）
            //卡板规则：PAHK02

            if (string.IsNullOrWhiteSpace(whCode) || string.IsNullOrWhiteSpace(pallet))
            {
                throw new ArgumentNullException($@"{nameof(whCode) }\{nameof(pallet)}", "传入参数不能为空！！");
            }

            var newPallet = string.Concat("PA", GetWhCode(whCode), pallet);

            //卡板规则：PAHK01*
            Regex regex = new Regex(@"PA[A-Z]{2}\d{2,}", RegexOptions.None);

            if (!regex.IsMatch(newPallet))
            {
                throw new Exception("卡板不符合规则！！");
                //continue;
            }
            else
            {
                using (var rep = new PvWmsRepository())
                {
                    //添加的数据是否已经存在
                    //var data = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.ID == newPallet);

                    if (!rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Any(item => item.ID == newPallet))
                    {

                        if (!this.places.Contains(newPallet))
                        {
                            this.places.Add(newPallet);
                        }

                        rep.Insert(new Layers.Data.Sqls.PvWms.Shelves
                        {
                            ID = newPallet,
                            WhCode = GetWhCode(whCode),
                            DoorCode = "",
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Status = (int)ShelvesStatus.Normal,
                            Summary = ""
                        });

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }

        /// <summary>
        /// 删除库区、卡板、库位
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            //删除货架、卡板、库位

            //库区正则 HK0A
            Regex regexRegion = new Regex(@"[A-Z]{2}[0][A-Z]{1}");

            ////货架正则 
            //Regex regexShelve = new Regex(@"P[A-Z]{2}[0][A-Z]{1}\d{2}");
            //库位正则 PHK0A01101
            Regex regexPlace = new Regex(@"P[A-Z]{2}[0][A-Z]{1}\d{3,}");
            //卡板正则 PAHK01
            Regex regexPallet = new Regex(@"PA[A-Z]{2}\d{2,}");

            using (var rep = new PvWmsRepository())
            {
                #region 删除库区
                //删除库区
                if (regexRegion.IsMatch(id) && id.Length == 4)
                {
                    //根据ID获得对应的库区信息
                    var region = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.ID == id).ToArray();
                    //库区个数为0证明不存在该库区，无法删除
                    if (region.Count() == 0)
                    {
                        throw new ShelveLeasedException("该库房并不存在该库区，删除失败！！");
                    }

                    //获得该库区下的所有库位信息
                    var shelves = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.ID.StartsWith("P" + id)).ToArray();
                    //查找对应的库位ID信息
                    var shelveIDs = shelves.Select(item => item.ID).ToArray();

                    //查找库区下所有库位的库存情况
                    var storages = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => shelveIDs.Contains(item.ShelveID));

                    //库位上没有任何货物并且库位没有被租赁出去才可以删除
                    if (storages.Count() == 0 && (shelves.Any(item => string.IsNullOrWhiteSpace(item.LeaseID)) || shelves.Count() == 0))
                    {
                        rep.Delete<Layers.Data.Sqls.PvWms.Shelves>(item => shelveIDs.Contains(item.ID) || item.ID == id);

                        //this.places删除对应的库区以及对应的库位
                        if (this.places.Contains(id))
                        {
                            this.places.Remove(id);
                        }

                        foreach (var shelveID in shelveIDs)
                        {
                            //var sID = shelveID;
                            if (this.places.Contains(shelveID))
                            {
                                //this.places.TryTake(out sID);
                                this.places.Remove(shelveID);
                            }
                        }
                    }
                    else
                    {

                        #region 一、单例+事件 返回删除失败事件(暂时采用第二种)

                        //ShelveLeasedEventHandler handler = shelveLeased;

                        //if (this != null && shelveLeased != null)
                        //{
                        //    shelveLeased("库区里对应的库位还有货物或者被租赁，无法删除！！", new EventArgs());
                        //}
                        //return;

                        #endregion

                        #region 二、根据异常的ex.message返回删除失败事件
                        throw new ShelveLeasedException("库区里对应的库位还有货物或者被租赁，无法删除！！");
                        #endregion
                    }
                }
                #endregion

                #region 删除库位
                if (regexPlace.IsMatch(id))
                {
                    //获取库存
                    var storages = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => item.ShelveID.StartsWith(id));
                    //根据ID获得库位
                    var shelve = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.ID == id);
                    if (shelve.Count() == 0)
                    {
                        throw new ShelveLeasedException("该库房并不存在该库位，删除失败！！");
                    }

                    //库位上没有任何货物并且库位没有被租赁出去才可以删除
                    if (storages.Count() == 0 && string.IsNullOrWhiteSpace(shelve.FirstOrDefault().LeaseID))
                    {
                        rep.Delete<Layers.Data.Sqls.PvWms.Shelves>(item => item.ID == id);
                        if (this.places.Contains(id))
                        {
                            //this.places.TryTake(out id);
                            this.places.Remove(id);
                        }
                    }
                    else
                    {
                        #region 一、单例+事件 返回删除失败事件(暂时采用第二种)

                        //ShelveLeasedEventHandler handler = shelveLeased;

                        //if (this != null && handler != null)
                        //{
                        //    handler("库位还有货物或者被租赁，无法删除！！", new EventArgs());
                        //}

                        //return;
                        #endregion

                        #region  二、根据异常的ex.message返回删除失败事件
                        throw new ShelveLeasedException("库位还有货物或者被租赁，无法删除！！");
                        #endregion
                    }

                }
                #endregion

                #region 删除卡板
                if (regexPallet.IsMatch(id))
                {
                    //获取库存
                    var storages = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(item => item.ShelveID.StartsWith(id));
                    //根据ID获得卡板
                    var pallet = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.ID == id);

                    if (pallet.Count() == 0)
                    {
                        throw new ShelveLeasedException("该库房并不存在该卡板，删除失败！！");
                    }

                    //卡板不会被租赁出去吧？陈翰回答:会
                    if (storages.Count() == 0 && string.IsNullOrWhiteSpace(pallet.FirstOrDefault().LeaseID))
                    {
                        rep.Delete<Layers.Data.Sqls.PvWms.Shelves>(item => item.ID == id);

                        if (this.places.Contains(id))
                        {
                            this.places.Remove(id);
                            //this.places.TryTake(out id);
                        }

                    }
                    else
                    {

                        #region 一、单例+事件 返回删除失败事件(暂时采用第二种)

                        //ShelveLeasedEventHandler handler = shelveLeased;

                        //if (this != null && handler != null)
                        //{
                        //    handler("卡板上还有货物或者被租赁，无法删除！！", new EventArgs());
                        //}

                        // return;
                        #endregion

                        #region  二、根据异常的ex.message返回删除失败事件
                        throw new ShelveLeasedException("卡板上还有货物或者被租赁，无法删除！！");
                        #endregion
                    }
                }
                #endregion

            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return places.OrderBy(item => item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 返回库区/货架/库位分组
        /// </summary>
        /// <param name="whid">库房ID</param>
        /// <returns></returns>
        //public object[] ToGroups(string whid)
        //{
        //    //返回卡板？
        //    //返回库位？

        //    var linqs = from item in this
        //                where item.StartsWith(whid)
        //                //group item by item.Substring(0, 4) into groupRegin
        //                select new
        //                {
        //                    Region = item,
        //                    Shelve = (from s in this
        //                              where s.StartsWith("P" + item)
        //                              group s by s.Substring(0, 7) into shelves
        //                              select new
        //                              {
        //                                  //Type="货架",
        //                                  Shelve = shelves.Key,
        //                                  Code = shelves.ToArray(),
        //                                  Count = shelves.Count()
        //                              })
        //                };

        //    return linqs.ToArray();

        //    //var linqs = from item in this
        //    //            group item by item.Substring(0, 4) into groupRegin
        //    //            select new
        //    //            {
        //    //                Region = groupRegin.Key,
        //    //                Shelve = (from s in groupRegin
        //    //                          group s by s into shelves
        //    //                          select new
        //    //                          {
        //    //                              Shelve = shelves.Key,
        //    //                              Code = shelves.ToArray()
        //    //                          })
        //    //            };
        //}

        /// <summary>
        /// 根据库房编号获得所有库区
        /// </summary>
        /// <param name="whid">库房编号</param>
        /// <returns></returns>
        public object ToRegions(string whid, int pageIndex = 1, int pageSize = 20)
        {
            using (var rep = new PvWmsRepository())
            {
                var linq = from item in rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
                           where item.ID.StartsWith(GetWhCode(whid)) && item.ID.Length == 4
                           select new
                           {
                               RegionIDs = item.ID,
                               RegionCode = item.ID.Substring(2, 2),//截取ID后两位数字为库区编号，方便修改
                               Name = item.Name,
                           };
                return linq.Paging(pageIndex, pageSize);
            }


        }

        /// <summary>
        /// 根据库房编号获得所有卡板
        /// </summary>
        /// <param name="whid">库房编号</param>
        /// <returns></returns>
        public object ToPallets(string whid, int pageIndex = 1, int pageSize = 20)
        {
            var linq = from item in this
                       where item.StartsWith("PA" + GetWhCode(whid))
                       select new
                       {
                           PalletIDs = item,
                       };
            return linq.Paging(pageIndex, pageSize);
        }

        /// <summary>
        /// 根据库房编号获得所有库位
        /// </summary>
        /// <param name="whid">库房编号</param>
        /// <returns></returns>
        public object ToPlaces(string whid, int pageIndex = 1, int pageSize = 20)
        {
            using (var rep = new PvWmsRepository())
            {
                var linq = from item in rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()//应乔霞要求更改为从数据库读取数据，因为缓存中并没办法获得租赁ID（打印库位需要获得租赁ID）
                           where item.ID.StartsWith("P" + GetWhCode(whid))
                           select new
                           {
                               WarehouseID = item.WhCode,//库房ID
                               RegionID = item.ID.Substring(4, 1),//库区ID
                               ShelveID = item.ID.Substring(5, 2),//货架ID
                               PositionID = item.ID.Substring(7), //位号ID
                               PlaceIDs = item.ID,    //库位ID
                               LeaseID = item.LeaseID //租赁ID
                           };

                return linq.Paging(pageIndex, pageSize);

            }

            //var linq = from item in this
            //           where item.StartsWith("P" + whid)
            //           select new
            //           {
            //               WarehouseID = whid,
            //               RegionID = item.Substring(4, 1),
            //               ShelveID = item.Substring(5, 2),
            //               PositionID = item.Substring(7),
            //               PlaceIDs = item,
            //           };
            //return linq.Paging(pageIndex, pageSize);

        }

        /// <summary>
        /// 深圳的库位号显示
        /// </summary>
        /// <param name="whCode">库房编号</param>
        /// <returns></returns>
        public object SZShow(string whCode)
        {
            if (string.IsNullOrWhiteSpace(whCode))
            {
                throw new ArgumentNullException($@"{nameof(whCode)}", "不能为空！");
            }
            using (var repository = new PvWmsRepository())
            {
                var linq = from shelve in repository.ReadTable<Layers.Data.Sqls.PvWms.Shelves>()
                           where shelve.ID.StartsWith("P" + GetWhCode(whCode))
                           select new
                           {
                               ShelveID = shelve.ID.Substring(3)
                           };
                return linq.ToArray();
            }
        }

        /// <summary>
        /// 深圳乱入库位
        /// </summary>
        /// <param name="whCode">库房编号</param>
        /// <param name="place">库位</param>
        public void SZEnter(string whCode, string place)
        {
            var newPlace = string.Concat("P", GetWhCode(whCode), place);
            using (var rep = new PvWmsRepository())
            {
                //添加的数据是否已经存在
                var exsit = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Any(item => item.ID == newPlace);
                if (!rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Any(item => item.ID == newPlace))
                {
                    //添加库位
                    rep.Insert(new Layers.Data.Sqls.PvWms.Shelves
                    {
                        ID = newPlace,
                        WhCode = GetWhCode(whCode),
                        DoorCode = "",
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        Status = (int)ShelvesStatus.Normal,
                        Summary = ""
                    });
                }
            }
        }

        //public void GetPlaces(params string[] places)
        //{
        //    //this.places.Contains
        //}

        /// <summary>
        /// 增加货架的库位
        /// </summary>
        static Func<int, int, int, int, string[]> SetShelvePlace = new Func<int, int, int, int, string[]>(delegate (int start, int shelveCount, int rowCount, int columnCount)
        {
            List<string> list = new List<string>();

            for (int reffer = start; reffer <= start + shelveCount; reffer++)
            {
                for (int row = 1; row <= rowCount; row++)
                    for (int index = 1; index <= columnCount; index++)
                    {
                        list.Add($"{reffer.ToString().PadLeft(2, '0')}{row}{index.ToString().PadLeft(2, '0')}");
                    }
            }
            return list.ToArray();
        });

        /// <summary>
        /// 增加卡板的库位
        /// </summary>
        static Func<int, string[]> SetPalletPlace = new Func<int, string[]>(delegate (int count)
        {
            var arry = new string[count];
            for (int index = 1; index <= count; index++)
            {
                arry[index] = $"{index.ToString().PadLeft(2, '0')}";
            }
            return arry;
        });

        /// <summary>
        /// 过滤库房
        /// </summary>
        /// <param name="whid"></param>
        /// <returns></returns>
        string GetWhCode(string whid)
        {
            return whid.Length > 2 ? whid.Substring(0, 2) : whid;
        }

        /// <summary>
        /// 调用
        /// </summary>
        void Init()
        {
            //0E:分拣区
            //0S:出库区

            //Current.SetRegion("HK", 1);
            //增加库区
            this.SetRegion("HK", "0A", "A");//香港库区初始化默认值
            this.SetRegion("HK", "0B", "B");//香港库区初始化默认值
            this.SetRegion("SZ", "0A", "A");//深圳库区初始化默认值
            this.SetPallet("HK", "01");//香港卡板初始化默认值
            this.SetPallet("SZ", "01");//深圳卡板初始化默认值

            foreach (var item in SetShelvePlace(1, 6, 3, 3))//香港库位初始化默认值
            {
                this.SetPlace("HK", "0A" + item);
            }

            foreach (var item in SetShelvePlace(1, 12, 3, 3))//深圳库位初始化默认值
            {
                this.SetPlace("SZ", "0A" + item);
            }

            // 从数据库中读取未存在this.places中则增加
            var action = new Action(delegate ()
            {
                using (var rep = new PvWmsRepository())
                {
                    var data = rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Select(item => item.ID);
                    foreach (var code in data)
                    {
                        if (!this.places.Contains(code))
                        {
                            this.places.Add(code);
                        }
                    }
                }
            });
            action();//为啥写两遍？？

            // 填充 
            Thread thread = new Thread(delegate ()
            {
                while (true)
                {
                    try
                    {
                        action();//为啥写两遍？？
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        Thread.Sleep(1);
                    }
                }
            });
            thread.Start();

            //Current.SetPallet("HK", SetPalletPlace(15));
            //Current.SetPallet("SZ", SetPalletPlace(20));
        }
    }

    public enum ShelvesStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 400,

        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        StopUsing = 500,
    }

    public class ShelveCoder
    {
        /// <summary>
        /// P： 货架库位(PHK0A01101) P[A-Z]{2}[0][A-Z]{1}\d{3}\d{2,}
        /// PA：卡板库位(PAHK02)  PA[A-Z]{2}\d{2,}
        /// T： 临时库位(THKA00102)
        /// TB：临时箱号(HKA00101-TBWLHK200302001)
        /// B： 箱号    (HKA00101-BWLHK200302001)
        /// 库位[HK][Q][JJ][PP*]
        /// Q:库区：经[0-9A-Z] A0至Z9
        /// J 架号：纬[0-9]
        /// P 位置：立体[0-9]
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称（库区用）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 库房编号
        /// </summary>
        public string WhCode { get; set; }

        /// <summary>
        /// 库房门牌编号
        /// </summary>
        public string DoorCode { get; set; }

        /// <summary>
        /// 库区编号
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// 库位编号（[货架][位置]  or  [卡板] or [箱号]）
        /// </summary>
        public string PlaceCode { get; set; }

        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 状态值
        /// </summary>
        public ShelvesStatus Status { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 租赁（合同）ID
        /// </summary>
        public string LeaseID { get; set; }

        /// <summary>
        /// //库位所属人
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 合同结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

    }

    /// <summary>
    /// 库位租赁异常
    /// </summary>
    public class ShelveLeasedException : Exception
    {
        public ShelveLeasedException(string message) : base(message)
        {
        }
    }
}

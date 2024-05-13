using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using Wms.Services.chonggous.Views;
using Yahv.Services.Enums;
using Yahv.Utils.Converters;

namespace Wms.Services.chonggous
{
    /// <summary>
    /// 箱子编码者
    /// </summary>
    public class BoxCoder
    {
        /// <summary>
        /// 参见:箱号格式
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        internal string EnterCode { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        internal int PartTotal { get; set; }
    }

    /// <summary>
    /// 箱号已经选择事件参数
    /// </summary>
    /// <remarks>
    /// 备用
    /// </remarks>
    public class BoxSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; private set; }

        public BoxSelectedEventArgs(string message)
        {
            this.Message = message;
        }
    }

    //乔霞通讯回来的数据(entercode , BoxCode)给高会航进行处理(存储)
    //临时记录 BoxCode 目前给谁在使用  
    //boxcode 要返回  正常可用的箱号 与 该箱子中的型号个数

    public delegate void BoxSelectedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// 不支持该箱号的手动输入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void NotSupportedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// 箱号管理
    /// </summary>
    /// <remarks>
    /// 那些箱号已经管理了?
    /// 那些箱号没有被管理?
    /// 区分内单、外单箱号
    /// 一个箱子最多可以放：内单50个，外单20个（条），一个箱子中只能放一个小订单的货物！
    /// </remarks>
    public class BoxManage
    {
        /// <summary>
        /// 这个数量可以通过配置拿到或是可以通过设置拿到
        /// </summary>
        const int MaxCode = 50;
        /// <summary>
        /// 箱号的序号的字长
        /// </summary>
        const int BoxIndexCharLenght = 3;

        BoxManage()
        {
            //this.Init("NL");
            //this.Init("WL");
        }

        bool Check(int maxIndex, out int max)
        {
            var date = DateTime.Now.Date;
            string whCode = nameof(Yahv.Services.WhSettings.HK);
            using (var rep = new PvWmsRepository())
            {
                max = rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Where(item => item.WhCode == whCode
                            && item.CreateDate >= date
                            && item.CreateDate < date.AddDays(1)).Max(item => (int?)item.Index) ?? 0;
                return max < maxIndex;
            }
        }

        void Init(string name, int quantity = 500)
        {
            int max;
            if (Check(quantity, out max))
            {
                lock (this)
                {
                    if (Check(quantity, out max))
                    {
                        string whCode = nameof(Yahv.Services.WhSettings.HK);
                        using (var rep = new PvWmsRepository())
                        {
                            var date = DateTime.Now.Date;
                            var boxes = new int[quantity].Select((item, index) =>
                            {
                                string code = $"{name}{DateTime.Now.Date.ToString("yyMMdd")}{(max + index + 1).ToString().PadLeft(BoxIndexCharLenght, '0')}";
                                return new Layers.Data.Sqls.PvWms.Boxes
                                {
                                    ID = code,
                                    AdminID = null,
                                    CreateDate = DateTime.Now,
                                    Day = date.Day,
                                    Series = null,
                                    Index = max + index,
                                    EnterCode = code,
                                    WhCode = whCode,
                                    PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                                };
                            });
                            rep.Insert(boxes);
                        }
                    }
                }
            }
        }

        #region 单例+事件 箱号已经被选择事件，不支持该箱号的手动输入事件

        object objectLock = new object();


        /// <summary>
        /// 箱号已经被选择事件
        /// </summary>
        event BoxSelectedEventHandler boxSelected;

        /// <summary>
        /// 箱号已经被选择事件（香港）
        /// </summary>
        public event BoxSelectedEventHandler BoxSelected
        {
            add
            {
                lock (objectLock)
                {
                    var events = Thread.GetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(boxSelected)}")) as BoxSelectedEventHandler;
                    if (events == null)
                    {
                        events = value;
                        Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(boxSelected)}"), value);
                        boxSelected += value;
                    }
                }
            }
            remove
            {
                lock (objectLock)
                {
                    boxSelected -= value;
                    Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(boxSelected)}"), null);
                }
            }
        }

        /// <summary>
        /// 不支持此箱号格式的手动输入
        /// </summary>
        event NotSupportedEventHandler notSupported;

        /// <summary>
        /// 不支持此箱号格式的手动输入（香港）
        /// </summary>
        public event NotSupportedEventHandler NotSupported
        {
            add
            {
                lock (objectLock)
                {
                    var events = Thread.GetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(notSupported)}")) as NotSupportedEventHandler;
                    if (events == null)
                    {
                        events = value;
                        Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(boxSelected)}"), value);
                        notSupported += value;
                    }
                }
            }
            remove
            {
                lock (objectLock)
                {
                    notSupported -= value;
                    Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(notSupported)}"), null);
                }
            }
        }

        /// <summary>
        /// 箱号已经被选择事件(深圳)
        /// </summary>
        event BoxSelectedEventHandler SZboxSelected;
        public event BoxSelectedEventHandler SZBoxSelected
        {
            add
            {
                lock (objectLock)
                {
                    var events = Thread.GetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(SZboxSelected)}")) as BoxSelectedEventHandler;
                    if (events == null)
                    {
                        events = value;
                        Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(SZboxSelected)}"), value);
                        SZboxSelected += value;
                    }
                }
            }
            remove
            {
                lock (objectLock)
                {
                    SZboxSelected -= value;
                    Thread.SetData(Thread.GetNamedDataSlot($"{nameof(BoxManage)}_{nameof(SZboxSelected)}"), null);
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取箱号标识
        /// </summary>
        /// <param name="whCode">库房标识</param>
        /// <param name="enterCode">入仓单号,入仓号以wl开头的就代表外单</param>
        /// <param name="date">箱号日期</param>
        /// <param name="orderID">当前使用的订单ID</param>
        /// 箱号格式：[xx][yy][date][zzz*]
        /// x:WL\NL
        /// y:库房标识 HK,SZ
        /// z:实际的需要
        /// date:yyMMdd
        /// </returns>
        [Obsolete("根据荣检要求变更做法:已经使用的箱号不能重复使用")]
        public IEnumerable<BoxCoder> this[string enterCode, string orderID, string whCode = null, DateTime? date = null]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(whCode) || string.IsNullOrWhiteSpace(enterCode))
                {
                    //第一个参数：导致异常的参数的名称
                    //第二个参数：描述错误的消息。
                    throw new ArgumentNullException($@"{nameof(whCode)}\{nameof(enterCode)}", "不能为空！");
                }
                if (whCode.Length > 2)
                {
                    //第一个参数：解释异常原因的错误消息
                    //第二个参数：导致当前异常的参数的名称。
                    throw new ArgumentException("长度不能超过2位！", $@"{nameof(whCode)}");
                }

                DateTime current;

                if (date.HasValue)
                {
                    current = date.Value.Date;
                }
                else
                {
                    current = DateTime.Now.Date;
                }


                Expression<Func<Layers.Data.Sqls.PvWms.Boxes, bool>> predicate;
                if (string.IsNullOrWhiteSpace(enterCode)
                    || string.IsNullOrWhiteSpace(whCode))
                {
                    predicate = item => item.CreateDate >= current
                        && item.CreateDate < current.AddDays(1);
                }
                else
                {
                    predicate = item => item.WhCode == whCode
                        && item.CreateDate >= current
                        && item.CreateDate < current.AddDays(1);
                }

                using (var rep = new PvWmsRepository())
                {
                    //获得所有箱号数据(内单/外单的箱号都会有)
                    var linq_box = from box in rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Where(predicate)
                                   select box;
                    var boxArry = linq_box.ToArray();

                    //获得箱号信息便于下一步分组
                    var data = from box in boxArry
                               select new
                               {
                                   Code = box.ID,
                                   Series = box.Series,
                                   EnterCode = box.EnterCode,
                                   PartTotal = 0,
                                   box.Index,
                                   box.AdminID,
                                   box.OrderID
                               };
                    //以Series分组
                    var series_data = from d in data
                                      group d by d.Series into groups
                                      select new
                                      {
                                          Code = groups.Key,
                                          EnterCode = groups.Select(item => item.EnterCode).FirstOrDefault(),
                                          PartTotal = groups.Sum(item => item.PartTotal),
                                          Items = groups.Select(item => item.Index).ToArray(), //为了实现快速
                                          AdminID = groups.Select(item => item.AdminID).FirstOrDefault(),
                                          OrdersID = groups.Select(item => item.OrderID),
                                      };

                    // 不满 50 个 还是需要显示(内单/外单的箱号都会有)
                    var exsits = series_data.ToArray();

                    var rslt = new List<BoxCoder>(MaxCode + (data.Max(item => (int?)item.Index) ?? 0));
                    string prex = enterCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";

                    for (int index = 0; index < rslt.Capacity; index++)
                    {
                        var entity = exsits.SingleOrDefault(item => item.Items.Contains(index + 1) && item.EnterCode == enterCode);
                        if (entity != null)
                        {
                            if (!rslt.Any(item => item.Code == entity.Code)
                                && exsits.Where(item => item.EnterCode == enterCode && item.Items.Contains(index + 1) && item.OrdersID.Contains(orderID)).Count() > 0)
                            {
                                rslt.Add(new BoxCoder
                                {
                                    Code = entity.Code,
                                    EnterCode = entity.EnterCode,
                                    PartTotal = entity.PartTotal
                                });
                            }

                            continue;
                        }

                        if (this == HkDeclare)
                        {
                            rslt.Add(new BoxCoder
                            {
                                Code = string.Concat(prex, current.ToString("yyMMdd"), (index + 1).ToString().PadLeft(BoxIndexCharLenght, '0')),
                            });
                        }

                        if (this == Current)
                        {
                            rslt.Add(new BoxCoder
                            {
                                Code = string.Concat(prex, whCode.ToUpper(), current.ToString("yyMMdd"), (index + 1).ToString().PadLeft(BoxIndexCharLenght, '0')),
                            });
                        }
                    }

                    return rslt;

                }
            }
        }

        public IEnumerable<BoxCoder> this[string enterCode, DateTime? date = null]
        {
            get
            {
                string whCode = nameof(Yahv.Services.WhSettings.HK);

                /*
                 逻辑流程：就看有没有，有的话就不展示了
                 */
                //就看有没有

                if (string.IsNullOrWhiteSpace(whCode) || string.IsNullOrWhiteSpace(enterCode))
                {
                    //第一个参数：导致异常的参数的名称
                    //第二个参数：描述错误的消息。
                    throw new ArgumentNullException($@"{nameof(whCode)}\{nameof(enterCode)}", "不能为空！");
                }
                if (whCode.Length > 2)
                {
                    //第一个参数：解释异常原因的错误消息
                    //第二个参数：导致当前异常的参数的名称。
                    throw new ArgumentException("长度不能超过2位！", $@"{nameof(whCode)}");
                }

                DateTime current;

                if (date.HasValue)
                {
                    current = date.Value.Date;
                }
                else
                {
                    current = DateTime.Now.Date;
                }


                Expression<Func<Layers.Data.Sqls.PvWms.Boxes, bool>> predicate;
                if (string.IsNullOrWhiteSpace(enterCode)
                      || string.IsNullOrWhiteSpace(whCode))
                {
                    predicate = item => item.CreateDate >= current
                        && item.CreateDate < current.AddDays(1);
                }
                else
                {
                    predicate = item => item.WhCode == whCode
                        && item.CreateDate >= current
                        && item.CreateDate < current.AddDays(1);
                }

                using (var rep = new PvWmsRepository())
                {
                    //获得所有箱号数据(内单/外单的箱号都会有)
                    var linq_box = from box in rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Where(predicate)
                                   select box;
                    var boxArry = linq_box.ToArray();

                    //获得箱号信息便于下一步分组
                    var data = from box in boxArry
                               select new
                               {
                                   Code = box.ID,
                                   Series = box.Series,
                                   EnterCode = box.EnterCode,
                                   PartTotal = 0,
                                   box.Index,
                                   box.AdminID,
                                   //box.OrderID
                               };
                    //以Series分组
                    var series_data = from d in data
                                      group d by d.Series into groups
                                      select new
                                      {
                                          Code = groups.Key,
                                          EnterCode = groups.Select(item => item.EnterCode).FirstOrDefault(),
                                          PartTotal = groups.Sum(item => item.PartTotal),
                                          //Items = groups.Select(item => item.Index).ToArray(), //为了实现快速
                                          AdminID = groups.Select(item => item.AdminID).FirstOrDefault(),
                                          MaxIndex = groups.Max(item => item.Index),
                                          MinIndex = groups.Min(item => item.Index)

                                          //OrdersID = groups.Select(item => item.OrderID),
                                      };

                    // 不满 50 个 还是需要显示(内单/外单的箱号都会有)
                    var exsits = series_data.ToArray();

                    var rslt = new List<BoxCoder>(MaxCode + (data.Max(item => (int?)item.Index) ?? 0));
                    string prex = enterCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";

                    for (int index = 0; index < rslt.Capacity; index++)
                    {
                        var currentIndex = index + 1;

                        var entity = exsits.Any(item => currentIndex >= item.MinIndex && currentIndex <= item.MaxIndex);
                        //var entity = exsits.Any(item => item.MinIndex >= currentIndex && item.MaxIndex <= currentIndex);
                        if (entity)
                        {
                            continue;
                        }

                        if (this == HkDeclare)
                        {
                            rslt.Add(new BoxCoder
                            {
                                Code = string.Concat(prex, current.ToString("yyMMdd"), currentIndex.ToString().PadLeft(BoxIndexCharLenght, '0')),
                            });
                        }

                        if (this == Current)
                        {
                            rslt.Add(new BoxCoder
                            {
                                Code = string.Concat(prex, whCode.ToUpper(), current.ToString("yyMMdd"), currentIndex.ToString().PadLeft(BoxIndexCharLenght, '0')),
                            });
                        }
                    }

                    return rslt;

                }
            }
        }

        /*
        一键入箱的时候，请前端判断是否属于同一小订单ID。不属于，就提示错误！
        前端以禁用方式展示箱号列
        */


        //箱号的规则
        Regex regex_boxCode = new Regex(@"^(WL|WLSZ|NL|NLSZ)\d{9}$", RegexOptions.None);
        //连续箱号的规则
        Regex regex_seriesBoxCode = new Regex(@"^(NL|WL|NLSZ|WLSZ)\d{9}\-(NL|WL|NLSZ|WLSZ)\d{9}$", RegexOptions.None);

        /// <summary>
        /// 保存连续箱号
        /// </summary>
        /// <param name="enterCode">入仓单号</param>
        /// <param name="code">箱号</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        [Obsolete("根据荣检要求变更做法:已经使用的箱号不能重复使用")]
        public string EnterSeries(string enterCode, string orderID, string tinyOrderID, int quantity, DateTime? date, string adminID)
        {
            /// 箱号格式：[xx][yy][date][zzz*]
            /// x:WL\NL
            /// y:库房标识 HK,SZ
            /// z:实际的需要
            /// date:yyMMdd

            // 2-9
            // 得到 数量后 要循环增加
            // "10-20"
            //\d+\-\d+
            if (quantity <= 1)
            {
                throw new ArgumentException("参数有误：必须大于1！");
            }

            if (this != hkDeclare)
            {
                throw new NotSupportedException("本方法只支持香港入仓号的箱号管理");
            }
            lock (this)  //先简化事务
            {
                using (var repository = new PvWmsRepository())
                {
                    var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
                                                                                   //获取最大的箱号
                    var linq = from item in repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                               where item.CreateDate >= current && item.CreateDate < current.AddDays(1)
                               select item;
                    var maxIndex = (linq.Max(item => (int?)item.Index) + 1) ?? 1;


                    //连续箱号的生成完全由服务端来保证生成，因此不做以外处理
                    //应该运行在事物中

                    string prex = enterCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";//前缀
                    var ienums = new int[quantity].Select((item, index) => new
                    {
                        Code = string.Concat(prex, current.Date.ToString("yyMMdd"), (index + maxIndex).ToString().PadLeft(3, '0')),
                        Index = (index + maxIndex)
                    });

                    var first = ienums.First().Code;
                    var last = ienums.Last().Code;

                    string series = $"{first}-{last}";

                    var tboxes = (from item in ienums
                                  select new Layers.Data.Sqls.PvWms.TBoxes
                                  {
                                      ID = item.Code,
                                      //AdminID = adminID,
                                      CreateDate = DateTime.Now,
                                      EnterCode = enterCode,
                                      //WhCode = nameof(Yahv.Services.WhSettings.HK),
                                      //TinyOrderID = tinyOrderID,
                                      PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                                  }).ToArray();
                    var boxes = (from item in ienums
                                 select new Layers.Data.Sqls.PvWms.Boxes
                                 {
                                     ID = item.Code,
                                     AdminID = adminID,
                                     CreateDate = DateTime.Now,
                                     Day = current.Day,
                                     Series = series,
                                     Index = item.Index,
                                     OrderID = orderID,
                                     EnterCode = enterCode,
                                     WhCode = nameof(Yahv.Services.WhSettings.HK),
                                     PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                                 }).ToArray();
                    repository.Insert(new[] { tboxes.First(), tboxes.Last() });
                    repository.Insert(new[] { boxes.First(), boxes.Last() });

                    new Thread(() =>
                    {
                        using (var r = new PvWmsRepository())
                        {
                            r.Insert((tboxes.Skip(1).Take(boxes.Length - 2)));
                            r.Insert((boxes.Skip(1).Take(boxes.Length - 2)));
                        }
                    })
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    }.Start();
                    return series;
                }
            }
        }

        /// <summary>
        /// 保存连续箱号
        /// </summary>
        /// <param name="enterCode">入仓单号</param>
        /// <param name="code">箱号</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        public string EnterSeries(string enterCode, int quantity, DateTime? date, string adminID)
        {
            /// 箱号格式：[xx][yy][date][zzz*]
            /// x:WL\NL
            /// y:库房标识 HK,SZ
            /// z:实际的需要
            /// date:yyMMdd

            // 2-9
            // 得到 数量后 要循环增加
            // "10-20"
            //\d+\-\d+
            if (quantity <= 1 || quantity > 999)
            {
                throw new ArgumentException("参数有误：必须大于1或者小于1000！");
            }

            if (this != hkDeclare)
            {
                throw new NotSupportedException("本方法只支持香港入仓号的箱号管理");
            }
            lock (this)  //先简化事务
            {
                using (var repository = new PvWmsRepository())
                {
                    var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
                                                                                   //获取最大的箱号
                    var linq = from item in repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                               where item.CreateDate >= current && item.CreateDate < current.AddDays(1)
                               select item;
                    var maxIndex = (linq.Max(item => (int?)item.Index) + 1) ?? 1;

                    //连续箱号的生成完全由服务端来保证生成，因此不做以外处理
                    //应该运行在事物中

                    string prex = enterCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";//前缀
                    var ienums = new int[quantity].Select((item, index) => new
                    {
                        Code = string.Concat(prex, current.Date.ToString("yyMMdd"), (index + maxIndex).ToString().PadLeft(3, '0')),
                        Index = (index + maxIndex)
                    });

                    var first = ienums.First().Code;
                    var last = ienums.Last().Code;

                    string series = $"{first}-{last}";

                    var boxes = (from item in ienums
                                 select new Layers.Data.Sqls.PvWms.Boxes
                                 {
                                     ID = item.Code,
                                     AdminID = adminID,
                                     CreateDate = DateTime.Now,
                                     Day = current.Day,
                                     Series = series,
                                     Index = item.Index,
                                     //OrderID = orderID,
                                     EnterCode = enterCode,//理论上应该加上
                                     WhCode = nameof(Yahv.Services.WhSettings.HK),
                                     PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                                 }).ToArray();
                    repository.Insert(new[] { boxes.First(), boxes.Last() });

                    new Thread(() =>
                    {
                        using (var r = new PvWmsRepository())
                        {
                            r.Insert((boxes.Skip(1).Take(boxes.Length - 2)));
                        }
                    })
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Highest
                    }.Start();
                    return series;
                }
            }
        }

        ///// <summary>
        ///// 修改连续箱号
        ///// </summary>
        ///// <param name="enterCode">入仓单号</param>
        ///// <param name="code">箱号</param>
        ///// <param name="date">时间</param>
        ///// <param name="adminID">操作人</param>
        //public string ModifySeries(string enterCode, string orderID, string tinyOrderID, int quantity, DateTime? date, string adminID, string storageID)
        //{
        //    /// 箱号格式：[xx][yy][date][zzz*]
        //    /// x:WL\NL
        //    /// y:库房标识 HK,SZ
        //    /// z:实际的需要
        //    /// date:yyMMdd

        //    // 2-9
        //    // 得到 数量后 要循环增加
        //    // "10-20"
        //    //\d+\-\d+
        //    if (quantity <= 1)
        //    {
        //        throw new ArgumentException("参数有误：必须大于1！");
        //    }

        //    if (this != hkDeclare)
        //    {
        //        throw new NotSupportedException("本方法只支持香港入仓号的箱号管理");
        //    }
        //    lock (this)  //先简化事务
        //    {
        //        using (var repository = new PvWmsRepository())
        //        {
        //            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
        //                                                                           //获取最大的箱号
        //            var linq = from item in repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
        //                       where item.CreateDate >= current && item.CreateDate < current.AddDays(1)
        //                       select item;
        //            var maxIndex = (linq.Max(item => (int?)item.Index) + 1) ?? 1;


        //            //连续箱号的生成完全由服务端来保证生成，因此不做以外处理
        //            //应该运行在事物中

        //            string prex = enterCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";//前缀
        //            var ienums = new int[quantity].Select((item, index) => new
        //            {
        //                Code = string.Concat(prex, current.Date.ToString("yyMMdd"), (index + maxIndex).ToString().PadLeft(3, '0')),
        //                Index = (index + maxIndex)
        //            });

        //            var first = ienums.First().Code;
        //            var last = ienums.Last().Code;

        //            string series = $"{first}-{last}";

        //            var tboxes = (from item in ienums
        //                          select new Layers.Data.Sqls.PvWms.TBoxes
        //                          {
        //                              ID = item.Code,
        //                              AdminID = adminID,
        //                              CreateDate = DateTime.Now,
        //                              EnterCode = enterCode,
        //                              WhCode = nameof(Yahv.Services.WhSettings.HK),
        //                              TinyOrderID = tinyOrderID,
        //                              PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
        //                          }).ToArray();
        //            var boxes = (from item in ienums
        //                         select new Layers.Data.Sqls.PvWms.Boxes
        //                         {
        //                             ID = item.Code,
        //                             AdminID = adminID,
        //                             CreateDate = DateTime.Now,
        //                             Day = current.Day,
        //                             Series = series,
        //                             Index = item.Index,
        //                             OrderID = orderID,
        //                             EnterCode = enterCode,
        //                             WhCode = nameof(Yahv.Services.WhSettings.HK),
        //                             PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
        //                         }).ToArray();
        //            repository.Insert(new[] { tboxes.First(), tboxes.Last() });
        //            repository.Insert(new[] { boxes.First(), boxes.Last() });

        //            new Thread(() =>
        //            {
        //                using (var r = new PvWmsRepository())
        //                {
        //                    r.Insert((tboxes.Skip(1).Take(boxes.Length - 2)));
        //                    r.Insert((boxes.Skip(1).Take(boxes.Length - 2)));
        //                }
        //            })
        //            {
        //                IsBackground = true,
        //                Priority = ThreadPriority.Highest
        //            }.Start();

        //            //修改到货历史中的箱号
        //            using (var view = new CgSortingsView())
        //            {
        //                view.ModifyBoxCode(storageID, series);
        //            }

        //            return series;
        //        }
        //    }
        //}

        ///// <summary>
        ///// 修改单条的数据的箱号
        ///// </summary>
        ///// <param name="enterCode">入仓单号</param>
        ///// <param name="code">箱号</param>
        ///// <param name="date">时间</param>
        ///// <param name="adminID">操作人</param>
        //public void Modify(string storageID, string adminID, string boxCode)
        //{

        //}

        /// <summary>
        /// 修改箱号本身
        /// </summary>
        /// <param name="enterCode">入仓单号</param>
        /// <param name="code">箱号</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        public void ModifyBoxCode(string adminID, string oldBoxCode, string newBoxCode)
        {
            using (var repository = new PvWmsRepository())
            {

                //处理旧箱号
                if (repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Any(item => item.ID == oldBoxCode || item.Series == oldBoxCode))
                {
                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == oldBoxCode)
                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == oldBoxCode))
                    {
                        //删除旧的箱号
                        repository.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => item.ID == oldBoxCode || item.Series == oldBoxCode);
                    }
                }

                //以下是处理新箱号
                string whCode;
                int index = 0;

                if (this == HkDeclare)
                {
                    whCode = nameof(Yahv.Services.WhSettings.HK);
                }
                else if (this == Current)
                {
                    whCode = nameof(Yahv.Services.WhSettings.SZ);
                }
                else
                {
                    throw new Exception("无法分离序号");
                }
                if (regex_boxCode.IsMatch(newBoxCode))
                {
                    if (this == HkDeclare)
                    {
                        index = int.Parse(newBoxCode.Substring(8));//从code中分析出来 WL200311 994
                    }
                    else if (this == Current)
                    {
                        index = int.Parse(newBoxCode.Substring(10));//从code中分析出来 WLSZ200311 994
                    }
                }

                //处理连续箱号
                List<TempBox> list = new List<TempBox>();
                if (regex_seriesBoxCode.IsMatch(newBoxCode))
                {

                    if (this == HkDeclare)
                    {
                        var series = newBoxCode.Split('-');//通过 '-' 分组获得数组中的个数就是箱号的个数，非连续箱号个数为1

                        //第一个箱号
                        var firstBoxCode = series[0]; //NL200603001
                        //最后一个箱号
                        var lastBoxCode = series[1];  //NL200603006

                        var minIndex = int.Parse(firstBoxCode.Substring(8));
                        var maxIndex = int.Parse(lastBoxCode.Substring(8));

                        string prex = firstBoxCode.StartsWith("wl", StringComparison.OrdinalIgnoreCase) ? "WL" : "NL";//前缀

                        for (int i = minIndex; i <= maxIndex; i++)
                        {
                            list.Add(new TempBox
                            {
                                //如果考虑时间（根据时间产生箱号）的问题，那就把时间date传值过来（DateTime.Now.Date换成date.Date）
                                Code = string.Concat(prex, DateTime.Now.Date.ToString("yyMMdd"), i.ToString().PadLeft(3, '0')),
                                Index = i
                            });
                        }
                    }
                    //暂不考虑深圳
                    //else if (this == Current)
                    //{
                    //    index = int.Parse(newBoxCode.Substring(10));//从code中分析出来 WLSZ200311 994
                    //}
                }

                if (!string.IsNullOrWhiteSpace(newBoxCode))
                {

                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Any(item => item.ID == newBoxCode || item.Series == newBoxCode))
                    {
                        //对连续箱号添加的处理
                        if (regex_seriesBoxCode.IsMatch(newBoxCode))
                        {
                            var boxes = (from item in list
                                         select new Layers.Data.Sqls.PvWms.Boxes
                                         {
                                             ID = item.Code,
                                             AdminID = adminID,
                                             CreateDate = DateTime.Now,
                                             Day = DateTime.Now.Day,//如果考虑时间（根据时间产生箱号）的问题，那就把时间date传值过来（DateTime.Now.Day换成date.Day）
                                             Series = newBoxCode,
                                             Index = item.Index,
                                             //OrderID = orderID,
                                             //EnterCode = enterCode,
                                             WhCode = nameof(Yahv.Services.WhSettings.HK),
                                             PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                                         }).ToArray();
                            repository.Insert(new[] { boxes.First(), boxes.Last() });

                            new Thread(() =>
                            {
                                using (var r = new PvWmsRepository())
                                {
                                    r.Insert((boxes.Skip(1).Take(boxes.Length - 2)));
                                }
                            })
                            {
                                IsBackground = true,
                                Priority = ThreadPriority.Highest
                            }.Start();
                        }
                        //普通添加的处理
                        else
                        {
                            //添加箱号信息
                            repository.Insert(new Layers.Data.Sqls.PvWms.Boxes
                            {
                                ID = newBoxCode,
                                Series = newBoxCode,
                                AdminID = adminID,
                                CreateDate = DateTime.Now,
                                Day = DateTime.Now.Day,
                                Index = index,
                                //OrderID = orderID,
                                //EnterCode = enterCode,
                                WhCode = whCode,
                                PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                            });

                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存已经选择的箱号
        /// </summary>
        /// <param name="enterCode">入仓号</param>
        /// <param name="code">箱号</param>
        /// <param name="tinyOrderID">小订单编号</param>
        /// <param name="orderID">订单ID</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        /// <param name="storageID">分拣ID</param>
        [Obsolete("根据荣检要求变更做法:已经使用的箱号不能重复使用")]
        public void Enter(string enterCode, string code, string tinyOrderID, string orderID, DateTime? date, string adminID)
        {
            lock (this) //先简化事务
            {
                using (var repository = new PvWmsRepository())
                {
                    //先保存Tbox
                    var tbox = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().FirstOrDefault(item => item.ID == code);
                    if (tbox == null)
                    {
                        repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
                        {
                            ID = code,
                            //WhCode = nameof(Yahv.Services.WhSettings.HK),
                            //AdminID = adminID,
                            CreateDate = DateTime.Now,
                            EnterCode = enterCode,
                            PackageType = PackageTypes.CartonBox.GBCode,//默认包装类型：纸箱
                            //TinyOrderID = tinyOrderID
                        });
                    }
                    //判断如果存在就更新
                    else
                    {
                        //增加判断如果一个箱号中有不同的tinyOrderID的情况，就甩出事件提示：不能将两个小订单的产品放在一个箱子中
                        //不错！高会航提示的对！
                        //if (tbox.TinyOrderID != tinyOrderID)
                        //{
                        //    if (this != null && this.boxSelected != null)
                        //    {
                        //        this.boxSelected(this, new EventArgs());
                        //    }
                        //    return;
                        //}
                        repository.Update<Layers.Data.Sqls.PvWms.TBoxes>(new
                        {
                            AdminID = adminID,
                        }, item => item.ID == code);
                    }
                    ////不是临时箱号走下面的流程
                    //if (!code.StartsWith("T"))
                    //{
                    //获得箱号信息
                    var cbox = repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().SingleOrDefault(item => item.ID == code);

                    //增加个判断 code 是否属于 entercode，是否是自己的箱号
                    //不是自己的箱号报错误：箱号已经被选择
                    if (cbox != null && cbox.EnterCode != enterCode)
                    {
                        if (this != null && this.boxSelected != null)
                        {
                            this.boxSelected(this, new EventArgs());
                        }
                        return;
                    }
                    if (cbox != null && cbox.OrderID != orderID)
                    {
                        if (this != null && this.boxSelected != null)
                        {
                            this.boxSelected(this, new EventArgs());
                        }
                        return;
                    }

                    var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
                    string whCode;

                    int index;
                    if (this == HkDeclare)
                    {
                        index = int.Parse(code.Substring(8));//从code中分析出来 WL200311 994
                        whCode = nameof(Yahv.Services.WhSettings.HK);
                    }
                    else if (this == Current)
                    {
                        index = int.Parse(code.Substring(10));//从code中分析出来 WLSZ200311 994
                        whCode = nameof(Yahv.Services.WhSettings.SZ);
                    }
                    else
                    {
                        throw new Exception("无法分离序号");
                    }

                    if (cbox != null)
                    {
                        return;
                    }

                    //添加箱号信息
                    repository.Insert(new Layers.Data.Sqls.PvWms.Boxes
                    {
                        ID = code,
                        Series = code,
                        AdminID = adminID,
                        CreateDate = DateTime.Now,
                        Day = current.Day,
                        Index = index,
                        OrderID = orderID,
                        EnterCode = enterCode,
                        WhCode = whCode,
                        PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                    });

                }
                //}
            }
        }



        /// <summary>
        /// 保存已经选择的箱号
        /// </summary>
        /// <param name="code">箱号</param>
        /// <param name="date">时间</param>
        /// <param name="adminID">操作人</param>
        public void Enter(string enterCode, string code, DateTime? date, string adminID)
        {
            lock (this) //先简化事务
            {
                using (var repository = new PvWmsRepository())
                {
                    //不支持此箱号格式（连续箱号）的手动输入
                    if (regex_seriesBoxCode.IsMatch(code))
                    {
                        if (this != null && this.notSupported != null)
                        {
                            this.notSupported(this, new EventArgs());
                        }
                        return;
                    }
                    var cbox = repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().SingleOrDefault(item => item.ID == code);


                    //只要箱号被选择并且Picking或Sorting里已经使用就抛出 箱号已经被选择事件
                    if (cbox != null && (repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == code  && item.CreateDate.Date == DateTime.Now.Date)
                     || repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == code && item.CreateDate.Date == DateTime.Now.Date)))
                    {
                        if (this != null && this.boxSelected != null)
                        {
                            this.boxSelected(this, new EventArgs());
                        }
                        return;
                    }

                    //要增加乱录入的箱号的判断
                    //如果是乱录入的箱号就直接插入

                    //if()
                    var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间

                    string whCode;
                    int index = 0;

                    if (this == HkDeclare)
                    {
                        whCode = nameof(Yahv.Services.WhSettings.HK);
                    }
                    else if (this == Current)
                    {
                        whCode = nameof(Yahv.Services.WhSettings.SZ);
                    }
                    else
                    {
                        throw new Exception("无法分离序号");
                    }
                    if (regex_boxCode.IsMatch(code))
                    {
                        if (this == HkDeclare)
                        {
                            index = int.Parse(code.Substring(8));//从code中分析出来 WL200311 994
                        }
                        else if (this == Current)
                        {
                            index = int.Parse(code.Substring(10));//从code中分析出来 WLSZ200311 994
                        }
                    }

                    if (cbox == null)
                    {
                        //添加箱号信息
                        repository.Insert(new Layers.Data.Sqls.PvWms.Boxes
                        {
                            ID = code,
                            Series = code,
                            AdminID = adminID,
                            CreateDate = DateTime.Now,
                            Day = DateTime.Now.Day,
                            Index = index,
                            //OrderID = orderID,
                            EnterCode = enterCode,
                            WhCode = whCode,
                            PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                        });
                    }

                }
            }
        }

        ///// <summary>
        ///// 测试箱号多选方法
        ///// </summary>
        ///// <param name="enterCode"></param>
        ///// <param name="code"></param>
        ///// <param name="data"></param>
        ///// <param name="adminID"></param>
        //public void TestEnter(JObject jobject)
        //{
        //    //获得所有箱号
        //    var boxCodes = jobject["boxCodes"];
        //    var minCode = "";
        //    var maxCode = "";
        //    var series = "";
        //    //var enterCode = jobject["enterCode"]?.Value<string>();
        //    //var date = jobject["date"]?.Value<DateTime?>();
        //    //var adminID = jobject["adminID"]?.Value<string>();
        //    //var minCode = "";

        //    //List<int> list = new List<int>();


        //    var codes = boxCodes.OrderBy(item => item["code"]).ToArray();
        //    for (int i = 0; i < codes.Length; i++)
        //    {
        //        //获得当前箱号
        //        var code = codes[i]["code"].Value<string>();
        //        var nextCode = "";
        //        //防止索引超过数组界限
        //        if ((i + 1) <= codes.Length)
        //        {
        //            //获得下一个箱号
        //             nextCode = codes[i + 1]["code"].Value<string>();
        //        }
        //        if (!string.IsNullOrWhiteSpace(nextCode))
        //        {
        //            //判断是否是连续箱号
        //            if (int.Parse(nextCode.Substring(8)) - int.Parse(code.Substring(8)) == 1)
        //            {
        //                if (code == maxCode)
        //                {
        //                    maxCode = nextCode;
        //                }
        //                else
        //                {
        //                    minCode = code;
        //                    maxCode = nextCode;
        //                }
        //                series = $"{minCode}-{maxCode}";
        //            }
        //            else
        //            {

        //            }
        //        }
        //    }

        //}

        public void Delete(string boxCode)
        {
            using (var repository = new PvWmsRepository())
            {

                //删除箱号
                if (repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Any(item => item.ID == boxCode || item.Series == boxCode))
                {
                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == boxCode && item.CreateDate.Date == DateTime.Now.Date)
                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == boxCode && item.CreateDate.Date == DateTime.Now.Date))
                    {
                        //删除旧的箱号
                        repository.Delete<Layers.Data.Sqls.PvWms.Boxes>(item => item.ID == boxCode || item.Series == boxCode);
                    }
                }
            }
        }

        ///// <summary>
        ///// 修改箱号信息
        ///// </summary>
        ///// <param name="enterCode"></param>
        ///// <param name="code"></param>
        ///// <param name="tinyOrderID"></param>
        ///// <param name="orderID"></param>
        ///// <param name="date"></param>
        ///// <param name="adminID"></param>
        ///// <param name="storageID"></param>
        //public void Modify(string enterCode, string code, string tinyOrderID, string orderID, DateTime? date, string adminID, string storageID)
        //{
        //    lock (this) //先简化事务
        //    {
        //        using (var repository = new PvWmsRepository())
        //        {

        //            Regex regex = new Regex(@"^(NL|WL|NLSZ|WLSZ)\d{9}\-(NL|WL|NLSZ|WLSZ)\d{9}$", RegexOptions.None);

        //            //连续箱号规则的处理（理论上传值 连续箱号是一定在数据库存在的）
        //            if (regex.IsMatch(code))
        //            {

        //                if (repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Any(item => item.Series == code))
        //                {
        //                    //修改到货历史中的箱号
        //                    using (var view = new CgSortingsView())
        //                    {
        //                        view.ModifyBoxCode(storageID, code);
        //                    }
        //                    return;
        //                }
        //                else
        //                {
        //                    throw new Exception("箱号不存在");
        //                }
        //            }

        //            //先保存Tbox
        //            var tbox = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().FirstOrDefault(item => item.ID == code);
        //            if (tbox == null)
        //            {
        //                repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
        //                {
        //                    ID = code,
        //                    WhCode = nameof(Yahv.Services.WhSettings.HK),
        //                    AdminID = adminID,
        //                    CreateDate = DateTime.Now,
        //                    EnterCode = enterCode,
        //                    PackageType = PackageTypes.CartonBox.GBCode,//默认包装类型：纸箱
        //                    TinyOrderID = tinyOrderID
        //                });
        //            }
        //            //判断如果存在就更新
        //            else
        //            {
        //                //增加判断如果一个箱号中有不同的tinyOrderID的情况，就甩出事件提示：不能将两个小订单的产品放在一个箱子中
        //                //不错！高会航提示的对！
        //                if (tbox.TinyOrderID != tinyOrderID)
        //                {
        //                    if (this != null && this.boxSelected != null)
        //                    {
        //                        this.boxSelected(this, new EventArgs());
        //                    }
        //                    return;
        //                }
        //                repository.Update<Layers.Data.Sqls.PvWms.TBoxes>(new
        //                {
        //                    AdminID = adminID,
        //                }, item => item.ID == code);
        //            }
        //            ////不是临时箱号走下面的流程
        //            //if (!code.StartsWith("T"))
        //            //{
        //            //获得箱号信息
        //            var cbox = repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().SingleOrDefault(item => item.ID == code);

        //            //增加个判断 code 是否属于 entercode，是否是自己的箱号
        //            //不是自己的箱号报错误：箱号已经被选择
        //            if (cbox != null && cbox.EnterCode != enterCode)
        //            {
        //                if (this != null && this.boxSelected != null)
        //                {
        //                    this.boxSelected(this, new EventArgs());
        //                }
        //                return;
        //            }
        //            if (cbox != null && cbox.OrderID != orderID)
        //            {
        //                if (this != null && this.boxSelected != null)
        //                {
        //                    this.boxSelected(this, new EventArgs());
        //                }
        //                return;
        //            }

        //            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
        //            string whCode;

        //            int index;
        //            if (this == HkDeclare)
        //            {
        //                index = int.Parse(code.Substring(8));//从code中分析出来
        //                whCode = nameof(Yahv.Services.WhSettings.HK);
        //            }
        //            else if (this == Current)
        //            {
        //                index = int.Parse(code.Substring(10));//从code中分析出来 WLHK200311 994
        //                whCode = nameof(Yahv.Services.WhSettings.SZ);
        //            }
        //            else
        //            {
        //                throw new Exception("无法分离序号");
        //            }

        //            if (cbox != null)
        //            {
        //                //修改到货历史中的箱号
        //                using (var view = new CgSortingsView())
        //                {
        //                    view.ModifyBoxCode(storageID, code);
        //                }
        //                return;
        //            }

        //            //添加箱号信息
        //            repository.Insert(new Layers.Data.Sqls.PvWms.Boxes
        //            {
        //                ID = code,
        //                Series = code,
        //                AdminID = adminID,
        //                CreateDate = DateTime.Now,
        //                Day = current.Day,
        //                Index = index,
        //                OrderID = orderID,
        //                EnterCode = enterCode,
        //                WhCode = whCode,
        //                PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
        //            });

        //            //修改到货历史中的箱号
        //            using (var view = new CgSortingsView())
        //            {
        //                view.ModifyBoxCode(storageID, code);
        //            }

        //        }
        //        //}
        //    }
        //}

        /// <summary>
        /// 修改箱号的包装种类
        /// </summary>
        /// <param name="boxCode">箱号</param>
        /// <param name="packageType">包装种类</param>
        /// <param name="adminID">操作人</param> 
        public void EnterPackageType(string boxCode, string packageType, string adminID)
        {
            if (string.IsNullOrWhiteSpace(boxCode) || string.IsNullOrWhiteSpace(packageType))
            {
                throw new ArgumentNullException($@"{nameof(boxCode)}\{nameof(packageType)}", "不能为空！");
            }

            using (var repository = new PvWmsRepository())
            {
                var data = repository.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Where(item => item.ID == boxCode);
                if (data.Count() > 0)
                {
                    repository.Update<Layers.Data.Sqls.PvWms.Boxes>(new
                    {
                        AdminID = adminID,
                        PackageType = packageType
                    }, item => item.ID == boxCode);
                }
            }
        }

        /// <summary>
        /// 传递过来的箱号和对应的重量/件数进行处理后返回前台
        /// </summary>
        /// <param name="datas">参考 箱签打印.json 参数</param>
        /// <returns></returns>
        public object GetPrintInfo(JToken datas, int source)
        {

            #region 逻辑流程
            /*
             逻辑注释：传递过来的箱号和对应的重量/件数，再取出对应的分拣或者拣货信息的对应箱号的对应重量/件数，两者相加起来组织成数据返回给前台才是最正确的数据             
             */
            #endregion

            List<object> list = new List<object>();

            //对传值过来的数据通过BoxCode进行分组
            var newDatas = from data in datas
                           group data by data["BoxCode"] into groups
                           select new
                           {
                               BoxCode = groups.Key.ToString(),
                               Weight = groups.Sum(item => (decimal)item["Weight"]),
                               Quantity = groups.Sum(item => (int)item["Quantity"]),
                               //Count=groups.Count()
                           };

            foreach (var boxData in newDatas)
            {
                var boxCode = boxData.BoxCode;
                var series = boxCode.Split('-');//通过 '-' 分组获得数组中的个数就是箱号的个数，非连续箱号个数为1

                var count = int.Parse(series.LastOrDefault().Substring(8)) - int.Parse(series.FirstOrDefault().Substring(8)) + 1; //连续箱号的个数

                var weight = boxData.Weight;
                var quantity = boxData.Quantity;
                //var count = boxData.Count;

                using (var rep = new PvWmsRepository())
                {
                    CgNoticeSource noticeSource = (CgNoticeSource)source;
                    //代报关：取入库的信息
                    if (noticeSource == CgNoticeSource.AgentBreakCustoms)
                    {
                        var ienum_sortings = from sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                                             group sorting by sorting.BoxCode into sortings
                                             //where sortings.Key == boxCode
                                             select new
                                             {
                                                 BoxCode = sortings.Key,
                                                 Weight = sortings.Sum(item => item.Weight),
                                                 Quantity = sortings.Sum(item => item.Quantity)
                                             };
                        weight = (ienum_sortings?.FirstOrDefault(item => item.BoxCode == boxCode)?.Weight ?? 0) + weight;
                        quantity = (int)(ienum_sortings?.FirstOrDefault(item => item.BoxCode == boxCode)?.Quantity ?? 0) + quantity;

                    }

                    //转报关：取出库的信息
                    if (noticeSource == CgNoticeSource.AgentCustomsFromStorage)
                    {
                        var ienum_pickings = from picking in rep.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                                             group picking by picking.BoxCode into pickings
                                             select new
                                             {
                                                 BoxCode = pickings.Key,
                                                 Weight = pickings.Sum(item => item.Weight),
                                                 Quantity = pickings.Sum(item => item.Quantity)
                                             };
                        weight = (ienum_pickings?.FirstOrDefault(item => item.BoxCode == boxCode)?.Weight ?? 0) + weight;
                        quantity = (int)(ienum_pickings?.FirstOrDefault(item => item.BoxCode == boxCode)?.Quantity ?? 0) + quantity;
                    }


                    //var count = rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>().Where(item => item.Series == boxCode).Count();

                    var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                               where boxcode.Series == boxCode
                               select new
                               {
                                   ID = boxcode.ID,
                                   PackageType = boxcode.PackageType,
                                   EnterCode = boxcode.EnterCode,
                                   WhCode = boxcode.WhCode,
                                   WhName = boxcode.WhCode == nameof(Yahv.Services.WhSettings.HK) ? "香港库房" : "深圳库房",
                                   CreateDate = boxcode.CreateDate,
                                   AdminID = boxcode.AdminID,
                                   AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
                                   Quantity = quantity / count,
                                   Weight = (weight / count).Twoh()
                               };

                    list.Add(new
                    {
                        Series = boxCode,
                        BoxMessage = data.ToArray()
                    });

                }
            }



            return list;
        }

        public object GetPrintInfo(int source, string waybillID)
        {
            /*
            逻辑注释：判断source的值，以此获得是入库还是出库信息。然后根据waybillID获得对应的分拣或者拣货信息的对应箱号的对应重量/件数，如果无对应的分拣/拣货信息，则不能打印             
            */

            List<object> list = new List<object>();

            //代报关：取入库的信息
            if (source == (int)CgNoticeSource.AgentBreakCustoms)
            {
                using (var rep = new PvWmsRepository())
                {
                    var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
                              on notice.ID equals sorting.NoticeID
                               where notice.WaybillID == waybillID
                               group sorting by sorting.BoxCode into sortings
                               select new
                               {
                                   BoxCode = sortings.Key,
                                   Weight = sortings.Sum(item => (decimal)item.Weight),
                                   Quantity = sortings.Sum(item => (decimal)item.Quantity)
                               };

                    var boxes = linq.ToArray();

                    foreach (var boxData in boxes)
                    {
                        var boxCode = boxData.BoxCode;
                        int count = 1;
                        if (regex_seriesBoxCode.IsMatch(boxCode))
                        {
                            var series = boxCode.Split('-');//通过 '-' 分组获得数组中的个数就是箱号的个数，非连续箱号个数为1
                            count = int.Parse(series.LastOrDefault().Substring(8)) - int.Parse(series.FirstOrDefault().Substring(8)) + 1; //连续箱号的个数或者单个箱号的个数
                        }

                        var weight = boxData.Weight;
                        var quantity = boxData.Quantity;

                        var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                                   where boxcode.Series == boxCode
                                   select new
                                   {
                                       ID = boxcode.ID,
                                       PackageType = boxcode.PackageType,
                                       EnterCode = boxcode.EnterCode,
                                       WhCode = boxcode.WhCode,
                                       WhName = boxcode.WhCode == nameof(Yahv.Services.WhSettings.HK) ? "香港库房" : "深圳库房",
                                       CreateDate = boxcode.CreateDate,
                                       AdminID = boxcode.AdminID,
                                       AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
                                       Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
                                       Weight = (weight / count).Twoh()
                                   };

                        list.Add(new
                        {
                            Series = boxCode,
                            BoxMessage = data.ToArray()
                        });

                    }
                }

            }
            //转报关：取出库的信息
            if (source == (int)CgNoticeSource.AgentCustomsFromStorage)
            {
                using (var rep = new PvWmsRepository())
                {
                    var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                               join picking in rep.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
                               on notice.ID equals picking.NoticeID
                               where notice.WaybillID == waybillID
                               group picking by picking.BoxCode into pickings
                               select new
                               {
                                   BoxCode = pickings.Key,
                                   Weight = pickings.Sum(item => (decimal)item.Weight),
                                   Quantity = pickings.Sum(item => (decimal)item.Quantity)
                               };
                    var boxes = linq.ToArray();

                    foreach (var boxData in boxes)
                    {
                        var boxCode = boxData.BoxCode;
                        int count = 1;
                        if (regex_seriesBoxCode.IsMatch(boxCode))
                        {
                            var series = boxCode.Split('-');//通过 '-' 分组获得数组中的个数就是箱号的个数，非连续箱号个数为1
                            count = int.Parse(series.LastOrDefault().Substring(8)) - int.Parse(series.FirstOrDefault().Substring(8)) + 1; //连续箱号的个数或者单个箱号的个数
                        }

                        var weight = boxData.Weight;
                        var quantity = boxData.Quantity;

                        var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.Boxes>()
                                   where boxcode.Series == boxCode
                                   select new
                                   {
                                       ID = boxcode.ID,
                                       PackageType = boxcode.PackageType,
                                       EnterCode = boxcode.EnterCode,
                                       WhCode = boxcode.WhCode,
                                       WhName = boxcode.WhCode == nameof(Yahv.Services.WhSettings.HK) ? "香港库房" : "深圳库房",
                                       CreateDate = boxcode.CreateDate,
                                       AdminID = boxcode.AdminID,
                                       AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
                                       Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
                                       Weight = (weight / count).Twoh()
                                   };
                        list.Add(new
                        {
                            Series = boxCode,
                            BoxMessage = data.ToArray()
                        });

                    }
                }
            }

            return list;
        }


        static BoxManage current;
        static object locker = new object();

        /// <summary>
        /// 全局实例
        /// </summary>
        static public BoxManage Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new BoxManage();
                        }
                    }
                }
                return current;
            }
        }

        static BoxManage hkDeclare;
        /// <summary>
        /// 全局实例
        /// </summary>
        static public BoxManage HkDeclare
        {
            get
            {
                if (hkDeclare == null)
                {
                    lock (locker)
                    {
                        if (hkDeclare == null)
                        {
                            hkDeclare = new BoxManage();
                        }
                    }
                }
                return hkDeclare;
            }
        }

        #region 帮助类

        /// <summary>
        /// 临时箱号数据存放
        /// </summary>
        public class MyTempBox
        {
            public string BoxCode { get; set; }
            public string EnterCode { get; set; }
            public string AdminID { get; set; }
            public string TinyOrderID { get; set; }

            /// <summary>
            /// 已重写 GetHashCode
            /// </summary>
            /// <returns>GetHashCode 值</returns>
            public override int GetHashCode()
            {
                return string.Concat(this.BoxCode, this.EnterCode, this.AdminID, this.TinyOrderID).GetHashCode();
            }

            /// <summary>
            /// 已重写 Equals
            /// </summary>
            /// <param name="obj">对比</param>
            /// <returns>知否一致</returns>
            public override bool Equals(object obj)
            {
                if (obj is MyTempBox)
                {
                    return this.GetHashCode() == obj.GetHashCode();
                }
                return false;
            }

        }

        /// <summary>
        /// 临时使用的箱号
        /// </summary>
        public class TempBox
        {
            public string Code { get; set; }
            public int Index { get; set; }
        }
        #endregion
    }
}

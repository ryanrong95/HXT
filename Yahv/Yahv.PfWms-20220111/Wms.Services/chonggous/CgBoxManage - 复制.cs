//using Layers.Data.Sqls;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text.RegularExpressions;
//using System.Threading;
//using Yahv.Services.Enums;
//using Yahv.Utils.Converters;
//using System.Collections;
//using System.Transactions;

//namespace Wms.Services.chonggous
//{

//    public delegate void CgBoxSelectedEventHandler(object sender, EventArgs e);

//    /// <summary>
//    /// 箱号管理
//    /// </summary>
//    /// <remarks>
//    /// 那些箱号已经管理了?
//    /// 那些箱号没有被管理?
//    /// 区分内单、外单箱号
//    /// 一个箱子最多可以放：内单50个，外单20个（条），一个箱子中只能放一个小订单的货物！
//    /// </remarks>
//    public class CgBoxManage : IEnumerable<tBoxCode>
//    {
//        object objectLock = new object();
//        /// <summary>
//        /// 箱号已经被选择事件
//        /// </summary>
//        event CgBoxSelectedEventHandler boxSelected;

//        /// <summary>
//        /// 箱号已经被选择事件（香港）
//        /// </summary>
//        public event CgBoxSelectedEventHandler BoxSelected
//        {
//            add
//            {
//                lock (objectLock)
//                {
//                    var events = Thread.GetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}")) as CgBoxSelectedEventHandler;
//                    if (events == null)
//                    {
//                        events = value;
//                        Thread.SetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}"), value);
//                        boxSelected += value;
//                    }
//                }
//            }
//            remove
//            {
//                lock (objectLock)
//                {
//                    boxSelected -= value;
//                    Thread.SetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}"), null);
//                }
//            }
//        }

//        /// <summary>
//        /// 箱号的正则表达式
//        /// </summary>
//        Regex reg = new Regex(@"^(\D*)(\d*)[-](\D*)(\d*)$");

//        Regex regTime = new Regex(@"^\[\d{8}\](\D*)(\d*)[-](\D*)(\d*)$");
//        HashSet<tBoxCode> codes;

//        void GenCode(string code, DateTime date)
//        {
//            if (code.Contains('-'))
//            {
//                var match = reg.Match(code);
//                var pre = match.Groups[1].Value;
//                var first = match.Groups[2].Value;
//                var last = match.Groups[4].Value;

//                var padLen = Math.Min(first.Length, last.Length);

//                //获得第一个箱号下标值
//                var index1 = int.Parse(match.Groups[2].Value);
//                //获得第二个箱号下标值
//                var index2 = int.Parse(match.Groups[4].Value);

//                for (int index = index1; index <= index2; index++)
//                {
//                    codes.Add(new tBoxCode
//                    {
//                        Code = pre + index.ToString().PadLeft(padLen, '0'),
//                        Date = date,
//                    });
//                }
//            }
//            else
//            {
//                codes.Add(new tBoxCode
//                {
//                    Code = code,
//                    Date = date,
//                });
//            }
//        }

//        void DelCode(string code, DateTime date)
//        {
//            if (code.Contains('-'))
//            {
//                var match = reg.Match(code);
//                var pre = match.Groups[1].Value;
//                var first = match.Groups[2].Value;
//                var last = match.Groups[4].Value;

//                var padLen = Math.Min(first.Length, last.Length);

//                //获得第一个箱号下标值
//                var index1 = int.Parse(match.Groups[2].Value);
//                //获得第二个箱号下标值
//                var index2 = int.Parse(match.Groups[4].Value);

//                for (int index = index1; index <= index2; index++)
//                {
//                    codes.Remove(new tBoxCode
//                    {
//                        Code = pre + index.ToString().PadLeft(padLen, '0'),
//                        Date = date,
//                    });
//                }
//            }
//            else
//            {
//                codes.Remove(new tBoxCode
//                {
//                    Code = code,
//                    Date = date,
//                });
//            }
//        }

//        //void Verification(string pre, string small, string large,DateTime? date)
//        //{
//        //    //获得第一个箱号下标值
//        //    var index1 = int.Parse(small);
//        //    //获得第二个箱号下标值
//        //    var index2 = int.Parse(large);

//        //    for (int index = index1; index <= index2; index++)
//        //    {
//        //        if (this.SingleOrDefault(item => item.Code == (pre + index.ToString().PadLeft(large.Length, '0'))&&item.Date==date) != null)
//        //        {
//        //            if (this != null && this.boxSelected != null)
//        //            {
//        //                this.boxSelected(this, new EventArgs());
//        //            }
//        //            return;
//        //        }
//        //    }
//        //}

//        CgBoxManage()
//        {

//            //删除箱号
//            //new Thread(DeleteBox).Start();

//            var codes = this.codes = new HashSet<tBoxCode>();
//            using (var repository = new PvWmsRepository())
//            {
//                var boxes = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().ToArray();
//                foreach (var box in boxes)
//                {
//                    if (regTime.IsMatch(box.ID))
//                    {
//                        var match = reg.Match(box.ID);
//                        var date = match.Groups[1].Value;
//                        date = date.Insert(4, "-");
//                        //符合时间格式
//                        date = date.Insert(7, "-");
//                        var code = box.Code;
//                        this.GenCode(code, DateTime.Parse(date));
//                    }
                   
//                }
//            }


//        }

//        public IEnumerator<tBoxCode> GetEnumerator()
//        {
//            return this.codes.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return this.GetEnumerator();
//        }

//        /// <summary>
//        /// 实时保存至数据库
//        /// </summary>
//        /// <param name="enterCode"></param>
//        /// <param name="code"></param>
//        /// <param name="date"></param>
//        /// <param name="adminID"></param>
//        public string Enter(string enterCode, string code, DateTime? date, string adminID)
//        {
//            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间

//            using (var repository = new PvWmsRepository())
//            //开启事务
//            using (var tran = repository.OpenTransaction())
//            {
//                try
//                {
//                    if (this.SingleOrDefault(item => item.Code == code && item.Date == current) != null)
//                    {
//                        if (this != null && this.boxSelected != null)
//                        {
//                            this.boxSelected(this, new EventArgs());
//                        }
//                        return null;
//                    }

//                    if (code.Contains('-'))
//                    {
//                        var match = reg.Match(code);
//                        var prexfirst = match.Groups[1].Value;
//                        var prexlast = match.Groups[3].Value;
//                        var first = match.Groups[2].Value;
//                        var last = match.Groups[4].Value;

//                        //也可以箱号前缀硬改成第一个
//                        if (prexfirst != prexlast)
//                        {
//                            prexlast = prexfirst;
//                            code = string.Concat(prexfirst, first, "-", prexlast, last);
//                        }

//                        //也有办法可以交换大小并用短的长度进行补齐
//                        if (first.Length > last.Length)
//                        {
//                            if (int.Parse(first) > int.Parse(last))
//                            {

//                                //WL011-WL04大的值11
//                                var large = first;
//                                //小的值4
//                                var small = last.PadLeft(large.Length, '0');

//                                //验证箱号是否已经存在
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(small);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(large);

//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                code = string.Concat(prexfirst, small, "-", prexlast, large);
//                            }

//                            else
//                            {
//                                var small = first;
//                                var large = last.PadLeft(first.Length, '0');

//                                //验证箱号是否已经存在
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(small);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(large);

//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                //WL001-WL04
//                                code = string.Concat(prexfirst, first, "-", prexlast, large);
//                            }
//                        }
//                        if (first.Length < last.Length)
//                        {
//                            if (int.Parse(first) > int.Parse(last))
//                            {
//                                //验证箱号是否已经存在
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(first);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(last);

//                                //小的值4
//                                var small = index2.ToString().PadLeft(first.Length,'0');
//                                //WL099-WL0004大的值99
//                                var large = first;

//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                code = string.Concat(prexfirst, small, "-", prexlast, large);
//                            }

//                            else
//                            {
//                                //
//                                //WL01-WL004(情况一:处理成WL01-WL04)

//                                //WL99-WL103（情况二：code维持原状不用考虑）
//                                //
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(first);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(last);

//                                var small = first;
//                                var large = index2.ToString().PadLeft(first.Length, '0');

//                                //验证箱号是否已经存在
//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                //WL01-WL004(这种情况暂不考虑)
//                                code = string.Concat(prexfirst, small, "-", prexlast, last);
//                            }

//                        }
//                        //考虑大小的情况
//                        if (first.Length == last.Length)
//                        {
//                            if (int.Parse(first) > int.Parse(last))
//                            {
//                                //小的值4
//                                var small = last;
//                                //WL09-WL04大的值9
//                                var large = first;

//                                //验证箱号是否已经存在
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(small);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(large);

//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                code = string.Concat(prexfirst, small, "-", prexlast, large);
//                            }

//                            else
//                            {
//                                //小的值4
//                                var small = first;
//                                //WL04-WL09大的值9
//                                var large = last;
//                                //验证箱号是否已经存在
//                                //获得第一个箱号下标值
//                                var index1 = int.Parse(small);
//                                //获得第二个箱号下标值
//                                var index2 = int.Parse(large);

//                                for (int index = index1; index <= index2; index++)
//                                {
//                                    if (this.SingleOrDefault(item => item.Code == (prexfirst + index.ToString().PadLeft(large.Length, '0')) && item.Date == date) != null)
//                                    {
//                                        if (this != null && this.boxSelected != null)
//                                        {
//                                            this.boxSelected(this, new EventArgs());
//                                        }
//                                        return null;
//                                    }
//                                }

//                                //WL01-WL004
//                                code = string.Concat(prexfirst, first, "-", prexlast, last);
//                            }
//                        }


//                        var boxesView = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>();
//                        var cbox = boxesView.SingleOrDefault(item => item.ID == code);

//                        if (cbox != null)
//                        {
//                            if (this != null && this.boxSelected != null)
//                            {
//                                this.boxSelected(this, new EventArgs());
//                            }
//                            return null;
//                        }

//                    }

//                    this.GenCode(code, current);

//                    var id = string.Concat("[", current.ToString("yyyyMMdd"), "]", code);

//                    //添加箱号信息
//                    repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
//                    {
//                        ID = id,
//                        Code = code,
//                        PackerID = adminID,
//                        CreateDate = DateTime.Now,
//                        EnterCode = enterCode,
//                        PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
//                    });

//                    tran.Commit();
//                    return id;

//                }
//                finally
//                {
//                    tran.Dispose();
//                }

//            }
//        }


//        public void Delete(string boxCode, DateTime? date)
//        {
//            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
//            using (var repository = new PvWmsRepository())
//            {

//                var box = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().FirstOrDefault(item => item.ID == boxCode);
//                //删除箱号
//                if (repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == boxCode))
//                {
//                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == boxCode)
//                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == boxCode))
//                    {
//                        //内存中也删除旧的箱号
//                        DelCode(box.Code, current);
//                        //删除旧的箱号
//                        repository.Delete<Layers.Data.Sqls.PvWms.TBoxes>(item => item.ID == boxCode);
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 修改历史到货中的箱号
//        /// </summary>
//        /// <param name="adminID"></param>
//        /// <param name="oldBoxCode"></param>
//        /// <param name="newBoxCode"></param>
//        /// <param name="date"></param>
//        public void ModifyCode(string adminID, string oldBoxCode, string newBoxCode, DateTime? date)
//        {
//            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间

//            using (var repository = new PvWmsRepository())
//            {

//                //处理旧箱号
//                if (repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == oldBoxCode))
//                {
//                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == oldBoxCode)
//                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == oldBoxCode))
//                    {
//                        //删除旧的箱号
//                        repository.Delete<Layers.Data.Sqls.PvWms.TBoxes>(item => item.ID == oldBoxCode);
//                    }
//                }

//                //新箱号处理(如果数据库不存在就保存到数据库)
//                if (!repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == newBoxCode))
//                {
//                    var id = string.Concat("[", current.ToString("yyyyMMdd"), "]", newBoxCode);
//                    //添加箱号信息
//                    repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
//                    {
//                        ID = id,
//                        Code=newBoxCode,
//                        PackerID = adminID,
//                        CreateDate = DateTime.Now,
//                        PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
//                    });
//                }


//            }
//        }

//        /// <summary>
//        /// 获得打印信息
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="waybillID"></param>
//        /// <returns></returns>
//        //public object GetPrintInfo(int source, string waybillID)
//        //{
//        //    /*
//        //    逻辑注释：判断source的值，以此获得是入库还是出库信息。然后根据waybillID获得对应的分拣或者拣货信息的对应箱号的对应重量/件数，如果无对应的分拣/拣货信息，则不能打印             
//        //    */

//        //    List<object> list = new List<object>();

//        //    //代报关：取入库的信息
//        //    if (source == (int)CgNoticeSource.AgentBreakCustoms)
//        //    {
//        //        using (var rep = new PvWmsRepository())
//        //        {
//        //            var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//        //                       join sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
//        //                      on notice.ID equals sorting.NoticeID
//        //                       where notice.WaybillID == waybillID
//        //                       group sorting by sorting.BoxCode into sortings
//        //                       select new
//        //                       {
//        //                           BoxCode = sortings.Key,
//        //                           Weight = sortings.Sum(item => (decimal)item.Weight),
//        //                           Quantity = sortings.Sum(item => (decimal)item.Quantity)
//        //                       };

//        //            var boxes = linq.ToArray();

//        //            foreach (var boxData in boxes)
//        //            {
//        //                var boxCode = boxData.BoxCode;
//        //                int count = 1;

//        //                //匹配正则
//        //                var matches = reg.Matches(boxCode);

//        //                if (reg.IsMatch(boxCode))
//        //                {
//        //                    foreach (Match match in matches)
//        //                    {
//        //                        //获得第一个箱号下标值
//        //                        var index1 = int.Parse(match.Groups[2].ToString());
//        //                        //获得第二个箱号下标值
//        //                        var index2 = int.Parse(match.Groups[4].ToString());

//        //                        //获得箱号的总个数
//        //                        count = index2 - index1 + 1;
//        //                    }
//        //                }

//        //                var weight = boxData.Weight;
//        //                var quantity = boxData.Quantity;

//        //                var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
//        //                           where boxcode.ID == boxCode
//        //                           select new
//        //                           {
//        //                               ID = boxcode.ID,
//        //                               PackageType = boxcode.PackageType,
//        //                               EnterCode = boxcode.EnterCode,
//        //                               WhCode = nameof(Yahv.Services.WhSettings.HK),
//        //                               WhName = "香港库房",//打印需要的元素
//        //                               CreateDate = boxcode.CreateDate,
//        //                               AdminID = boxcode.AdminID,
//        //                               AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
//        //                               Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
//        //                               Weight = (weight / count).Twoh()
//        //                           };

//        //                list.Add(new
//        //                {
//        //                    Series = boxCode,
//        //                    BoxMessage = data.ToArray()
//        //                });

//        //            }
//        //        }

//        //    }
//        //    //转报关：取出库的信息
//        //    if (source == (int)CgNoticeSource.AgentCustomsFromStorage)
//        //    {
//        //        using (var rep = new PvWmsRepository())
//        //        {
//        //            var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//        //                       join picking in rep.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
//        //                       on notice.ID equals picking.NoticeID
//        //                       where notice.WaybillID == waybillID
//        //                       group picking by picking.BoxCode into pickings
//        //                       select new
//        //                       {
//        //                           BoxCode = pickings.Key,
//        //                           Weight = pickings.Sum(item => (decimal)item.Weight),
//        //                           Quantity = pickings.Sum(item => (decimal)item.Quantity)
//        //                       };
//        //            var boxes = linq.ToArray();

//        //            foreach (var boxData in boxes)
//        //            {
//        //                var boxCode = boxData.BoxCode;
//        //                int count = 1;

//        //                //匹配正则
//        //                var matches = reg.Matches(boxCode);

//        //                if (reg.IsMatch(boxCode))
//        //                {
//        //                    foreach (Match match in matches)
//        //                    {
//        //                        //获得第一个箱号下标值
//        //                        var index1 = int.Parse(match.Groups[2].ToString());
//        //                        //获得第二个箱号下标值
//        //                        var index2 = int.Parse(match.Groups[4].ToString());

//        //                        //获得箱号的总个数
//        //                        count = index2 - index1 + 1;
//        //                    }
//        //                }

//        //                var weight = boxData.Weight;
//        //                var quantity = boxData.Quantity;


//        //                /* 如何处置打印并且更改包装类型*/
//        //                var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
//        //                           where boxcode.ID == boxCode
//        //                           select new
//        //                           {
//        //                               ID = boxcode.ID,
//        //                               PackageType = boxcode.PackageType,
//        //                               EnterCode = boxcode.EnterCode,
//        //                               WhCode = nameof(Yahv.Services.WhSettings.HK),
//        //                               WhName = "香港库房",//打印需要的元素
//        //                               CreateDate = boxcode.CreateDate,
//        //                               AdminID = boxcode.AdminID,
//        //                               AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
//        //                               Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
//        //                               Weight = (weight / count).Twoh()
//        //                           };
//        //                list.Add(new
//        //                {
//        //                    Series = boxCode,
//        //                    BoxMessage = data.ToArray()
//        //                });

//        //            }
//        //        }
//        //    }

//        //    return list;

//        //    return null;
//        //}

//        /// <summary>
//        /// 把有效日期是今天之前产生的箱号删除掉（弃用）
//        /// </summary>
//        void DeleteBox()
//        {
//            //using (var repository = new PvWmsRepository())
//            //{
//            //    var now = DateTime.Now.Date;
//            //    Layers.Data.Sqls.PvWms.TBoxes sqlb;
//            //    //Layers.Data.Sqls.PvWms.Pickings sqlp;

//            //    //废弃
//            //    //                repository.Command(string.Format($@"delete from {nameof(Layers.Data.Sqls.PvWms.TBoxes)}  where {nameof(sqlb.CreateDate)} 
//            //    //between '{{0}}' and  '{{1}}' and [ID] in  (SELECT distinct [BoxCode] FROM {nameof(Layers.Data.Sqls.PvWms.Pickings)}   where {nameof(sqlp.CreateDate)}  between '{{0}}' and  '{{1}}') ", now.AddDays(-1).ToString("yyyy-MM-dd"), now.ToString("yyyy-MM-dd")));

//            //    //箱号过了有效期限就删除掉（不用考虑Pickings）
//            //    //repository.Command(string.Format($@"delete from {nameof(Layers.Data.Sqls.PvWms.TBoxes)}  where {nameof(sqlb.EffectiveDate)} <'{{0}}' ", now.ToString("yyyy-MM-dd")));
//            //}
//        }

//        static CgBoxManage current;
//        static object locker = new object();

//        /// <summary>
//        /// 全局实例
//        /// </summary>
//        static public CgBoxManage Current
//        {
//            get
//            {
//                if (current == null)
//                {
//                    lock (locker)
//                    {
//                        if (current == null)
//                        {
//                            current = new CgBoxManage();
//                        }
//                    }
//                }
//                return current;
//            }
//        }
//    }

//    /// <summary>
//    /// 临时箱号
//    /// </summary>
//    public class tBoxCode
//    {
//        public string Code { get; set; }

//        public DateTime Date { get; set; }

//        /// <summary>
//        /// 重写Hash
//        /// </summary>
//        /// <returns></returns>
//        public override int GetHashCode()
//        {
//            return (this.Date.Ticks + this.Code).GetHashCode();
//        }

//        /// <summary>
//        /// 重写判断
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        public override bool Equals(object obj)
//        {
//            if (obj is tBoxCode)
//            {
//                return this.GetHashCode() == obj.GetHashCode();
//            }

//            return false;
//        }

//    }

//    /// <summary>
//    /// 私有箱号设计
//    /// </summary>
//    public class MyBox
//    {
//        /// <summary>
//        /// 箱号  
//        /// </summary>
//        public string ID { get; set; }

//        /// <summary>
//        /// 前缀
//        /// </summary>
//        /// <remarks>
//        /// 同时可以理解为分类
//        /// </remarks>
//        public string Prex { get; set; }

//        /// <summary>
//        /// 有效日期
//        /// </summary>
//        public DateTime EffectiveDate { get; set; }

//        /// <summary>
//        /// 入仓号
//        /// </summary>
//        public string EnterCode { get; set; }

//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        /// 操作人
//        /// </summary>
//        public string Packer { get; set; }

//        /// <summary>
//        /// 包装类型
//        /// </summary>
//        public string PackageType { get; set; }
//    }
//}

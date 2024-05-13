using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using Yahv.Services.Enums;
using Yahv.Utils.Converters;
using System.Collections;
using System.Transactions;

namespace Wms.Services.chonggous
{

    public delegate void CgBoxSelectedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// 箱号管理
    /// </summary>
    /// <remarks>
    /// 那些箱号已经管理了?
    /// 那些箱号没有被管理?
    /// 区分内单、外单箱号
    /// 一个箱子最多可以放：内单50个，外单20个（条），一个箱子中只能放一个小订单的货物！
    /// </remarks>
    public class CgBoxManage : IEnumerable<string>
    {
        object objectLock = new object();
        /// <summary>
        /// 箱号已经被选择事件
        /// </summary>
        event CgBoxSelectedEventHandler boxSelected;

        /// <summary>
        /// 箱号已经被选择事件（香港）
        /// </summary>
        public event CgBoxSelectedEventHandler BoxSelected
        {
            add
            {
                lock (objectLock)
                {
                    var events = Thread.GetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}")) as CgBoxSelectedEventHandler;
                    if (events == null)
                    {
                        events = value;
                        Thread.SetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}"), value);
                        boxSelected += value;
                    }
                }
            }
            remove
            {
                lock (objectLock)
                {
                    boxSelected -= value;
                    Thread.SetData(Thread.GetNamedDataSlot($"{nameof(CgBoxManage)}_{nameof(boxSelected)}"), null);
                }
            }
        }


        /// <summary>
        /// 为避免重复使用正则表达式用切割
        /// </summary>
        static Regex regex_number = new Regex(@"^(\D*)(\d+)$", RegexOptions.Singleline);
        /// <summary>
        /// 为避免重复使用正则表达式用切割
        /// </summary>
        static Regex regex_boxPrex = new Regex(@"^\[\d{8}\]", RegexOptions.Singleline);

        HashSet<string> codes;

        //void Verification(string pre, string small, string large,DateTime? date)
        //{
        //    //获得第一个箱号下标值
        //    var index1 = int.Parse(small);
        //    //获得第二个箱号下标值
        //    var index2 = int.Parse(large);

        //    for (int index = index1; index <= index2; index++)
        //    {
        //        if (this.SingleOrDefault(item => item.Code == (pre + index.ToString().PadLeft(large.Length, '0'))&&item.Date==date) != null)
        //        {
        //            if (this != null && this.boxSelected != null)
        //            {
        //                this.boxSelected(this, new EventArgs());
        //            }
        //            return;
        //        }
        //    }
        //}

        CgBoxManage()
        {
            using (var repository = new PvWmsRepository())
            {
                //获取近体之前的
                var boxes = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
                      .Where(item => item.CreateDate >= DateTime.Now.Date)
                      .Select(item => item.ID).ToArray();

                var codes = this.codes = new HashSet<string>(boxes.SelectMany(code => GenCode(code)));
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.codes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 如果是连续箱号获得每个独立的箱号集合，如果非连续箱号获得本身
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IEnumerable<string> GenCode(string code)
        {
            if (!regex_boxPrex.IsMatch(code))
            {
                return null;
            }

            var context = code.Substring(10);
            var prexd = code.Substring(0, 10);
            var splits = context.Split('-');
            if (splits.Length == 1)
            {
                return new[] { code };
            }

            var first = splits[0];
            var last = splits[1];

            var prex = regex_number.Match(first).Groups[1].Value;
            var firstTxt = regex_number.Match(first).Groups[2].Value;
            var lastTxt = regex_number.Match(last).Groups[2].Value;

            // 添加新的判断逻辑,来支持新的要求，WL06-01, WL06-02算一个箱号
            var lastPrex = regex_number.Match(last).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(lastPrex))
            {
                return new[] { code };
            }

            var firstNum = int.Parse(firstTxt);
            var lastNum = int.Parse(lastTxt);

            var start = Math.Min(firstNum, lastNum);
            var end = Math.Max(firstNum, lastNum);

            var pad = Math.Min(firstTxt.Length, lastTxt.Length);

            HashSet<string> sets = new HashSet<string>();
            //这样写主要就是为可以报错！
            for (int index = start; index <= end; index++)
            {
                sets.Add($"{prexd}{prex}{index.ToString().PadLeft(pad, '0')}");
            }
            return sets;
        }

        /// <summary>
        /// 获取正确的箱号Code
        /// </summary>
        /// <param name="code">提交过来的箱号Code</param>
        /// <returns></returns>
        string GetCorrectCode(string code)
        {
            //var context = code.Substring(10);
            //var prexd = code.Substring(0, 10);
            var splits = code.Split('-');
            if (splits.Length == 1)
            {
                return code;
            }

            var first = splits[0];
            var last = splits[1];

            var prex = regex_number.Match(first).Groups[1].Value;
            var firstTxt = regex_number.Match(first).Groups[2].Value;
            var lastTxt = regex_number.Match(last).Groups[2].Value;

            //添加逻辑为了满足公司新的箱号要求WL06-01, WL06-02算一个箱号
            var lastPrex = regex_number.Match(last).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(lastPrex))
            {
                return code;
            }

            var firstNum = int.Parse(firstTxt);
            var lastNum = int.Parse(lastTxt);

            var start = Math.Min(firstNum, lastNum);
            var end = Math.Max(firstNum, lastNum);

            //如果两者长度不一致，补齐短的长度
            var pad = Math.Min(firstTxt.Length, lastTxt.Length);

            var newCode = string.Concat(prex,
            start.ToString().PadLeft(pad, '0'), "-", prex, end.ToString().PadLeft(pad, '0'));

            return newCode;
        }

        /// <summary>
        /// 实时保存至数据库
        /// </summary>
        /// <param name="enterCode"></param>
        /// <param name="code"></param>
        /// <param name="date"></param>
        /// <param name="adminID"></param>
        public string Enter(string enterCode, string code, DateTime? date, string adminID)
        {
            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间
            //var id = $"[{current.ToString("yyyyMMdd")}]{code}";

            //正确的code
            var newCode = GetCorrectCode(code);

            //获取正确的箱号编号
            var newId = $"[{current.ToString("yyyyMMdd")}]{newCode}";
            var ienums = GenCode(newId);


            if (ienums.Any(item => this.codes.Contains(item)))
            {
                //做事件返回事件说明

                if (this != null && this.boxSelected != null)
                {
                    this.boxSelected(this, new EventArgs());
                }
                return null;

            }

            using (var repository = new PvWmsRepository())
            using (var tran = repository.OpenTransaction())
            {
                var boxesView = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>();
                var cbox = boxesView.SingleOrDefault(item => item.ID == newId);

                if (cbox != null)
                {
                    if (this != null && this.boxSelected != null)
                    {
                        this.boxSelected(this, new EventArgs());
                    }
                    return null;
                }

                foreach (var item in ienums)
                {
                    //操作内存
                    this.codes.Add(item);
                }

                //操作数据库
                repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
                {
                    ID = newId,
                    Code = newCode,
                    EnterCode = enterCode,
                    PackerID = adminID,
                    CreateDate = DateTime.Now,
                    PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                });

                tran.Commit();
            }


            return newId;
        }


        public void Delete(string boxCode)
        {
            using (var repository = new PvWmsRepository())
            {

                var box = repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().FirstOrDefault(item => item.ID == boxCode);
                //删除箱号
                if (repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == boxCode))
                {
                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == boxCode)
                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == boxCode)
                      && !repository.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>().Any(item => item.BoxCode == boxCode && item.IsCurrent == true))
                    {
                        //获取正确的箱号编号
                        var ienums = GenCode(boxCode);
                        //内存中也删除旧的箱号
                        foreach (var item in ienums)
                        {
                            if (this.codes.Contains(item))
                            {
                                this.codes.Remove(item);
                            }
                        }

                        //删除旧的箱号
                        repository.Delete<Layers.Data.Sqls.PvWms.TBoxes>(item => item.ID == boxCode);
                    }
                }
            }
        }

        /// <summary>
        /// 修改历史到货中的箱号
        /// </summary>
        /// <param name="adminID"></param>
        /// <param name="oldBoxCode"></param>
        /// <param name="newBoxCode"></param>
        /// <param name="date"></param>
        public void ModifyCode(string adminID, string oldBoxCode, string newBoxCode, DateTime? date)
        {
            var current = (date.HasValue ? date.Value : DateTime.Now).Date;//当前时间

            using (var repository = new PvWmsRepository())
            {

                //处理旧箱号
                if (repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == oldBoxCode))
                {
                    if (!repository.ReadTable<Layers.Data.Sqls.PvWms.Sortings>().Any(item => item.BoxCode == oldBoxCode)
                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Pickings>().Any(item => item.BoxCode == oldBoxCode)
                     && !repository.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>().Any(item => item.BoxCode == oldBoxCode && item.IsCurrent == true))
                    {
                        //内存中也删除旧的箱号
                        this.codes.Remove(oldBoxCode);
                        //删除旧的箱号
                        repository.Delete<Layers.Data.Sqls.PvWms.TBoxes>(item => item.ID == oldBoxCode);
                    }
                }

                //新箱号处理(如果数据库不存在就保存到数据库)
                if (!repository.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>().Any(item => item.ID == newBoxCode))
                {
                    //正确的code
                    var newCode = GetCorrectCode(newBoxCode);
                    //获取正确的箱号编号
                    var newId = $"[{current.ToString("yyyyMMdd")}]{newCode}";

                    //内存中也新增新的箱号
                    this.codes.Add(newId);
                    //添加箱号信息
                    repository.Insert(new Layers.Data.Sqls.PvWms.TBoxes
                    {
                        ID = newId,
                        Code = newCode,
                        PackerID = adminID,
                        CreateDate = DateTime.Now,
                        PackageType = PackageTypes.CartonBox.GBCode//默认包装类型：纸箱
                    });
                }


            }
        }

        public object GetPrintInfo(string waybillID)
        {
            /*新版打印信息：箱号、入仓号、客户公司（委托方）、装箱人、型号数、重量（毛重）。这些数据理论上都要返回前端，前端展示只展示  箱号、连续箱号和箱子类型，打印需要每个箱号（连续箱号分成单独的每个箱号）都打印出来，箱号打印的时候展示上面的信息 */

            List<object> list = new List<object>();

            //根据连续箱号和类型分组
            using (var rep = new PvWmsRepository())
            {
                var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
                           join storage in rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>()
                           on notice.StorageID equals storage.ID
                           join logStorage in rep.ReadTable<Layers.Data.Sqls.PvWms.Logs_Storage>()
                          on storage.ID equals logStorage.StorageID
                           join boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
                           on logStorage.BoxCode equals boxcode.ID
                           join client in rep.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>()
                           on boxcode.EnterCode equals client.EnterCode
                           join admin in rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>()
                           on boxcode.PackerID equals admin.ID
                           //where notice.WaybillID == waybillID
                           //&& logStorage.IsCurrent == true
                           where logStorage.IsCurrent == true

                           //group logStorage by new
                           //{
                           //    logStorage.BoxCode,
                           //    boxcode.PackageType
                           //} into messages
                           select new
                           {
                               WaybillID = notice.WaybillID,
                               AdminID = boxcode.PackerID,//装箱人
                               AdminName = admin.RealName,//装箱人
                               EnterCode = boxcode.EnterCode,
                               PackageType = boxcode.PackageType,
                               ClientCompanyName = client.Name,//客户公司（委托方）
                               BoxCode = logStorage.BoxCode,
                               Weight = logStorage.Weight,
                               Quantity = storage.Quantity
                           };

                //分拣中的箱号
                if (!string.IsNullOrWhiteSpace(waybillID))
                {
                    linq = linq.Where(item => item.WaybillID == waybillID);
                }
                //箱号管理中的箱号
                var boxes = linq.ToArray();

                var linqBoxes = from box in boxes
                                group box by new
                                {
                                    box.EnterCode,
                                    box.BoxCode,
                                    box.AdminID,
                                    box.AdminName,
                                    box.PackageType,
                                    box.ClientCompanyName
                                } into boxMsg
                                select new
                                {
                                    boxMsg.Key.BoxCode,
                                    boxMsg.Key.EnterCode,
                                    boxMsg.Key.PackageType,
                                    boxMsg.Key.AdminID,
                                    boxMsg.Key.AdminName,
                                    boxMsg.Key.ClientCompanyName,
                                    Weight = boxMsg.Sum(item => item.Weight),
                                    Quantity = boxMsg.Sum(item => item.Quantity)
                                };
                var newBoxes = linqBoxes.ToArray();

                foreach (var boxData in newBoxes)
                {
                    //获得正确的箱号：[20200806]WL01-WL03获得[20200806]WL01/[20200806]WL02/[20200806]WL03
                    var correctBox = GenCode(boxData.BoxCode);
                    int count = correctBox.Count();
                    var weight = boxData.Weight;
                    var quantity = boxData.Quantity;

                    foreach (var cbox in correctBox)
                    {
                        var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
                                   where boxcode.ID == boxData.BoxCode
                                   select new
                                   {
                                       ID = cbox.Substring(10),
                                       WhCode = nameof(Yahv.Services.WhSettings.HK),
                                       WhName = "香港库房",//打印需要的元素
                                       CreateDate = boxcode.CreateDate,
                                       //AdminID = boxcode.PackerID,//装箱人
                                       //AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.PackerID).FirstOrDefault().RealName,//装箱人
                                       Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),//型号数
                                       Weight = (weight / count).Twoh(),//重量
                                   };

                        list.Add(new
                        {
                            EnterCode = boxData.EnterCode,
                            Series = boxData.BoxCode.Substring(10),
                            PackageType = boxData.PackageType,
                            ClientCompanyName = boxData.ClientCompanyName,//客户公司（委托方）
                            AdminID = boxData.AdminID,
                            BoxManage = data
                        });
                    }

                }

            }
            return list;

        }

        /// <summary>
        /// 获得打印信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="waybillID"></param>
        /// <returns></returns>
        //public object GetPrintInfo(int source, string waybillID)
        //{
        //    /*
        //    逻辑注释：判断source的值，以此获得是入库还是出库信息。然后根据waybillID获得对应的分拣或者拣货信息的对应箱号的对应重量/件数，如果无对应的分拣/拣货信息，则不能打印             
        //    */

        //    List<object> list = new List<object>();

        //    //代报关：取入库的信息
        //    if (source == (int)CgNoticeSource.AgentBreakCustoms)
        //    {
        //        using (var rep = new PvWmsRepository())
        //        {
        //            var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
        //                       join sorting in rep.ReadTable<Layers.Data.Sqls.PvWms.Sortings>()
        //                      on notice.ID equals sorting.NoticeID
        //                       where notice.WaybillID == waybillID
        //                       group sorting by sorting.BoxCode into sortings
        //                       select new
        //                       {
        //                           BoxCode = sortings.Key,
        //                           Weight = sortings.Sum(item => (decimal)item.Weight),
        //                           Quantity = sortings.Sum(item => (decimal)item.Quantity)
        //                       };

        //            var boxes = linq.ToArray();

        //            foreach (var boxData in boxes)
        //            {
        //                var boxCode = boxData.BoxCode;
        //                int count = 1;

        //                //匹配正则
        //                var matches = reg.Matches(boxCode);

        //                if (reg.IsMatch(boxCode))
        //                {
        //                    foreach (Match match in matches)
        //                    {
        //                        //获得第一个箱号下标值
        //                        var index1 = int.Parse(match.Groups[2].ToString());
        //                        //获得第二个箱号下标值
        //                        var index2 = int.Parse(match.Groups[4].ToString());

        //                        //获得箱号的总个数
        //                        count = index2 - index1 + 1;
        //                    }
        //                }

        //                var weight = boxData.Weight;
        //                var quantity = boxData.Quantity;

        //                var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
        //                           where boxcode.ID == boxCode
        //                           select new
        //                           {
        //                               ID = boxcode.ID,
        //                               PackageType = boxcode.PackageType,
        //                               EnterCode = boxcode.EnterCode,
        //                               WhCode = nameof(Yahv.Services.WhSettings.HK),
        //                               WhName = "香港库房",//打印需要的元素
        //                               CreateDate = boxcode.CreateDate,
        //                               AdminID = boxcode.AdminID,
        //                               AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
        //                               Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
        //                               Weight = (weight / count).Twoh()
        //                           };

        //                list.Add(new
        //                {
        //                    Series = boxCode,
        //                    BoxMessage = data.ToArray()
        //                });

        //            }
        //        }

        //    }
        //    //转报关：取出库的信息
        //    if (source == (int)CgNoticeSource.AgentCustomsFromStorage)
        //    {
        //        using (var rep = new PvWmsRepository())
        //        {
        //            var linq = from notice in rep.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
        //                       join picking in rep.ReadTable<Layers.Data.Sqls.PvWms.Pickings>()
        //                       on notice.ID equals picking.NoticeID
        //                       where notice.WaybillID == waybillID
        //                       group picking by picking.BoxCode into pickings
        //                       select new
        //                       {
        //                           BoxCode = pickings.Key,
        //                           Weight = pickings.Sum(item => (decimal)item.Weight),
        //                           Quantity = pickings.Sum(item => (decimal)item.Quantity)
        //                       };
        //            var boxes = linq.ToArray();

        //            foreach (var boxData in boxes)
        //            {
        //                var boxCode = boxData.BoxCode;
        //                int count = 1;

        //                //匹配正则
        //                var matches = reg.Matches(boxCode);

        //                if (reg.IsMatch(boxCode))
        //                {
        //                    foreach (Match match in matches)
        //                    {
        //                        //获得第一个箱号下标值
        //                        var index1 = int.Parse(match.Groups[2].ToString());
        //                        //获得第二个箱号下标值
        //                        var index2 = int.Parse(match.Groups[4].ToString());

        //                        //获得箱号的总个数
        //                        count = index2 - index1 + 1;
        //                    }
        //                }

        //                var weight = boxData.Weight;
        //                var quantity = boxData.Quantity;


        //                /* 如何处置打印并且更改包装类型*/
        //                var data = from boxcode in rep.ReadTable<Layers.Data.Sqls.PvWms.TBoxes>()
        //                           where boxcode.ID == boxCode
        //                           select new
        //                           {
        //                               ID = boxcode.ID,
        //                               PackageType = boxcode.PackageType,
        //                               EnterCode = boxcode.EnterCode,
        //                               WhCode = nameof(Yahv.Services.WhSettings.HK),
        //                               WhName = "香港库房",//打印需要的元素
        //                               CreateDate = boxcode.CreateDate,
        //                               AdminID = boxcode.AdminID,
        //                               AdminName = rep.ReadTable<Layers.Data.Sqls.PvWms.AdminsTopView>().Where(item => item.ID == boxcode.AdminID).FirstOrDefault().RealName,
        //                               Quantity = (int)Math.Round((quantity / count), MidpointRounding.AwayFromZero),
        //                               Weight = (weight / count).Twoh()
        //                           };
        //                list.Add(new
        //                {
        //                    Series = boxCode,
        //                    BoxMessage = data.ToArray()
        //                });

        //            }
        //        }
        //    }

        //    return list;

        //    return null;
        //}

        /// <summary>
        /// 把有效日期是今天之前产生的箱号删除掉（弃用）
        /// </summary>
        void DeleteBox()
        {
            //using (var repository = new PvWmsRepository())
            //{
            //    var now = DateTime.Now.Date;
            //    Layers.Data.Sqls.PvWms.TBoxes sqlb;
            //    //Layers.Data.Sqls.PvWms.Pickings sqlp;

            //    //废弃
            //    //                repository.Command(string.Format($@"delete from {nameof(Layers.Data.Sqls.PvWms.TBoxes)}  where {nameof(sqlb.CreateDate)} 
            //    //between '{{0}}' and  '{{1}}' and [ID] in  (SELECT distinct [BoxCode] FROM {nameof(Layers.Data.Sqls.PvWms.Pickings)}   where {nameof(sqlp.CreateDate)}  between '{{0}}' and  '{{1}}') ", now.AddDays(-1).ToString("yyyy-MM-dd"), now.ToString("yyyy-MM-dd")));

            //    //箱号过了有效期限就删除掉（不用考虑Pickings）
            //    //repository.Command(string.Format($@"delete from {nameof(Layers.Data.Sqls.PvWms.TBoxes)}  where {nameof(sqlb.EffectiveDate)} <'{{0}}' ", now.ToString("yyyy-MM-dd")));
            //}
        }

        static CgBoxManage current;
        static object locker = new object();

        /// <summary>
        /// 全局实例
        /// </summary>
        static public CgBoxManage Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new CgBoxManage();
                        }
                    }
                }
                return current;
            }
        }
    }

    /// <summary>
    /// 临时箱号
    /// </summary>
    public class tBoxCode
    {
        public string Code { get; set; }

        public DateTime Date { get; set; }

        /// <summary>
        /// 重写Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (this.Date.Ticks + this.Code).GetHashCode();
        }

        /// <summary>
        /// 重写判断
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is tBoxCode)
            {
                return this.GetHashCode() == obj.GetHashCode();
            }

            return false;
        }

    }

    /// <summary>
    /// 私有箱号设计
    /// </summary>
    public class MyBox
    {
        /// <summary>
        /// 箱号  
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        /// <remarks>
        /// 同时可以理解为分类
        /// </remarks>
        public string Prex { get; set; }

        /// <summary>
        /// 有效日期
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Packer { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public string PackageType { get; set; }
    }
}

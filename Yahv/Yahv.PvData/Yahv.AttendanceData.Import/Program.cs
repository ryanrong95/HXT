using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yahv.AttendanceData.Import.Extends;
using Yahv.AttendanceData.Import.ServiceReference1;
using Yahv.AttendanceData.Import.Services;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.AttendanceData.Import
{
    class Program
    {
        static void Main(string[] args)
        {
            //ErmDataService service = new ErmDataService();
            //service.DataImport();
            //var date = DateTime.Parse($"{2019}-{11}-{10}").Date;
            //var bl = DataManager.Current.BreastfeedingLeaves.SingleOrDefault(item => item.Name == "赖翠红" && item.StartDate <= date && date <= item.EndDate);

            //var date1 = DateTime.Parse("2020-01-01");
            //var date2 = DateTime.Parse("2020-01-01").AddHours(11);
            //var date3 = DateTime.Parse("2020-01-01").AddHours(12);
            //var date4 = DateTime.Parse("2020-01-01").AddHours(13);
            //if (date1 < date2)
            //{
            //    Console.WriteLine(date1);
            //    Console.WriteLine(date2);
            //}

            var voteFlows = DataManager.Current.VoteFlows;
            var xdtStaffs = DataManager.Current.XdtStaffs.ToArray();
            var schedulings = DataManager.Current.Schedulings;
            var leaves = DataManager.Current.BreastfeedingLeaves;

            //new Services.AttendanceService2().Read(2019, 12).Encapsule().Enter();

            //TestRead();
            //TestInsert();
            //DataImport();
            //TestRandom();
            //TestRegex();
            //TestDyjService();
            //TestTimeSpan();

            //DataImport();
            //new Services.AttendanceService2().Read(2020, 5).Encapsule().Enter();
            new Services.VacationService().Read().Encapsule().Enter();

            Console.ReadKey();
        }

        static void DataImport()
        {
            //审批流基础数据导入
            //new Services.VoteFlowService().Encapsule().Enter();

            //按月导入
            //DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"));
            //foreach (var fileInfo in dirInfo.GetFiles())
            //{
            //    if (fileInfo.Length == 0)
            //        continue;

            //    new Services.AttendanceService().Read(fileInfo.FullName).Encapsule().Enter();
            //}

            //2019年考勤数据
            for (int i = 5; i <= 12; i++)
            {
                new Services.AttendanceService2().Read(2019, i).Encapsule().Enter();
            }
            //2020年考勤数据
            for (int i = 1; i <= 4; i++)
            {
                new Services.AttendanceService2().Read(2020, i).Encapsule().Enter();
            }

            //员工假期数据维护
            //new Services.VacationService().Read().Encapsule().Enter();
        }

        #region 测试方法

        static void TestRead()
        {
            int year = 2020;
            int month = 1;
            string _FILE = @"D:\公司业务\人事考勤\报关部2016年以来考勤汇总\2020年\2020-1.csv";

            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(@"(.+?)\[(.+?):(.+?)-(.+?):(.+?)\]", options);

            using (StreamReader sr = new StreamReader(_FILE, Encoding.Default))
            {
                int lineIndex = 0;
                string headerLine = sr.ReadLine();

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    lineIndex++;

                    var datas = line.Split(',');

                    Console.WriteLine($"UserID: {datas[0].Replace("\"", "").Trim()} Name: {datas[1].Replace("\"", "").Trim()}");

                    int days = DateTime.DaysInMonth(year, month);
                    for (int i = 1; i <= days; i++)
                    {
                        string desc = new DateTime(year, month, i).DayOfWeek.GetDescription();
                        var data = datas[i + 1].Replace("\"", "").Trim();
                        if (!string.IsNullOrEmpty(data))
                        {
                            if (data.Contains("["))
                            {
                                var groups = regex.Match(data).Groups;
                                var date_AM = new DateTime(year, month, i, int.Parse(groups[2].Value), int.Parse(groups[3].Value), 0);
                                var date_PM = new DateTime(year, month, i, int.Parse(groups[4].Value), int.Parse(groups[5].Value), 0);
                                Console.WriteLine($"    [{i}号], [{desc}], [{groups[1].Value}], [{date_AM}], [{date_PM}]");
                            }
                            else
                            {
                                Console.WriteLine($"    [{i}号], [{desc}], [{data}]");
                            }
                        }
                    }

                    Console.WriteLine("============================================");
                }
            }
        }

        static void TestInsert()
        {
            var logsAttend = new List<Layers.Data.Sqls.PvbErm.Logs_Attend>();
            logsAttend.Add(new Layers.Data.Sqls.PvbErm.Logs_Attend()
            {
                ID = Layers.Data.PKeySigner.Pick(PKeyType.AttendLog),
                Date = DateTime.Parse($"{2020}-{4}-{1}").Date,
                StaffID = "NPC",
                CreateDate = DateTime.Now,
            });
            logsAttend.Add(new Layers.Data.Sqls.PvbErm.Logs_Attend()
            {
                ID = Layers.Data.PKeySigner.Pick(PKeyType.AttendLog),
                Date = DateTime.Parse($"{2020}-{4}-{2}").Date,
                StaffID = "NPC",
                CreateDate = DateTime.Now,
            });

            using (var conn = ConnManager.Current.PvbErm)
            {
                conn.BulkInsert(logsAttend);
            }
        }

        static void TestRandom()
        {
            for (int i = 0; i < 100; i++)
            {
                DateTime amStartDate = Utils.DateUtil.GetAmStartTime(2020, 5, 12);
                DateTime amEndDate = Utils.DateUtil.GetAmEndTime(2020, 5, 12);
                DateTime pmStartDate = Utils.DateUtil.GetPmEndTime(2020, 5, 12);
                DateTime pmEndDate = Utils.DateUtil.GetPmEndTime(2020, 5, 12);

                Console.WriteLine("上午上班打卡时间：" + amStartDate);
                Console.WriteLine("上午下班打卡时间：" + amEndDate);
                Console.WriteLine("下午上班打卡时间：" + pmStartDate);
                Console.WriteLine("下午下班打卡时间：" + pmEndDate);

                System.Threading.Thread.Sleep(10);
            }
        }

        static void TestRegex()
        {
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            Regex regex = new Regex(@"(.+?)\[(.+?):(.+?)-(.+?):(.+?)\]\[(.+?)\]", options);

            string a = "迟到早退[12:51-15:51]";
            string b = "迟到早退[12:51-15:51][B班]";
            string c = "迟到早退[12:51-15:51][C班]";

            if (a.Contains("["))
            {
                if (!a.Contains("班"))
                {
                    a += "[A班]";
                }
            }
            var groups1 = regex.Match(a).Groups;
            string value1 = groups1[6].Value;
            var groups2 = regex.Match(b).Groups;
            string value2 = groups2[6].Value;
            var groups3 = regex.Match(c).Groups;
            string value3 = groups3[6].Value;
        }

        static void TestDyjService()
        {
            RSServerClient client = new RSServerClient();
            var r = client.GetWorkDate("3873", DateTime.Parse("2019-11-01"), DateTime.Parse("2019-11-30")).JsonTo<Models.DyjResponse<Models.DyjModel>>();
            var workDate = r.data;

            var a = client.GetQingJiaList("3873", DateTime.Parse("2019-11-01"), DateTime.Parse("2019-11-30")).JsonTo<Models.DyjResponse<Models.QingJia>>();
            var qingjia = a.data;
        }

        static void TestTimeSpan()
        {
            Layers.Data.Sqls.PvbErm.Logs_Attend log1;
            Layers.Data.Sqls.PvbErm.Logs_Attend log2;
            Layers.Data.Sqls.PvbErm.Schedulings s;

            using (PvbErmReponsitory reponsitory = new PvbErmReponsitory())
            {
                log1 = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_Attend>().First(item => item.ID == "AttendLog2020043000774");
                log2 = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Logs_Attend>().First(item => item.ID == "AttendLog2020043000775");
                s = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Schedulings>().First(item => item.ID == "Scheing00001");
            }

            TimeSpan start = s.AmStartTime.Value;

            DateTime startTime = new DateTime(2019, 5, 31, start.Hours, start.Minutes, start.Seconds);

            DateTime date1 = log1.CreateDate;
            TimeSpan log1Date = new TimeSpan(date1.Hour, date1.Minute, date1.Second);
            int value1 = log1Date.CompareTo(start);

            DateTime date2 = log2.CreateDate;
            TimeSpan log2Date = new TimeSpan(date2.Hour, date2.Minute, date2.Second);
            int value2 = log2Date.CompareTo(start);

            TimeSpan log3Date = new TimeSpan(9, 10, 0);
            int value3 = log3Date.CompareTo(start);
        }

        #endregion
    }
}

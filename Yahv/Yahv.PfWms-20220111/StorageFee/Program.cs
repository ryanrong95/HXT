using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Payments;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace StorageFee
{
    class Program
    {

        static void Main(string[] args)
        {
            ConsoleApp.vTaskers.Services.WhRecords.Current.SqlError += Current_SqlError; ;
            ConsoleApp.vTaskers.Services.WhRecords.Current.Start();


        }

        private static void Current_SqlError(object sender, EventArgs e)
        {
            Console.WriteLine(sender.ToString());
        }
    }

    #region 张晓辉以前所写

    //static void Main(string[] args)
    //{
    //    #region 张晓辉以前所写
    //    var shelves = new List<Shelves>();
    //    using (var rep = new Layers.Data.Sqls.PvWmsRepository())
    //    {

    //        //日志文件处理
    //        var dir = Path.Combine(AppContext.BaseDirectory, "log");
    //        if (!Directory.Exists(dir))
    //        {
    //            Directory.CreateDirectory(dir);
    //        }

    //        var errordir = Path.Combine(AppContext.BaseDirectory, "errorlog");
    //        if (!Directory.Exists(errordir))
    //        {
    //            Directory.CreateDirectory(errordir);
    //        }
    //        //清除10天之前的日志
    //        var delFiles = Directory.GetFiles(dir).Where(item => DateTime.Parse(new FileInfo(item).CreationTime.AddDays(10).ToString("yyyy-MM-dd")) < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd")));
    //        foreach (var item in delFiles)
    //        {
    //            File.Delete(item);
    //        }


    //        var logs = new SortedSet<string>();
    //        var logfile = Path.Combine(dir, DateTime.Now.ToString("yyyyMMdd") + ".log");
    //        if (File.Exists(logfile))
    //        {
    //            logs = File.ReadAllText(logfile).JsonTo<SortedSet<string>>();
    //        }

    //        var errorlogs = new SortedSet<string>();
    //        var errorlogfile = Path.Combine(dir, DateTime.Now.ToString("error_yyyyMMdd") + ".log");
    //        if (File.Exists(errorlogfile))
    //        {
    //            errorlogs = File.ReadAllText(errorlogfile).JsonTo<SortedSet<string>>();
    //        }
    //        shelves.AddRange(rep.ReadTable<Layers.Data.Sqls.PvWms.Shelves>().Where(item => item.Status == (int)ShelvesStatus.Normal).Select(item => new Shelves { ID = item.ID, LeaseID = item.LeaseID }).ToList());

    //        foreach (var item in shelves)
    //        {

    //            try
    //            {
    //                //取到所有的库位
    //                var storages = rep.ReadTable<Layers.Data.Sqls.PvWms.Storages>().Where(tem => tem.ShelveID == item.ID).Select(tem => new Storage { ID = tem.ID, InputID = tem.InputID }).ToList();
    //                foreach (var storage in storages)
    //                {
    //                    string rbId = null;
    //                    //库位上的库存数
    //                    var quantity = decimal.Parse("0");
    //                    var forms = rep.ReadTable<Layers.Data.Sqls.PvWms.Forms>().Where(tem => tem.StorageID == storage.ID && tem.Status == (int)FormStatus.Facted);
    //                    if (forms.Count() > 0)
    //                    {
    //                        quantity = forms.Sum(tem => tem == null ? 0 : tem.Quantity);
    //                    }

    //                    var inputClientID = "";
    //                    if (!string.IsNullOrEmpty(item.LeaseID) || quantity > 0)
    //                    {

    //                        //加仓储费

    //                        var inputs = rep.ReadTable<Layers.Data.Sqls.PvWms.Inputs>().Where(tem => tem.ID == storage.InputID).ToList();
    //                        if (inputs.Count > 0)
    //                        {
    //                            inputClientID = inputs.FirstOrDefault().ClientID;
    //                            if (!string.IsNullOrEmpty(inputClientID))
    //                            {
    //                                try
    //                                {
    //                                    var logstr = inputClientID + item.ID;
    //                                    if (!logs.Contains(logstr))
    //                                    {
    //                                        //仓储费 记录
    //                                        rbId = PaymentManager.Npc[inputClientID, "深圳市华芯通供应链管理有限公司"]["代仓储"]
    //                                             .Receivable["杂费", "仓储费"].RecordStorage(Currency.CNY, 20);
    //                                        logs.Add(logstr);
    //                                        File.WriteAllText(logfile, logs.Json());

    //                                    }
    //                                }
    //                                catch (Exception ex)
    //                                {
    //                                    var errstr = string.Join("**error**", inputClientID, item.ID);
    //                                    if (!errorlogs.Contains(errstr))
    //                                    {
    //                                        errorlogs.Add(errstr);
    //                                        File.WriteAllText(errorlogfile, errorlogs.Json());
    //                                    }
    //                                }

    //                                if (!string.IsNullOrEmpty(item.LeaseID))
    //                                {
    //                                    //减免仓储费
    //                                    var clientid = rep.ReadTable<Layers.Data.Sqls.PvWms.LsNotice>().AsEnumerable().Where(tem => tem.ID == item.LeaseID && DateTime.Parse(tem.EndDate.ToString("yyy-MM-dd")) >= DateTime.Parse(DateTime.Now.ToString("yyy-MM-dd"))).FirstOrDefault().ClientID;
    //                                    //占用库位的客户和租赁客户是同一客户时才进行减免
    //                                    if (!string.IsNullOrEmpty(clientid) && inputClientID == clientid)
    //                                    {
    //                                        try
    //                                        {
    //                                            var logstr = clientid + item.ID;
    //                                            if (!logs.Contains(logstr))
    //                                            {
    //                                                //仓储费 减免
    //                                                PaymentManager.Npc.Received.For(rbId).Reduction(Currency.CNY, 20);
    //                                                logs.Add(logstr);
    //                                                File.WriteAllText(logfile, logs.Json());
    //                                            }
    //                                        }
    //                                        catch (Exception ex)
    //                                        {
    //                                            var errstr = string.Join("**error**", clientid, item.ID);
    //                                            if (!errorlogs.Contains(errstr))
    //                                            {
    //                                                errorlogs.Add(errstr);
    //                                                File.WriteAllText(errorlogfile, errorlogs.Json());
    //                                            }
    //                                        }
    //                                    }

    //                                }
    //                            }
    //                        }
    //                    }

    //                }

    //                Console.WriteLine(string.Concat(item.ID, " 完成！", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

    //            }
    //            catch (Exception ex)
    //            {
    //                var mes = ex.Message;
    //            }
    //        }



    //    }
    //    Console.WriteLine(string.Concat(" 全部完成！", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
    //    System.Diagnostics.Process.GetCurrentProcess().Kill();

    //    #endregion
    //}

    //public class Shelves
    //{
    //    public string ID { get; set; }
    //    public string LeaseID { get; set; }
    //}

    //public class Storage
    //{
    //    public string ID { get; set; }
    //    public string InputID { get; set; }
    //}

    #endregion

}

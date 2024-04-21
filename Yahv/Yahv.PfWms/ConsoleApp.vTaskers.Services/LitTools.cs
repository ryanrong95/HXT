using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.vTaskers.Services
{
    /// <summary>
    /// 临时使用记录日志
    /// </summary>
    public class LitTools
    {
        static object llocker = new object();

        LitTools() : this("unions")
        {

        }

        string name;
        LitTools(string name)
        {

            this.name = name;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="lines"></param>
        public void Log(params string[] lines)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.name + ".txt");
            lock (llocker)
            {
                File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"), Encoding.UTF8);
                File.AppendAllLines(path, lines.Select(item => item + "\r\n"), Encoding.UTF8);
                File.AppendAllText(path, "\r\n");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LitTools this[string index]
        {
            get { return new LitTools(index); }
        }

        static LitTools current;
        static object locker = new object();

        /// <summary>
        /// 访问单例
        /// </summary>
        static public LitTools Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new LitTools();
                        }
                    }
                }
                return current;
            }
        }


        /// <summary>
        /// 记录数据库执行日志
        /// </summary>
        /// <param name="mainID">主要ID</param>
        /// <param name="json"></param>
        public void Record(string mainID, string json = null, string name = "")
        {
            this.records(mainID, json, Logs_TaskPoolStatus.Done, name: name);
        }

        /// <summary>
        /// 记录数据库失败日志
        /// </summary>
        /// <param name="mainID">主要ID</param>
        /// <param name="json"></param>
        public void RecordFailed(string mainID, string json = null)
        {
            this.records(mainID, json, Logs_TaskPoolStatus.Failed);

            // 当单一窗口申报深圳出库任务执行失败后自动插入一条新的任务到TasksPool中
            string nameNew = "单一窗口申报后深圳出库";
            using (var reponsitory = new PvCenterReponsitory())
            {
                string id = Layers.Data.PKeySigner.Pick(Wms.Services.PkeyType.TasksPool);

                // 当任务失败后才重新插入回TaskPool中，再次执行一遍
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.TasksPool
                {
                    Context = json,
                    CreateDate = DateTime.Now,
                    ID = id,
                    MainID = mainID,
                    Name = nameNew,
                });
            }

        }

        /// <summary>
        /// 记录数据库日志
        /// </summary>
        /// <param name="mainID">主要ID</param>
        /// <param name="json"></param>
        void records(string mainID, string json, Logs_TaskPoolStatus status, string name = "单一窗口申报后深圳出库")
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                string id = Layers.Data.PKeySigner.Pick(Wms.Services.PkeyType.TasksPool);
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_TaskPool
                {
                    Context = json,
                    Status = (int)status,
                    CreateDate = DateTime.Now,
                    ID = id,
                    MainID = mainID,
                    Name = name,
                });                
            }
        }
    }

    /// <summary>
    /// 任务池日志状态
    /// </summary>
    public enum Logs_TaskPoolStatus
    {
        /// <summary>
        /// 程序执行了
        /// </summary>
        Done = 100,
        /// <summary>
        /// 成功的是不用关注的
        /// </summary>
        [Obsolete("暂时用不到")]
        Succes = 200,
        /// <summary>
        /// 失败是需要关注的
        /// </summary>
        Failed = 500,
    }
}

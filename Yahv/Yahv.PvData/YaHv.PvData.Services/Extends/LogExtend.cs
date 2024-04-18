using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Extends
{
    public static class LogExtend
    {
        /// <summary>
        /// 归类操作日志
        /// </summary>
        /// <param name="classifyProduct">归类产品</param>
        /// <param name="logType">日志类型：归类、锁定</param>
        /// <param name="summary">日志内容</param>
        /// <param name="reponsitory"></param> 
        public static void Log(this Interfaces.IClassifyProduct classifyProduct, LogType logType, string summary, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifyOperatingLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifyOperating()
            {
                ID = id,
                MainID = classifyProduct.ID,
                CreatorID = classifyProduct.CreatorID,
                LogType = (int)logType,
                CreateDate = DateTime.Now,
                Summary = summary
            });
        }

        /// <summary>
        /// 归类变更日志
        /// </summary>
        /// <param name="orderedProduct">归类产品</param>
        /// <param name="summary">日志内容</param>
        /// <param name="reponsitory"></param>
        public static void Log(this Models.OrderedProduct orderedProduct, string summary, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifyModifiedLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifyModified()
            {
                ID = id,
                PartNumber = orderedProduct.PartNumber,
                Manufacturer = orderedProduct.Manufacturer,
                CreatorID = orderedProduct.CreatorID,
                CreateDate = DateTime.Now,
                Summary = summary
            });
        }

        /// <summary>
        /// 预归类变更日志
        /// </summary>
        /// <param name="preProduct">预归类产品</param>
        /// <param name="summary">日志内容</param>
        /// <param name="reponsitory"></param>
        public static void Log(this Models.PreProduct preProduct, string summary, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifyModifiedLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ClassifyModified()
            {
                ID = id,
                PartNumber = preProduct.PartNumber,
                Manufacturer = preProduct.Manufacturer,
                CreatorID = preProduct.CreatorID,
                CreateDate = DateTime.Now,
                Summary = summary
            });
        }

        /// <summary>
        /// 归类历史数据变更日志
        /// </summary>
        /// <param name="classifiedHistory">归类历史数据</param>
        /// <param name="summary">日志内容</param>
        /// <param name="reponsitory"></param>
        public static void Log(this Models.ClassifiedHistory classifiedHistory, string summary, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ClassifiedModifiedPast);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Pasts_ClassifiedModified()
            {
                ID = id,
                PartNumber = classifiedHistory.PartNumber,
                Manufacturer = classifiedHistory.Manufacturer,
                CreatorID = classifiedHistory.CreatorID,
                CreateDate = DateTime.Now,
                Summary = summary
            });
        }

        /// <summary>
        /// 申报要素品牌操作日志
        /// </summary>
        /// <param name="em">申报要素品牌</param>
        /// <param name="from">变更前的中文或外文名称</param>
        /// <param name="to">变更后的中文或外文名称</param>
        /// <param name="summary"></param>
        /// <param name="reponsitory"></param>
        public static void Log(this Models.ElementsManufacturer em, string from , string to, string summary, PvDataReponsitory reponsitory)
        {
            string id = Layers.Data.PKeySigner.Pick(PKeyType.ElementsManufacturerLog);
            reponsitory.Insert(new Layers.Data.Sqls.PvData.Logs_ElementsManufacturers()
            {
                ID = id,
                Manufacturer = em.Manufacturer,
                From = from,
                To = to,
                CreatorID = em.Admin.ID,
                CreateDate = DateTime.Now,
                Summary = summary
            });
        }
    }
}

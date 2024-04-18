using Needs.Model;
using Needs.Wl.Logs.Services.Model;
using System;

namespace Needs.Wl.Logs.Services
{
    public static class LogExtends
    {
        /// <summary>
        /// 对象日志记录
        /// </summary>
        /// <param name="modle">需要记录操作日志的对象</param>
        /// <param name="adminID">操作人ID,可以是管理员ID，UserID</param>
        /// <param name="summery">操作内容</param>
        public static void Log(this IModel modle, string adminID, string summery)
        {
            modle.Log(adminID, summery, "");
        }

        /// <summary>
        /// 对象日志记录
        /// </summary>
        /// <param name="modle">需要记录操作日志的对象</param>
        /// <param name="adminID">操作人ID,可以是管理员ID，UserID</param>
        /// <param name="summery">操作内容</param>
        /// <param name="dataJson">对象Json数据</param>
        public static void Log(this IModel modle, string adminID, string summery, string dataJson)
        {
            TableAttribute tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(modle.GetType(), typeof(TableAttribute));
            if (tableAttribute == null)
            {
                throw new Exception("未设置对象的日志名称");
            }

            Log log = new Log
            {
                Name = tableAttribute.Name,
                MainID = modle.ID,
                AdminID = adminID,
                Summary = summery,
                CreateDate = DateTime.Now,
                Json = dataJson
            };

            log.Enter();
        }
    }
}
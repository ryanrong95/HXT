using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// Api接口返回数据定义
    /// </summary>
    public class ApiResultModel
    {
        #region 公共模型

        /// <summary>
        /// 基础类
        /// </summary>
        public class BaseMeg
        {
            /// <summary>
            /// 代码
            /// </summary>
            public string code { get; set; } = "0";

            /// <summary>
            /// 描述
            /// </summary>
            public string desc { get; set; } = "操作成功";

            /// <summary>
            /// 系统内部错误描述
            /// </summary>
            public string exdesc { get; set; } = "";

        }

        /// <summary>
        /// 公共模型
        /// </summary>
        public class CommonStringModel : BaseMeg
        {
            /// <summary>
            /// 返回信息
            /// </summary>
            public string data { get; set; } = "";
        }

        /// <summary>
        /// 公共模型(含有 T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class CommonClassModel<T> : BaseMeg
        {
            /// <summary>
            /// 指定类信息
            /// </summary>
            public T data { get; set; }
        }

        /// <summary>
        /// 公共类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class CommonList<T>
        {
            /// <summary>
            /// 指定类信息
            /// </summary>
            public List<T> items { get; set; } = new List<T>();
        }
      
        #endregion

        #region 公共方法
        /// <summary>
        ///返回HttpResponseMessage
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static HttpResponseMessage OutputResult(object obj)
        {
            string strJson = JsonConvert.SerializeObject(obj);
            return new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
        }
        #endregion

    }
}

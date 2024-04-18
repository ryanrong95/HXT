using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    
    public class ItemsItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// <em>北京</em>大学
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string estiblishTime { get; set; }
        /// <summary>
        /// 郝平
        /// </summary>
        public string legalPersonName { get; set; }
    }

    public class ResultItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int num { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ItemsItem> items { get; set; }
    }

    public class ResultMsg
    {
        /// <summary>
        /// 
        /// </summary>
        public int error_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResultItem result { get; set; }
    }

    public class SearchByKeywordModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string charge { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remain { get; set; }
        /// <summary>
        /// 查询成功
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ResultMsg result { get; set; }
    }
}

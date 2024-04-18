using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Classify
{
    /// <summary>
    /// 当前查询条件
    /// </summary>
    public class CurrentSc
    {
        /// <summary>
        /// 初始 Url
        /// </summary>
        public string InitUrl { get; set; } = string.Empty;

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 页面尺寸
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否显示被他人锁定的
        /// </summary>
        public bool IsShowLocked { get; set; } = false;

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 品名
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// 海关编号
        /// </summary>
        public string HSCode { get; set; } = string.Empty;

        /// <summary>
        /// 上一次归类开始时间
        /// </summary>
        public string LastClassifyTimeBegin { get; set; } = string.Empty;

        /// <summary>
        /// 上一次归类结束时间
        /// </summary>
        public string LastClassifyTimeEnd { get; set; } = string.Empty;

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; } = string.Empty;

        /// <summary>
        /// 产品变更添加日期 Begin
        /// </summary>
        public string ProductChangeAddTimeBegin { get; set; } = string.Empty;

        /// <summary>
        /// 产品变更添加日期 End
        /// </summary>
        public string ProductChangeAddTimeEnd { get; set; } = string.Empty;
    }
}
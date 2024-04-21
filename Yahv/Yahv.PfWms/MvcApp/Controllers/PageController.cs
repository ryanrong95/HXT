using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApp.Controllers
{
    public class PageController : Controller
    {
        /// <summary>
        /// 返回分页数据
        /// </summary>
        public class ResponsePageList<T>
        {

            public ResponsePageList()
            {
            }

            public ResponsePageList(IEnumerable<T> source, int pageindex, int pagesize)
            {
                if (source != null)
                {
                    this.Total = source.Count();
                    this.PageSize = pagesize;
                    this.PageIndex = pageindex;
                    this.Data = source.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                }
            }
            public ResponsePageList(IQueryable<T> source, int pageindex, int pagesize)
            {
                if (source != null)
                {
                    this.Total = source.Count();
                    this.PageSize = pagesize;
                    this.PageIndex = pageindex;
                    this.Data = source.Skip((pageindex - 1) * pagesize).Take(pagesize).ToList();
                }
            }

            int pageindex;
            /// <summary>
            /// 页码
            /// </summary>
            public int PageIndex
            {
                get
                {
                    if (this.pageindex < 1)
                    {
                        this.pageindex = 1;
                    }
                    else if (this.pageindex > this.Pages)
                    {
                        this.pageindex = this.Pages;
                    }
                    return pageindex;
                }
                set
                {
                    this.pageindex = value;
                }
            }
            /// <summary>
            /// 每页条数
            /// </summary>
            public int PageSize { get; set; } = 10;
            /// <summary>
            /// 总条数
            /// </summary>
            public int Total { get; set; }

            /// <summary>
            /// 总页数
            /// </summary>
            public int Pages
            {
                get
                {
                    return (int)Math.Ceiling((decimal)this.Total / this.PageSize);
                    //return this.Total % this.PageSize == 0 ? this.Total / this.PageSize : (this.Total / this.PageSize) + 1;
                }
            }
            /// <summary>
            /// 数据
            /// </summary>
            public List<T> Data { get; set; }

        }
    }
}
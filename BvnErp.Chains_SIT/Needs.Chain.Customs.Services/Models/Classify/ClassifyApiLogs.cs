using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 归类接口调用日志
    /// </summary>
    public class ClassifyApiLogs : IUnique
    {
        #region

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// OrderItemID或PreProduct
        /// </summary>
        public string ClassifyProductID { get; set; }

        /// <summary>
        /// 接口地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        public string RequestContent { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        public string ResponseContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 摘要备注
        /// </summary>
        public string Summary {get; set;}

        #endregion

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClassifyApiLogs()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ClassifyProductID = this.ClassifyProductID,
                    Url = this.Url,
                    RequestContent = this.RequestContent,
                    ResponseContent = this.ResponseContent,
                    CreateDate = DateTime.Now,
                    Summary = this.Summary
                });
            }
        }
    }
}

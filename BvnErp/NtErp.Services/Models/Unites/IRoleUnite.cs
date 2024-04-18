using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 颗粒化接口
    /// </summary>
    public interface IRoleUnite : Needs.Linq.IUnique, Needs.Linq.IPersistence, Needs.Linq.IFulError, Needs.Linq.IFulSuccess
    {
        /// <summary>
        /// 配置类型
        /// </summary>
        RoleUniteType Type { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        string Menu { get; set; }
       
        /// <summary>
        /// 标签Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// /// <summary>
        /// 标签Title
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 当前页面url
        /// </summary>
        string Url { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime CreateDate { get; set; }
    }


}

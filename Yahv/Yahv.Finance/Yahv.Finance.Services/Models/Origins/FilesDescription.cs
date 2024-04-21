using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class FilesDescription : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        #endregion

        #region 数据库属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public FileDescType Type { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 客户自定义名称
        /// </summary>
        public string CustomName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        #endregion

        #region 其它属性

        /// <summary>
        /// FilesMapsIEnum
        /// </summary>
        public IEnumerable<FilesMap> FilesMapsIEnum { get; set; }

        /// <summary>
        /// FilesMapsArray
        /// </summary>
        public FilesMap[] FilesMapsArray { get; set; }

        #endregion


    }
}

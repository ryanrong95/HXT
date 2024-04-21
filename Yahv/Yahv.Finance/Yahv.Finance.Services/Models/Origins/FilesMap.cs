using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    public class FilesMap : IUnique
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
        /// 文件描述ID
        /// </summary>
        public string FileID { get; set; }

        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; }

        #endregion






    }
}

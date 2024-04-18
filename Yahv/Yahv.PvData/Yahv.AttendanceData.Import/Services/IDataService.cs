using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import.Services
{
    public interface IDataService
    {
        /// <summary>
        /// 数据读取
        /// </summary>
        /// <returns></returns>
        IDataService Read(string path = null);

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        IDataService Encapsule();

        /// <summary>
        /// 数据持久化
        /// </summary>
        void Enter();
    }
}

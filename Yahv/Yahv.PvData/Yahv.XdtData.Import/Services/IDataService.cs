using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Services
{
    public interface IDataService
    {
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        IDataService Query();

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        IDataService Encapsule();

        /// <summary>
        /// 数据持久化
        /// </summary>
        /// <returns></returns>
        void Enter();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.CrmPlus.Service
{
    public interface IMyCloneable : ICloneable
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCloneDb">是否同步复制数据中的数据并返回数据库对象</param>
        /// <returns></returns>
        object Clone(bool isCloneDb);

    }
}

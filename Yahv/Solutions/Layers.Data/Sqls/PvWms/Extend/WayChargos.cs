using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls.PvWms
{
    public partial class WayChargos : PropertyChanged
    {

        partial void OnCreated()
        {
            if (this.PropertyChanged == null)
            {
                lock (this)
                {
                    if (this.PropertyChanged == null)
                    {
                        this.PropertyChanged += _PropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// 要求每一个类都要保留
        /// </summary>
        partial void OnLoaded()
        {
            // 暂时保留，防止多线程🔒
        }

    }
}

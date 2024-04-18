using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layers.Data.Sqls.PvCenter
{
    public partial class WayChcd
    {
        List<string> changeds = new List<string>();

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

        private void _PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
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

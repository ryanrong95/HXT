using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls.ScCustoms
{
    public partial class CustomsOriginTariffs
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
                        this.PropertyChanged += CustomsOriginTariffs_PropertyChanged;
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

        private void CustomsOriginTariffs_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }
    }
}

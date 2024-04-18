using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layer.Data.Sqls.ScCustoms
{
    public partial class SwapNoticeItems
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
                        this.PropertyChanged += SwapNoticeItems_PropertyChanged;
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

        private void SwapNoticeItems_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            changeds.Add(e.PropertyName);
        }
    }
}

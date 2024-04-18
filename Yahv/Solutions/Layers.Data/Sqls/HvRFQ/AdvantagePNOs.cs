using System.Collections.Generic;

namespace Layers.Data.Sqls.HvRFQ
{
    /// <summary>
    /// 扩展
    /// </summary>
    /// <example>泛型Update，记录更新的字段</example>
    public partial class AdvantagePNOs
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

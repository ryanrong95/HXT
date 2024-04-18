using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Services
{
    /// <summary>
    /// 基础样式设置
    /// </summary>
    public class BaseStyleSetting
    {
        /// <summary>
        /// grid表鼠标经过的颜色
        /// </summary>
        public static Color DefaultCellStyle_BackColor = Color.FromArgb(242, 244, 252);

        /// <summary>
        /// 导航按钮点击时的颜色
        /// </summary>
        public static Color Button_Click_BackColor = Color.FromArgb(63, 161, 235);

        /// <summary>
        /// 导航按钮初始化的颜色
        /// </summary>
        public static Color Button_Init_BackColor = Color.FromArgb(243, 244, 248);
    }
}

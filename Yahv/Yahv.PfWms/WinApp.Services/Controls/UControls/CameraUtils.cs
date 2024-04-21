using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters;

namespace WinApp.Services.Controls.UControls
{
    public class CameraUtils
    {

        /// <summary>
        /// 保存bmp
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>保存的地址</returns>
        static public string SaveFile(Bitmap bmp)
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photos",
                DateTime.Now.ToString("yyyyMMdd"),
                "photo" + DateTime.Now.LinuxTicks() + ".jpg");

            FileInfo fi = new FileInfo(fileName);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            using (var writer = fi.OpenWrite())
            {
                bmp.Save(writer, ImageFormat.Jpeg);
            }

            return fileName;
        }


        /// <summary>
        /// 摄像头的个数
        /// </summary>
        /// <returns></returns>
        static public int CameraCount()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            return videoDevices.Count;
        }
    }

    public class CameraOkEventArgs : EventArgs
    {
        public string FileName { get; private set; }
        public CameraOkEventArgs(string filename)
        {
            this.FileName = filename;
        }
    }

    public delegate void CameraOkEventHandler(object sender, CameraOkEventArgs e);

}

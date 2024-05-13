using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.IO;
using System.Drawing.Imaging;

namespace WinApp.Controls
{
    public partial class UcCameras : UserControl
    {
        public UcCameras()
        {
            InitializeComponent();
        }



        private void UcCameras_Load(object sender, EventArgs e)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count <= 0)
            {
                MessageBox.Show("没有发现视频设备！");
                return;
            }

            this.cmbxCamera.Items.AddRange(videoDevices.Cast<FilterInfo>().Select(item => item.Name).ToArray());
            this.cmbxCamera.SelectedIndex = 0;

            this.FindForm().FormClosing += UcCameras_FormClosing;
        }


        private void btnPhoto_Click(object sender, EventArgs e)
        {
            //拍照
            if (!this.vspMain.IsRunning)
            {
                MessageBox.Show("未检测到运行的摄像头影像");
            }

            using (var bmp = this.vspMain.GetCurrentVideoFrame())
            {
                //CameraUtils.SaveFile(bmp);
            }
        }

        private void cmbxCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.vspMain.SignalToStop();
            this.vspMain.WaitForStop();
            CameraConn();
        }

        private void CameraConn()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[this.cmbxCamera.SelectedIndex].MonikerString);
            this.vspMain.VideoSource = videoSource;
            this.vspMain.Start();
        }

        private void vspMain_DoubleClick(object sender, EventArgs e)
        {
            this.btnPhoto_Click(sender, e);
        }

        private void UcCameras_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭摄像头
            vspMain.SignalToStop();
            vspMain.WaitForStop();
            //Environment.Exit(0);
        }
    }
}

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

namespace Yahv.PsWms.DappForm.Services.Controls.UControls
{
    public partial class UcCamera2 : UserControl
    {
        public UcCamera2()
        {
            InitializeComponent();
        }

        public event CameraOkEventHandler OnOk;

        private void UcCamera_Load(object sender, EventArgs e)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count <= 0)
            {
                MessageBox.Show("没有发现视频设备！");
                return;
            }

            this.FindForm().FormClosing += UcCameras_FormClosing;
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            var caps = videoSource.SnapshotCapabilities.Length > videoSource.VideoCapabilities.Length ?
                videoSource.SnapshotCapabilities : videoSource.VideoCapabilities;

            if (caps.Length > 0)
            {
                //屏幕分辨率
                //int sw1 = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

                //设置摄像头自身的视频分辨率是2048在1920的屏幕和1600的屏幕上都可以清晰拍照
                videoSource.VideoResolution = videoSource.SnapshotResolution = caps
                 .Where(item => item.FrameSize.Width == 2048).FirstOrDefault();

                //设置<=屏幕分辨率的 视频分辨率和快照分辨率（摄像头自身的 视频分辨率和快照分辨率）
                //videoSource.VideoResolution = videoSource.SnapshotResolution = caps
                // .Where(item => item.FrameSize.Width <= sw1)
                // .OrderBy(item => item.FrameSize.Width).Last();

                //videoSource.VideoResolution = videoSource.SnapshotResolution = caps
                //.OrderBy(item => item.FrameSize.Width).Last();
            }

            this.vspMain.VideoSource = videoSource;
            this.vspMain.Start();
        }

        private void UcCameras_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭摄像头
            this.vspMain.SignalToStop();
            this.vspMain.WaitForStop();
            //Environment.Exit(0);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.vspMain_DoubleClick(sender, e);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void vspMain_DoubleClick(object sender, EventArgs e)
        {
            if (!this.vspMain.IsRunning)
            {
                MessageBox.Show("未检测到运行的摄像头影像");
            }

            using (var bmp = this.vspMain.GetCurrentVideoFrame())
            {
                string fileName = CameraUtils.SaveFile(bmp);

                if (this != null && this.OnOk != null)
                {
                    this.OnOk(this, new CameraOkEventArgs(fileName));
                }
            }
        }
    }
}

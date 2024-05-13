using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.Underly;

namespace WinApp.Services
{
    public partial class PhotoForm : Form
    {
        FilterInfoCollection videoDevices;
        public string base64Str;
        string key;
        public PhotoForm(string value)
        {
            key = value;
            InitializeComponent();
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count <= 0)
            {
                MessageBox.Show("没有发现视频设备！");
                return;
            }
            comboBox1.Items.AddRange(videoDevices.Cast<FilterInfo>().Select(item => item.Name).ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void PhotoForm_Load(object sender, EventArgs e)
        {

        }
        private void CameraConn()
        {
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
            videoSourcePlayer1.VideoSource = videoSource;
            videoSourcePlayer1.Start();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            CameraConn();
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoSourcePlayer1.IsRunning)
                {

                    var bmp = videoSourcePlayer1.GetCurrentVideoFrame();

                    var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormat.Jpeg);
                    byte[] bt = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bt, 0, bt.Length);

                    base64Str = "data:image/jpeg;base64," + Convert.ToBase64String(bt);

                    pictureBox1.Image = Image.FromStream(stream);
                    stream.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("摄像头异常：" + ex.Message);
            }
        }

        private void PhotoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭摄像头
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            System.Environment.Exit(0);
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            videoSourcePlayer1.SignalToStop();
            videoSourcePlayer1.WaitForStop();
            CameraConn();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            #region 实时上传，保存本地路径和服务器路径

            //Image data = pictureBox1.Image;
            //if (data == null)
            //{
            //    return;
            //}

            //try
            //{
            //    var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo"); ;
            //    if (!Directory.Exists(filepath))
            //    {
            //        Directory.CreateDirectory(filepath);
            //    }

            //    Bitmap bmp = new Bitmap(data);

            //    var photopath = Path.Combine(filepath + "/" + Guid.NewGuid().ToString() + ".jpg");
            //    bmp.Save(photopath, ImageFormat.Jpeg);

            //    var url = $"{WebApp.Services.FromType.Scheme.GetDescription()}://{WebApp.Services.FromType.WebApi.GetDescription()}/FileUpload?key=" + key;

            //    using (WebClient clinet = new WebClient())
            //    {
            //        clinet.UploadFile(url, "POST", photopath);
            //    }

            //    //服务器路径保存后及时删除本地路径
            //    if (File.Exists(photopath))
            //    {
            //        File.Delete(photopath);
            //    }

            //}
            //catch
            //{
            //    MessageBox.Show("拍照异常");
            //}

            #endregion

            #region 返回base64字符串给前台
            //((HomePage)Application.OpenForms["HomePage"]).Base64Str = base64Str;
            //GeckoHelper.Base64Str = string.Concat($"var {key}=", base64Str);
            //GeckoHelper.PhotoUploadPostBack();

            this.Close();
            #endregion 
        }
    }
}

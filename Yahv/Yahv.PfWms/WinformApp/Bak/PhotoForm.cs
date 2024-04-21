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
using WinApp.Services;
using Yahv.Underly;

namespace WinApp
{

    /// <summary>
    /// 拍照模板化开发
    /// </summary>
    public partial class PhotoForm : Form
    {
        public PhotoForm()
        {
            InitializeComponent();

        }

        private void PhotoForm_Load(object sender, EventArgs e)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count <= 0)
            {
                MessageBox.Show("没有发现视频设备！");
                return;
            }

            this.comboBox1.Items.AddRange(videoDevices.Cast<FilterInfo>().Select(item => item.Name).ToArray());
            this.comboBox1.SelectedIndex = 0;
        }

        private void CameraConn()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
            vspleft.VideoSource = videoSource;
            vspleft.Start();
        }

        //连接
        private void button1_Click(object sender, EventArgs e)
        {
            vspleft.SignalToStop();
            vspleft.WaitForStop();
            CameraConn();
        }

        //拍照
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (vspleft.IsRunning)
                {
                    var bmp = vspleft.GetCurrentVideoFrame();

 

                    var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormat.Jpeg);

                    byte[] bt = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bt, 0, bt.Length);

                    //base64Str = "data:image/jpeg;base64,"+Convert.ToBase64String(bt);     

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
            vspleft.SignalToStop();
            vspleft.WaitForStop();
            System.Environment.Exit(0);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            vspleft.SignalToStop();
            vspleft.WaitForStop();
            CameraConn();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            #region 实时上传，保存本地路径和服务器路径

            Image data = pictureBox1.Image;
            if (data == null)
            {
                return;
            }

            try
            {
                var filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo"); ;
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                Bitmap bmp = new Bitmap(data);

                var photopath = Path.Combine(filepath + "/" + Guid.NewGuid().ToString() + ".jpg");
                bmp.Save(photopath, ImageFormat.Jpeg);

                var url = $"{Config.SchemeName}://{Config.DomainName}/FileUpload?key=";

                using (WebClient clinet = new WebClient())
                {
                    clinet.UploadFile(url, "POST", photopath);
                }

                //服务器路径保存后及时删除本地路径
                if (File.Exists(photopath))
                {
                    File.Delete(photopath);
                }

            }
            catch
            {
                MessageBox.Show("拍照异常");
            }

            #endregion

            #region 返回base64字符串给前台
            //((Main)Application.OpenForms["Main"]).Base64Str = base64Str;
            //this.Close();
            #endregion 
        }

        //获得路径
        private string GetImagePath()
        {
            string personImgPath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)
                         + Path.DirectorySeparatorChar.ToString() + "PersonImg";
            if (!Directory.Exists(personImgPath))
            {
                Directory.CreateDirectory(personImgPath);
            }

            return personImgPath;
        }

        private void vspleft_DoubleClick(object sender, EventArgs e)
        {

        }
    }
}

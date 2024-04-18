using AForge.Video.DirectShow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.PsWms.DappForm.Services.Controls.UControls;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services.Controls
{
    public partial class PhotoPage2 : Form
    {
        /*
         业务逻辑：
         1.拍张图片在FlowLayoutPanel（流式布局面板）里显示出来，拍几张显示几张；
         2.在图片右上方角落里加一个X号做删除功能；
         3.显示已经存在的图片
             */

        public PhotoPage2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 用双缓冲绘制窗口的所有子控件，解决窗体加载慢、卡顿的问题
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }

        List<FileResult> fileResults = new List<FileResult>();

        private void PhotoPage2_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            //线程异常
            //throw new Exception("6");
#if !DEBUG
            this.TopMost = true;
#endif

            int count = CameraUtils.CameraCount();

            //this.Controls.Clear();

            if (count >= 1)
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

            if (count == 0)
            {
                MessageBox.Show("未检测到运行的摄像设备");
            }
        }

        private void UcCameras_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭摄像头
            this.vspMain.SignalToStop();
            this.vspMain.WaitForStop();
            //Environment.Exit(0);
        }

        PhotoMap map;
        float height;

        public void SetUploadParams(Services.PhotoMap map)
        {
            //隐藏掉第三行或者显示第三行
            var table = this.tableLayoutPanel1;
            this.height = table.Height * 0.2f;

            this.map = map;

            var mainID = map.Data.MainID;
            string url = Config.ApiUrlPrex + "/Files/Show";
            JObject jObject = new JObject();
            jObject.Add("MainID", mainID);
            var message = Yahv.Utils.Http.ApiHelper.Current.JPost(url, jObject);
            JObject obj = message.JsonTo<JObject>();
            var data = obj["data"].ToObject<PcFile[]>();

            fileResults.AddRange(data.Select(item => new FileResult
            {
                ID = item.ID,
                Url = item.Url,
                CustomName = item.CustomName
            }));
           

            //数量大于0则显示第三行
            if (fileResults.Count > 0)
            {
                LoadFlowLayoutPanel();
            }
            else
            {
                //隐藏第三行
                table.RowStyles[2].Height = 0;
            }


            #region 废弃
            //if (this.fileNames.Count() >= 6)
            //{
            //    this.btnOk.Enabled = false;
            //    //throw new Exception("理论上不可能出现如此错误！！");
            //}
            //if (this.fileNames.Count() >= 1)
            //{
            //    var table = this.tableLayoutPanel1;
            //    var rowHeight = table.Height;
            //    table.RowStyles[0].Height = rowHeight * 0.8f;
            //    table.RowStyles[1].Height = 35;

            //    FlowLayoutPanel flow = new FlowLayoutPanel();

            //    if (table.RowCount > 2)
            //    {
            //        for (int i = 2; i < table.Controls.Count; i++)
            //        {
            //            // 删除当前行的所有控件
            //            for (int j = 0; j < table.ColumnCount; j++)
            //            {
            //                table.Controls.RemoveAt(i);
            //            }
            //            //移除最后一行，最后为空白行
            //            table.RowStyles.RemoveAt(table.RowCount - 1);
            //            table.RowCount = table.RowCount - 1;
            //        }

            //    }

            //    if (table.RowCount == 2)
            //    {
            //        table.RowCount++;
            //        // 行高
            //        table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, table.Height * 0.2f));

            //        //flow.BackColor = Color.Red;
            //        flow.Dock = DockStyle.Fill;
            //    }

            //    var count = 0;

            //    List<PictureBox> pictureBoxes = new List<PictureBox>();
            //    foreach (var fileName in this.fileNames)
            //    {
            //        PictureBox picture = new PictureBox();
            //        //picture.Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.16);
            //        picture.SizeMode = PictureBoxSizeMode.Zoom;
            //        picture.Name = "picture" + count;
            //        picture.Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.16);
            //        picture.Load(fileName);

            //        pictureBoxes.Add(picture);

            //        Label label = new Label();
            //        label.Text = "X";
            //        label.Name = "label" + count;
            //        label.Dock = DockStyle.Right;
            //        picture.Controls.Clear();
            //        picture.Controls.Add(label);

            //        //flow.Controls.Add(picture);

            //        //flow.Controls.Add(picture);
            //        count++;
            //    }

            //    flow.Controls.Clear();
            //    flow.Controls.AddRange(pictureBoxes.ToArray());
            //    table.Controls.Add(flow, 1, 2);
            //}
            //if (this.ucPhotos1 == null)
            //{
            //    return;
            //}
            //this.ucPhotos1.SetUploadParams(map);

            //if (this.ucPhotos2 == null)
            //{
            //    return;
            //}
            //this.ucPhotos2.SetUploadParams(map);

            #endregion
        }

        static PhotoPage2 current;
        static public PhotoPage2 Current
        {
            get
            {
                if (current == null)
                {
                    current = new PhotoPage2();
                }

                return current;
            }
        }

        private void PhotoPage2_FormClosed(object sender, FormClosedEventArgs e)
        {
            current = null;
        }

        private void PhotoPage2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                //ucPhotos2.KeyDownSuccess -= UcPhotos2_KeyDownSuccess;
                //ucPhotos2.KeyDownSuccess += UcPhotos2_KeyDownSuccess;
                //ucPhotos2.KeyDownCall();
            }
        }

        private void UcPhotos2_KeyDownSuccess(object sender, SuccessEventArgs e)
        {
            var image = (Image)e.Object;
            MaxPhotos photo = new MaxPhotos();
            photo.TopMost = true;
            photo.image = image;
            photo.Show();

        }

        //List<string> fileNames = new List<string>();

        //private static object locker = new object();
        private void btnOk_Click(object sender, EventArgs e)
        {
            #region 第一种思路：tableLayoutPanel控件里添加一行加载FlowLayoutPanel控件去显示图片

            //按钮太快的话会导致多拍好几张??
            //后续=6和>6分开写
            if (this.fileResults.Count() >= 6)
            {
                this.btnOk.Enabled = false;

                return;
            }

            //if (this.fileResults.Count() > 6)
            //{
            //    throw new NotSupportedException("不可思议的错误");
            //}


            var fileName = GetFileName();
            //上传服务器
            var uploader = new GeckoUploader(map, fileName);

            uploader.Success += Uploader_Success;
            uploader.Upload("WsCamera");

            //var fileName = this.fileUrl;



            #endregion

            #region 第二种思路：tableLayoutPanel变为原来的80%，在tableLayoutPanel下方加入flowLayoutPanel，实现流式布局（涉及到tableLayoutPanel的Dock=DockStyle.None反而更难实现）
            //var height = Screen.PrimaryScreen.WorkingArea.Height;
            //this.tableLayoutPanel1.Dock = DockStyle.None;
            //this.tableLayoutPanel1.Height = Convert.ToInt32(height * 0.8);
            //FlowLayoutPanel flow = new FlowLayoutPanel();
            //flow.BackColor = Color.Red;
            //flow.Height = Convert.ToInt32(height * 0.2f);
            //this.Controls.Add(flow);
            #endregion
        }

        private void Uploader_Success(object sender, SuccessEventArgs e)
        {
            var fileResult = (FileResult)e.Object;
            //var fileUrl = fileResult.Url;
            this.fileResults.Add(fileResult);

            LoadFlowLayoutPanel();
        }

        double scale = (double)480 / 640;

        private void LoadFlowLayoutPanel()
        {
            var table = this.tableLayoutPanel1;


            //table.RowStyles[2].Height = this.height;
            var flow = this.flowLayoutPanel1;

            //后续=6和>6分开写
            if (this.fileResults.Count() >= 6)
            {
                this.btnOk.Enabled = false;
            }
            //if (this.fileResults.Count() > 6)
            //{
            //    throw new NotSupportedException("不可思议的错误");
            //}
            var count = 0;
             
            List<PictureBox> pictureBoxes = new List<PictureBox>();
            List<Label> labels = new List<Label>();

            foreach (var fileResult in this.fileResults)
            {
                PictureBox picture = new PictureBox();
                //picture.Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.16);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Name = "picture_" + fileResult.ID;
                picture.Width = Convert.ToInt32(this.FindForm().Width * 0.15);
                picture.Height = Convert.ToInt32(picture.Width * this.scale);
                picture.Load(fileResult.Url);

                pictureBoxes.Add(picture);

                Label label = new Label();
                label.Text = "X";
                label.Width=10;
                label.Name = "label_" + fileResult.ID;
                label.Dock = DockStyle.Right;
                picture.Controls.Clear();
                labels.Add(label);

                picture.Controls.Add(label);

                count++;
            }

            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].Click += new EventHandler(this.Label_Click);
            }

            //处理图片单击放大的事情：考虑将图片放大到哪里：
            //1.取消videoSource控件的显示，用一张图片替代，但是后续拍照如何解决；
            //2.是否需要一个新的页面去加载图片
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                pictureBoxes[i].Click += new EventHandler(this.PictureBox_Click);
            }

            flow.Controls.Clear();
            flow.Controls.AddRange(pictureBoxes.ToArray());
            table.Controls.Add(flow, 1, 2);



        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            if (pb.Image != null)
            {
                MaxPhotos photo = new MaxPhotos();
                //photo.TopMost = true;
                photo.image = pb.Image;
                photo.Show();
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            /*
             删除的时候不要删除fileResults，否则this.fileResults[index]会报错（超出索引值），点击拍照的时候重新获取下fileresults从数据库里
             */

            Label label = (Label)sender;
            var name = label.Name;
            var splitNames = name.Split('_');

            var id = splitNames[1];

            string url = Config.ApiUrlPrex + "/Files/Delete";
            var message = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { ID = id });
            var pictureBox = (PictureBox)label.Parent;
            pictureBox.Image = null;
            pictureBox.Dispose();

            var fileResult = this.fileResults.Where(item => item.ID == id).FirstOrDefault();
            this.fileResults.Remove(fileResult);

            if (this.fileResults.Count() >= 6)
            {
                this.btnOk.Enabled = false;
            }
            else
            {
                this.btnOk.Enabled = true;
            }

            //如果图片删除完了就把第三行删除
            if (this.fileResults.Count() == 0)
            {
                var table = this.tableLayoutPanel1;
                //隐藏第三行
                table.RowStyles[2].Height = 0;

                //var rowHeight = table.Height;
                //table.RowStyles[0].Height = rowHeight * 0.8f;
                //table.RowStyles[1].Height = 35;

                //FlowLayoutPanel flow = new FlowLayoutPanel();

                //if (table.RowCount > 2)
                //{
                //    for (int i = 2; i < table.Controls.Count; i++)
                //    {
                //        // 删除当前行的所有控件
                //        for (int j = 0; j < table.ColumnCount; j++)
                //        {
                //            table.Controls.RemoveAt(i);
                //        }
                //        //移除最后一行，最后为空白行
                //        table.RowStyles.RemoveAt(table.RowCount - 1);
                //        table.RowCount = table.RowCount - 1;
                //    }

                //}
            }
            //var fileName=
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            current = null;
            this.FindForm().Close();
        }

        /// <summary>
        /// 获取拍照的文件名字
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            if (!this.vspMain.IsRunning)
            {
                MessageBox.Show("未检测到运行的摄像头影像");
            }

            using (var bmp = this.vspMain.GetCurrentVideoFrame())
            {
                string fileName = CameraUtils.SaveFile(bmp);
                return fileName;
            }
        }

        private void PhotoPage2_Resize(object sender, EventArgs e)
        {
            var table = this.tableLayoutPanel1;

            var flow = this.flowLayoutPanel1;

            //后续=6和>6分开写
            if (this.fileResults.Count() >= 6)
            {
                this.btnOk.Enabled = false;
            }
            //if (this.fileResults.Count() > 6)
            //{
            //    throw new NotSupportedException("不可思议的错误");
            //}
            var count = 0;

            List<PictureBox> pictureBoxes = new List<PictureBox>();
            List<Label> labels = new List<Label>();

            foreach (var fileResult in this.fileResults)
            {
                PictureBox picture = new PictureBox();
                //picture.Width = Convert.ToInt32(Screen.PrimaryScreen.WorkingArea.Width * 0.16);
                picture.SizeMode = PictureBoxSizeMode.Zoom;
                picture.Name = "picture_" + fileResult.ID;
                picture.Width = Convert.ToInt32(this.FindForm().Width * 0.15);
                picture.Height = Convert.ToInt32(picture.Width * this.scale);
                picture.Load(fileResult.Url);

                pictureBoxes.Add(picture);

                Label label = new Label();
                label.Text = "X";
                label.Width = 10;
                label.Name = "label_" + fileResult.ID;
                label.Dock = DockStyle.Right;
                picture.Controls.Clear();
                labels.Add(label);

                picture.Controls.Add(label);

                count++;
            }

            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].Click += new EventHandler(this.Label_Click);
            }

            //处理图片单击放大的事情：考虑将图片放大到哪里：
            //1.取消videoSource控件的显示，用一张图片替代，但是后续拍照如何解决；
            //2.是否需要一个新的页面去加载图片
            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                pictureBoxes[i].Click += new EventHandler(this.PictureBox_Click);
            }

            flow.Controls.Clear();
            flow.Controls.AddRange(pictureBoxes.ToArray());
            table.Controls.Add(flow, 1, 2);
        }
    }

}

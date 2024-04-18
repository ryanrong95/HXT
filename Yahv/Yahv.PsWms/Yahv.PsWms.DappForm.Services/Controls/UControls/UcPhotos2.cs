using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.Usually;
using System.Threading;

namespace Yahv.PsWms.DappForm.Services.Controls.UControls
{
    public partial class UcPhotos2 : UserControl
    {
        object objectLock = new object();
        //event SuccessHanlder keyDownSuccess;
        public event SuccessHanlder KeyDownSuccess;

        public event EventHandler OnRePhoto;

        List<FileResult> fileResults = new List<FileResult>();
        public UcPhotos2()
        {
            InitializeComponent();
            this.Disposed += UcPhotos_Disposed;
        }


        //写个外部调用的keyDown事件
        public void KeyDownCall()
        {
            if (this != null && this.KeyDownSuccess != null)
            {
                if (this.pbMain.Image != null)
                {
                    this.KeyDownSuccess(this, new SuccessEventArgs(this.pbMain.Image));
                }
            }
        }

        public string FileName { get; private set; }

        public void LoadPicture(string fileName)
        {
            this.FileName = fileName;
            this.pbMain.Load(fileName);
        }

        PhotoMap map;

        public void SetUploadParams(PhotoMap map)
        {
            this.map = map;
        }

        private void UcPhotos_Load(object sender, EventArgs e)
        {
            this.pbMain.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

        private void UcPhotos_Disposed(object sender, EventArgs e)
        {
            this.pbMain.Dispose();
        }

        private void btnRePhoto_Click(object sender, EventArgs e)
        {
            if (this != null && this.OnRePhoto != null)
            {
                this.OnRePhoto(this, new EventArgs());
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string fileName = this.FileName;
            var uploader = new GeckoUploader(map, fileName);

            uploader.Success += Uploader_Success;
            uploader.Upload("WsCamera");
            //var fileResult = GeckoUploader.fileResult;
        }

        private void Uploader_Success(object sender, Usually.SuccessEventArgs e)
        {
            var fileResult = (FileResult)e.Object;
            if (this.pictureBox1.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox1.Load(fileResult.Url);
            }
            else if (this.pictureBox1.Image != null && this.pictureBox2.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox2.Load(fileResult.Url);
            }
            else if (this.pictureBox1.Image != null && this.pictureBox2.Image != null && this.pictureBox3.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox3.Load(fileResult.Url);
            }
            else if (this.pictureBox1.Image != null && this.pictureBox2.Image != null && this.pictureBox3.Image != null && this.pictureBox4.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox4.Load(fileResult.Url);
            }
            else if (this.pictureBox1.Image != null && this.pictureBox2.Image != null && this.pictureBox3.Image != null && this.pictureBox4.Image != null && this.pictureBox5.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox5.Load(fileResult.Url);
            }
            else if (this.pictureBox1.Image != null && this.pictureBox2.Image != null && this.pictureBox3.Image != null && this.pictureBox4.Image != null && this.pictureBox5.Image != null && this.pictureBox6.Image == null)
            {
                fileResults.Add(fileResult);
                this.pictureBox6.Load(fileResult.Url);
            }
            if (this.pictureBox1.Image != null && this.pictureBox2.Image != null && this.pictureBox3.Image != null && this.pictureBox4.Image != null && this.pictureBox5.Image != null && this.pictureBox6.Image != null)
            {
                this.btnOk.Enabled = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                this.pbMain.Image = this.pictureBox1.Image;
                getMaxPhotos();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (this.pictureBox2.Image != null)
            {
                this.pbMain.Image = this.pictureBox2.Image;
                getMaxPhotos();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (this.pictureBox3.Image != null)
            {
                this.pbMain.Image = this.pictureBox3.Image;
                getMaxPhotos();
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            if (this.pictureBox4.Image != null)
            {
                this.pbMain.Image = this.pictureBox4.Image;
                getMaxPhotos();
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            if (this.pictureBox5.Image != null)
            {
                this.pbMain.Image = this.pictureBox5.Image;
                getMaxPhotos();
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            if (this.pictureBox6.Image != null)
            {
                this.pbMain.Image = this.pictureBox6.Image;
                getMaxPhotos();
            }
        }

        private void pbMain_Click(object sender, EventArgs e)
        {
            if (this.pbMain.Image != null)
            {
                getMaxPhotos();
            }
        }

        private void getMaxPhotos()
        {
            MaxPhotos photo = new MaxPhotos();
            photo.TopMost = true;
            photo.image = this.pbMain.Image;
            photo.Show();
        }

        static object locker = new object();

        static UcPhotos2 current;
        static public UcPhotos2 Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new UcPhotos2();
                        }
                    }
                }
                return current;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox1.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox1.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox1.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox2.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox2.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox2.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox3.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox3.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox3.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox4.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox4.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox4.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox5.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox5.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox5.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox6.Image;
            if (image != null)
            {
                var pictureUrl = this.pictureBox6.ImageLocation;

                DeleteFile(pictureUrl);

                this.pictureBox6.Image = null;
                image.Dispose();

                this.btnOk.Enabled = true;
            }
        }

        private void DeleteFile(string pictureUrl)
        {
            var result = this.fileResults.Where(item => item.Url == pictureUrl).FirstOrDefault();
            if (result == null)
            {
                MessageBox.Show("不可思议的错误！！");
            }

            string url = Config.ApiUrlPrex + "/Files/Delete";
            var message = Yahv.Utils.Http.ApiHelper.Current.JPost(url, new { ID = result.ID });
        }
    }
}

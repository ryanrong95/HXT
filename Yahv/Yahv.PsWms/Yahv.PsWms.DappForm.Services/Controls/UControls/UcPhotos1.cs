using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yahv.PsWms.DappForm.Services.Controls.UControls
{
    public partial class UcPhotos1 : UserControl
    {
        public event EventHandler OnRePhoto;
        public UcPhotos1()
        {
            InitializeComponent();
            this.Disposed += UcPhotos_Disposed;
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
        /// 确定（并上传）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            string fileName = this.FileName;
            var uploader = new GeckoUploader(map, fileName);
            uploader.Upload("WsCamera");

            //this.FindForm().Close();
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

        /// <summary>
        /// 重拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRePhoto_Click(object sender, EventArgs e)
        {
            if (this != null && this.OnRePhoto != null)
            {
                this.OnRePhoto(this, new EventArgs());
            }
        }

        private void UcPhotos_Disposed(object sender, EventArgs e)
        {
            this.pbMain.Dispose();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pbMain_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

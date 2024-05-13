using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Yahv.Utils.Serializers;
using WinApp.Services;

namespace WinApp.Services.Controls.UControls
{
    public partial class MulUcPhotos : UserControl
    {
        public event EventHandler OnRePhoto;
        public MulUcPhotos()
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

        PhotoMaps maps;

        public void SetUploadParams(PhotoMaps maps)
        {
            this.maps = maps;
        }

        private void UcPhotos_Load(object sender, EventArgs e)
        {
            this.pbMain.Dock = DockStyle.Fill;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string fileName = this.FileName;
            var uploader = new GeckoUploader(maps, fileName);
            uploader.Uploads("WsCamera");

            this.FindForm().Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }

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
    }
}

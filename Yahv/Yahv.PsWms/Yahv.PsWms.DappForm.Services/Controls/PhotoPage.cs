using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.PsWms.DappForm.Services.Controls.UControls;

namespace Yahv.PsWms.DappForm.Services.Controls
{
    public partial class PhotoPage : Form
    {
        public PhotoPage()
        {
            InitializeComponent();
        }

        UcCamera ucCamera;
        UcPhotos ucPhotos;

        private void PhotoPage_Load(object sender, EventArgs e)
        {
            //线程异常
            //throw new Exception("6");
#if !DEBUG
            this.TopMost = true;
#endif
            int count = CameraUtils.CameraCount();

            this.Controls.Clear();

            if (count >= 1)
            {
                this.ucCamera = new UcCamera();
                this.ucCamera.Dock = DockStyle.Fill;
                this.Controls.Add(this.ucCamera);
                this.ucCamera.OnOk += Uc_OnOk;

                this.ucPhotos = new UcPhotos
                {
                    Visible = false
                };
                this.ucPhotos.Dock = DockStyle.Fill;
                this.Controls.Add(this.ucPhotos);
                this.ucPhotos.OnRePhoto += UcPhotos_OnRePhoto;
            }

            if (count == 0)
            {
                MessageBox.Show("未检测到运行的摄像设备");
            }
        }

        private void UcPhotos_OnRePhoto(object sender, EventArgs e)
        {

            this.ucCamera.Show();
            this.ucPhotos.Hide();
            this.ucCamera.Refresh();
            //this.ucCamera.Visible = true;
            //this.ucPhotos.Visible = false;
        }

        private void Uc_OnOk(object sender, CameraOkEventArgs e)
        {
            this.ucPhotos.LoadPicture(e.FileName);

            this.ucCamera.Hide();
            this.ucPhotos.Show();
            this.ucPhotos.Refresh();

            //this.ucCamera.Visible = false;
            //this.ucPhotos.Visible = true;
        }

        public void SetUploadParams(Services.PhotoMap map)
        {
            if (this.ucPhotos == null)
            {
                return;
            }
            this.ucPhotos.SetUploadParams(map);
        }

        static PhotoPage current;
        static public PhotoPage Current
        {
            get
            {
                if (current == null)
                {
                    current = new PhotoPage();
                }

                return current;
            }
        }

        private void PhotoPage_FormClosed(object sender, FormClosedEventArgs e)
        {
            current = null;
        }
    }
}

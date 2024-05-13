using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp.Services.Controls
{
    public partial class PictureShow : Form
    {
        public PictureShow()
        {
            InitializeComponent();
        }

        public static string url;
        GeckoWebBrowser firefox;


        private void PictureShow_Load(object sender, EventArgs e)
        {

            //线程异常
            //throw new Exception("7");
            this.Text = "图片显示";
            var firefox = this.firefox = new GeckoWebBrowser
            {
                Dock = DockStyle.Fill,
                NoDefaultContextMenu = false,
            };

            this.firefox.Navigate(url, GeckoLoadFlags.FirstLoad);
            this.Controls.Add(firefox);
        }

        static PictureShow current;
        static public PictureShow Current
        {
            get
            {
                if (current == null)
                {
                    current = new PictureShow();
                }
                return current;
            }
        }

        private void PictureShow_FormClosed(object sender, FormClosedEventArgs e)
        {
            current = null;
        }
    }
}
